using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMng.Shared.Model
{
    public class Account
    {
        public int Account_ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime Date_Registered { get; set; }
    }
}
