using MedicationMngApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace MedicationMngApp.ViewModels
{
    public class MedicationDetailViewModel : BaseViewModel
    {
        private bool isedit = false;
        private bool listviewVisibility = false;
        private bool medcountenabled = true;
        private string medcountplaceholder = "Count On Hand";
        private string medcountcriticalplaceholder = "Critical Count";

        private List<Med_Type> medtypes;

        private int? medcount, medcountcritical;
        private string medname = string.Empty;
        private Med_Type selectedMedType = null;
        private readonly Med_Take selectedMedTake = null;

        public ObservableCollection<Med_Take_Schedule> MedTakeSchedules { get; }

        public Command AddScheduleCommand { get; }
        public Command RemoveScheduleCommand { get; }
        public Command SaveScheduleCommand { get; }
        public Command DeleteMedTakeCommand { get; }

        private readonly List<Med_Take_Schedule> DeleteMedTakeSchedules;
        private List<Med_Take_Schedule> UpdateMedTakeSchedules;
        private List<Med_Take_Schedule> CreateMedTakeSchedules;

        public MedicationDetailViewModel() //new med take
        {
            Title = "Add Medication";
            medcount = null;
            AddScheduleCommand = new Command(OnAddScheduleClicked);
            RemoveScheduleCommand = new Command(OnRemoveScheduleClicked);
            SaveScheduleCommand = new Command(OnSaveScheduleClicked);

            MedTakeSchedules = new ObservableCollection<Med_Take_Schedule>();
        }

        public MedicationDetailViewModel(Med_Take medtake) //edit med take
        {
            IsEdit = true;
            Title = medtake.Med_Name;
            medcount = medtake.Med_Count;
            medcountcritical = medtake.Med_Count_Critical;
            medname = medtake.Med_Name;
            selectedMedTake = medtake;
            AddScheduleCommand = new Command(OnAddScheduleClicked);
            RemoveScheduleCommand = new Command(OnRemoveScheduleClicked);
            SaveScheduleCommand = new Command(OnSaveScheduleClicked);
            DeleteMedTakeCommand = new Command(OnDeleteMedTakeClicked);
            MedTakeSchedules = new ObservableCollection<Med_Take_Schedule>();

            DeleteMedTakeSchedules = new List<Med_Take_Schedule>();
            UpdateMedTakeSchedules = new List<Med_Take_Schedule>();
            CreateMedTakeSchedules = new List<Med_Take_Schedule>();
        }

        #region Bindings
        public bool IsEdit
        {
            get => isedit;
            set => SetProperty(ref isedit, value);
        }
        private bool CanEdit
        {
            get
            {
                return isedit && selectedMedTake != null && MedTypes != null;
            }
        }
        public List<Med_Type> MedTypes
        {
            get => medtypes;
            set => SetProperty(ref medtypes, value);
        }
        public bool ListViewVisibility
        {
            get => listviewVisibility;
            set => SetProperty(ref listviewVisibility, value);
        }
        public bool DeleteButtonVisibility
        {
            get
            {
                return IsEdit;
            }
        }
        public string MedName
        {
            get => medname;
            set => SetProperty(ref medname, value);
        }
        public string MedCount
        {
            get => medcount == null ? string.Empty : medcount.Value.ToString();
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    SetProperty(ref medcount, null);
                else
                    SetProperty(ref medcount, int.Parse(value));
            }
        }
        public string MedCountCritical
        {
            get => medcountcritical == null ? string.Empty : medcountcritical.Value.ToString();
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    SetProperty(ref medcountcritical, null);
                else
                    SetProperty(ref medcountcritical, int.Parse(value));
            }
        }
        public bool MedCountEnabled
        {
            get => medcountenabled;
            set
            {
                SetProperty(ref medcountenabled, value);
                if (value == false)
                {
                    MedCount = null;
                    MedCountCritical = null;
                    MedCountPlaceholder = "Not applicable";
                    MedCountCriticalPlaceholder = "Not applicable";
                }
                else
                {
                    MedCountPlaceholder = "Count On Hand";
                    MedCountCriticalPlaceholder = "Critical Count";
                }
            }
        }
        public string MedCountPlaceholder
        {
            get => medcountplaceholder;
            set => SetProperty(ref medcountplaceholder, value);
        }
        public string MedCountCriticalPlaceholder
        {
            get => medcountcriticalplaceholder;
            set => SetProperty(ref medcountcriticalplaceholder, value);
        }
        public Med_Type SelectedMedType
        {
            get => selectedMedType;
            set
            {
                SetProperty(ref selectedMedType, value);
                OnMedTypeSelected(value);
            }
        }

        #endregion //Bindings

        #region Commands
        public async Task InitializeAsync()
        {
            await LoadMedTypes();

            if (selectedMedTake != null)
                await LoadMedTakeSchedules();
        }

        private async Task LoadMedTakeSchedules()
        {
            if (selectedMedTake != null)
            {
                try
                {
                    if (NetworkStatus.IsInternet())
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Common.SERVICE_CREDENTIALS));

                            using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_MED_TAKE_SCHEDULES(selectedMedTake.Med_Take_ID)))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    var jData = await response.Content.ReadAsStringAsync();
                                    if (!string.IsNullOrWhiteSpace(jData))
                                    {
                                        GetMedTakeSchedulesResult result = JsonConvert.DeserializeObject<GetMedTakeSchedulesResult>(jData);
                                        if (result != null && result.results != null)
                                        {
                                            foreach (var schedule in result.results)
                                            {
                                                MedTakeSchedules.Add(schedule);
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
        }

        private async void OnDeleteMedTakeClicked()
        {
            if (await Common.ShowAlertConfirmation("Do you want to delete?"))
            {
                if (CanSubmit && selectedMedTake != null)
                {
                    IsBusy = true;
                    try
                    {
                        if (NetworkStatus.IsInternet())
                        {

                            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Deleting medication...", configuration: Common.LoadingDialogConfig))
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Common.SERVICE_CREDENTIALS));

                                    using (HttpResponseMessage response = await client.DeleteAsync(Common.DELETE_DELETE_MED_TAKE(selectedMedTake.Med_Take_ID)))
                                    {
                                        if (response.IsSuccessStatusCode)
                                        {
                                            string jData = await response.Content.ReadAsStringAsync();
                                            if (!string.IsNullOrWhiteSpace(jData))
                                            {
                                                DeleteMedTakeResult result = JsonConvert.DeserializeObject<DeleteMedTakeResult>(jData);
                                                if (result.result > 0)
                                                {
                                                    await Common.NavigateBack();
                                                }
                                                else
                                                    await Common.ShowMessageAsyncUnknownError();
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
                    finally
                    {
                        IsBusy = false;
                    }
                }
            }
        }

        private void OnRemoveScheduleClicked(object obj)
        {
            MedTakeSchedules.Remove((Med_Take_Schedule)obj);

            if (CanEdit)
                DeleteMedTakeSchedules.Add((Med_Take_Schedule)obj);
        }

        private void OnAddScheduleClicked()
        {
            MedTakeSchedules.Add(new Med_Take_Schedule
            {
                Day_Of_Week = (int)DateTime.Now.DayOfWeek,
                Time = DateTime.Now.TimeOfDay
            });
        }

        private async Task LoadMedTypes()
        {
            IsBusy = true;
            ListViewVisibility = false;
            try
            {
                if (NetworkStatus.IsInternet())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Common.SERVICE_CREDENTIALS));

                        using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_MED_TYPES))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var jData = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(jData))
                                {
                                    GetMedTypesResult result = JsonConvert.DeserializeObject<GetMedTypesResult>(jData);
                                    MedTypes = result.result;

                                    if (CanEdit)
                                        SelectedMedType = MedTypes.Find(mt => mt.Med_Type_ID == selectedMedTake.Med_Type_ID);
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
            finally
            {
                IsBusy = false;
                ListViewVisibility = true;
            }
        }

        private void OnMedTypeSelected(Med_Type value)
        {
            if (value == null)
                return;

            if (!value.IsCount)
                MedCountEnabled = false;
            else
                MedCountEnabled = true;
        }

        private async void OnSaveScheduleClicked()
        {
            if (CanSubmit)
            {
                try
                {
                    if (Validate())
                    {
                        if (NetworkStatus.IsInternet())
                        {
                            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Saving medication...", configuration: Common.LoadingDialogConfig))
                            {
                                IsBusy = true;
                                if (CanEdit)
                                {
                                    UpdateSortSchedules();
                                    using (HttpClient client = new HttpClient())
                                    {
                                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Common.SERVICE_CREDENTIALS));

                                        UpdateMedTakeRequestObject obj = new UpdateMedTakeRequestObject
                                        {
                                            medtake = selectedMedTake,
                                            createmedtakeschedules = CreateMedTakeSchedules,
                                            updatemedtakeschedules = UpdateMedTakeSchedules,
                                            deletemedtakeschedules = DeleteMedTakeSchedules
                                        };
                                        obj.medtake.Med_Name = medname;
                                        obj.medtake.Med_Count = medcount;
                                        obj.medtake.Med_Count_Critical = medcountcritical;
                                        obj.medtake.Med_Type_ID = selectedMedType.Med_Type_ID;
                                        string serializedObject = JsonConvert.SerializeObject(obj, Formatting.Indented);
                                        using (HttpContent content = new StringContent(serializedObject, Encoding.UTF8, Common.HEADER_CONTENT_TYPE))
                                        {
                                            using (HttpResponseMessage response = await client.PutAsync(Common.PUT_UPDATE_MED_TAKE, content))
                                            {
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    string jData = await response.Content.ReadAsStringAsync();
                                                    if (!string.IsNullOrWhiteSpace(jData))
                                                    {
                                                        UpdateMedTakeResult result = JsonConvert.DeserializeObject<UpdateMedTakeResult>(jData);
                                                        if (result.result > 0)
                                                        {
                                                            await Common.NavigateBack();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else //new
                                {
                                    using (HttpClient client = new HttpClient())
                                    {
                                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Common.SERVICE_CREDENTIALS));

                                        AddMedTakeRequestObject obj = new AddMedTakeRequestObject
                                        {
                                            medtake = new Med_Take
                                            {
                                                Account_ID = PersistentSettings.AccountID,
                                                Med_Name = medname,
                                                Med_Count = medcount,
                                                Med_Count_Critical = medcountcritical,
                                                Med_Type_ID = selectedMedType.Med_Type_ID
                                            },
                                            medtakeschedules = MedTakeSchedules.ToList()
                                        };

                                        string serializedObject = JsonConvert.SerializeObject(obj, Formatting.Indented);
                                        using (HttpContent content = new StringContent(serializedObject, Encoding.UTF8, Common.HEADER_CONTENT_TYPE))
                                        {
                                            using (HttpResponseMessage response = await client.PostAsync(Common.POST_ADD_MED_TAKE, content))
                                            {
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    string jData = await response.Content.ReadAsStringAsync();
                                                    if (!string.IsNullOrWhiteSpace(jData))
                                                    {
                                                        AddMedTakeResult result = JsonConvert.DeserializeObject<AddMedTakeResult>(jData);
                                                        if (result.result > 0)
                                                        {
                                                            await Common.NavigateBack();
                                                        }
                                                        else
                                                            await Common.ShowMessageAsyncUnknownError();
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

        #region Functions
        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(medname)
                && selectedMedType != null
                && MedTakeSchedules.Count > 0
                && ((selectedMedType != null && selectedMedType.IsCount && medcount != null && medcount.HasValue)
                    || (selectedMedType != null && selectedMedType.IsCount && medcountcritical != null && medcountcritical.HasValue)
                    || (selectedMedType != null && !selectedMedType.IsCount && medcount == null));
        }

        private async void ValidateMessage()
        {
            if (string.IsNullOrWhiteSpace(medname))
                await Common.ShowSnackbarMessage(message: "Please enter a valid medicine name.",
                                                isDurationLong: true,
                                                isError: true);
            else if (selectedMedType == null)
                await Common.ShowSnackbarMessage(message: "Please select the type of medicine.",
                                                isDurationLong: true,
                                                isError: true);
            else if (MedTakeSchedules.Count == 0)
                await Common.ShowSnackbarMessage(message: "Please add atleast one schedule.",
                                                isDurationLong: true,
                                                isError: true);
            else if (selectedMedType.IsCount && medcount == null)
                await Common.ShowSnackbarMessage(message: "Please enter # count on hand.",
                                                isDurationLong: true,
                                                isError: true);
            else if (selectedMedType.IsCount && medcountcritical == null)
                await Common.ShowSnackbarMessage(message: "Please enter # critical count.",
                                                isDurationLong: true,
                                                isError: true);
        }

        private void UpdateSortSchedules()
        {
            UpdateMedTakeSchedules = MedTakeSchedules.Where(s => s.Med_Take_Schedule_ID > 0).ToList();
            CreateMedTakeSchedules = MedTakeSchedules.Where(s => s.Med_Take_Schedule_ID <= 0).ToList();
        }
        #endregion //End Functions
    }
}
