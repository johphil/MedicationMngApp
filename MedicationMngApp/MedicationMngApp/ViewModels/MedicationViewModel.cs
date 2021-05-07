using MedicationMngApp.Models;
using MedicationMngApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace MedicationMngApp.ViewModels
{
    public class MedicationViewModel : BaseViewModel
    {
        private Med_Take SelectedMedTake;

        public ObservableCollection<Med_Take> MedTakes { get; }

        public Command LoadMedTakesCommand { get; }
        public Command AddMedTakeCommand { get; }
        public Command AddCommand { get; }
        public Command<Item> MedTakeTapped { get; }

        public MedicationViewModel()
        {
            AddCommand = new Command(OnAddClicked);
        }

        private async void OnAddClicked()
        {
            await Common.NavigatePage(new MedicationDetailPage());
        }
    }
}
