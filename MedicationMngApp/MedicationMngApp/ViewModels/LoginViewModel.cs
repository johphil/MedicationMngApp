using MedicationMngApp.Models;
using MedicationMngApp.Models;
using MedicationMngApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MedicationMngApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        
        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");

            //Check network status   
            if (NetworkStatus.IsInternet())
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync("http://10.0.2.2/MedicationMngWebAppServices/Service.svc/GetAccounts"))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string accountsJson = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrWhiteSpace(accountsJson))
                            {
                                List<Account> accounts = new List<Account>();
                                //Converting JSON Array Objects into generic list  
                                JsonConvert.PopulateObject(accountsJson, accounts);
                            }
                        }
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Network Error", "Internet is unavailable. Please try again.", "OK");
            }
        }
    }
}
