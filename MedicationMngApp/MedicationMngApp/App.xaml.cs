using MedicationMngApp.Models;
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
            Common.NavigateNewPage(new LoginPage());
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
