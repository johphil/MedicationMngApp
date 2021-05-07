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
            //Login if username & password is not null
            if (!string.IsNullOrWhiteSpace(PersistentSettings.UserName) && !string.IsNullOrWhiteSpace(PersistentSettings.PassWord))
            {
                ProceedLogin(PersistentSettings.UserName, PersistentSettings.PassWord);
            }

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

        private async void ProceedLogin(string uname, string pword)
        {
            IsBusy = true;
            try
            {
                if (NetworkStatus.IsInternet())
                {
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
                                    if (result.result > 0)
                                    {
                                        PersistentSettings.UserName = uname;
                                        PersistentSettings.PassWord = pword;
                                        PersistentSettings.AccountID = result.result;
                                        Common.NavigateNewPage(new MainPage());
                                    }
                                    else
                                        ErrorMessage = "Invalid username or password.";
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

        private void OnLoginClicked(object obj)
        {
            ProceedLogin(Username, password);
        }
    }
}
