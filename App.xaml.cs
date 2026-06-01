namespace CobranzaCostas
{
    public partial class App : Application
    {
        // AppShell inyectado desde DI — registrado como Singleton en MauiProgram
        public App(AppShell shell)
        {
            InitializeComponent();
            MainPage = shell;
        }
    }
}
