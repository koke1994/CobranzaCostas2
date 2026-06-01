using CobranzaCostas.Models;
using CobranzaCostas.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace CobranzaCostas.ViewModels;

public partial class GestorViewModel : ObservableObject
{
    private readonly FirestoreService _firestoreService;
    private readonly SessionService   _sessionService;
    private readonly ILogger<GestorViewModel> _logger;

    [ObservableProperty]
    private AvanceDiario avanceActual = new();

    [ObservableProperty]
    private string corteActual = "C1"; // Por defecto, podrías hacerlo dinámico después

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string mensajeValidacion = string.Empty;

    public string FechaHoy => DateTime.Now.ToString("yyyy-MM-dd");

    public GestorViewModel(
        FirestoreService firestoreService,
        SessionService sessionService,
        ILogger<GestorViewModel> logger)
    {
        _firestoreService = firestoreService;
        _sessionService   = sessionService;
        _logger           = logger;
    }

    [RelayCommand]
    private async Task GuardarCompromisoAsync()
    {
        IsBusy = true;
        MensajeValidacion = string.Empty;

        try
        {
            // Guardamos el documento completo (incluye Compromiso)
            bool exito = await _firestoreService.GuardarAvanceAsync(AvanceActual.Id, AvanceActual);
            MensajeValidacion = exito ? "Compromiso guardado exitosamente." : "No se pudo guardar el compromiso.";
        }
        catch (Exception ex)
        {
            MensajeValidacion = "Ocurrió un error al guardar el compromiso.";
            _logger.LogError(ex, "Error en GuardarCompromisoAsync");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CargarAvanceAsync()
    {
        var usuario = _sessionService.UsuarioActual;
        if (usuario is null) return;

        IsBusy = true;
        MensajeValidacion = string.Empty;

        try
        {
            string docId = FirestoreService.BuildAvanceDocId(
                usuario.Region, usuario.Gerencia, usuario.NoEmpleado, FechaHoy, CorteActual);

            var avanceDb = await _firestoreService.GetAvanceAsync(docId);

            if (avanceDb != null)
            {
                AvanceActual = avanceDb;
            }
            else
            {
                // Si no existe, preparamos un documento nuevo limpio
                AvanceActual = new AvanceDiario
                {
                    Id         = docId,
                    NoEmpleado = usuario.NoEmpleado,
                    Region     = usuario.Region,
                    Gerencia   = usuario.Gerencia,
                    Fecha      = FechaHoy,
                    Corte      = CorteActual
                };
            }
        }
        catch (Exception ex)
        {
            MensajeValidacion = "Error al cargar el avance del día.";
            _logger.LogError(ex, "Error en CargarAvanceAsync");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GuardarAvanceAsync()
    {
        IsBusy = true;
        MensajeValidacion = string.Empty;

        try
        {
            // Guarda todo el documento por primera vez (sobreescribe si existe)
            bool exito = await _firestoreService.GuardarAvanceAsync(AvanceActual.Id, AvanceActual);
            MensajeValidacion = exito ? "Avance guardado exitosamente." : "No se pudo guardar el avance.";
        }
        catch (Exception ex)
        {
            MensajeValidacion = "Ocurrió un error al guardar el avance.";
            _logger.LogError(ex, "Error en GuardarAvanceAsync");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ActualizarAvanceAsync()
    {
        IsBusy = true;
        MensajeValidacion = string.Empty;

        // Pasamos el diccionario con el campo exacto de "Avance" (todo el bloque)
        var updates = new Dictionary<string, object>
        {
            { "Avance", AvanceActual.Avance }
        };

        bool exito = await _firestoreService.ActualizarCamposAvanceAsync(AvanceActual.Id, updates);
        MensajeValidacion = exito ? "Avance actualizado correctamente." : "Error al actualizar el avance.";
        IsBusy = false;
    }

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        await _sessionService.CerrarSesionGlobalAsync();
        await Shell.Current.GoToAsync("//LoginView");
    }
}