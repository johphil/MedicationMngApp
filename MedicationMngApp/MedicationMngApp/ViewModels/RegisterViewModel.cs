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

        private bool ValidateRegistration(object obj)
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

        private async void ValidateRegistrationMessage()
        {
            if (string.IsNullOrWhiteSpace(firstname))
                await Common.ShowMessageAsync("Invalid First Name", "Please enter a valid First Name.", "OK");
            else if (string.IsNullOrWhiteSpace(lastname))
                await Common.ShowMessageAsync("Invalid Last Name", "Please enter a valid Last Name.", "OK");
            else if (birthday == MinimumBirthday)
                await Common.ShowMessageAsync("Invalid Birthday", "Please select a valid Date of Birth.", "OK");
            else if (string.IsNullOrWhiteSpace(email))
                await Common.ShowMessageAsync("Invalid Email", "Please enter a valid Email.", "OK");
            else if (!(new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Match(email).Success))
                await Common.ShowMessageAsync("Invalid Email", "Please enter a valid Email.", "OK");
            else if (string.IsNullOrWhiteSpace(username))
                await Common.ShowMessageAsync("Invalid Username", "Please enter a valid Username.", "OK");
            else if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmpassword))
                await Common.ShowMessageAsync("Invalid Password", "Please enter a valid Password.", "OK");
            else if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(confirmpassword) && password != confirmpassword)
                await Common.ShowMessageAsync("Invalid Password", "Passwords do not match.", "OK");
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

        private async void OnRegisterClicked(object obj)
        {
            if (ValidateRegistration(obj))
            {
                if (NetworkStatus.IsInternet())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        AddAccountRequestObject accountWrapper = new AddAccountRequestObject 
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
                        string serializedObject = JsonConvert.SerializeObject(accountWrapper, Formatting.Indented);
                        using (HttpContent contentPost = new StringContent(serializedObject, Encoding.UTF8, Common.HEADER_CONTENT_TYPE))
                        {
                            using (HttpResponseMessage response = await client.PostAsync(Common.POST_ADD_ACCOUNT, contentPost))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    string jData = await response.Content.ReadAsStringAsync();
                                    if (!string.IsNullOrWhiteSpace(jData))
                                    {
                                        AddAccountResult result = JsonConvert.DeserializeObject<AddAccountResult>(jData);
                                        if (result.result > 0)
                                        {
                                            await Common.ShowMessageAsync("Registration Success", "You have successfully registered.", "OK");
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
                ValidateRegistrationMessage();
            }
        }
    }
}
