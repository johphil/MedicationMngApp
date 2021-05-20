using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    /// <summary>
    /// Used to record information about the user's medication intake
    /// </summary>
    public class Intake_Log
    {
        [JsonProperty(PropertyName = "Intake_Log_ID")]
        public int Intake_Log_ID { get; set; }

        [JsonProperty(PropertyName = "Account_ID")]
        public int Account_ID { get; set; }

        [JsonProperty(PropertyName = "Med_Name")]
        public string Med_Name { get; set; }

        [JsonProperty(PropertyName = "Dosage_Count")]
        public int Dosage_Count { get; set; }

        [JsonProperty(PropertyName = "Med_Type_Name")]
        public string Med_Type_Name { get; set; }

        [JsonProperty(PropertyName = "Image")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "Taken")]
        public DateTime Taken { get; set; }

        public string GetTaken
        {
            get
            {
                return Taken.ToString("MMM dd, yyyy hh:mm tt");
            }
        }
    }

    #region API Helpers
    public class GetIntakeLogsResult
    {
        [JsonProperty(PropertyName = nameof(GetIntakeLogsResult))]
        public List<Intake_Log> results { get; set; }
    }
    #endregion //END API Helpers
}
