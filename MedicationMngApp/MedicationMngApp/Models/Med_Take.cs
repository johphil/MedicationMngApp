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

        [JsonProperty(PropertyName = "Med_Count_Critical")]
        public int? Med_Count_Critical { get; set; }

        [JsonProperty(PropertyName = "IsActive")]
        public bool IsActive { get; set; }

        public string Med_Description_Image
        {
            get
            {
                if (IsCritical)
                    return "icon_exclamation";
                else
                    return IsCount ? Image : "icon_infinity.png";
            }
        }

        public bool IsCritical
        {
            get
            {
                return !IsCount ? false : Med_Count <= Med_Count_Critical;
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

    public class DeleteMedTakeResult
    {
        [JsonProperty(PropertyName = nameof(DeleteMedTakeResult))]
        public int result { get; set; }
    }

    public class UpdateMedTakeResult
    {
        [JsonProperty(PropertyName = nameof(UpdateMedTakeResult))]
        public int result { get; set; }
    }
    public class UpdateMedTakeEnableResult
    {
        [JsonProperty(PropertyName = nameof(UpdateMedTakeEnableResult))]
        public int result { get; set; }
    }

    public class UpdateMedTakeRequestObject
    {
        [JsonProperty(PropertyName = nameof(medtake))]
        public Med_Take medtake { get; set; }

        [JsonProperty(PropertyName = nameof(deletemedtakeschedules))]
        public List<Med_Take_Schedule> deletemedtakeschedules;

        [JsonProperty(PropertyName = nameof(updatemedtakeschedules))]
        public List<Med_Take_Schedule> updatemedtakeschedules;

        [JsonProperty(PropertyName = nameof(createmedtakeschedules))]
        public List<Med_Take_Schedule> createmedtakeschedules;
    }
}
