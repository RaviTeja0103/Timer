using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace TimerService
{
    public class Timer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalSeconds { get; set; }
        public int ElapsedSeconds { get; set; }
        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }
    }

    public class PredefinedTimer
    {
        public string Name { get; set; }
        public int Seconds { get; set; }
    }

    public class TimerManager
    {
        private static TimerManager _instance;
        private List<Timer> _timers = new List<Timer>();
        private List<PredefinedTimer> _predefinedTimers = new List<PredefinedTimer>();
        private int _nextTimerId = 1;
        private const int MAX_TIMERS = 5;

        public event EventHandler<TimerCompleteEventArgs> OnTimerComplete;

        public static TimerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TimerManager();
                }
                return _instance;
            }
        }

        private TimerManager()
        {
            LoadPredefinedTimers();
        }

        public int CreateTimer(string name, int seconds)
        {
            if (_timers.Count >= MAX_TIMERS)
            {
                return -1;
            }

            var timer = new Timer
            {
                Id = _nextTimerId++,
                Name = name,
                TotalSeconds = seconds,
                ElapsedSeconds = 0,
                IsRunning = false,
                IsPaused = false
            };

            _timers.Add(timer);
            return timer.Id;
        }

        public bool StartTimer(int timerId)
        {
            var timer = GetTimer(timerId);
            if (timer == null || timer.IsRunning)
            {
                return false;
            }

            timer.IsRunning = true;
            timer.IsPaused = false;
            
            // Start background timer update
            System.Threading.ThreadPool.QueueUserWorkItem(_ => TimerWorker(timerId));
            return true;
        }

        public bool PauseTimer(int timerId)
        {
            var timer = GetTimer(timerId);
            if (timer == null || !timer.IsRunning)
            {
                return false;
            }

            timer.IsPaused = true;
            return true;
        }

        public bool ResumeTimer(int timerId)
        {
            var timer = GetTimer(timerId);
            if (timer == null || !timer.IsRunning)
            {
                return false;
            }

            timer.IsPaused = false;
            return true;
        }

        public bool StopTimer(int timerId)
        {
            var timer = GetTimer(timerId);
            if (timer == null)
            {
                return false;
            }

            timer.IsRunning = false;
            timer.IsPaused = false;
            timer.ElapsedSeconds = 0;
            return true;
        }

        public bool DeleteTimer(int timerId)
        {
            var timer = GetTimer(timerId);
            if (timer == null)
            {
                return false;
            }

            StopTimer(timerId);
            _timers.Remove(timer);
            return true;
        }

        public Timer GetTimer(int timerId)
        {
            foreach (var timer in _timers)
            {
                if (timer.Id == timerId)
                {
                    return timer;
                }
            }
            return null;
        }

        public List<Timer> GetAllTimers()
        {
            return new List<Timer>(_timers);
        }

        public bool IsTimerRunning(int timerId)
        {
            var timer = GetTimer(timerId);
            return timer != null && timer.IsRunning && !timer.IsPaused;
        }

        public int GetTimerProgress(int timerId)
        {
            var timer = GetTimer(timerId);
            if (timer == null || timer.TotalSeconds == 0)
            {
                return -1;
            }

            return (timer.ElapsedSeconds * 100) / timer.TotalSeconds;
        }

        public bool SavePredefinedTimer(string name, int seconds)
        {
            var existing = _predefinedTimers.Find(t => t.Name == name);
            if (existing != null)
            {
                existing.Seconds = seconds;
            }
            else
            {
                _predefinedTimers.Add(new PredefinedTimer { Name = name, Seconds = seconds });
            }

            SavePredefinedTimersToFile();
            return true;
        }

        public bool DeletePredefinedTimer(string name)
        {
            var timer = _predefinedTimers.Find(t => t.Name == name);
            if (timer == null)
            {
                return false;
            }

            _predefinedTimers.Remove(timer);
            SavePredefinedTimersToFile();
            return true;
        }

        public List<PredefinedTimer> GetPredefinedTimers()
        {
            return new List<PredefinedTimer>(_predefinedTimers);
        }

        public int CreateTimerFromPredefined(string predefinedName)
        {
            var predefined = _predefinedTimers.Find(t => t.Name == predefinedName);
            if (predefined == null)
            {
                return -1;
            }

            return CreateTimer(predefined.Name, predefined.Seconds);
        }

        private void TimerWorker(int timerId)
        {
            while (true)
            {
                var timer = GetTimer(timerId);
                if (timer == null || !timer.IsRunning)
                {
                    break;
                }

                System.Threading.Thread.Sleep(1000);

                if (!timer.IsPaused && timer.IsRunning)
                {
                    timer.ElapsedSeconds++;

                    if (timer.ElapsedSeconds >= timer.TotalSeconds)
                    {
                        timer.IsRunning = false;
                        OnTimerComplete?.Invoke(this, new TimerCompleteEventArgs
                        {
                            TimerId = timerId,
                            TimerName = timer.Name
                        });
                        break;
                    }
                }
            }
        }

        private void LoadPredefinedTimers()
        {
            string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TimerApp");
            string filePath = Path.Combine(dataDir, "predefined_timers.txt");

            if (File.Exists(filePath))
            {
                try
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        var parts = line.Split(' ');
                        if (parts.Length == 2 && int.TryParse(parts[1], out int seconds))
                        {
                            _predefinedTimers.Add(new PredefinedTimer { Name = parts[0], Seconds = seconds });
                        }
                    }
                }
                catch { }
            }
            else
            {
                // Create default predefined timers
                _predefinedTimers = new List<PredefinedTimer>
                {
                    new PredefinedTimer { Name = "Quick Timer", Seconds = 60 },
                    new PredefinedTimer { Name = "5 Minutes", Seconds = 300 },
                    new PredefinedTimer { Name = "10 Minutes", Seconds = 600 },
                    new PredefinedTimer { Name = "30 Minutes", Seconds = 1800 }
                };
                SavePredefinedTimersToFile();
            }
        }

        private void SavePredefinedTimersToFile()
        {
            try
            {
                string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TimerApp");
                Directory.CreateDirectory(dataDir);

                string filePath = Path.Combine(dataDir, "predefined_timers.txt");
                using (var writer = new StreamWriter(filePath))
                {
                    foreach (var timer in _predefinedTimers)
                    {
                        writer.WriteLine($"{timer.Name} {timer.Seconds}");
                    }
                }
            }
            catch { }
        }
    }

    public class TimerCompleteEventArgs : EventArgs
    {
        public int TimerId { get; set; }
        public string TimerName { get; set; }
    }
}
