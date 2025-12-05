using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Urenregistratie_Applicatie.Services;  

namespace Urenregistratie_Applicatie
{
    public partial class App : Application
    {
        public App(DatabaseService dbService)
        {
            InitializeComponent();

            // Start asynchrone seeding zonder blokkering
            _ = dbService.InitAsync();

            // Globale foutafhandeling
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            MainPage = new AppShell();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                ShowGlobalError(ex);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            ShowGlobalError(e.Exception);
            e.SetObserved();
        }

        private void ShowGlobalError(Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await MainPage.DisplayAlert("Onverwachte fout", ex.Message, "OK");
            });
        }
    }
}
