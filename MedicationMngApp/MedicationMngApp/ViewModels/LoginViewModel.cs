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
        private string username = string.Empty;
        private string password = string.Empty;

        public Command LoginCommand { get; }
        
        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private async void OnLoginClicked(object obj)
        {
            if (NetworkStatus.IsInternet())
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(Common.GET_LOGIN_ACCOUNT(username, password)))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jData = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(jData))
                            {
                                LoginAccountResult result = JsonConvert.DeserializeObject<LoginAccountResult>(jData);
                                if (result.result == 2)
                                {
                                    await Common.ShowMessageAsync("Welcome!", "You will be logged in now.", "OK");
                                    Application.Current.MainPage = new NavigationPage(new AboutPage());
                                }
                                else
                                    await Common.ShowMessageAsync("Invalid Login", "Invalid username or password.", "OK");
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
