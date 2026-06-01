using Plugin.Firebase.Firestore;

namespace CobranzaCostas.Models;

/// <summary>
/// Representa a un usuario del sistema almacenado en Firestore.
/// El documento ID en la colección "usuarios" es igual al UID de Firebase Auth (no_empleado).
/// Los atributos [FirestoreProperty] son OBLIGATORIOS para que ToObject<Usuario>()
/// mapee correctamente los campos del documento.
/// </summary>
[FirestoreData]
public class Usuario
{
    [FirestoreProperty("NoEmpleado")]
    public string NoEmpleado { get; set; } = string.Empty;

    [FirestoreProperty("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [FirestoreProperty("Email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Valores válidos: "Director" | "Regional" | "Gerente" | "Gestor"
    /// </summary>
    [FirestoreProperty("Rol")]
    public string Rol { get; set; } = string.Empty;

    [FirestoreProperty("Region")]
    public string Region { get; set; } = string.Empty;

    [FirestoreProperty("Gerencia")]
    public string Gerencia { get; set; } = string.Empty;

    [FirestoreProperty("Activo")]
    public bool Activo { get; set; } = true;
}
