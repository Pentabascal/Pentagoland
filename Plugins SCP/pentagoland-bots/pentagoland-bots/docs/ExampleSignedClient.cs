// Ejemplo de cliente C# para que TU plugin del servidor hable con el bot Vínculo.
// - Compatible con .NET Framework 4.8 (sin `using var`).
// - El secreto (API_SECRET_SERVER) va en la config del plugin, NUNCA en el código público.
// - Cada petición se firma con HMAC-SHA256 sobre "{timestamp}.{cuerpo}". No reutilices firmas: cada intento se firma de nuevo.
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class PentagolandApi
{
    private static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    private readonly string _baseUrl;
    private readonly byte[] _key;

    public PentagolandApi(string baseUrl, string secret)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _key = Encoding.UTF8.GetBytes(secret);
    }

    public async Task<string> PostAsync(string path, string json)
    {
        string ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        string sig;
        using (var h = new HMACSHA256(_key))
        {
            byte[] hash = h.ComputeHash(Encoding.UTF8.GetBytes(ts + "." + json));
            sig = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        var req = new HttpRequestMessage(HttpMethod.Post, _baseUrl + path)
        {
            Content = new StringContent(json, new UTF8Encoding(false), "application/json")
        };
        req.Headers.Add("X-Pentagoland-Timestamp", ts);
        req.Headers.Add("X-Pentagoland-Signature", sig);
        HttpResponseMessage res = await Http.SendAsync(req);
        return await res.Content.ReadAsStringAsync();
    }
}

// Uso (sustituye los valores por los reales del servidor; no se inventa nada):
//   var api = new PentagolandApi("https://api.tudominio.com", config.Secret);
//   await api.PostAsync("/v1/server/status",
//        "{\"online\":true,\"players\":12,\"maxPlayers\":30,\"version\":\"<versión real>\"}");
//   string r = await api.PostAsync("/v1/link/code", "{\"steamId\":\"<UserId real del jugador>\"}");
//   // r = {"code":"AB3K9XQ2","expiresInSeconds":600}  → muéstraselo al jugador en el juego.
//   await api.PostAsync("/v1/server/stats",
//        "{\"players\":[{\"steamId\":\"<UserId>\",\"kills\":10,\"deaths\":4,\"secondsPlayed\":7200,\"scpKills\":1,\"breakdown\":{\"chaos\":3}}]}");
