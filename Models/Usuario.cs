namespace CobranzaCostas.Models;

/// <summary>
/// Representa a un usuario del sistema almacenado en Firestore.
/// El documento ID en la colección "usuarios" es igual al UID de Firebase Auth.
/// </summary>
public class Usuario
{
    public string NoEmpleado { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Valores válidos: "Director" | "Regional" | "Gerente" | "Gestor"
    /// </summary>
    public string Rol { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string Gerencia { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
