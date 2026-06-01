using CobranzaCostas.Models;
using System.Threading.Tasks;

namespace CobranzaCostas.Services;

/// <summary>
/// Servicio singleton que mantiene el estado de la sesión activa.
/// Se inyecta en el LoginViewModel (escritura) y en los ViewModels de roles (lectura).
/// </summary>
public class SessionService
{
    private readonly FirebaseAuthService _authService;

    public SessionService(FirebaseAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Usuario autenticado y validado en Firestore.
    /// Es null antes de iniciar sesión.
    /// </summary>
    public Usuario? UsuarioActual { get; set; }

    public bool HaySesionActiva => UsuarioActual is not null;

    public void CerrarSesion()
    {
        UsuarioActual = null;
    }

    public async Task CerrarSesionGlobalAsync()
    {
        UsuarioActual = null;
        await _authService.LogoutAsync();
    }
}
