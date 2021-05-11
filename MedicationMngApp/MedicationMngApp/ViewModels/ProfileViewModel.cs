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
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;

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

        private bool editbuttonvisibility = true;
        private bool savebuttonvisibility = false;

        private bool isenabledfield = false;

        public Command LogoutCommand { get; }
        public Command LoadProfileCommand { get; }
        public Command ChangePasswordCommand { get; }
        public Command EditCommand { get; }
        public Command SaveCommand { get; }

        public ProfileViewModel()
        {
            LogoutCommand = new Command(OnLogoutClicked);
            ChangePasswordCommand = new Command(OnChangePasswordClicked);
            EditCommand = new Command(OnEditClicked);
            SaveCommand = new Command(OnSaveClicked);

            LoadProfile();
        }

        private void OnSaveClicked()
        {
            SaveProfile();
        }

        private void OnEditClicked()
        {
            EditButtonVisibility = false;
            SaveButtonVisibility = true;
            IsEnabledField = true;
        }

        #region Bindings
        //Fields
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

        //Errors
        private bool isErrorFirstName = false;
        private bool isErrorLastName = false;
        public bool IsErrorFirstName
        {
            get => isErrorFirstName;
            set => SetProperty(ref isErrorFirstName, value);
        }
        public bool IsErrorLastName
        {
            get => isErrorLastName;
            set => SetProperty(ref isErrorLastName, value);
        }

        //Others
        public bool IsEnabledField
        {
            get => isenabledfield;
            set => SetProperty(ref isenabledfield, value);
        }
        public bool EditButtonVisibility
        {
            get => editbuttonvisibility;
            set => SetProperty(ref editbuttonvisibility, value); 
        }
        public bool SaveButtonVisibility
        {
            get => savebuttonvisibility;
            set => SetProperty(ref savebuttonvisibility, value);
        }
        #endregion //Bindings

        public bool Validate()
        {
            ResetErrors();
            return !string.IsNullOrWhiteSpace(firstname)
                && !string.IsNullOrWhiteSpace(lastname);
        }

        private void ValidateMessage()
        {
            if (string.IsNullOrWhiteSpace(firstname))
            {
                IsErrorFirstName = true;
                ErrorMessage = "First name should not be empty.";
            }
            if (string.IsNullOrWhiteSpace(lastname))
            {
                IsErrorLastName = true;
                ErrorMessage = "Last name should not be empty.";
            }
        }

        void ResetErrors()
        {
            IsErrorFirstName = false;
            IsErrorLastName = false;
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

        public async void SaveProfile()
        {
            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Saving profile...", configuration: Common.loadingDialogConfig))
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
                                using (HttpClient client = new HttpClient())
                                {
                                    UpdateAccountDetailsRequstObject obj = new UpdateAccountDetailsRequstObject
                                    {
                                        account = new Account
                                        {
                                            Account_ID = PersistentSettings.AccountID,
                                            FirstName = firstname,
                                            LastName = lastname
                                        }
                                    };
                                    string serializedObject = JsonConvert.SerializeObject(obj, Formatting.Indented);
                                    using (HttpContent content = new StringContent(serializedObject, Encoding.UTF8, Common.HEADER_CONTENT_TYPE))
                                    {
                                        using (HttpResponseMessage response = await client.PutAsync(Common.PUT_UPDATE_ACCOUNT_DETAILS, content))
                                        {
                                            if (response.IsSuccessStatusCode)
                                            {
                                                string jData = await response.Content.ReadAsStringAsync();
                                                if (!string.IsNullOrWhiteSpace(jData))
                                                {
                                                    UpdateAccountPasswordResult result = JsonConvert.DeserializeObject<UpdateAccountPasswordResult>(jData);
                                                    if (result.result >= 0)
                                                    {
                                                        EditButtonVisibility = true;
                                                        SaveButtonVisibility = false;
                                                        IsEnabledField = false;
                                                        ErrorMessage = string.Empty;
                                                        await Common.ShowSnackbarMessage("Profile updated successfully!");
                                                    }
                                                    else
                                                        await Common.ShowMessageAsyncUnknownError();
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

        private void OnLogoutClicked(object obj)
        {
            PersistentSettings.UserName = string.Empty;
            PersistentSettings.PassWord = string.Empty;
            Common.NavigateNewPage(new LoginPage());
        }

        private void OnChangePasswordClicked(object obj)
        {
            Common.NavigatePage(new ChangePasswordPage());
        }
    }
}
