using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.Components;
using TimerUI.Pages;
using TimerService;

namespace TimerUI
{
    class Program : NUIApplication
    {
        private MainWindow mainWindow;
        private TimerManager timerManager;

        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
            timerManager = TimerManager.Instance;
            mainWindow = new MainWindow();
            Window.Instance.KeyEvent += OnKeyEvent;
        }

        private void Initialize()
        {
            // Initialize any required resources
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && e.Key.KeyPressedName == "XF86Back")
            {
                this.Exit();
            }
        }

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
