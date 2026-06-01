using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CobranzaCostas.Services;
using Microsoft.Extensions.Logging;

namespace CobranzaCostas.ViewModels;

public partial class DirectorViewModel : ObservableObject
{
    private readonly FirestoreService _firestoreService;
    private readonly SessionService   _sessionService;
    private readonly ILogger<DirectorViewModel> _logger;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string mensajeValidacion = string.Empty;

    [ObservableProperty]
    private string nombreDirector = string.Empty;

    public string FechaHoy => DateTime.Now.ToString("yyyy-MM-dd");

    public DirectorViewModel(
        FirestoreService firestoreService,
        SessionService sessionService,
        ILogger<DirectorViewModel> logger)
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
            NombreDirector = usuario.Nombre;
            
            // TODO: Aquí agregaremos más adelante la consulta a Firestore 
            // para traer las métricas globales a nivel nacional.
            await Task.Delay(500); // Animación temporal de carga
        }
        catch (Exception ex)
        {
            MensajeValidacion = "Error al cargar los datos de dirección.";
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