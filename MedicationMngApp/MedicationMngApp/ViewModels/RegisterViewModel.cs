using MedicationMngApp.Models;
using MedicationMngApp.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace MedicationMngApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string firstname = string.Empty;
        private string lastname = string.Empty;
        private DateTime? birthday = null;
        private string email = string.Empty;
        private string username = string.Empty;
        private string password = string.Empty;
        private string confirmpassword = string.Empty;

        public Command RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new Command(OnRegisterClicked);
        }

        private bool Validate()
        {
            ResetErrors();
            return !string.IsNullOrWhiteSpace(firstname)
            && !string.IsNullOrWhiteSpace(lastname)
            && birthday != null
            && !string.IsNullOrWhiteSpace(email)
            && new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Match(email).Success
            && !string.IsNullOrWhiteSpace(username)
            && !string.IsNullOrWhiteSpace(password)
            && !string.IsNullOrWhiteSpace(confirmpassword)
            && password == confirmpassword;
        }

        private void ValidateMessage()
        {
            if (string.IsNullOrWhiteSpace(firstname))
            {
                IsErrorFirstName = true;
                ErrorMessage = "Please enter a valid first name.";
            }
            if (string.IsNullOrWhiteSpace(lastname))
            {
                IsErrorLastName = true;
                ErrorMessage = "Please enter a valid last name.";
            }
            if (birthday == null)
            {
                IsErrorBirthday = true;
                ErrorMessage = "Please select a valid date of birth.";
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                IsErrorEmail = true;
                ErrorMessage = "Please enter a valid email.";
            }
            if (!(new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Match(email).Success))
            {
                IsErrorEmail = true;
                ErrorMessage = "Please enter a valid email format.";
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                IsErrorUsername = true;
                ErrorMessage = "Please enter a valid username.";
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                IsErrorPassword = true;
                ErrorMessage = "Please enter a valid password.";
            }
            if (string.IsNullOrWhiteSpace(confirmpassword))
            {
                IsErrorConfirmPassword = true;
                ErrorMessage = "Please enter a valid confirm password.";
            }
            if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(confirmpassword) && password != confirmpassword)
            {
                IsErrorPassword = true;
                IsErrorConfirmPassword = true;
                ErrorMessage = "Passwords do not match.";
            }
        }

        #region Bindings
        //Fields
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
        public DateTime? Birthday
        {
            get => birthday;
            set => SetProperty(ref birthday, value.Value);
        }
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
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
        public string ConfirmPassword
        {
            get => confirmpassword;
            set => SetProperty(ref confirmpassword, value);
        }

        //Errors
        private bool isErrorFirstName = false;
        private bool isErrorLastName = false;
        private bool isErrorBirthday = false;
        private bool isErrorEmail = false;
        private bool isErrorUsername = false;
        private bool isErrorPassword = false;
        private bool isErrorConfirmPassword = false;
        private string errorFirstName = string.Empty;
        private string errorLastName = string.Empty;
        private string errorBirthday = string.Empty;
        private string errorEmail = string.Empty;
        private string errorUsername = string.Empty;
        private string errorPassword = string.Empty;
        private string errorConfirmPassword = string.Empty;

        void ResetErrors()
        {
            IsErrorFirstName = false;
            IsErrorLastName = false;
            IsErrorBirthday = false;
            IsErrorEmail = false;
            IsErrorUsername = false;
            IsErrorPassword = false;
            IsErrorConfirmPassword = false;
        }
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
        public bool IsErrorBirthday
        {
            get => isErrorBirthday;
            set => SetProperty(ref isErrorBirthday, value);
        }
        public bool IsErrorEmail
        {
            get => isErrorEmail;
            set => SetProperty(ref isErrorEmail, value);
        }
        public bool IsErrorUsername
        {
            get => isErrorUsername;
            set => SetProperty(ref isErrorUsername, value);
        }
        public bool IsErrorPassword
        {
            get => isErrorPassword;
            set => SetProperty(ref isErrorPassword, value);
        }
        public bool IsErrorConfirmPassword
        {
            get => isErrorConfirmPassword;
            set => SetProperty(ref isErrorConfirmPassword, value);
        }

        public string ErrorFirstName
        {
            get => errorFirstName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorFirstName = false;
                else
                    IsErrorFirstName = true;

                SetProperty(ref errorFirstName, value);
            }
        }
        public string ErrorLastName
        {
            get => errorLastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorLastName = false;
                else
                    IsErrorLastName = true;

                SetProperty(ref errorLastName, value);
            }
        }
        public string ErrorBirthday
        {
            get => errorBirthday;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorBirthday = false;
                else
                    IsErrorBirthday = true;

                SetProperty(ref errorBirthday, value);
            }
        }
        public string ErrorEmail
        {
            get => errorEmail;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorEmail = false;
                else
                    IsErrorEmail = true;

                SetProperty(ref errorEmail, value);
            }
        }
        public string ErrorUsername
        {
            get => errorUsername;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorUsername = false;
                else
                    IsErrorUsername = true;

                SetProperty(ref errorUsername, value);
            }
        }
        public string ErrorPassword
        {
            get => errorPassword;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorPassword = false;
                else
                    IsErrorPassword = true;

                SetProperty(ref errorPassword, value);
            }
        }
        public string ErrorConfirmPassword
        {
            get => errorConfirmPassword;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    IsErrorConfirmPassword = false;
                else
                    IsErrorConfirmPassword = true;

                SetProperty(ref errorConfirmPassword, value);
            }
        }
        #endregion //Bindings


        private async void OnRegisterClicked()
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
                            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Signing you up...", configuration: Common.loadingDialogConfig))
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    AddAccountRequestObject accountObj = new AddAccountRequestObject
                                    {
                                        account = new Account
                                        {
                                            FirstName = firstname,
                                            LastName = lastname,
                                            Birthday = birthday.Value,
                                            Email = email,
                                            Username = username,
                                            Password = confirmpassword
                                        }
                                    };
                                    string serializedObject = JsonConvert.SerializeObject(accountObj, Formatting.Indented);
                                    using (HttpContent content = new StringContent(serializedObject, Encoding.UTF8, Common.HEADER_CONTENT_TYPE))
                                    {
                                        using (HttpResponseMessage response = await client.PostAsync(Common.POST_ADD_ACCOUNT, content))
                                        {
                                            if (response.IsSuccessStatusCode)
                                            {
                                                string jData = await response.Content.ReadAsStringAsync();
                                                if (!string.IsNullOrWhiteSpace(jData))
                                                {
                                                    AddAccountResult result = JsonConvert.DeserializeObject<AddAccountResult>(jData);
                                                    switch (result.result)
                                                    {
                                                        case -69://sql return value -69
                                                            {
                                                                ErrorMessage = "Username already exists!";
                                                                break;
                                                            }
                                                        case -70://sql return value -70
                                                            {
                                                                ErrorMessage = "Email already exists!";
                                                                break;
                                                            }
                                                        case 1://sql return value 1
                                                            {
                                                                await Common.ShowMessageAsync("Registration Success", "You have successfully registered.", "OK");
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
