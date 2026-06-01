using CobranzaCostas.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace CobranzaCostas.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly FirebaseAuthService _authService;
    private readonly FirestoreService    _firestoreService;
    private readonly SessionService      _sessionService;
    private readonly ILogger<LoginViewModel> _logger;

    // ── Propiedades observables ──────────────────────────────────
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    // ── Constructor ──────────────────────────────────────────────
    public LoginViewModel(
        FirebaseAuthService authService,
        FirestoreService    firestoreService,
        SessionService      sessionService,
        ILogger<LoginViewModel> logger)
    {
        _authService      = authService;
        _firestoreService = firestoreService;
        _sessionService   = sessionService;
        _logger           = logger;
    }

    // ── Comandos ─────────────────────────────────────────────────
    [RelayCommand]
    private async Task LoginAsync()
    {
        // Normalizar entrada de correo antes de validar
        Email = Email?.Trim() ?? string.Empty;

        // Validación básica de campos
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Ingresa tu correo y contraseña.";
            return;
        }

        IsBusy       = true;
        ErrorMessage = string.Empty;

        try
        {
            // 1. Autenticar con Firebase Auth
            var uid = await _authService.LoginAsync(Email, Password);

            if (uid is null)
            {
                ErrorMessage = "Credenciales incorrectas. Verifica tus datos.";
                return;
            }

            // 2. Obtener perfil del usuario desde Firestore
            var usuario = await _firestoreService.GetUsuarioAsync(uid);

            if (usuario is null)
            {
                ErrorMessage = "Usuario no encontrado en el sistema. Contacta al administrador.";
                return;
            }

            if (!usuario.Activo)
            {
                ErrorMessage = "Tu cuenta está inactiva. Contacta al administrador.";
                return;
            }

            // 3. Guardar sesión en memoria
            _sessionService.UsuarioActual = usuario;

            // 4. Redirigir según rol
            var rutaDestino = usuario.Rol switch
            {
                "Director" => "//DirectorPage",
                "Regional" => "//RegionalPage",
                "Gerente"  => "//GerentePage",
                "Gestor"   => "//GestorPage",
                _          => null
            };

            if (rutaDestino is null)
            {
                ErrorMessage = $"Rol '{usuario.Rol}' no reconocido. Contacta al administrador.";
                return;
            }

            await Shell.Current.GoToAsync(rutaDestino);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error de conexión. Verifica tu red e intenta nuevamente.";
            _logger.LogError(ex, "Excepción no controlada durante el proceso de inicio de sesión.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task VerificarSesionAsync()
    {
        // Si Firebase no tiene una sesión activa, simplemente terminamos y mostramos el login
        if (!_authService.IsLoggedIn) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var uid = _authService.GetCurrentUserId();
            if (string.IsNullOrEmpty(uid)) return;

            // Buscamos el perfil usando el UID persistido
            var usuario = await _firestoreService.GetUsuarioAsync(uid);

            if (usuario is null || !usuario.Activo)
            {
                await _sessionService.CerrarSesionGlobalAsync();
                ErrorMessage = "Tu sesión expiró o la cuenta fue desactivada.";
                return;
            }

            // Restauramos la sesión en RAM y redirigimos
            _sessionService.UsuarioActual = usuario;
            var rutaDestino = usuario.Rol switch
            {
                "Director" => "//DirectorPage",
                "Regional" => "//RegionalPage",
                "Gerente"  => "//GerentePage",
                "Gestor"   => "//GestorPage",
                _          => null
            };

            if (rutaDestino != null) await Shell.Current.GoToAsync(rutaDestino);
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo restaurar tu sesión automáticamente.";
            _logger.LogError(ex, "Error restaurando sesión persistente.");
            await _sessionService.CerrarSesionGlobalAsync();
        }
        finally { IsBusy = false; }
    }
}
