using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    #region API Helpers
    public class LoginAccountResult
    {
        [JsonProperty(PropertyName = "LoginAccountResult")]
        public int result { get; set; }
    }
    #endregion //END API Helpers
}
