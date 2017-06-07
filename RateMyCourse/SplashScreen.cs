
using Android.App;
using Android.OS;
using System.Threading;

namespace RateMyClass
{
    //Set MainLauncher = true makes this Activity Shown First on running this Application  
    //Theme property set the Custom Theme for this Activity  
    //No History= true removes the Activity from BackStack when user navigates away from the Activity  
    [Activity(Label = "Rate My Class", MainLauncher = true, Theme = "@style/Theme.Splash", NoHistory = true, Icon = "@drawable/staricon")]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Display Splash Screen for 4 Sec  
            //Thread.Sleep(4000);
            //Start main Activity  
            StartActivity(typeof(MainActivity));
        }
    }
}