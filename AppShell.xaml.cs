using CobranzaCostas.Views.Director;
using CobranzaCostas.Views.Gerente;
using CobranzaCostas.Views.Gestor;
using CobranzaCostas.Views.Regional;

namespace CobranzaCostas;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegistrarRutas();
    }

    private void RegistrarRutas()
    {
        // Para usar "//" (navegación absoluta que limpia la pila) desde el LoginViewModel,
        // MAUI exige que las rutas formen parte de la jerarquía de Shell programáticamente.
        Items.Add(new ShellContent { Route = "DirectorPage", ContentTemplate = new DataTemplate(typeof(DirectorPage)) });
        Items.Add(new ShellContent { Route = "RegionalPage", ContentTemplate = new DataTemplate(typeof(RegionalPage)) });
        Items.Add(new ShellContent { Route = "GerentePage",  ContentTemplate = new DataTemplate(typeof(GerentePage)) });
        Items.Add(new ShellContent { Route = "GestorPage",   ContentTemplate = new DataTemplate(typeof(GestorPage)) });
    }
}
