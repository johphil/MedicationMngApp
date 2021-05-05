using MedicationMngApp.Models;
using MedicationMngApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MedicationMngApp.Utils;

namespace MedicationMngApp.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private string username = string.Empty;
        private string email = string.Empty;
        private string firstname= string.Empty;
        private string lastname = string.Empty;
        private DateTime birthday = DateTime.MinValue;
        private DateTime date_registered = DateTime.MinValue;

        public Command LogoutCommand { get; }
        public Command LoadProfileCommand { get; }
        public Command ChangePasswordCommand { get; }
        public ProfileViewModel()
        {
            LogoutCommand = new Command(OnLogoutClicked);
            ChangePasswordCommand = new Command(OnChangePasswordClicked);

            LoadProfile();
        }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }
        public string FirstName
        {
            get => firstname;
            set => SetProperty(ref firstname, value);
        }
        public string LastName
        {
            get => lastname;
            set => SetProperty(ref lastname, value);
        }
        public string Birthday
        {
            get => birthday.ToDateOnly();
            set => SetProperty(ref birthday, value.ToDateTime());
        }
        public string Date_Registered
        {
            get => date_registered.ToDateWithTime();
            set => SetProperty(ref date_registered, value.ToDateTime());
        }

        public async void LoadProfile()
        {
            IsBusy = true;
            try
            {
                if (NetworkStatus.IsInternet())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_ACCOUNT_DETAILS(PersistentSettings.AccountID)))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var jData = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(jData))
                                {
                                    GetAccountDetailsResult result = JsonConvert.DeserializeObject<GetAccountDetailsResult>(jData);
                                    Username = result.account.Username;
                                    Email = result.account.Email;
                                    FirstName = result.account.FirstName;
                                    LastName = result.account.LastName;
                                    Birthday = result.account.Birthday.ToDateOnly();
                                    Date_Registered = result.account.Date_Registered.ToDateWithTime();
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

        private void OnLogoutClicked(object obj)
        {
            PersistentSettings.UserName = string.Empty;
            PersistentSettings.PassWord = string.Empty;
            Common.NavigateNewPage(new LoginPage());
        }

        private void OnChangePasswordClicked(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
