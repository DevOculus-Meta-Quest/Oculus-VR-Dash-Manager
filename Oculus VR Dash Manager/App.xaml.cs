using System;
using System.Threading.Tasks;
using System.Windows;

namespace OVR_Dash_Manager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow mainWindow = new MainWindow();

            if (OVR_Dash_Manager.Properties.Settings.Default.ShowSplashScreen)
            {
                Splash splashScreen = new Splash();
                splashScreen.Show();

                // Use Dispatcher to handle the delay and closing of the splash screen
                Dispatcher.Invoke(async () =>
                {
                    await Task.Delay(7000);
                    splashScreen.Close();
                    mainWindow.Show();
                });
            }
            else
            {
                mainWindow.Show();
            }
        }
    }
}
