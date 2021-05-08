using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    public class Med_Type
    {
        [JsonProperty(PropertyName = "Med_Type_ID")]
        public int Med_Type_ID { get; set; }

        [JsonProperty(PropertyName = "Med_Type_Name")]
        public string Med_Type_Name { get; set; }

        [JsonProperty(PropertyName = "IsCount")]
        public bool IsCount { get; set; }

        [JsonProperty(PropertyName = "Image")]
        public string Image { get; set; }

    }

    public class GetMedTypesResult
    {
        [JsonProperty(PropertyName = nameof(GetMedTypesResult))]
        public List<Med_Type> result { get; set; }
    }
}
