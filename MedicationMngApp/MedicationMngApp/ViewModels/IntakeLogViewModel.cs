using MedicationMngApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace MedicationMngApp.ViewModels
{
    public class IntakeLogViewModel : BaseViewModel
    {
        public List<Intake_Log> intakeLogs;

        public Command LoadIntakeLogsCommand { get; }

        public IntakeLogViewModel()
        {

            LoadIntakeLogsCommand = new Command(ExecuteLoadMedTakesCommand);
        }

        private async void ExecuteLoadMedTakesCommand()
        {
            try
            {
                if (NetworkStatus.IsInternet())
                {
                    using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Loading...", configuration: Common.LoadingDialogConfig))
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_INTAKE_LOGS(PersistentSettings.AccountID)))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    var jData = await response.Content.ReadAsStringAsync();
                                    if (!string.IsNullOrWhiteSpace(jData))
                                    {
                                        GetIntakeLogsResult result = JsonConvert.DeserializeObject<GetIntakeLogsResult>(jData);
                                        if (result != null)
                                        {
                                            IntakeLogs = result.results;
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
            catch (Exception error)
            {
                await Common.ShowMessageAsyncApplicationError(error.Message);
            }
        }

        #region Bindings
        public List<Intake_Log> IntakeLogs
        {
            get => intakeLogs;
            set => SetProperty(ref intakeLogs, value);
        }
        #endregion //End Bindings

        internal void OnAppearing()
        {
            LoadIntakeLogsCommand.Execute(null);
        }
    }
}
