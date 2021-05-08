using MedicationMngApp.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    public class Med_Take : Med_Type
    {
        [JsonProperty(PropertyName = "Med_Take_ID")]
        public int Med_Take_ID { get; set; }

        [JsonProperty(PropertyName = "Account_ID")]
        public int Account_ID { get; set; }

        [JsonProperty(PropertyName = "Med_Name")]
        public string Med_Name { get; set; }

        [JsonProperty(PropertyName = "Med_Count")]
        public int? Med_Count { get; set; }

        [JsonProperty(PropertyName = "Med_Description")]
        public string Med_Description
        {
            get
            {
                return string.Format("{0} remaining", Med_Count);
            }
        }
    }

    public class AddMedTakeRequestObject
    {
        [JsonProperty(PropertyName = nameof(medtake))]
        public Med_Take medtake { get; set; }

        [JsonProperty(PropertyName = nameof(medtakeschedules))]
        public List<Med_Take_Schedule> medtakeschedules { get; set; }
    }

    public class AddMedTakeResult
    {
        [JsonProperty(PropertyName = nameof(AddMedTakeResult))]
        public int result { get; set; }
    }

    public class GetMedTakesResult
    {
        [JsonProperty(PropertyName = nameof(GetMedTakesResult))]
        public List<Med_Take> results { get; set; }
    }
}
