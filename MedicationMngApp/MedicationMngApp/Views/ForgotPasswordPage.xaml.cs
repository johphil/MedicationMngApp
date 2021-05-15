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
	public partial class ForgotPasswordPage : ContentPage
	{
		public ForgotPasswordPage ()
		{
			InitializeComponent ();
			BindingContext = new ForgotPasswordViewModel();
		}

		protected override bool OnBackButtonPressed()
		{
			Common.NavigateNewPage(new LoginPage());
			return true;
		}
	}
}