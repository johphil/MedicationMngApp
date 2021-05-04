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
            InitLogin();
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

        private async void ProceedLogin(string uname, string pword)
        {
            if (NetworkStatus.IsInternet())
            {
                IsBusy = true;
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(Common.GET_LOGIN_ACCOUNT(uname, pword)))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jData = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(jData))
                            {
                                LoginAccountResult result = JsonConvert.DeserializeObject<LoginAccountResult>(jData);
                                if (result.result == 2)
                                {
                                    PersistentSettings.UserName = uname;
                                    PersistentSettings.PassWord = pword;
                                    Application.Current.MainPage = new AppShell();
                                }
                                else
                                    await Common.ShowMessageAsync("Invalid Login", "Invalid username or password.", "OK");
                            }
                        }
                    }
                }
                IsBusy = false;
            }
            else
            {
                await Common.ShowMessageAsync("Network Error", "Internet is unavailable. Please try again.", "OK");
            }
        }

        private void OnLoginClicked(object obj)
        {
            ProceedLogin(Username, password);
        }

        private void InitLogin()
        {
            if (!string.IsNullOrWhiteSpace(PersistentSettings.UserName) &&
                !string.IsNullOrWhiteSpace(PersistentSettings.PassWord))
            {
                ProceedLogin(PersistentSettings.UserName, PersistentSettings.PassWord);
            }
        }
    }
}
