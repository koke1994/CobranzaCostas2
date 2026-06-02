using CobranzaCostas.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace CobranzaCostas.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly FirebaseAuthService _authService;
    private readonly FirestoreService _firestoreService;
    private readonly SessionService _sessionService;
    private readonly ILogger<LoginViewModel> _logger;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public LoginViewModel(
        FirebaseAuthService authService,
        FirestoreService firestoreService,
        SessionService sessionService,
        ILogger<LoginViewModel> logger)
    {
        _authService = authService;
        _firestoreService = firestoreService;
        _sessionService = sessionService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        Email = Email?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Ingresa tu correo y contraseña.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            // PASO 1: Auth
            var uid = await _authService.LoginAsync(Email, Password);

            // DEBUG TEMPORAL — muestra el UID en pantalla
            if (uid is null)
            {
                ErrorMessage = "Auth falló — UID es null. Verifica correo/contraseña.";
                return;
            }

            ErrorMessage = $"Auth OK. UID={uid}. Buscando en Firestore...";
            await Task.Delay(1500); // pausa para leer el mensaje

            // PASO 2: Firestore
            var usuario = await _firestoreService.GetUsuarioAsync(uid);
            if (usuario is null)
            {
                ErrorMessage = $"No encontrado en Firestore. UID buscado: '{uid}'";
                return;
            }

            if (!usuario.Activo)
            {
                ErrorMessage = "Tu cuenta está inactiva. Contacta al administrador.";
                return;
            }

            _sessionService.UsuarioActual = usuario;

            var rutaDestino = usuario.Rol switch
            {
                "Director" => "//DirectorPage",
                "Regional" => "//RegionalPage",
                "Gerente" => "//GerentePage",
                "Gestor" => "//GestorPage",
                _ => null
            };

            if (rutaDestino is null)
            {
                ErrorMessage = $"Rol '{usuario.Rol}' no reconocido.";
                return;
            }

            await Shell.Current.GoToAsync(rutaDestino);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Excepción: {ex.Message}";
            _logger.LogError(ex, "Error en login.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task VerificarSesionAsync()
    {
        if (!_authService.IsLoggedIn) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var uid = _authService.GetCurrentUserId();
            if (string.IsNullOrEmpty(uid)) return;

            var usuario = await _firestoreService.GetUsuarioAsync(uid);
            if (usuario is null || !usuario.Activo)
            {
                await _sessionService.CerrarSesionGlobalAsync();
                ErrorMessage = "Tu sesión expiró o la cuenta fue desactivada.";
                return;
            }

            _sessionService.UsuarioActual = usuario;

            var rutaDestino = usuario.Rol switch
            {
                "Director" => "//DirectorPage",
                "Regional" => "//RegionalPage",
                "Gerente" => "//GerentePage",
                "Gestor" => "//GestorPage",
                _ => null
            };

            if (rutaDestino != null)
                await Shell.Current.GoToAsync(rutaDestino);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error sesión: {ex.Message}";
            _logger.LogError(ex, "Error restaurando sesión.");
            await _sessionService.CerrarSesionGlobalAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }
}