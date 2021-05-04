using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    public class LoginAccountResult
    {
        [JsonProperty(PropertyName = "LoginAccountResult")]
        public int result { get; set; }
    }
}
