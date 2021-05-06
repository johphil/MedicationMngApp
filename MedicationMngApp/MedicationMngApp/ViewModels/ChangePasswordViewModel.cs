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
                await Common.ShowMessageAsync("Invalid Password", "Please fill up all fields.", "OK");
            else if (newpassword != newpasswordconfirm)
                await Common.ShowMessageAsync("Invalid Password", "New Password and Confirm Password do not match.", "OK");
            else
                await Common.ShowMessageAsync("Something went wrong", "Error.", "Dismiss");
        }


        private async void OnChangePasswordClicked()
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
                            new_password = newpasswordconfirm
                        };
                        string serializedObject = JsonConvert.SerializeObject(updatePasswordObj, Formatting.Indented);
                        using (HttpContent contentPost = new StringContent(serializedObject, Encoding.UTF8, Common.HEADER_CONTENT_TYPE))
                        {
                            using (HttpResponseMessage response = await client.PutAsync(Common.PUT_UPDATE_ACCOUNT_PASSWORD, contentPost))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    string jData = await response.Content.ReadAsStringAsync();
                                    if (!string.IsNullOrWhiteSpace(jData))
                                    {
                                        UpdateAccountPasswordResult result = JsonConvert.DeserializeObject<UpdateAccountPasswordResult>(jData);
                                        if (result.result > 0)
                                        {
                                            await Common.ShowMessageAsync("Change Password Success", "You have successfully changed your password.", "OK");
                                            await Common.NavigateBack();
                                        }
                                    }
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
            else
            {
                ValidateMessage();
            }
        }
    }
}
