using CobranzaCostas.Models;
using Plugin.Firebase.Firestore;

namespace CobranzaCostas.Services;

public class FirestoreService
{
    private readonly IFirebaseFirestore _db;

    public FirestoreService()
    {
        _db = CrossFirebaseFirestore.Current;
    }

    // ════════════════════════════════════════════════════════════════
    // USUARIOS
    // ════════════════════════════════════════════════════════════════
    public async Task<Usuario?> GetUsuarioAsync(string noEmpleado)
    {
        try
        {
            var snapshot = await _db.GetCollection("usuarios").GetDocument(noEmpleado).GetDocumentSnapshotAsync<Usuario>();
            return SnapshotToObject<Usuario>(snapshot);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Firestore] GetUsuario error: {ex.Message}");
            return null;
        }
    }

    // ════════════════════════════════════════════════════════════════
    // AVANCES DIARIOS
    // ════════════════════════════════════════════════════════════════
    public async Task<AvanceDiario?> GetAvanceAsync(string docId)
    {
        try
        {
            var snapshot = await _db.GetCollection("avances").GetDocument(docId).GetDocumentSnapshotAsync<AvanceDiario>();
            return SnapshotToObject<AvanceDiario>(snapshot);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Firestore] GetAvance error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> GuardarAvanceAsync(string docId, AvanceDiario avance)
    {
        try
        {
            await _db.GetCollection("avances").GetDocument(docId).SetDataAsync(avance);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Firestore] GuardarAvance error: {ex.Message}");
            return false;
        }
    }

    // Sobrecarga especial para recibir el diccionario de Claude y convertirlo al tipo estricto del plugin
    public async Task<bool> ActualizarCamposAvanceAsync(string docId, Dictionary<string, object> updates)
    {
        try
        {
            var nativeUpdates = new Dictionary<object, object>();
            foreach (var kvp in updates)
            {
                nativeUpdates.Add(kvp.Key, kvp.Value);
            }
            await _db.GetCollection("avances").GetDocument(docId).UpdateDataAsync(nativeUpdates);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Firestore] ActualizarCamposAvance error: {ex.Message}");
            return false;
        }
    }

    // ════════════════════════════════════════════════════════════════
    // COMPROMISOS RELACIONALES
    // ════════════════════════════════════════════════════════════════
    public async Task<CompromisoRelacional?> GetCompromisoRelacionalAsync(string docId)
    {
        try
        {
            var snapshot = await _db.GetCollection("compromisos_relacional").GetDocument(docId).GetDocumentSnapshotAsync<CompromisoRelacional>();
            return SnapshotToObject<CompromisoRelacional>(snapshot);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Firestore] GetCompromisoRelacional error: {ex.Message}");
            return null;
        }
    }

    // Helper que extrae el objeto T desde el snapshot usando reflexión.
    // Maneja distintos shape de API del plugin: puede exponer Exists/ToObject, o propiedades Document/Data/Value.
    private static T? SnapshotToObject<T>(object? snapshot)
    {
        if (snapshot is null) return default;

        var snapType = snapshot.GetType();

        // Si existe propiedad 'Exists' y es false -> no existe
        var existsProp = snapType.GetProperty("Exists");
        if (existsProp != null)
        {
            try
            {
                var existsVal = existsProp.GetValue(snapshot);
                if (existsVal is bool b && !b) return default;
            }
            catch { /* ignore reflection errors */ }
        }

        // Intentar método ToObject()
        var toObjMethod = snapType.GetMethod("ToObject", Type.EmptyTypes);
        if (toObjMethod != null)
        {
            try
            {
                var obj = toObjMethod.Invoke(snapshot, null);
                if (obj is T tObj) return tObj;
                return (T?)obj;
            }
            catch { }
        }

        // Intentar propiedades comunes
        foreach (var name in new[] { "Document", "Data", "Value", "Entity" })
        {
            var p = snapType.GetProperty(name);
            if (p != null)
            {
                try
                {
                    var val = p.GetValue(snapshot);
                    if (val is T tVal) return tVal;
                }
                catch { }
            }
        }

        // Si el snapshot ya es del tipo T
        if (snapshot is T direct) return direct;

        return default;
    }

    public async Task<bool> GuardarCompromisoRelacionalAsync(string docId, CompromisoRelacional compromiso)
    {
        try
        {
            await _db.GetCollection("compromisos_relacional").GetDocument(docId).SetDataAsync(compromiso);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Firestore] GuardarCompromisoRelacional error: {ex.Message}");
            return false;
        }
    }

    public static string BuildAvanceDocId(string region, string gerencia, string noEmpleado, string fecha, string corte)
        => $"{region}_{gerencia}_{noEmpleado}_{fecha}_{corte}";

    public static string BuildRelacionalDocId(string region, string gerencia, string noEmpleado, string fecha)
        => $"{region}_{gerencia}_{noEmpleado}_{fecha}";
}