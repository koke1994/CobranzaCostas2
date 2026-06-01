using CobranzaCostas.ViewModels;

namespace CobranzaCostas.Views;

public partial class LoginView : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Ejecuta la verificación de sesión en cuanto la vista aparece
        if (_viewModel.VerificarSesionCommand.CanExecute(null))
        {
            _viewModel.VerificarSesionCommand.Execute(null);
        }
    }
}