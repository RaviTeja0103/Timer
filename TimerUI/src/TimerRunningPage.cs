using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;
using System;
using System.Timers;
using System.Threading;
using TimerService;

namespace TimerUI.Pages
{
    public class TimerRunningPage : View
    {
        public event Action OnNavigateToList;
        public event Action<int, string> OnTimerComplete;

        private int currentTimerId = -1;
        private string currentTimerName = "";
        private int totalSeconds = 0;
        private int elapsedSeconds = 0;
        private System.Timers.Timer uiUpdateTimer;
        private bool isRunning = false;
        private TimerManager timerManager;

        private TextLabel timerDisplayLabel;
        private Button playPauseButton;
        private Button resetButton;
        private TextLabel timerNameLabel;
        private View progressView;

        public TimerRunningPage() : base()
        {
            Size2D = new Size2D(Window.Instance.WindowSize.Width, Window.Instance.WindowSize.Height);
            BackgroundColor = new Color(0.95f, 0.95f, 0.95f, 1.0f);
            
            timerManager = TimerManager.Instance;
            timerManager.OnTimerComplete += (s, e) =>
            {
                if (e.TimerId == currentTimerId)
                {
                    OnTimerComplete?.Invoke(currentTimerId, currentTimerName);
                }
            };
            
            CreateUI();
            uiUpdateTimer = new System.Timers.Timer(1000);
            uiUpdateTimer.Elapsed += UpdateTimeDisplay;
        }

