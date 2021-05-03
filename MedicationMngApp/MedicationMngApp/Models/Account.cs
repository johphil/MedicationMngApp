using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    public class Account
    {
        [JsonProperty(PropertyName = "Account_ID")]
        public int Account_ID { get; set; }
        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "Birthday")]
        public DateTime Birthday { get; set; }
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "Username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "Password")]
        public string Password { get; set; }
        [JsonProperty(PropertyName = "Date_Registered")]
        public DateTime Date_Registered { get; set; }
    }

    public class AddAccountObject
    {
        [JsonProperty(PropertyName = "account")]
        public Account account { get; set; }
    }
    public class AddAccountResult
    {
        [JsonProperty(PropertyName = "AddAccountResult")]
        public int result { get; set; }
    }

    public class GetAccountsResult
    {
        [JsonProperty(PropertyName = "GetAccountsResult")]
        public List<Account> accounts { get; set; }
    }
}
