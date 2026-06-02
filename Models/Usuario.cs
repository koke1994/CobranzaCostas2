namespace CobranzaCostas.Models;

/// <summary>
/// Representa un usuario del sistema almacenado en Firestore.
/// El ID del documento en la colección "usuarios" es igual al UID de Firebase Auth (no_empleado).
/// Los campos se mapean manualmente en FirestoreService para evitar dependencia
/// de atributos de reflexión incompatibles con net10.0-android.
/// </summary>
public class Usuario
{
    public string NoEmpleado { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    /// <summary>Valores válidos: "Director" | "Regional" | "Gerente" | "Gestor"</summary>
    public string Rol { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Gerencia { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}