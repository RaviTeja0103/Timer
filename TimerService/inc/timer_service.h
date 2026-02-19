#ifndef TIMER_SERVICE_H
#define TIMER_SERVICE_H

#include <vector>
#include <map>
#include <string>
#include <thread>
#include <mutex>
#include <condition_variable>
#include <dlog.h>

#define LOG_TAG "TimerService"

struct Timer {
    int id;
    int totalSeconds;
    int elapsedSeconds;
    bool isRunning;
    bool isPaused;
    std::string name;
};

struct PredefinedTimer {
    std::string name;
    int seconds;
};

class TimerService {
public:
    TimerService();
    ~TimerService();

    // Timer management
    int createTimer(const std::string& name, int seconds);
    bool startTimer(int timerId);
    bool pauseTimer(int timerId);
    bool resumeTimer(int timerId);
    bool stopTimer(int timerId);
    bool deleteTimer(int timerId);
    
    // Timer queries
    Timer getTimer(int timerId);
    std::vector<Timer> getAllTimers();
    bool isTimerRunning(int timerId);
    int getTimerProgress(int timerId);  // Returns 0-100
    
    // Predefined timers
    bool savePredefinedTimer(const std::string& name, int seconds);
    bool deletePredefinedTimer(const std::string& name);
    std::vector<PredefinedTimer> getPredefinedTimers();
    int createTimerFromPredefined(const std::string& predefinedName);
    
    // Callback for timer completion
    void setOnTimerCompleteCallback(void (*callback)(int timerId, const std::string& name));
    
    // Config
    static const int MAX_TIMERS = 5;

private:
    std::vector<Timer> timers;
    std::vector<PredefinedTimer> predefinedTimers;
    std::map<int, std::thread> timerThreads;
    std::mutex timersMutex;
    std::condition_variable cv;
    int nextTimerId;
    bool running;
    
    void (*onTimerCompleteCallback)(int, const std::string&);
    
    void timerWorker(int timerId);
    void loadPredefinedTimers();
    void savePredefinedTimersToFile();
};

#endif // TIMER_SERVICE_H
