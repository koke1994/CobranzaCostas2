namespace CobranzaCostas.Models;

public class CompromisoRelacional
{
    public string Id { get; set; } = string.Empty;
    public string NoEmpleado { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Gerencia { get; set; } = string.Empty;
    public string Fecha { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public int VisitasDeApoyo { get; set; }
    public int ReunionesEquipo { get; set; }
    public decimal CobranzaSupervisada { get; set; }
}