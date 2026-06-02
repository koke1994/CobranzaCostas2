using Android.App;
using Android.Content.PM;
using Android.OS;

namespace CobranzaCostas
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges =
            ConfigChanges.ScreenSize |
            ConfigChanges.Orientation |
            ConfigChanges.UiMode |
            ConfigChanges.ScreenLayout |
            ConfigChanges.SmallestScreenSize |
            ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Firebase se inicializa automáticamente via google-services.json
            // en Plugin.Firebase v5.x — no se requiere CrossFirebase.Initialize()
        }
    }
}