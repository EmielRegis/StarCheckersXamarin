using Android.App;
using Android.Content.PM;
using Android.OS;

namespace StarCheckersAndroid
{
    [Activity(Label = "StarCheckersAndroid"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
		, ScreenOrientation = ScreenOrientation.Portrait
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            StarCheckersWindows.Game1.Activity = this;
            var g = new StarCheckersWindows.Game1();
            SetContentView(g.Window);
            g.Run();
        }
    }
}

