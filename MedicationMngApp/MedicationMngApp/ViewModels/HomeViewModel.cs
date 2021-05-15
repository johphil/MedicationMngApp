using MedicationMngApp.Models;
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
    public class HomeViewModel : BaseViewModel
    {
        private bool isLoaded = false;
        public ObservableCollection<Med_Take_Today> UpcomingMedTakes { get; }
        public ObservableCollection<Account_Log> AccountLogs { get; }
        public Command LoadHomepage { get; }

        public HomeViewModel()
        {
            TitleToday = $"Medications for Today {DateTime.Today.ToShortDateString()}";
            UpcomingMedTakes = new ObservableCollection<Med_Take_Today>();
            AccountLogs = new ObservableCollection<Account_Log>();
            LoadHomepage = new Command(async () => await ExecuteLoadHomepage());
        }

        public string TitleToday { get; }

        public void OnAppearing()
        {
            if (!isLoaded)
                IsBusy = true;
        }

        private async Task ExecuteLoadHomepage()
        {
            IsBusy = true;
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
                                        isLoaded = true;
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
                                        isLoaded = true;
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
