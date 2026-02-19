using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;
using System;

namespace TimerUI.Pages
{
    public class MainPage : View
    {
        public event Action OnNavigateToListPage;
        public event Action OnNavigateToCreate;

        private View contentView;
        private Button createNewTimerButton;
        private Button myTimersButton;

        public MainPage() : base()
        {
            Size2D = new Size2D(Window.Instance.WindowSize.Width, Window.Instance.WindowSize.Height);
            BackgroundColor = new Color(0.95f, 0.95f, 0.95f, 1.0f);
            
            CreateUI();
        }

        private void CreateUI()
        {
            // Title
            var titleText = new TextLabel
            {
                Text = "Timer App",
                Size2D = new Size2D(Window.Instance.WindowSize.Width, 80),
                Position2D = new Position2D(0, 30),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                PointSize = 48.0f,
                TextColor = new Color(0.2f, 0.2f, 0.2f, 1.0f)
            };
            Add(titleText);

            // Create content view
            contentView = new View
            {
                Size2D = new Size2D(Window.Instance.WindowSize.Width, Window.Instance.WindowSize.Height - 120),
                Position2D = new Position2D(0, 120)
            };
            Add(contentView);

            // Create New Timer Button
            createNewTimerButton = new Button
            {
                Text = "Create New Timer",
                Size2D = new Size2D(300, 80),
                Position2D = new Position2D((Window.Instance.WindowSize.Width - 300) / 2, 100),
                BackgroundColor = new Color(0.2f, 0.6f, 1.0f, 1.0f),
                PointSize = 20.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            createNewTimerButton.Clicked += (s, e) =>
            {
                OnNavigateToCreate?.Invoke();
            };
            contentView.Add(createNewTimerButton);

            // My Timers Button
            myTimersButton = new Button
            {
                Text = "My Timers",
                Size2D = new Size2D(300, 80),
                Position2D = new Position2D((Window.Instance.WindowSize.Width - 300) / 2, 200),
                BackgroundColor = new Color(0.2f, 0.8f, 0.4f, 1.0f),
                PointSize = 20.0f,
                TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            };
            myTimersButton.Clicked += (s, e) =>
            {
                OnNavigateToListPage?.Invoke();
            };
            contentView.Add(myTimersButton);

            // Predefined Timers Label
            var predefinedLabel = new TextLabel
            {
                Text = "Predefined Timers",
                Size2D = new Size2D(Window.Instance.WindowSize.Width, 50),
                Position2D = new Position2D(0, 300),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = new Color(0.3f, 0.3f, 0.3f, 1.0f),
                PointSize = 24.0f
            };
            contentView.Add(predefinedLabel);

            // Predefined Timer Buttons - get from service
            var predefinedTimers = TimerService.TimerManager.Instance.GetPredefinedTimers();

            int yPos = 360;
            foreach (var timer in predefinedTimers)
            {
                var btn = new Button
                {
                    Text = timer.Name,
                    Size2D = new Size2D(280, 50),
                    Position2D = new Position2D((Window.Instance.WindowSize.Width - 280) / 2, yPos),
                    BackgroundColor = new Color(0.8f, 0.8f, 0.8f, 1.0f),
                    PointSize = 16.0f,
                    TextColor = new Color(0.2f, 0.2f, 0.2f, 1.0f)
                };
                
                var timerData = timer;
                btn.Clicked += (s, e) =>
                {
                    // Create predefined timer
                    int timerId = TimerService.TimerManager.Instance.CreateTimerFromPredefined(timerData.Name);
                    OnNavigateToCreate?.Invoke();
                };
                
                contentView.Add(btn);
                yPos += 60;
            }
        }
    }
}
