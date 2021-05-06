using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MedicationMngApp.Models
{
    public static class Common
    {

        #region Json URIs & Settings
        public static string HEADER_CONTENT_TYPE = "application/json";
        public static string SERVICE_IP = "192.168.222.111";//"192.168.88.250";// 
        public static string POST_ADD_ACCOUNT = $"http://{ SERVICE_IP }/MedicationMngWebAppServices/Service.svc/AddAccount";
        public static string GET_LOGIN_ACCOUNT(string username, string password)
        {
            return $"http://{ SERVICE_IP }/MedicationMngWebAppServices/Service.svc/LoginAccount/{username}/{password}";
        }
        public static string GET_GET_ACCOUNT_DETAILS(int id)
        {
            return $"http://{ SERVICE_IP }/MedicationMngWebAppServices/Service.svc/GetAccountDetails/{id}";
        }
        public static string PUT_UPDATE_ACCOUNT_PASSWORD = $"http://{ SERVICE_IP }/MedicationMngWebAppServices/Service.svc/UpdateAccountPassword";
        
        #endregion

        #region Asynchronous Tasks Message Popup & Navigation
        public static async Task ShowMessageAsync(string title, string msgBody, string buttonText)
        {
            await Application.Current.MainPage.DisplayAlert(title, msgBody, buttonText);
        }

        public static async Task ShowMessageAsyncNetworkError()
        {
            await Common.ShowMessageAsync("Network Error", "Internet is unavailable. Please try again.", "OK");
        }

        public static async Task ShowMessageAsyncApplicationError(string error)
        {
            await Common.ShowMessageAsync("Application Error", error, "Dismiss");
        }

        public static void NavigateNewPage(Page page)
        {
            Application.Current.MainPage = new NavigationPage(page);
        }

        public static async Task NavigatePage(Page page)
        {
            await Application.Current.MainPage.Navigation.PushAsync(page, true);
        }

        public static async Task NavigateBack()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        #endregion
    }
}
