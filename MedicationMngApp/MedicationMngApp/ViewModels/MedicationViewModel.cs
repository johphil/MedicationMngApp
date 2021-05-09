using MedicationMngApp.Models;
using MedicationMngApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MedicationMngApp.ViewModels
{
    public class MedicationViewModel : BaseViewModel
    {
        private Med_Take selectedMedTake;

        public ObservableCollection<Med_Take> MedTakes { get; set; }
        public Command LoadMedTakesCommand { get; }
        public Command AddMedTakeCommand { get; }
        public Command AddCommand { get; }
        public Command<Med_Take> MedTakeTapped { get; }

        public MedicationViewModel()
        {
            IsBusy = true;
            selectedMedTake = null;
            MedTakes = new ObservableCollection<Med_Take>();
            AddCommand = new Command(OnAddClicked);
            MedTakeTapped = new Command<Med_Take>(OnMedTakeSelected);
            LoadMedTakesCommand = new Command(async () => await ExecuteLoadMedTakesCommand());
        }

        private async void OnMedTakeSelected(Med_Take obj)
        {
            if (obj == null)
                return;

            await Common.NavigatePage(new MedicationDetailPage(obj));
        }

        private async Task ExecuteLoadMedTakesCommand()
        {
            IsBusy = true;
            try
            {
                if (NetworkStatus.IsInternet())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_MED_TAKES(PersistentSettings.AccountID)))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var jData = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(jData))
                                {
                                    GetMedTakesResult result = JsonConvert.DeserializeObject<GetMedTakesResult>(jData);
                                    if (result != null)
                                    {
                                        MedTakes.Clear();
                                        foreach (var item in result.results)
                                            MedTakes.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    await Common.ShowMessageAsyncNetworkError();
                }
            }
            catch (Exception error)
            {
                await Common.ShowMessageAsyncApplicationError(error.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnAddClicked()
        {
            await Common.NavigatePage(new MedicationDetailPage());
        }
    }
}
