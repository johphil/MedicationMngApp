using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    public class Account
    {
        [JsonProperty]
        public int Account_ID { get; set; }
        [JsonProperty]
        public string FirstName { get; set; }
        [JsonProperty]
        public string LastName { get; set; }
        [JsonProperty]
        public DateTime Birthday { get; set; }
        [JsonProperty]
        public string Email { get; set; }
        [JsonProperty]
        public string Username { get; set; }
        [JsonProperty]
        public string Password { get; set; }
        [JsonProperty]
        public DateTime Date_Registered { get; set; }
    }
    
    public class AccountWrapper
    {
        [JsonProperty(PropertyName = "account")]
        public Account account { get; set; }
    }
}
