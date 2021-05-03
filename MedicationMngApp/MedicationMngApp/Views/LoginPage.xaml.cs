using MedicationMngApp.Models;
using MedicationMngApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedicationMngApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Common.NavigatePage(new RegisterPage());
        }
    }
}