using MedicationMngApp.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    /// <summary>
    /// Used to store and get medication schedules set by the user
    /// </summary>
    public class Med_Take_Schedule
    {
        [JsonProperty(PropertyName = "Med_Take_Schedule_ID")]
        public int Med_Take_Schedule_ID { get; set; }

        [JsonProperty(PropertyName = "Med_Take_ID")]
        public int Med_Take_ID { get; set; }

        [JsonProperty(PropertyName = "Day_Of_Week")]
        public int Day_Of_Week { get; set; }

        [JsonProperty(PropertyName = "Dosage_Count")]
        public int Dosage_Count { get; set; }

        [JsonProperty(PropertyName = "Time")]
        public TimeSpan Time { get; set; }

        [JsonProperty(PropertyName = "Last_Take")]
        public DateTime? Last_Take { get; set; }
    }

    #region API Helpers
    public class GetMedTakeSchedulesResult
    {
        [JsonProperty(PropertyName = nameof(GetMedTakeSchedulesResult))]
        public List<Med_Take_Schedule> results { get; set; }
    }
    #endregion //END API Helpers
}
