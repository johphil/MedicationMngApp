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
	public partial class MedicationDetailPage : ContentPage
	{
		MedicationDetailViewModel vm;
		public MedicationDetailPage()
		{
			InitializeComponent ();
			vm = new MedicationDetailViewModel();
			BindingContext = vm;
		}

		public MedicationDetailPage(Med_Take medtake)
		{
			InitializeComponent();
			vm = new MedicationDetailViewModel(medtake);
			BindingContext = vm;
		}

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
			ListView lv = (ListView)sender;
			lv.ScrollTo(lv.SelectedItem, ScrollToPosition.MakeVisible, true);
        }

        protected override async void OnAppearing()
        {
			await vm.InitializeAsync();
        }
    }
}