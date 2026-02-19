using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;
using System;
using System.Collections.Generic;

namespace TimerUI.Pages
{
    public class TimerListPage : View
    {
        public event Action OnNavigateToMain;
        public event Action<int, string> OnTimerSelected;

        private View contentView;
        private View timerListContainer;

        public TimerListPage() : base()
        {
            Size2D = new Size2D(Window.Instance.WindowSize.Width, Window.Instance.WindowSize.Height);
            BackgroundColor = new Color(0.95f, 0.95f, 0.95f, 1.0f);
            
            CreateUI();
        }

        private void CreateUI()
        {
            // Header with back button and title
            var headerView = new View
            {
                Size2D = new Size2D(Window.Instance.WindowSize.Width, 80),
                Position2D = new Position2D(0, 0),
                BackgroundColor = new Color(0.2f, 0.6f, 1.0f, 1.0f)
            };
            Add(headerView);

            // Back button
            var backButton = new Button
            {
                Text = "←",
                Size2D = new Size2D(70, 70),
                Position2D = new Position2D(10, 5),
                PointSize = 32.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            backButton.Clicked += (s, e) =>
            {
                OnNavigateToMain?.Invoke();
            };
            headerView.Add(backButton);

            // Title
            var titleText = new TextLabel
            {
                Text = "My Timers",
                Size2D = new Size2D(Window.Instance.WindowSize.Width - 100, 80),
                Position2D = new Position2D(80, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                PointSize = 32.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            headerView.Add(titleText);

            // Content area
            contentView = new View
            {
                Size2D = new Size2D(Window.Instance.WindowSize.Width, Window.Instance.WindowSize.Height - 80),
                Position2D = new Position2D(0, 80)
            };
            Add(contentView);

            // Timer list container with scrolling
            timerListContainer = new View
            {
                Size2D = new Size2D(Window.Instance.WindowSize.Width, Window.Instance.WindowSize.Height - 80),
                Position2D = new Position2D(0, 0)
            };
            contentView.Add(timerListContainer);

            RefreshTimerList();
        }

        public void RefreshTimerList()
        {
            // Clear existing timers
            uint childCount = timerListContainer.ChildCount;
            for (int i = (int)childCount - 1; i >= 0; i--)
            {
                timerListContainer.Remove(timerListContainer.GetChildAt((uint)i));
            }

            // Get timers from service
            var allTimers = TimerService.TimerManager.Instance.GetAllTimers();
            int yPos = 10;

            if (allTimers.Count == 0)
            {
                var emptyLabel = new TextLabel
                {
                    Text = "No timers created yet",
                    Size2D = new Size2D(Window.Instance.WindowSize.Width, 50),
                    Position2D = new Position2D(0, 100),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextColor = new Color(0.6f, 0.6f, 0.6f, 1.0f),
                    PointSize = 20.0f
                };
                timerListContainer.Add(emptyLabel);
            }
            else
            {
                foreach (var timer in allTimers)
                {
                    CreateTimerListItem(yPos, timer.Id, timer.Name, timer.ElapsedSeconds, timer.TotalSeconds);
                    yPos += 100;
                }
            }

            // Add new timer button
            var addButton = new Button
            {
                Text = "+ Add New Timer",
                Size2D = new Size2D(Window.Instance.WindowSize.Width - 20, 60),
                Position2D = new Position2D(10, yPos + 20),
                BackgroundColor = new Color(0.2f, 0.8f, 0.4f, 1.0f),
                PointSize = 18.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            addButton.Clicked += (s, e) =>
            {
                OnTimerSelected?.Invoke(-1, "New Timer");
            };
            timerListContainer.Add(addButton);
        }

        private void CreateTimerListItem(int yPos, int timerId, string timerName, int elapsed, int total)
        {
            var itemView = new View
            {
                Size2D = new Size2D(Window.Instance.WindowSize.Width - 20, 90),
                Position2D = new Position2D(10, yPos),
                BackgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };

            // Add border effect
            var border = new View
            {
                Size2D = new Size2D(Window.Instance.WindowSize.Width - 20, 90),
                Position2D = new Position2D(0, 0),
                BackgroundColor = new Color(0.9f, 0.9f, 0.9f, 0.5f)
            };
            itemView.Add(border);

            // Timer Name
            var nameLabel = new TextLabel
            {
                Text = timerName,
                Size2D = new Size2D(Window.Instance.WindowSize.Width - 40, 30),
                Position2D = new Position2D(10, 10),
                PointSize = 20.0f,
                TextColor = new Color(0.2f, 0.2f, 0.2f, 1.0f)
            };
            itemView.Add(nameLabel);

            // Time remaining
            var timeLabel = new TextLabel
            {
                Text = $"{elapsed}s / {total}s",
                Size2D = new Size2D(Window.Instance.WindowSize.Width - 40, 25),
                Position2D = new Position2D(10, 45),
                PointSize = 14.0f,
                TextColor = new Color(0.6f, 0.6f, 0.6f, 1.0f)
            };
            itemView.Add(timeLabel);

            // Progress bar
            var progressBar = new View
            {
                Size2D = new Size2D((int)((float)(elapsed) / total * (Window.Instance.WindowSize.Width - 40)), 5),
                Position2D = new Position2D(10, 75),
                BackgroundColor = new Color(0.2f, 0.6f, 1.0f, 1.0f)
            };
            itemView.Add(progressBar);

            // Click handler
            itemView.TouchEvent += (s, e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Up)
                {
                    OnTimerSelected?.Invoke(timerId, timerName);
                }
                return true;
            };

            timerListContainer.Add(itemView);
        }
    }
}
