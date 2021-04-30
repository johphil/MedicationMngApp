using MedicationMngApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MedicationMngApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}