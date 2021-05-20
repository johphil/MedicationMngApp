using MedicationMngApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace MedicationMngApp.ViewModels
{
    public class RatingsViewModel : BaseViewModel
    {
        private int rate = 0;
        private string recommendation = string.Empty;
        private float rate1Opacity = 0.5f;
        private float rate2Opacity = 0.5f;
        private float rate3Opacity = 0.5f;
        private float rate4Opacity = 0.5f;
        private float rate5Opacity = 0.5f;

        public Command Rate1Command { get; }
        public Command Rate2Command { get; }
        public Command Rate3Command { get; }
        public Command Rate4Command { get; }
        public Command Rate5Command { get; }
        public Command SubmitCommand { get; }

        public RatingsViewModel()
        {
            Rate1Command = new Command(OnRate1Clicked);
            Rate2Command = new Command(OnRate2Clicked);
            Rate3Command = new Command(OnRate3Clicked);
            Rate4Command = new Command(OnRate4Clicked);
            Rate5Command = new Command(OnRate5Clicked);
            SubmitCommand = new Command(OnSubmitClicked);
        }

        #region Commands
        private void OnRate1Clicked()
        {
            SetRate(1);
        }
        private void OnRate2Clicked()
        {
            SetRate(2);
        }
        private void OnRate3Clicked()
        {
            SetRate(3);
        }
        private void OnRate4Clicked()
        {
            SetRate(4);
        }
        private void OnRate5Clicked()
        {
            SetRate(5);
        }
        private async void OnSubmitClicked()
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
                            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Sending feedback...", configuration: Common.LoadingDialogConfig))
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Common.SERVICE_CREDENTIALS));

                                    AddRatingsRecommendationRequestObject obj = new AddRatingsRecommendationRequestObject
                                    {
                                        ratings = new Ratings_Recommendation
                                        {
                                            Account_ID = PersistentSettings.AccountID,
                                            Ratings = rate,
                                            Recommendation = recommendation
                                        }
                                    };
                                    string serializedObject = JsonConvert.SerializeObject(obj, Formatting.Indented);
                                    using (HttpContent content = new StringContent(serializedObject, Encoding.UTF8, Common.HEADER_CONTENT_TYPE))
                                    {
                                        using (HttpResponseMessage response = await client.PostAsync(Common.POST_ADD_RATINGS, content))
                                        {
                                            if (response.IsSuccessStatusCode)
                                            {
                                                string jData = await response.Content.ReadAsStringAsync();
                                                if (!string.IsNullOrWhiteSpace(jData))
                                                {
                                                    AddRatingsRecommendationResult result = JsonConvert.DeserializeObject<AddRatingsRecommendationResult>(jData);
                                                    if (result.result > 0)
                                                    {
                                                        await Common.ShowAlertAsync("Thank you!", "Thank you for sending us your feedback.", "OK");
                                                        SetRate(0);
                                                        Recommendation = string.Empty;
                                                        Common.ShowNotification("Ratings Submission", "Thank you for your feedback!");
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
        #endregion //End Commands

        #region Bindings
        public string Recommendation
        {
            get => recommendation;
            set => SetProperty(ref recommendation, value);
        }
        public float Rate1Opacity
        {
            get => rate1Opacity;
            set => SetProperty(ref rate1Opacity, value);
        }
        public float Rate2Opacity
        {
            get => rate2Opacity;
            set => SetProperty(ref rate2Opacity, value);
        }
        public float Rate3Opacity
        {
            get => rate3Opacity;
            set => SetProperty(ref rate3Opacity, value);
        }
        public float Rate4Opacity
        {
            get => rate4Opacity;
            set => SetProperty(ref rate4Opacity, value);
        }
        public float Rate5Opacity
        {
            get => rate5Opacity;
            set => SetProperty(ref rate5Opacity, value);
        }
        #endregion //End Bindings

        #region Functions
        private void SetRate(int selectedRate)
        {
            rate = selectedRate;
            Rate1Opacity = 0.5f;
            Rate2Opacity = 0.5f;
            Rate3Opacity = 0.5f;
            Rate4Opacity = 0.5f;
            Rate5Opacity = 0.5f;

            switch (rate)
            {
                case 1:
                    {
                        Rate1Opacity = 1.0f;
                        break;
                    }
                case 2:
                    {
                        Rate2Opacity = 1.0f;
                        break;
                    }
                case 3:
                    {
                        Rate3Opacity = 1.0f;
                        break;
                    }
                case 4:
                    {
                        Rate4Opacity = 1.0f;
                        break;
                    }
                case 5:
                    {
                        Rate5Opacity = 1.0f;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        private bool Validate()
        {
            return rate > 0 && !string.IsNullOrWhiteSpace(recommendation);
        }
        private async void ValidateMessage()
        {
            if (rate <= 0)
            {
                await Common.ShowSnackbarMessage(message: "What do you think of Medication Manager App?",
                                                isDurationLong: true,
                                                isError: true);
            }
            else if (string.IsNullOrWhiteSpace(recommendation))
            {
                await Common.ShowSnackbarMessage(message: "What are your thoughts?",
                                                isDurationLong: true,
                                                isError: true);
            }
        }
        #endregion //End Functions
    }
}
