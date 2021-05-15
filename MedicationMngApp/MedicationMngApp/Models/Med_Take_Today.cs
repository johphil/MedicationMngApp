using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MedicationMngApp.Models
{
    public class Med_Take_Today
    {
        [JsonProperty(PropertyName = "Time")]
        public string Time { get; set; }

        [JsonProperty(PropertyName = "Day_Of_Week")]
        public int Day_Of_Week { get; set; }

        [JsonProperty(PropertyName = "Med_Name")]
        public string Med_Name { get; set; }

        [JsonProperty(PropertyName = "Image")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "ActionImage")]
        public string ActionImage { get; set; }
    }

    public class GetMedTakeTodayResult
    {
        [JsonProperty(PropertyName = nameof(GetMedTakeTodayResult))]
        public List<Med_Take_Today> results { get; set; }
    }
}