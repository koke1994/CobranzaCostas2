using Plugin.Firebase.Firestore;

namespace CobranzaCostas.Models;

/// <summary>
/// Documento que representa el compromiso operativo y relacional de un Gerente.
/// Colección Firestore: compromisos_relacional/{region}_{gerencia}_{noEmpleado}_{fecha}
/// </summary>
[FirestoreData]
public class CompromisoRelacional
{
    [FirestoreProperty("Id")]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("NoEmpleado")]
    public string NoEmpleado { get; set; } = string.Empty;

    [FirestoreProperty("Region")]
    public string Region { get; set; } = string.Empty;

    [FirestoreProperty("Gerencia")]
    public string Gerencia { get; set; } = string.Empty;

    [FirestoreProperty("Fecha")]
    public string Fecha { get; set; } = string.Empty;

    [FirestoreProperty("FechaRegistro")]
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    [FirestoreProperty("VisitasDeApoyo")]
    public int VisitasDeApoyo { get; set; }

    [FirestoreProperty("ReunionesEquipo")]
    public int ReunionesEquipo { get; set; }

    [FirestoreProperty("CobranzaSupervisada")]
    public decimal CobranzaSupervisada { get; set; }
}
