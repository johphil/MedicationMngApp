using MedicationMngApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace MedicationMngApp.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        private string oldpassword = string.Empty;
        private string newpassword = string.Empty;
        private string newpasswordconfirm = string.Empty;

        public Command ChangePasswordCommand { get; }

        public ChangePasswordViewModel()
        {
            ChangePasswordCommand = new Command(OnChangePasswordClicked);
        }

        #region Bindings
        public string OldPassword
        {
            get => oldpassword;
            set => SetProperty(ref oldpassword, value);
        }
        public string NewPassword
        {
            get => newpassword;
            set => SetProperty(ref newpassword, value);
        }
        public string NewPasswordConfirm
        {
            get => newpasswordconfirm;
            set => SetProperty(ref newpasswordconfirm, value);
        }

        //Errors
        private bool iserroroldpassword = false;
        private bool iserrornewpassword = false;
        private bool iserrorconfirmpassword = false;
        private string erroroldpassword = string.Empty;
        private string errornewpassword = string.Empty;
        private string errorconfirmpassword = string.Empty;

        void ResetErrors()
        {
            IsErrorOldPassword = false;
            IsErrorNewPassword = false;
            IsErrorConfirmPassword = false;
        }
        public bool IsErrorOldPassword
        {
            get => iserroroldpassword;
            set => SetProperty(ref iserroroldpassword, value);
        }
        public bool IsErrorNewPassword
        {
            get => iserrornewpassword;
            set => SetProperty(ref iserrornewpassword, value);
        }
        public bool IsErrorConfirmPassword
        {
            get => iserrorconfirmpassword;
            set => SetProperty(ref iserrorconfirmpassword, value);
        }
        public string ErrorOldPassword
        {
            get => erroroldpassword;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorOldPassword = false;
                else
                    IsErrorOldPassword = true;

                SetProperty(ref erroroldpassword, value);
            }
        }
        public string ErrorNewPassword
        {
            get => errornewpassword;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorNewPassword = false;
                else
                    IsErrorNewPassword = true;

                SetProperty(ref errornewpassword, value);
            }
        }
        public string ErrorConfirmPassword
        {
            get => errorconfirmpassword;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorConfirmPassword = false;
                else
                    IsErrorConfirmPassword = true;

                SetProperty(ref errorconfirmpassword, value);
            }
        }

        #endregion //Bindings

        private bool Validate()
        {
            ResetErrors();
            return !string.IsNullOrWhiteSpace(oldpassword)
            && !string.IsNullOrWhiteSpace(newpassword)
            && !string.IsNullOrWhiteSpace(newpasswordconfirm)
            && newpassword == newpasswordconfirm;
        }

        private void ValidateMessage()
        {
            if (string.IsNullOrWhiteSpace(oldpassword))
            {
                ErrorOldPassword = "Fill this up.";
            }
            if (string.IsNullOrWhiteSpace(newpassword))
            {
                ErrorNewPassword = "Fill this up.";
            }
            if (string.IsNullOrWhiteSpace(newpasswordconfirm))
            {
                ErrorConfirmPassword = "Fill this up.";
            }
            else if (newpassword != newpasswordconfirm)
            {
                ErrorConfirmPassword = "New password and confirm password does not match.";
            }
        }


        private async void OnChangePasswordClicked()
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
                            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Changing Password...", configuration: Common.LoadingDialogConfig))
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Common.SERVICE_CREDENTIALS));

                                    UpdateAccountPasswordRequestObject obj = new UpdateAccountPasswordRequestObject
                                    {
                                        account_id = PersistentSettings.AccountID,
                                        old_password = oldpassword,
                                        new_password = newpasswordconfirm
                                    };
                                    string serializedObject = JsonConvert.SerializeObject(obj, Formatting.Indented);
                                    using (HttpContent content = new StringContent(serializedObject, Encoding.UTF8, Common.HEADER_CONTENT_TYPE))
                                    {
                                        using (HttpResponseMessage response = await client.PutAsync(Common.PUT_UPDATE_ACCOUNT_PASSWORD, content))
                                        {
                                            if (response.IsSuccessStatusCode)
                                            {
                                                string jData = await response.Content.ReadAsStringAsync();
                                                if (!string.IsNullOrWhiteSpace(jData))
                                                {
                                                    UpdateAccountPasswordResult result = JsonConvert.DeserializeObject<UpdateAccountPasswordResult>(jData);
                                                    switch (result.result)
                                                    {
                                                        case -69: //sql return value -69
                                                            {
                                                                ErrorOldPassword = "Current password is invalid.";
                                                                break;
                                                            }
                                                        case 1: //sql return value 1
                                                            {
                                                                await Common.ShowSnackbarMessage("Change password success!");
                                                                await Common.NavigateBack();
                                                                break;
                                                            }
                                                        default:
                                                            {
                                                                await Common.ShowMessageAsyncUnknownError();
                                                                break;
                                                            }

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
    }
}
