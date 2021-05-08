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

namespace MedicationMngApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string firstname = string.Empty;
        private string lastname = string.Empty;
        private DateTime birthday;
        private string email = string.Empty;
        private string username = string.Empty;
        private string password = string.Empty;
        private string confirmpassword = string.Empty;
        
        private int MaxAge = 100;
        private int MinAge = 10;

        public Command RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new Command(OnRegisterClicked);
            birthday = MinimumBirthday;
        }

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(firstname)
            && !string.IsNullOrWhiteSpace(lastname)
            && birthday != MinimumBirthday
            && !string.IsNullOrWhiteSpace(email)
            && new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Match(email).Success
            && !string.IsNullOrWhiteSpace(username)
            && !string.IsNullOrWhiteSpace(password)
            && !string.IsNullOrWhiteSpace(confirmpassword)
            && password == confirmpassword;
        }

        private async void ValidateMessage()
        {
            if (string.IsNullOrWhiteSpace(firstname))
                ErrorMessage = "Please enter a valid first name.";
            else if (string.IsNullOrWhiteSpace(lastname))
                ErrorMessage = "Please enter a valid last name.";
            else if (birthday == MinimumBirthday)
                ErrorMessage = "Please select a valid date of birth.";
            else if (string.IsNullOrWhiteSpace(email))
                ErrorMessage = "Please enter a valid email.";
            else if (!(new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Match(email).Success))
                ErrorMessage = "Please enter a valid email format.";
            else if (string.IsNullOrWhiteSpace(username))
                ErrorMessage = "Please enter a valid username.";
            else if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmpassword))
                ErrorMessage = "Please enter a valid password.";
            else if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(confirmpassword) && password != confirmpassword)
                ErrorMessage = "Passwords do not match.";
            else
                await Common.ShowMessageAsync("Something went wrong", "Error.", "Dismiss");
        }

        public DateTime MaximumBirthday
        {
            get
            {
                return DateTime.Now.Date.AddYears(-MinAge);
            }
        }

        public DateTime MinimumBirthday
        {
            get
            {
                return DateTime.Now.Date.AddYears(-MaxAge);
            }
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

        public DateTime Birthday
        {
            get => birthday;
            set => SetProperty(ref birthday, value);
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
                            using (HttpClient client = new HttpClient())
                            {
                                AddAccountRequestObject accountObj = new AddAccountRequestObject
                                {
                                    account = new Account
                                    {
                                        FirstName = firstname,
                                        LastName = lastname,
                                        Birthday = birthday,
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
