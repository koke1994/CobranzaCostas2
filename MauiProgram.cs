﻿﻿﻿using CobranzaCostas.Services;
using CobranzaCostas.ViewModels;
using CobranzaCostas.Views;
using CobranzaCostas.Views.Director;
using CobranzaCostas.Views.Gerente;
using CobranzaCostas.Views.Gestor;
using CobranzaCostas.Views.Regional;
﻿﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace CobranzaCostas;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // ── Registrar Servicios ──
        builder.Services.AddSingleton<FirebaseAuthService>();
        builder.Services.AddSingleton<FirestoreService>();
        builder.Services.AddSingleton<SessionService>();

        // ── Registrar ViewModels ──
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddSingleton<GestorViewModel>();
        builder.Services.AddSingleton<GerenteViewModel>();
        builder.Services.AddSingleton<RegionalViewModel>();
        builder.Services.AddSingleton<DirectorViewModel>();

        // ── Registrar Vistas ──
        builder.Services.AddTransient<LoginView>();
        builder.Services.AddSingleton<DirectorPage>();
        builder.Services.AddSingleton<RegionalPage>();
        builder.Services.AddSingleton<GerentePage>();
        builder.Services.AddSingleton<GestorPage>();
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}