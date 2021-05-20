using MedicationMngApp.Models;
using MedicationMngApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.Resources;
using XF.Material.Forms.Resources.Typography;

namespace MedicationMngApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            XF.Material.Forms.Material.Use("Material.Configuration");

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
