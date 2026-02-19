using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;
using System;
using System.Collections.Generic;

namespace TimerUI.Pages
{
    public class MainWindow : Window
    {
        private MainPage mainPage;
        private TimerListPage timerListPage;
        private TimerRunningPage timerRunningPage;
        private TimerFinishPage timerFinishPage;
        private View currentPage;

        public MainWindow() : base()
        {
            BackgroundColor = new Color(0.95f, 0.95f, 0.95f, 1.0f);
            
            InitializePages();
        }

        private void InitializePages()
        {
            mainPage = new MainPage();
            timerListPage = new TimerListPage();
            timerRunningPage = new TimerRunningPage();
            timerFinishPage = new TimerFinishPage();
            
            // Start with main page
            this.Add(mainPage);
            
            // Setup navigation callbacks
            mainPage.OnNavigateToListPage += () => NavigateToPage(timerListPage);
            mainPage.OnNavigateToCreate += () => NavigateToPage(timerRunningPage);
            
            timerListPage.OnNavigateToMain += () => NavigateToPage(mainPage);
            timerListPage.OnTimerSelected += (timerId, timerName) => 
            {
                timerRunningPage.SetTimer(timerId, timerName);
                NavigateToPage(timerRunningPage);
            };
            
            timerRunningPage.OnNavigateToList += () => NavigateToPage(timerListPage);
            timerRunningPage.OnTimerComplete += (timerId, timerName) => 
            {
                timerFinishPage.SetTimerInfo(timerId, timerName);
                NavigateToPage(timerFinishPage);
            };
            
            timerFinishPage.OnDismiss += () => NavigateToPage(timerListPage);
            timerFinishPage.OnReset += (timerId) => 
            {
                timerRunningPage.SetTimer(timerId, "");
                NavigateToPage(timerRunningPage);
            };
        }

        private void NavigateToPage(View page)
        {
            // Remove current page
            if (currentPage != null)
            {
                this.Remove(currentPage);
            }
            
            // Add new page
            currentPage = page;
            this.Add(page);
        }
    }
}
