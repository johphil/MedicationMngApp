using MedicationMngApp.Utils;
using MedicationMngApp.Views;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.Resources;
using XF.Material.Forms.Resources.Typography;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace MedicationMngApp.Models
{
    public static class Common
    {
        #region Json URIs & Settings
        public static string HEADER_CONTENT_TYPE = "application/json";
        public static string SERVICE_IP = "192.168.1.4";
        public static byte[] SERVICE_CREDENTIALS = Encoding.ASCII.GetBytes("johph:really");
        public static string SERVICE_ADDR_ENDPOINT = $"http://{ SERVICE_IP }/MedicationMngWebAppServices/Service.svc/";
        public static string POST_ADD_ACCOUNT = SERVICE_ADDR_ENDPOINT + "AddAccount";
        public static string POST_ADD_MED_TAKE = SERVICE_ADDR_ENDPOINT + "AddMedTake";
        public static string POST_ADD_RATINGS = SERVICE_ADDR_ENDPOINT + "AddRatingsRecommendation";
        public static string GET_LOGIN_ACCOUNT(string username, string password)
        {
            return SERVICE_ADDR_ENDPOINT + $"LoginAccount/{username}/{password}";
        }
        public static string GET_GET_ACCOUNT_DETAILS(int account_id)
        {
            return SERVICE_ADDR_ENDPOINT + $"GetAccountDetails/{account_id}";
        }
        public static string GET_GET_MED_TAKES(int account_id)
        {
            return SERVICE_ADDR_ENDPOINT + $"GetMedTakes/{account_id}";
        }
        public static string GET_GET_MED_TAKE_TODAY(int account_id)
        {
            int day_of_week = (int)DateTime.Today.DayOfWeek;
            return SERVICE_ADDR_ENDPOINT + $"GetMedTakeToday/{account_id}/{day_of_week}";
        }
        public static string GET_GET_MED_TAKE_SCHEDULES(int med_take_id)
        {
            return SERVICE_ADDR_ENDPOINT + $"GetMedTakeSchedules/{med_take_id}";
        }
        public static string GET_GET_ACCOUNT_PASSWORD(string email)
        {
            return SERVICE_ADDR_ENDPOINT + $"GetAccountPassword/{email}";
        }
        public static string GET_GET_ACCOUNT_LOGS(int account_id)
        {
            return SERVICE_ADDR_ENDPOINT + $"GetAccountLogs/{account_id}";
        }
        public static string GET_GET_INTAKE_LOGS(int account_id)
        {
            return SERVICE_ADDR_ENDPOINT + $"GetIntakeLogs/{account_id}";
        }
        public static string GET_GET_MED_TYPES = SERVICE_ADDR_ENDPOINT + "GetMedTypes";
        public static string PUT_UPDATE_ACCOUNT_PASSWORD = SERVICE_ADDR_ENDPOINT + "UpdateAccountPassword";
        public static string PUT_UPDATE_ACCOUNT_DETAILS = SERVICE_ADDR_ENDPOINT + "UpdateAccountDetails";
        public static string PUT_UPDATE_MED_TAKE = SERVICE_ADDR_ENDPOINT + "UpdateMedTake";
        public static string DELETE_DELETE_MED_TAKE(int med_take_id)
        {
            return SERVICE_ADDR_ENDPOINT + $"DeleteMedTake/{med_take_id}";
        }
        public static string PUT_UPDATE_MED_TAKE_STATUS(int med_take_id, int enabled)
        {
            return SERVICE_ADDR_ENDPOINT + $"UpdateMedTakeEnable/{med_take_id}/{enabled}"; 
        }
        public static string PUT_TAKE_MEDICINE(int med_take_schedule_id, int med_take_id)
        {
            return SERVICE_ADDR_ENDPOINT + $"TakeMedicine/{med_take_schedule_id}/{med_take_id}";
        }
        #endregion

        #region Asynchronous Tasks Message Popup & Navigation
        public static MaterialLoadingDialogConfiguration LoadingDialogConfig
        {
            get
            {
                return new MaterialLoadingDialogConfiguration()
                {
                    BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.PRIMARY),
                    MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY).MultiplyAlpha(0.8),
                    //MessageFontFamily = XF.Material.Forms.Material.GetResource<OnPlatform<string>>("FontFamily.OpenSansRegular"),
                    TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                    CornerRadius = 8,
                    ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32)
                };
            }
        }
        public static MaterialAlertDialogConfiguration AlertDialogConfig = new MaterialAlertDialogConfiguration
        {
            BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.PRIMARY),
            CornerRadius = 4,
            MessageTextColor = Color.White,
            ScrimColor = Color.Transparent,
            TintColor = Color.AliceBlue
        };

        //Alert Dialogs
        public static async Task ShowAlertAsync(string title, string msgBody, string buttonText, bool isError = false)
        {
            MaterialAlertDialogConfiguration madc = new MaterialAlertDialogConfiguration
            {
                BackgroundColor = isError ? XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ERROR)
                                        : XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.PRIMARY),
                TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                CornerRadius = 4,
                MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_PRIMARY),
                ScrimColor = Color.Transparent,
                TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SECONDARY),
            };
            await MaterialDialog.Instance.AlertAsync(message: msgBody,
                                       title: title,
                                       acknowledgementText: buttonText,
                                       configuration: madc);
        }

        public static async Task ShowMessageAsyncNetworkError()
        {
            await Common.ShowAlertAsync("Network Error", "Internet is unavailable. Please try again.", "OK", true);
        }

        public static async Task ShowMessageAsyncUnknownError()
        {
            await Common.ShowAlertAsync("Unknown Error", "An unknown error has occurred.", "Dismiss", true);
        }

        public static async Task ShowMessageAsyncApplicationError(string error)
        {
            await Common.ShowAlertAsync("Application Error", error, "Dismiss", true);
        }

        public static async Task ShowSnackbarMessage(string message, bool isDurationLong = false, bool isError = false)
        {
            MaterialSnackbarConfiguration msc = new MaterialSnackbarConfiguration
            {
                BackgroundColor = isError ? XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ERROR)
                                    : XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.PRIMARY),
                CornerRadius = 4,
                MessageTextColor = Color.White,
                ScrimColor = Color.Transparent,
                TintColor = Color.AliceBlue
            };
            int duration = isDurationLong ? MaterialSnackbar.DurationLong : MaterialSnackbar.DurationShort;

            await MaterialDialog.Instance.SnackbarAsync(message: message, 
                                                        msDuration: duration,
                                                        configuration: msc);
        }

        public static async Task<bool> ShowAlertConfirmation(string message)
        {
            return (bool)await MaterialDialog.Instance.ConfirmAsync(message: message, configuration: AlertDialogConfig);
        }

        public static async Task<bool> ShowAlertConfirmationWithButton(string message, string confirmText, string dismissText)
        {
            return (bool)await MaterialDialog.Instance.ConfirmAsync(message: message,
                                    confirmingText: confirmText,
                                    dismissiveText: dismissText, 
                                    configuration: AlertDialogConfig);
        }
        //Navigation
        public static void NavigateNewPage(Page page)
        {
            Application.Current.MainPage = new NavigationPage(page);
        }

        public static void NavigatePage(Page page)
        {
            Application.Current.MainPage.Navigation.PushAsync(page);
        }

        public static async Task NavigateBack()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        //Notifications
        public static void ShowNotification(string title, string message)
        {
            CrossLocalNotifications.Current.Show(title, message);
        }
        public static void SetNotification(string title, string message, int id, DateTime notifyTime)
        {
            CrossLocalNotifications.Current.Show(title, message, id, notifyTime);
        }
        #endregion
    }
}
