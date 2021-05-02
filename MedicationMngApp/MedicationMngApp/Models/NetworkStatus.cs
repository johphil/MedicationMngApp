using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace MedicationMngApp.Models
{
    public class NetworkStatus
    {
        public static bool IsInternet()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
                return true;
            else
                return false;
        }
    }
}
