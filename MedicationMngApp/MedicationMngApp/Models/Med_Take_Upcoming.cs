using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MedicationMngApp.Models
{
    public class Med_Take_Upcoming
    {
        [JsonProperty(PropertyName = "Time")]
        public string Time { get; set; }

        [JsonProperty(PropertyName = "Day_Of_Week")]
        public int Day_Of_Week { get; set; }

        [JsonProperty(PropertyName = "Med_Name")]
        public string Med_Name { get; set; }

        [JsonProperty(PropertyName = "Image")]
        public string Image { get; set; }
    }

    public class GetMedTakeUpcomingResult
    {
        [JsonProperty(PropertyName = nameof(GetMedTakeUpcomingResult))]
        public List<Med_Take_Upcoming> results { get; set; }
    }
}