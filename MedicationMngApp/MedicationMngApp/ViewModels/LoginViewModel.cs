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
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace MedicationMngApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string username = string.Empty;
        private string password = string.Empty;
        private bool iserrorusername = false;
        private bool iserrorpassword = false;

        public Command LoginCommand { get; }
        
        public LoginViewModel()
        {
            //Login if username & password is not null
            if (!string.IsNullOrWhiteSpace(PersistentSettings.UserName) && !string.IsNullOrWhiteSpace(PersistentSettings.PassWord))
            {
                Username = PersistentSettings.UserName;
                ProceedLogin(PersistentSettings.UserName, PersistentSettings.PassWord, true);
            }

            LoginCommand = new Command(OnLoginClicked);
        }

        #region Bindings
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
        public bool IsErrorUsername
        {
            get => iserrorusername;
            set => SetProperty(ref iserrorusername, value);
        }
        public bool IsErrorPassword
        {
            get => iserrorpassword;
            set => SetProperty(ref iserrorpassword, value);
        }
        #endregion //End Bindings

        #region Commands
        private void OnLoginClicked(object obj)
        {
            ProceedLogin(Username, password);
        }
        #endregion //Commands

        #region Functions
        private bool Validate(bool isPersistent)
        {
            if (isPersistent)
                return true;

            IsErrorUsername = false;
            IsErrorPassword = false;
            bool result = true;

            if (string.IsNullOrWhiteSpace(username))
            {
                IsErrorUsername = true;
                result = false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                IsErrorPassword = true;
                result = false;
            }

            return result;
        }
        private async void ProceedLogin(string uname, string pword, bool isPersistent = false)
        {
            if (CanSubmit)
            {
                if (Validate(isPersistent))
                {
                    IsBusy = true;
                    try
                    {
                        if (NetworkStatus.IsInternet())
                        {
                            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Signing you in...", configuration: Common.loadingDialogConfig))
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
        #endregion //End Functions
    }
}
