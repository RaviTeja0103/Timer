#include "timer_service.h"
#include <algorithm>
#include <fstream>
#include <iostream>
#include <chrono>
#include <cstdlib>

#ifdef DLOG_ENABLED
#define LOG(fmt, ...) dlog_print(DLOG_INFO, LOG_TAG, fmt, ##__VA_ARGS__)
#define LOG_ERR(fmt, ...) dlog_print(DLOG_ERROR, LOG_TAG, fmt, ##__VA_ARGS__)
#else
#define LOG(fmt, ...) fprintf(stdout, "[INFO] " fmt "\n", ##__VA_ARGS__)
#define LOG_ERR(fmt, ...) fprintf(stderr, "[ERROR] " fmt "\n", ##__VA_ARGS__)
#endif

TimerService::TimerService() 
    : nextTimerId(1), running(true), onTimerCompleteCallback(nullptr) {
    LOG("TimerService initialized");
    loadPredefinedTimers();
}

TimerService::~TimerService() {
    running = false;
    cv.notify_all();
    
    std::lock_guard<std::mutex> lock(timersMutex);
    for (auto& thread_pair : timerThreads) {
        if (thread_pair.second.joinable()) {
            thread_pair.second.join();
        }
    }
    LOG("TimerService destroyed");
}

int TimerService::createTimer(const std::string& name, int seconds) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    if (timers.size() >= MAX_TIMERS) {
        LOG_ERR("Maximum timers (%d) reached", MAX_TIMERS);
        return -1;
    }
    
    Timer newTimer;
    newTimer.id = nextTimerId++;
    newTimer.name = name;
    newTimer.totalSeconds = seconds;
    newTimer.elapsedSeconds = 0;
    newTimer.isRunning = false;
    newTimer.isPaused = false;
    
    timers.push_back(newTimer);
    LOG("Timer created: id=%d, name=%s, seconds=%d", newTimer.id, name.c_str(), seconds);
    
    return newTimer.id;
}

bool TimerService::startTimer(int timerId) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(timers.begin(), timers.end(),
        [timerId](const Timer& t) { return t.id == timerId; });
    
    if (it == timers.end()) {
        LOG_ERR("Timer %d not found", timerId);
        return false;
    }
    
    if (it->isRunning) {
        LOG_ERR("Timer %d already running", timerId);
        return false;
    }
    
    it->isRunning = true;
    it->isPaused = false;
    
    // Create worker thread for this timer
    timerThreads[timerId] = std::thread(&TimerService::timerWorker, this, timerId);
    LOG("Timer %d started", timerId);
    
    return true;
}

bool TimerService::pauseTimer(int timerId) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(timers.begin(), timers.end(),
        [timerId](const Timer& t) { return t.id == timerId; });
    
    if (it == timers.end() || !it->isRunning) {
        return false;
    }
    
    it->isPaused = true;
    LOG("Timer %d paused", timerId);
    return true;
}

bool TimerService::resumeTimer(int timerId) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(timers.begin(), timers.end(),
        [timerId](const Timer& t) { return t.id == timerId; });
    
    if (it == timers.end() || !it->isRunning) {
        return false;
    }
    
    it->isPaused = false;
    cv.notify_all();
    LOG("Timer %d resumed", timerId);
    return true;
}

bool TimerService::stopTimer(int timerId) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(timers.begin(), timers.end(),
        [timerId](const Timer& t) { return t.id == timerId; });
    
    if (it == timers.end()) {
        return false;
    }
    
    it->isRunning = false;
    it->isPaused = false;
    it->elapsedSeconds = 0;
    
    auto thread_it = timerThreads.find(timerId);
    if (thread_it != timerThreads.end() && thread_it->second.joinable()) {
        thread_it->second.join();
        timerThreads.erase(thread_it);
    }
    
    LOG("Timer %d stopped", timerId);
    return true;
}

bool TimerService::deleteTimer(int timerId) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    stopTimer(timerId);
    
    auto it = std::find_if(timers.begin(), timers.end(),
        [timerId](const Timer& t) { return t.id == timerId; });
    
    if (it != timers.end()) {
        timers.erase(it);
        LOG("Timer %d deleted", timerId);
        return true;
    }
    
    return false;
}

Timer TimerService::getTimer(int timerId) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(timers.begin(), timers.end(),
        [timerId](const Timer& t) { return t.id == timerId; });
    
    if (it != timers.end()) {
        return *it;
    }
    
    return Timer{-1, 0, 0, false, false, ""};
}

std::vector<Timer> TimerService::getAllTimers() {
    std::lock_guard<std::mutex> lock(timersMutex);
    return timers;
}

bool TimerService::isTimerRunning(int timerId) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(timers.begin(), timers.end(),
        [timerId](const Timer& t) { return t.id == timerId; });
    
    return it != timers.end() && it->isRunning && !it->isPaused;
}

