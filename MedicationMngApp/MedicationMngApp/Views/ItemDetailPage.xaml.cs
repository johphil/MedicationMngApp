using MedicationMngApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MedicationMngApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage(string ItemId)
        {
            InitializeComponent();
            ItemDetailViewModel idViewModel = new ItemDetailViewModel(ItemId);
            BindingContext = idViewModel;
        }
    }
}