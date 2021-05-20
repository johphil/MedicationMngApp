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
	public partial class IntakeLogPage : ContentPage
	{
        readonly IntakeLogViewModel _viewModel;

		public IntakeLogPage()
		{
			InitializeComponent();
			BindingContext = _viewModel = new IntakeLogViewModel();
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();
			_viewModel.OnAppearing();
		}
	}
}