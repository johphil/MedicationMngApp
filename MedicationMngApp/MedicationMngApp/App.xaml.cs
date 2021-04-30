using MedicationMngApp.Services;
using MedicationMngApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicationMngApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<ServiceItem>();
            var isLogged = Xamarin.Essentials.SecureStorage.GetAsync("isLogged").Result;
            if (isLogged == "1")
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new LoginPage();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
