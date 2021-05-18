using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MedicationMngApp.Models
{
    public class Med_Take_Today : Med_Take_Schedule
    {
        [JsonProperty(PropertyName = "Med_Name")]
        public string Med_Name { get; set; }

        [JsonProperty(PropertyName = "Image")]
        public string Image { get; set; }

        public bool IsTaken
        {
            get
            {
                if (!Last_Take.HasValue)
                    return false;
                else
                    return Last_Take.Value.Date == DateTime.Today.Date;
            }
        }

        public bool IsShouldBeTaken
        {
            get
            {
                return !IsTaken && DateTime.Now.TimeOfDay >= Time;
            }
        }

        public bool IsTooEarly
        {
            get
            {
                //if the current time is 10 or more minutes ahead before the intake time
                return !IsTaken && DateTime.Now.AddMinutes(10).TimeOfDay < Time;
            }
        }

        public string ActionImage 
        {
            get
            {
                if (IsTaken)
                    return "icon_check";
                else if (IsShouldBeTaken)
                    return "icon_exclamation";
                else
                    return "";
            }
        }

        public string GetTime
        {
            get
            {
                return DateTime.Today.Add(Time).ToString("hh:mm tt").ToUpper();
            }
        }
    }

    public class GetMedTakeTodayResult
    {
        [JsonProperty(PropertyName = nameof(GetMedTakeTodayResult))]
        public List<Med_Take_Today> results { get; set; }
    }

    public class TakeMedicineResult
    {
        [JsonProperty(PropertyName = nameof(TakeMedicineResult))]
        public int result { get; set; }
    }
}