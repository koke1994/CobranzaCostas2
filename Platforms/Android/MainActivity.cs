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
            // Inicialización explícita de Firebase eliminada para evitar errores de compilación
            // Si la versión del plugin que usas requiere inicialización en MainActivity, reintroduce la llamada
            // usando la API exacta del paquete instalado.
        }
    }
}
