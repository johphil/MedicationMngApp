using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicationMngApp.Droid
{
    [Activity(Theme="@style/Theme.Splash", MainLauncher = true, NoHistory = true, Label = "MedMan")]
    public class Splash : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        async void SimulateStartup()
        {
            await Task.Delay(300); // Simulate a bit of startup work.
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        public override void OnBackPressed() { }
    }
}