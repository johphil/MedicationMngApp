using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    public class Account_Log
    {
        [JsonProperty(PropertyName="Account_Log_ID")]
        public int Account_Log_ID { get; set; }

        [JsonProperty(PropertyName = "Account_ID")]
        public int Account_ID { get; set; }

        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "Tag")]
        public string Tag { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        public string SimpleDate
        {
            get
            {
                return Date.ToString("MMM dd\nhh:mm tt");
            }
        }
    }

    public class GetAccountLogsResult
    {
        [JsonProperty(PropertyName = nameof(GetAccountLogsResult))]
        public List<Account_Log> results { get; set; }
    }
}
