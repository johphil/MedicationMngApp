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
using XF.Material.Forms.UI.Dialogs;

namespace MedicationMngApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Med_Take_Today> UpcomingMedTakes { get; }
        public ObservableCollection<Account_Log> AccountLogs { get; }
        public Command LoadHomepage { get; }
        public Command CheckMedTakeCommand { get; }
        public Command ShowIntakeLogsCommand { get; }

        public HomeViewModel()
        {
            TitleToday = $"Medications for Today {DateTime.Today.ToShortDateString()}";
            UpcomingMedTakes = new ObservableCollection<Med_Take_Today>();
            AccountLogs = new ObservableCollection<Account_Log>();
            LoadHomepage = new Command(async () => await ExecuteLoadHomepage());
            CheckMedTakeCommand = new Command(OnMedTakeClicked);
            ShowIntakeLogsCommand = new Command(OnShowIntakeLogsClicked);
        }

        public string TitleToday { get; }

        private void OnShowIntakeLogsClicked(object obj)
        {
            Common.NavigatePage(new IntakeLogPage());
        }

        private async void OnMedTakeClicked(object obj)
        {
            var medtake = (Med_Take_Today)obj;
            if (!medtake.IsTaken)
            {
                bool cantake = true;
                if (medtake.IsTooEarly)
                    if (await Common.ShowAlertConfirmationWithButton("It's too early to take your medicine! Do you want to take it now?", "Yes", "No") == false)
                        cantake = false;

                if (cantake && await Common.ShowAlertConfirmationWithButton($"You should take {medtake.Dosage_Count} dosage(s) of {medtake.Med_Name}.", "OK", "No") == true)
                {
                    try
                    {
                        if (NetworkStatus.IsInternet())
                        {
                            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Taking Medicine...", configuration: Common.LoadingDialogConfig))
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    string v = Common.PUT_TAKE_MEDICINE(medtake.Med_Take_Schedule_ID, medtake.Med_Take_ID);
                                    using (HttpResponseMessage response = await client.PutAsync(Common.PUT_TAKE_MEDICINE(medtake.Med_Take_Schedule_ID, medtake.Med_Take_ID), null))
                                    {
                                        if (response.IsSuccessStatusCode)
                                        {
                                            var jData = await response.Content.ReadAsStringAsync();
                                            if (!string.IsNullOrWhiteSpace(jData))
                                            {
                                                TakeMedicineResult result = JsonConvert.DeserializeObject<TakeMedicineResult>(jData);
                                                switch(result.result)
                                                {
                                                    case 11: //Critical
                                                        {
                                                            await Common.ShowSnackbarMessage("Success!");
                                                            IsBusy = true;
                                                            Common.ShowNotification($"{medtake.Med_Name} Count is Critical!", $"{medtake.Med_Name} needs to be replenished.");
                                                            break;
                                                        }
                                                    case 22: //Not Critical
                                                        {
                                                            await Common.ShowSnackbarMessage("Success!");
                                                            IsBusy = true;
                                                            break;
                                                        }
                                                    default: //Error
                                                        {
                                                            break;
                                                        }
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
            else if (medtake.IsTaken)
            {
                await Common.ShowAlertAsync("Taken Already", "You have already taken this medicine!", "Ok");
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        private async Task ExecuteLoadHomepage()
        {
            try
            {
                if (NetworkStatus.IsInternet())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_MED_TAKE_TODAY(PersistentSettings.AccountID)))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var jData = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(jData))
                                {
                                    GetMedTakeTodayResult result = JsonConvert.DeserializeObject<GetMedTakeTodayResult>(jData);
                                    if (result != null)
                                    {
                                        UpcomingMedTakes.Clear();
                                        foreach (var item in result.results)
                                        {
                                            UpcomingMedTakes.Add(item);
                                        }
                                    }
                                }
                            }
                        }

                        using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_ACCOUNT_LOGS(PersistentSettings.AccountID)))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var jData = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(jData))
                                {
                                    GetAccountLogsResult result = JsonConvert.DeserializeObject<GetAccountLogsResult>(jData);
                                    if (result.results != null)
                                    {
                                        AccountLogs.Clear();
                                        foreach (var item in result.results)
                                        {
                                            AccountLogs.Add(item);
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
    }
}
