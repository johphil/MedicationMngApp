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
		public MedicationDetailPage ()
		{
			InitializeComponent ();
			BindingContext = new MedicationDetailViewModel();
		}
	}
}