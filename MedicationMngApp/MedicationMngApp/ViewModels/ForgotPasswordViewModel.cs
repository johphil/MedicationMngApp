using MedicationMngApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace MedicationMngApp.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private string email = string.Empty;
        private bool iserroremail = false;
        
        public Command ForgotPasswordCommand { get; }

        public ForgotPasswordViewModel()
        {
            ForgotPasswordCommand = new Command(OnForgotPasswordClicked);
        }

        #region Bindings
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        //Errors
        public bool IsErrorEmail
        {
            get => iserroremail;
            set => SetProperty(ref iserroremail, value);
        }
        #endregion //End Bindings

        #region Commands
        private async void OnForgotPasswordClicked()
        {
            if (CanSubmit)
            {
                IsBusy = true;
                try
                {
                    if (Validate())
                    {
                        if (NetworkStatus.IsInternet())
                        {
                            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Retrieving account...", configuration: Common.loadingDialogConfig))
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_ACCOUNT_PASSWORD(email)))
                                    {
                                        if (response.IsSuccessStatusCode)
                                        {
                                            var jData = await response.Content.ReadAsStringAsync();
                                            if (!string.IsNullOrWhiteSpace(jData))
                                            {
                                                GetAccountPasswordResult result = JsonConvert.DeserializeObject<GetAccountPasswordResult>(jData);
                                                if (!string.IsNullOrWhiteSpace(result.result))
                                                {
                                                    await Common.ShowAlertAsync("Here's your password", result.result, "GOT IT");
                                                }
                                            }
                                            else
                                            {
                                                await Common.ShowAlertAsync("Really?", "The email you provided does not exist.", "TRY AGAIN", true);
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
                    else
                    {
                        ValidateMessage();
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
        #endregion //End Commands

        #region Functions
        private bool Validate()
        {
            IsErrorEmail = false;
            return !string.IsNullOrWhiteSpace(email)
                && new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Match(email).Success;
        }
        private void ValidateMessage()
        {
            if (string.IsNullOrWhiteSpace(email) || !(new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Match(email).Success))
                IsErrorEmail = true;
        }
        #endregion //End Functions
    }
}
