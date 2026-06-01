# NOTAS DE INTEGRACIÓN — Fase 1

## 1. MauiProgram.cs

Agrega los siguientes bloques dentro de `CreateMauiApp()`, **después** de
`builder.Services.AddSingleton<IConnectivity>(Connectivity.Current)` o similar,
antes del `return builder.Build()`.

```csharp
// ── Servicios (Singleton = una sola instancia en toda la app) ──
builder.Services.AddSingleton<FirebaseAuthService>();
builder.Services.AddSingleton<FirestoreService>();
builder.Services.AddSingleton<SessionService>();

// ── ViewModels (Transient = nueva instancia cada vez) ──
builder.Services.AddTransient<LoginViewModel>();

// ── Vistas ────────────────────────────────────────────────────
builder.Services.AddTransient<LoginView>();

// Cascarones de rol (Singleton: una sola instancia por sesión)
builder.Services.AddSingleton<DirectorPage>();
builder.Services.AddSingleton<RegionalPage>();
builder.Services.AddSingleton<GerentePage>();
builder.Services.AddSingleton<GestorPage>();

// ── Shell ─────────────────────────────────────────────────────
builder.Services.AddSingleton<AppShell>();
```

### Usings necesarios al inicio de MauiProgram.cs
```csharp
using CobranzaCostas.Services;
using CobranzaCostas.ViewModels;
using CobranzaCostas.Views;
using CobranzaCostas.Views.Director;
using CobranzaCostas.Views.Regional;
using CobranzaCostas.Views.Gerente;
using CobranzaCostas.Views.Gestor;
```

---

## 2. App.xaml.cs

Modifica el constructor de `App` para recibir `AppShell` por inyección:

```csharp
public App(AppShell shell)
{
    InitializeComponent();
    MainPage = shell;
}
```

---

## 3. Nota sobre navegación con GoToAsync

Las rutas de rol están registradas con `Routing.RegisterRoute` en `AppShell.xaml.cs`,
**no** como `ShellContent` en el XAML. Por eso el `LoginViewModel` usa la ruta
con `//` que funciona como navegación absoluta a rutas registradas en MAUI Shell.

Si en tiempo de ejecución ves un error `Shell route not found`, verifica que:
- `AppShell.xaml.cs` se haya instanciado antes del primer `GoToAsync`
- El nombre exacto en `Routing.RegisterRoute` coincida con el string en `GoToAsync`

---

## 4. Advertencia de converters en LoginView.xaml

Los converters `InvertedBoolConverter` e `IsNotNullOrEmptyConverter` vienen de
`CommunityToolkit.Maui`. Para que funcionen en XAML necesitas el namespace:

```xml
xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
```

Ya está incluido en el `LoginView.xaml` generado. Asegúrate de que
`UseMauiCommunityToolkit()` esté llamado en `MauiProgram.cs` (ya indicaste que sí).

---

## 5. Firestore: Serialización de objetos anidados

`MetricasOperativas` dentro de `AvanceDiario` (Compromiso y Avance) se serializa
como **mapa anidado** en Firestore. Plugin.Firebase.Firestore maneja esto
automáticamente con `SetDataAsync` y `ToObject<T>()` sin atributos adicionales.

Si ves que los campos anidados se guardan vacíos, asegúrate de que las propiedades
de `MetricasOperativas` sean **public** con getter y setter (ya están así).
