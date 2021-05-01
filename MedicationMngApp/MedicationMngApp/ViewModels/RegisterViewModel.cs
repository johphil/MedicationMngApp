using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MedicationMngApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string firstname = string.Empty;
        private string lastname = string.Empty;
        private DateTime birthday = DateTime.Now;
        private string email = string.Empty;
        private string username = string.Empty;
        private string password = string.Empty;
        private string confirmpassword = string.Empty;

        private int MaxAge = 99;

        public List<int> Ages { get; }
        public Command RegisterCommand { get; }


        public RegisterViewModel()
        {
            RegisterCommand = new Command(OnRegisterClicked, ValidateRegistration);
            Ages = new List<int>();

            for (int i = 1; i <= MaxAge; i++)
                Ages.Add(i);
        }

        private bool ValidateRegistration(object obj)
        {
            return !String.IsNullOrWhiteSpace(firstname)
               && !String.IsNullOrWhiteSpace(lastname)
               && birthday != DateTime.MinValue
               && !String.IsNullOrWhiteSpace(email)
               && !String.IsNullOrWhiteSpace(username)
               && !String.IsNullOrWhiteSpace(password)
               && !String.IsNullOrWhiteSpace(confirmpassword)
               && password == confirmpassword;
        }

        public DateTime MaximumBirthday
        {
            get
            {
                return DateTime.Now;
            }
        }

        public DateTime MinimumBirthday
        {
            get
            {
                return DateTime.MinValue;
            }
        }

        public string FirstName
        {
            get => firstname;
            set => SetProperty(ref firstname, value, "ValidateRegistration");
        }

        public string LastName
        {
            get => lastname;
            set => SetProperty(ref lastname, value, "ValidateRegistration");
        }

        public DateTime Birthday
        {
            get => birthday;
            set => SetProperty(ref birthday, value, "ValidateRegistration");
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value, "ValidateRegistration");
        }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value, "ValidateRegistration");
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value, "ValidateRegistration");
        }

        public string ConfirmPassword
        {
            get => confirmpassword;
            set => SetProperty(ref confirmpassword, value, "ValidateRegistration");
        }

        private async void OnRegisterClicked(object obj)
        {
            if (ValidateRegistration(obj))
                await Application.Current.MainPage.DisplayAlert("Date of Birth", birthday.Date.ToShortDateString(), "OK PO");
            else
                await Application.Current.MainPage.DisplayAlert("Error", "Message Error", "OK");
        }
    }
}
