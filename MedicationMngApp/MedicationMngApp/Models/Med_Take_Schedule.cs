using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
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
    }
}
