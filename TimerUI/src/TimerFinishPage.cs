using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;
using System;

namespace TimerUI.Pages
{
    public class TimerFinishPage : View
    {
        public event Action OnDismiss;
        public event Action<int> OnReset;

        private int currentTimerId = -1;
        private string currentTimerName = "";
        private TextLabel timerNameLabel;
        private TextLabel finishMessageLabel;

        public TimerFinishPage() : base()
        {
            Size2D = new Size2D(Window.Instance.WindowSize.Width, Window.Instance.WindowSize.Height);
            BackgroundColor = new Color(0.95f, 0.95f, 0.95f, 1.0f);
            
            CreateUI();
        }

        private void CreateUI()
        {
            // Background overlay for completion
            var overlayView = new View
            {
                Size2D = new Size2D(Window.Instance.WindowSize.Width, Window.Instance.WindowSize.Height),
                Position2D = new Position2D(0, 0),
                BackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.3f)
            };
            Add(overlayView);

            // Completion popup
            var popupView = new View
            {
                Size2D = new Size2D(300, 400),
                Position2D = new Position2D(
                    (Window.Instance.WindowSize.Width - 300) / 2,
                    (Window.Instance.WindowSize.Height - 400) / 2
                ),
                BackgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            Add(popupView);

            // Completion icon/indicator
            var completionIndicator = new View
            {
                Size2D = new Size2D(80, 80),
                Position2D = new Position2D((300 - 80) / 2, 20),
                BackgroundColor = new Color(0.2f, 0.8f, 0.4f, 1.0f)
            };
            popupView.Add(completionIndicator);

            // Completion label
            var completionLabel = new TextLabel
            {
                Text = "✓",
                Size2D = new Size2D(80, 80),
                Position2D = new Position2D((300 - 80) / 2, 20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                PointSize = 50.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            popupView.Add(completionLabel);

            // Timer finished message
            var messageLabel = new TextLabel
            {
                Text = "Timer Complete!",
                Size2D = new Size2D(280, 50),
                Position2D = new Position2D(10, 110),
                HorizontalAlignment = HorizontalAlignment.Center,
                PointSize = 24.0f,
                TextColor = new Color(0.2f, 0.2f, 0.2f, 1.0f)
            };
            popupView.Add(messageLabel);

            // Timer name
            timerNameLabel = new TextLabel
            {
                Text = "Timer Name",
                Size2D = new Size2D(280, 40),
                Position2D = new Position2D(10, 160),
                HorizontalAlignment = HorizontalAlignment.Center,
                PointSize = 18.0f,
                TextColor = new Color(0.6f, 0.6f, 0.6f, 1.0f)
            };
            popupView.Add(timerNameLabel);

            // Finish message
            finishMessageLabel = new TextLabel
            {
                Text = "Time's up!",
                Size2D = new Size2D(280, 60),
                Position2D = new Position2D(10, 210),
                HorizontalAlignment = HorizontalAlignment.Center,
                PointSize = 16.0f,
                TextColor = new Color(0.4f, 0.4f, 0.4f, 1.0f),
                MultiLine = true
            };
            popupView.Add(finishMessageLabel);

            // Dismiss button
            var dismissButton = new Button
            {
                Text = "Dismiss",
                Size2D = new Size2D(130, 50),
                Position2D = new Position2D(10, 290),
                BackgroundColor = new Color(0.8f, 0.2f, 0.2f, 1.0f),
                PointSize = 16.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            dismissButton.Clicked += (s, e) =>
            {
                OnDismiss?.Invoke();
            };
            popupView.Add(dismissButton);

            // Reset button
            var resetButton = new Button
            {
                Text = "Reset",
                Size2D = new Size2D(130, 50),
                Position2D = new Position2D(150, 290),
                BackgroundColor = new Color(0.2f, 0.6f, 1.0f, 1.0f),
                PointSize = 16.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            resetButton.Clicked += (s, e) =>
            {
                OnReset?.Invoke(currentTimerId);
            };
            popupView.Add(resetButton);
        }

        public void SetTimerInfo(int timerId, string timerName)
        {
            currentTimerId = timerId;
            currentTimerName = timerName;
            timerNameLabel.Text = timerName;
        }
    }
}
