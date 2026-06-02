using Plugin.Firebase.Auth;

namespace CobranzaCostas.Services;

public class FirebaseAuthService
{
    public async Task<string?> LoginAsync(string email, string password)
    {
        try
        {
            var result = await CrossFirebaseAuth.Current
                .SignInWithEmailAndPasswordAsync(email, password);

            var uid = result?.Uid;
            Console.WriteLine($"[FirebaseAuth] Login — UID obtenido: '{uid}'");
            return uid;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FirebaseAuth] Error en login: {ex.Message}");
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        try { await CrossFirebaseAuth.Current.SignOutAsync(); }
        catch (Exception ex) { Console.WriteLine($"[FirebaseAuth] Logout error: {ex.Message}"); }
    }

    public string? GetCurrentUserId() =>
        CrossFirebaseAuth.Current.CurrentUser?.Uid;

    public bool IsLoggedIn =>
        CrossFirebaseAuth.Current.CurrentUser is not null;
}