        private void CreateUI()
        {
            // Header with back button
            var headerView = new View
            {
                Size2D = new Size2D(Window.Instance.WindowSize.Width, 60),
                Position2D = new Position2D(0, 0),
                BackgroundColor = new Color(0.2f, 0.6f, 1.0f, 1.0f)
            };
            Add(headerView);

            var backButton = new Button
            {
                Text = "←",
                Size2D = new Size2D(60, 60),
                Position2D = new Position2D(10, 0),
                PointSize = 28.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            backButton.Clicked += (s, e) =>
            {
                StopTimer();
                OnNavigateToList?.Invoke();
            };
            headerView.Add(backButton);

            var titleText = new TextLabel
            {
                Text = "Running Timer",
                Size2D = new Size2D(Window.Instance.WindowSize.Width - 80, 60),
                Position2D = new Position2D(70, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                PointSize = 24.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            headerView.Add(titleText);

            // Content area
            var contentView = new View
            {
                Size2D = new Size2D(Window.Instance.WindowSize.Width, Window.Instance.WindowSize.Height - 60),
                Position2D = new Position2D(0, 60)
            };
            Add(contentView);

            // Timer name
            timerNameLabel = new TextLabel
            {
                Text = "Timer",
                Size2D = new Size2D(Window.Instance.WindowSize.Width, 50),
                Position2D = new Position2D(0, 40),
                HorizontalAlignment = HorizontalAlignment.Center,
                PointSize = 28.0f,
                TextColor = new Color(0.2f, 0.2f, 0.2f, 1.0f)
            };
            contentView.Add(timerNameLabel);

            // Progress bar
            progressView = new View
            {
                Size2D = new Size2D(300, 300),
                Position2D = new Position2D((Window.Instance.WindowSize.Width - 300) / 2, 100),
                BackgroundColor = new Color(0.9f, 0.9f, 0.9f, 1.0f)
            };
            contentView.Add(progressView);

            // Timer display
            timerDisplayLabel = new TextLabel
            {
                Text = "00:00:00",
                Size2D = new Size2D(300, 300),
                Position2D = new Position2D((Window.Instance.WindowSize.Width - 300) / 2, 100),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                PointSize = 60.0f,
                TextColor = new Color(0.2f, 0.6f, 1.0f, 1.0f)
            };
            contentView.Add(timerDisplayLabel);

            // Control buttons
            playPauseButton = new Button
            {
                Text = "Play",
                Size2D = new Size2D(120, 60),
                Position2D = new Position2D((Window.Instance.WindowSize.Width - 260) / 2, 450),
                BackgroundColor = new Color(0.2f, 0.8f, 0.4f, 1.0f),
                PointSize = 18.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            playPauseButton.Clicked += (s, e) =>
            {
                TogglePlayPause();
            };
            contentView.Add(playPauseButton);

            resetButton = new Button
            {
                Text = "Reset",
                Size2D = new Size2D(120, 60),
                Position2D = new Position2D((Window.Instance.WindowSize.Width - 260) / 2 + 140, 450),
                BackgroundColor = new Color(0.8f, 0.2f, 0.2f, 1.0f),
                PointSize = 18.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            resetButton.Clicked += (s, e) =>
            {
                ResetTimer();
            };
            contentView.Add(resetButton);

            // Dismiss button
            var dismissButton = new Button
            {
                Text = "Dismiss",
                Size2D = new Size2D(Window.Instance.WindowSize.Width - 40, 50),
                Position2D = new Position2D(20, Window.Instance.WindowSize.Height - 120),
                BackgroundColor = new Color(0.6f, 0.6f, 0.6f, 1.0f),
                PointSize = 16.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            dismissButton.Clicked += (s, e) =>
            {
                StopTimer();
                OnNavigateToList?.Invoke();
            };
            contentView.Add(dismissButton);
        }

        public void SetTimer(int timerId, string timerName)
        {
            StopTimer();
            
            if (timerId == -1)
            {
                // Create new timer
                totalSeconds = 300; // Default 5 minutes
                currentTimerId = timerManager.CreateTimer("New Timer", totalSeconds);
                currentTimerName = "New Timer";
            }
            else
            {
                currentTimerId = timerId;
                var timer = timerManager.GetTimer(timerId);
                if (timer != null)
                {
                    currentTimerName = timer.Name;
                    totalSeconds = timer.TotalSeconds;
                    elapsedSeconds = timer.ElapsedSeconds;
                }
            }
            
            timerNameLabel.Text = currentTimerName;
            UpdateTimeDisplay(null, null);
            playPauseButton.Text = "Play";
        }

        private void TogglePlayPause()
        {
            if (isRunning)
            {
                timerManager.PauseTimer(currentTimerId);
                isRunning = false;
                uiUpdateTimer.Stop();
                playPauseButton.Text = "Play";
            }
            else
            {
                if (currentTimerId == -1)
                {
                    return;
                }
                
                var timer = timerManager.GetTimer(currentTimerId);
                if (timer != null && timer.ElapsedSeconds < timer.TotalSeconds)
                {
                    if (!timer.IsRunning)
                    {
                        timerManager.StartTimer(currentTimerId);
                    }
                    else
                    {
                        timerManager.ResumeTimer(currentTimerId);
                    }
                    
                    isRunning = true;
                    uiUpdateTimer.Start();
                    playPauseButton.Text = "Pause";
                }
            }
        }

        private void ResetTimer()
        {
            StopTimer();
            timerManager.StopTimer(currentTimerId);
            elapsedSeconds = 0;
            timerDisplayLabel.Text = FormatTime(totalSeconds);
            progressView.BackgroundColor = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        }

        private void StopTimer()
        {
            isRunning = false;
            uiUpdateTimer?.Stop();
            playPauseButton.Text = "Play";
        }

        private void UpdateTimeDisplay(object sender, ElapsedEventArgs e)
        {
            var timer = timerManager.GetTimer(currentTimerId);
            if (timer != null)
            {
                elapsedSeconds = timer.ElapsedSeconds;
                totalSeconds = timer.TotalSeconds;
                
                if (elapsedSeconds >= totalSeconds)
                {
                    isRunning = false;
                    uiUpdateTimer?.Stop();
                }
            }

            // Update UI on main thread
            int remaining = totalSeconds - elapsedSeconds;
            if (remaining < 0) remaining = 0;
            timerDisplayLabel.Text = FormatTime(remaining);
            
            // Update progress
            float progress = totalSeconds > 0 ? (float)elapsedSeconds / totalSeconds : 0;
            progressView.BackgroundColor = new Color(
                0.2f + (0.6f * progress),
                0.6f - (0.4f * progress),
                1.0f - (0.6f * progress),
                1.0f
            );
        }

        private string FormatTime(int seconds)
        {
            int hours = seconds / 3600;
            int minutes = (seconds % 3600) / 60;
            int secs = seconds % 60;
            
            return $"{hours:D2}:{minutes:D2}:{secs:D2}";
        }
    }
}
