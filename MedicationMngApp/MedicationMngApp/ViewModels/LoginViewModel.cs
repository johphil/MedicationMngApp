using MedicationMngApp.Models;
using MedicationMngApp.Models;
using MedicationMngApp.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicationMngApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        
        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            if (NetworkStatus.IsInternet())
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_ACCOUNTS))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jData = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(jData))
                            {
                                GetAccountsResult accounts = JsonConvert.DeserializeObject<GetAccountsResult>(jData);
                            }
                        }
                    }
                }
            }
            else
            {
                await Common.ShowMessageAsync("Network Error", "Internet is unavailable. Please try again.", "OK");
            }
        }
    }
}
