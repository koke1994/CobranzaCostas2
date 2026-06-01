using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CobranzaCostas.Models;
using CobranzaCostas.Services;
using Microsoft.Extensions.Logging;

namespace CobranzaCostas.ViewModels;

public partial class GerenteViewModel : ObservableObject
{
    private readonly FirestoreService _firestoreService;
    private readonly SessionService   _sessionService;
    private readonly ILogger<GerenteViewModel> _logger;

    [ObservableProperty]
    private CompromisoRelacional compromisoActual = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string mensajeValidacion = string.Empty;

    public string FechaHoy => DateTime.Now.ToString("yyyy-MM-dd");

    public GerenteViewModel(
        FirestoreService firestoreService,
        SessionService sessionService,
        ILogger<GerenteViewModel> logger)
    {
        _firestoreService = firestoreService;
        _sessionService   = sessionService;
        _logger           = logger;
    }

    [RelayCommand]
    private async Task CargarCompromisoAsync()
    {
        var usuario = _sessionService.UsuarioActual;
        if (usuario is null) return;

        IsBusy = true;
        MensajeValidacion = string.Empty;

        try
        {
            // Utilizamos el helper del servicio para mantener la consistencia en las llaves
            string docId = FirestoreService.BuildRelacionalDocId(
                usuario.Region, usuario.Gerencia, usuario.NoEmpleado, FechaHoy);

            var compromisoDb = await _firestoreService.GetCompromisoRelacionalAsync(docId);

            if (compromisoDb != null)
            {
                CompromisoActual = compromisoDb;
            }
            else
            {
                // Si no existe un registro de hoy, preparamos un documento nuevo
                CompromisoActual = new CompromisoRelacional
                {
                    Id         = docId,
                    NoEmpleado = usuario.NoEmpleado,
                    Region     = usuario.Region,
                    Gerencia   = usuario.Gerencia,
                    Fecha      = FechaHoy
                };
            }
        }
        catch (Exception ex)
        {
            MensajeValidacion = "Error al cargar el compromiso relacional.";
            _logger.LogError(ex, "Error en CargarCompromisoAsync");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GuardarCompromisoAsync()
    {
        IsBusy = true;
        MensajeValidacion = string.Empty;

        try
        {
            bool exito = await _firestoreService.GuardarCompromisoRelacionalAsync(CompromisoActual.Id, CompromisoActual);
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
    private async Task CerrarSesionAsync()
    {
        await _sessionService.CerrarSesionGlobalAsync();
        await Shell.Current.GoToAsync("//LoginView");
    }
}