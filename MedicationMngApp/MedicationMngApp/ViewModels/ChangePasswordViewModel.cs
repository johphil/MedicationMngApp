using MedicationMngApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

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

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(oldpassword)
            && !string.IsNullOrWhiteSpace(newpassword)
            && !string.IsNullOrWhiteSpace(newpasswordconfirm)
            && newpassword == newpasswordconfirm;
        }

        private async void ValidateMessage()
        {
            if (string.IsNullOrWhiteSpace(oldpassword) || string.IsNullOrWhiteSpace(newpassword) || string.IsNullOrWhiteSpace(newpasswordconfirm))
                ErrorMessage = "Please fill up all fields.";
            else if (newpassword != newpasswordconfirm)
                ErrorMessage = "New Password and Confirm Password do not match.";
            else
                await Common.ShowMessageAsyncUnknownError();
        }


        private async void OnChangePasswordClicked()
        {
            IsBusy = true;
            try
            {
                if (Validate())
                {
                    if (NetworkStatus.IsInternet())
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            UpdateAccountPasswordRequestObject updatePasswordObj = new UpdateAccountPasswordRequestObject
                            {
                                account_id = PersistentSettings.AccountID,
                                old_password = oldpassword,
                                new_password = newpasswordconfirm
                            };
                            string serializedObject = JsonConvert.SerializeObject(updatePasswordObj, Formatting.Indented);
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
                                                        ErrorMessage = "Old password do not match with the old one.";
                                                        break;
                                                    }
                                                case 1: //sql return value 1
                                                    {
                                                        await Common.ShowMessageAsync("Change Password Success", "You have successfully changed your password.", "OK");
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
