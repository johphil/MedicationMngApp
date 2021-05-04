using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MedicationMngApp.Models
{
    public static class Common
    {
        #region Json URIs
        public static string SERVICE_IP = "192.168.88.250";//"192.168.222.105";//
        public static string POST_ADD_ACCOUNT = $"http://{ SERVICE_IP }/MedicationMngWebAppServices/Service.svc/AddAccount";
        public static string GET_GET_ACCOUNTS = $"http://{ SERVICE_IP }/MedicationMngWebAppServices/Service.svc/GetAccounts";
        public static string GET_LOGIN_ACCOUNT(string username, string password)
        {
            return $"http://{ SERVICE_IP }/MedicationMngWebAppServices/Service.svc/LoginAccount/{username}/{password}";
        }
        #endregion

        #region Asynchronous Tasks Message Popup & Navigation
        public static async Task ShowMessageAsync(string title, string msgBody, string buttonText)
        {
            await Application.Current.MainPage.DisplayAlert(title, msgBody, buttonText);
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
