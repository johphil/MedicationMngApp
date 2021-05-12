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
        public ObservableCollection<Med_Take_Upcoming> UpcomingMedTakes { get; }
        public Command LoadHomepage { get; }

        public HomeViewModel()
        {
            TitleToday = $"Upcoming Medications {DateTime.Today.DayOfWeek.ToString()}";
            UpcomingMedTakes = new ObservableCollection<Med_Take_Upcoming>();
            LoadHomepage = new Command(async () => await ExecuteLoadHomepage());
        }

        public string TitleToday { get; }

        public void OnAppearing()
        {
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
                        using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_MED_TAKE_UPCOMING(PersistentSettings.AccountID)))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var jData = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(jData))
                                {
                                    GetMedTakeUpcomingResult result = JsonConvert.DeserializeObject<GetMedTakeUpcomingResult>(jData);
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
