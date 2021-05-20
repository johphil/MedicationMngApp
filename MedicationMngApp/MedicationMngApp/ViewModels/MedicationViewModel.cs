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
using XamEffects;
using XF.Material.Forms.UI.Dialogs;

namespace MedicationMngApp.ViewModels
{
    public class MedicationViewModel : BaseViewModel
    {
        private bool isSelectedMedTake = false;

        public ObservableCollection<Med_Take> MedTakes { get; set; }
        public Command LoadMedTakesCommand { get; }
        public Command AddMedTakeCommand { get; }
        public Command AddCommand { get; }
        public Command<Med_Take> MedTakeTapped { get; }
        public Command EnableMedTakeCommand { get; }

        public MedicationViewModel()
        {
            IsBusy = true;
            MedTakes = new ObservableCollection<Med_Take>();
            AddCommand = new Command(OnAddClicked);
            MedTakeTapped = new Command<Med_Take>(OnMedTakeSelected);
            LoadMedTakesCommand = new Command(async () => await ExecuteLoadMedTakesCommand());
            EnableMedTakeCommand = new Command(OnEnableMedTake);
        }

        private async void OnEnableMedTake(object obj)
        {
            Med_Take selectedMedTake = obj as Med_Take;
            if (selectedMedTake != null)
            {
                try
                {
                    if (NetworkStatus.IsInternet())
                    {
                        using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Processing...", configuration: Common.LoadingDialogConfig))
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Common.SERVICE_CREDENTIALS));

                                using (HttpResponseMessage response = await client.PutAsync(Common.PUT_UPDATE_MED_TAKE_STATUS(selectedMedTake.Med_Take_ID, Convert.ToInt32(!selectedMedTake.IsActive)), null))
                                {
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var jData = await response.Content.ReadAsStringAsync();
                                        if (!string.IsNullOrWhiteSpace(jData))
                                        {
                                            UpdateMedTakeEnableResult result = JsonConvert.DeserializeObject<UpdateMedTakeEnableResult>(jData);
                                            if (result.result > 0)
                                            {
                                                string message = String.Format("{0} updated!", selectedMedTake.Med_Name);
                                                await Common.ShowSnackbarMessage(message);
                                            }
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
            }
        }

        private void OnMedTakeSelected(Med_Take obj)
        {
            if (obj == null && isSelectedMedTake)
                return;

            isSelectedMedTake = true;
            Common.NavigatePage(new MedicationDetailPage(obj));
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
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Common.SERVICE_CREDENTIALS));

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
                                        {
                                            MedTakes.Add(item);
                                        }
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

        private void OnAddClicked()
        {
            Common.NavigatePage(new MedicationDetailPage());
        }

        public void OnAppearing()
        {
            isSelectedMedTake = false;
            IsBusy = true;
        }
    }
}
