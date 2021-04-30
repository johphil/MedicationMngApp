using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    public class Account
    {
        public int Account_ID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime Date_Registered { get; set; }
    }
}
