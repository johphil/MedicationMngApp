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

namespace MedicationMngApp.ViewModels
{
    public class MedicationDetailViewModel : BaseViewModel
    {
        private bool isedit = false;
        private bool listviewVisibility = false;
        private bool medcountenabled = true;
        private string medcountplaceholder = string.Empty;

        private List<Med_Type> medtypes;

        private int? medcount;
        private string medname = string.Empty;
        private Med_Type selectedMedType = null;
        public ObservableCollection<Med_Take_Schedule> MedTakeSchedules { get; }

        public Command AddScheduleCommand { get; }
        public Command RemoveScheduleCommand { get; }
        public Command SaveScheduleCommand { get; }

        public MedicationDetailViewModel() //new med take
        {
            Title = "Add New Medication Reminder";
            medcount = null;
            medcountplaceholder = "# Count On Hand";
            AddScheduleCommand = new Command(OnAddScheduleClicked);
            RemoveScheduleCommand = new Command(OnRemoveScheduleClicked);
            SaveScheduleCommand = new Command(OnSaveScheduleClicked);
            LoadMedTypes();

            MedTakeSchedules = new ObservableCollection<Med_Take_Schedule>();
        }

        public MedicationDetailViewModel(Med_Take medtake) //edit med take
        {
            Title = medtake.Med_Name;
            LoadMedTypes();
            IsEdit = true;
        }

        private void OnRemoveScheduleClicked(object obj)
        {
            MedTakeSchedules.Remove((Med_Take_Schedule)obj);
        }

        private void OnAddScheduleClicked()
        {
            MedTakeSchedules.Add(new Med_Take_Schedule
            {
                Day_Of_Week = (int)DateTime.Now.DayOfWeek - 1,
                Time = DateTime.Now.TimeOfDay
            });
        }

        private async void LoadMedTypes()
        {
            IsBusy = true;
            ListViewVisibility = false;
            try
            {
                if (NetworkStatus.IsInternet())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(Common.GET_GET_MED_TYPES))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var jData = await response.Content.ReadAsStringAsync();
                                if (!string.IsNullOrWhiteSpace(jData))
                                {
                                    GetMedTypesResult result = JsonConvert.DeserializeObject<GetMedTypesResult>(jData);
                                    MedTypes = result.result;
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

        public bool IsEdit
        {
            get => isedit;
            set => SetProperty(ref isedit, value);
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

        public string MedName
        {
            get => medname;
            set => SetProperty(ref medname, value);
        }

        public string MedCount
        {
            get => medcount == null ? "" : medcount.Value.ToString();
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    SetProperty(ref medcount, null);
                else
                    SetProperty(ref medcount, int.Parse(value));
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
                    MedCountPlaceholder = "Not applicable.";
                }
                else
                {
                    MedCountPlaceholder = "# Count On Hand";
                }
            }
        }

        public string MedCountPlaceholder
        {
            get => medcountplaceholder;
            set => SetProperty(ref medcountplaceholder, value);
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

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(medname)
                && selectedMedType != null
                && MedTakeSchedules.Count > 0;
        }

        private void ValidateMessage()
        {
            if (string.IsNullOrWhiteSpace(medname))
                ErrorMessage = "Please enter a valid medicine name.";
            else if (selectedMedType == null)
                ErrorMessage = "Please select the type of medicine.";
            else if (MedTakeSchedules.Count == 0)
                ErrorMessage = "Please add a schedule.";
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
            IsBusy = true;
            try
            {
                if (Validate())
                {
                    if (NetworkStatus.IsInternet())
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            AddMedTakeRequestObject requestObj = new AddMedTakeRequestObject
                            {
                                medtake = new Med_Take
                                {
                                    Account_ID = PersistentSettings.AccountID,
                                    Med_Name = medname,
                                    Med_Count = medcount,
                                    Med_Type_ID = selectedMedType.Med_Type_ID
                                },
                                medtakeschedules = MedTakeSchedules.ToList()
                            };
                            string serializedObject = JsonConvert.SerializeObject(requestObj, Formatting.Indented);
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
