using Plugin.Firebase.Firestore;

namespace CobranzaCostas.Models;

/// <summary>
/// Bloque reutilizable de métricas operativas.
/// Se usa tanto para el Compromiso (proyectado) como para el Avance (real).
/// </summary>
[FirestoreData]
public class MetricasOperativas
{
    [FirestoreProperty("Visitas")]
    public int Visitas { get; set; }

    [FirestoreProperty("Efectivas")]
    public int Efectivas { get; set; }

    [FirestoreProperty("Cobranza")]
    public decimal Cobranza { get; set; }

    /// <summary>
    /// Monto monetario correspondiente a las cuentas en categorías de morosidad
    /// de la ruta del Gestor (envejecimiento de cartera, NO turno laboral).
    /// </summary>
    [FirestoreProperty("Pase6a7")]
    public decimal Pase6a7 { get; set; }
}

/// <summary>
/// Documento de avance diario por Gestor.
/// Colección Firestore: avances/{region}_{gerencia}_{noEmpleado}_{fecha}_{corte}
/// </summary>
[FirestoreData]
public class AvanceDiario
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

    /// <summary>
    /// Identificador del corte. Los cortes son dinámicos y configurados
    /// por el Director (no hay número fijo ni están quemados en código).
    /// Ejemplo: "C1", "C2", "Cierre".
    /// </summary>
    [FirestoreProperty("Corte")]
    public string Corte { get; set; } = string.Empty;

    [FirestoreProperty("MetaSemanal")]
    public decimal MetaSemanal { get; set; }

    [FirestoreProperty("Compromiso")]
    public MetricasOperativas Compromiso { get; set; } = new();

    [FirestoreProperty("Avance")]
    public MetricasOperativas Avance { get; set; } = new();

    [FirestoreProperty("FechaRegistro")]
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
