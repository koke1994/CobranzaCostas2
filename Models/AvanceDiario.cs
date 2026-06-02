namespace CobranzaCostas.Models;

public class MetricasOperativas
{
    public int Visitas { get; set; }
    public int Efectivas { get; set; }
    public decimal Cobranza { get; set; }
    /// <summary>
    /// Monto de cartera en tramo de envejecimiento 6→7 (NO turno laboral).
    /// </summary>
    public decimal Pase6a7 { get; set; }
}

public class AvanceDiario
{
    public string Id { get; set; } = string.Empty;
    public string NoEmpleado { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Gerencia { get; set; } = string.Empty;
    public string Fecha { get; set; } = string.Empty;
    /// <summary>Corte dinámico configurado por el Director. Ej: "C1", "C2", "Cierre".</summary>
    public string Corte { get; set; } = string.Empty;
    public decimal MetaSemanal { get; set; }
    public MetricasOperativas Compromiso { get; set; } = new();
    public MetricasOperativas Avance { get; set; } = new();
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}