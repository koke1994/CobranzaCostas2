using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CobranzaCostas.Services;
using Microsoft.Extensions.Logging;

namespace CobranzaCostas.ViewModels;

public partial class RegionalViewModel : ObservableObject
{
    private readonly FirestoreService _firestoreService;
    private readonly SessionService   _sessionService;
    private readonly ILogger<RegionalViewModel> _logger;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string mensajeValidacion = string.Empty;

    [ObservableProperty]
    private string regionActual = string.Empty;

    public string FechaHoy => DateTime.Now.ToString("yyyy-MM-dd");

    public RegionalViewModel(
        FirestoreService firestoreService,
        SessionService sessionService,
        ILogger<RegionalViewModel> logger)
    {
        _firestoreService = firestoreService;
        _sessionService   = sessionService;
        _logger           = logger;
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        var usuario = _sessionService.UsuarioActual;
        if (usuario is null) return;

        IsBusy = true;
        MensajeValidacion = string.Empty;

        try
        {
            RegionActual = usuario.Region;
            
            // TODO: Aquí agregaremos más adelante la consulta a Firestore 
            // para traer las métricas agregadas de las gerencias de esta región.
            await Task.Delay(500); // Animación temporal de carga
        }
        catch (Exception ex)
        {
            MensajeValidacion = "Error al cargar los datos de la región.";
            _logger.LogError(ex, "Error en CargarDatosAsync");
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