int TimerService::getTimerProgress(int timerId) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(timers.begin(), timers.end(),
        [timerId](const Timer& t) { return t.id == timerId; });
    
    if (it == timers.end()) {
        return -1;
    }
    
    if (it->totalSeconds == 0) {
        return 0;
    }
    
    return (it->elapsedSeconds * 100) / it->totalSeconds;
}

void TimerService::timerWorker(int timerId) {
    LOG("Timer worker started for timer %d", timerId);
    
    while (running) {
        {
            std::unique_lock<std::mutex> lock(timersMutex);
            
            auto it = std::find_if(timers.begin(), timers.end(),
                [timerId](const Timer& t) { return t.id == timerId; });
            
            if (it == timers.end() || !it->isRunning) {
                break;
            }
            
            // Wait if paused
            cv.wait_for(lock, std::chrono::milliseconds(100), [this, it]() {
                return !it->isPaused;
            });
            
            if (!it->isPaused && it->isRunning) {
                it->elapsedSeconds++;
                
                LOG("Timer %d: %d/%d seconds", timerId, it->elapsedSeconds, it->totalSeconds);
                
                // Check if timer is complete
                if (it->elapsedSeconds >= it->totalSeconds) {
                    it->isRunning = false;
                    std::string timerName = it->name;
                    
                    if (onTimerCompleteCallback) {
                        onTimerCompleteCallback(timerId, timerName);
                    }
                    
                    LOG("Timer %d completed", timerId);
                    break;
                }
            }
        }
        
        std::this_thread::sleep_for(std::chrono::milliseconds(900));
    }
}

bool TimerService::savePredefinedTimer(const std::string& name, int seconds) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(predefinedTimers.begin(), predefinedTimers.end(),
        [&name](const PredefinedTimer& t) { return t.name == name; });
    
    if (it != predefinedTimers.end()) {
        it->seconds = seconds;
    } else {
        predefinedTimers.push_back({name, seconds});
    }
    
    savePredefinedTimersToFile();
    LOG("Predefined timer saved: %s = %d seconds", name.c_str(), seconds);
    return true;
}

bool TimerService::deletePredefinedTimer(const std::string& name) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(predefinedTimers.begin(), predefinedTimers.end(),
        [&name](const PredefinedTimer& t) { return t.name == name; });
    
    if (it != predefinedTimers.end()) {
        predefinedTimers.erase(it);
        savePredefinedTimersToFile();
        LOG("Predefined timer deleted: %s", name.c_str());
        return true;
    }
    
    return false;
}

std::vector<PredefinedTimer> TimerService::getPredefinedTimers() {
    std::lock_guard<std::mutex> lock(timersMutex);
    return predefinedTimers;
}

int TimerService::createTimerFromPredefined(const std::string& predefinedName) {
    std::lock_guard<std::mutex> lock(timersMutex);
    
    auto it = std::find_if(predefinedTimers.begin(), predefinedTimers.end(),
        [&predefinedName](const PredefinedTimer& t) { return t.name == predefinedName; });
    
    if (it == predefinedTimers.end()) {
        LOG_ERR("Predefined timer %s not found", predefinedName.c_str());
        return -1;
    }
    
    Timer newTimer;
    newTimer.id = nextTimerId++;
    newTimer.name = it->name;
    newTimer.totalSeconds = it->seconds;
    newTimer.elapsedSeconds = 0;
    newTimer.isRunning = false;
    newTimer.isPaused = false;
    
    timers.push_back(newTimer);
    LOG("Timer created from predefined: id=%d, name=%s", newTimer.id, it->name.c_str());
    
    return newTimer.id;
}

void TimerService::setOnTimerCompleteCallback(void (*callback)(int, const std::string&)) {
    onTimerCompleteCallback = callback;
}

void TimerService::loadPredefinedTimers() {
    std::ifstream file("/opt/usr/apps/com.example.timerapp/data/predefined_timers.txt");
    if (file.is_open()) {
        std::string name;
        int seconds;
        while (file >> name >> seconds) {
            predefinedTimers.push_back({name, seconds});
        }
        file.close();
        LOG("Loaded %d predefined timers", (int)predefinedTimers.size());
    } else {
        // Create default predefined timers
        predefinedTimers = {
            {"Quick Timer", 60},
            {"5 Minutes", 300},
            {"10 Minutes", 600},
            {"30 Minutes", 1800}
        };
        savePredefinedTimersToFile();
    }
}

void TimerService::savePredefinedTimersToFile() {
    const std::string dataDir = "/opt/usr/apps/com.example.timerapp/data";
    system(("mkdir -p " + dataDir).c_str());
    
    std::ofstream file(dataDir + "/predefined_timers.txt");
    if (file.is_open()) {
        for (const auto& timer : predefinedTimers) {
            file << timer.name << " " << timer.seconds << std::endl;
        }
        file.close();
        LOG("Predefined timers saved to file");
    } else {
        LOG_ERR("Failed to save predefined timers");
    }
}
