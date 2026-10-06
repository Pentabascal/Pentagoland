using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Pentagoland.ServerDoctor;

public static class ScpslGameplayConfigValidator
{
    private static readonly string[] RequiredKeys = ["server_ip", "ipv4_bind_ip", "ipv6_bind_ip"];

    public static IReadOnlyList<string> Validate(string content)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var duplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string line in content.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None))
        {
            string trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                continue;

            int separator = trimmed.IndexOf(':');
            if (separator <= 0)
                continue;

            string key = trimmed.Substring(0, separator).Trim();
            if (!RequiredKeys.Contains(key, StringComparer.OrdinalIgnoreCase))
                continue;

            string value = trimmed.Substring(separator + 1).Trim();
            int comment = value.IndexOf('#');
            if (comment >= 0)
                value = value.Substring(0, comment).Trim();
            value = value.Trim('"', '\'');

            if (values.ContainsKey(key))
                duplicates.Add(key);
            else
                values.Add(key, value);
        }

        var findings = new List<string>();
        foreach (string key in RequiredKeys)
        {
            if (duplicates.Contains(key))
                findings.Add($"{key}: clave duplicada; revisa manualmente cuál valor aplica.");
            else if (!values.TryGetValue(key, out string? value) || value is null)
                findings.Add($"{key}: no está definida explícitamente; confirma el valor predeterminado de tu versión.");
            else
                ValidateValue(key, value, findings);
        }

        if (findings.Count == 0)
            findings.Add("Las tres opciones de conexión tienen valores que coinciden con las comprobaciones básicas de verificación.");

        findings.Add("Comprobación orientativa: no valida NAT, firewall, puertos abiertos ni accesibilidad desde Internet.");
        return findings;
    }

    private static void ValidateValue(string key, string value, ICollection<string> findings)
    {
        if (key.Equals("server_ip", StringComparison.OrdinalIgnoreCase))
        {
            if (!value.Equals("auto", StringComparison.OrdinalIgnoreCase) &&
                (!IPAddress.TryParse(value, out IPAddress? address) ||
                 address is null ||
                 address.AddressFamily != AddressFamily.InterNetwork))
                findings.Add("server_ip: usa 'auto' o una dirección IPv4 válida.");
            return;
        }

        string expected = key.Equals("ipv4_bind_ip", StringComparison.OrdinalIgnoreCase) ? "0.0.0.0" : "::";
        if (!string.Equals(value, expected, StringComparison.OrdinalIgnoreCase))
            findings.Add($"{key}: el valor esperado para la verificación pública básica es '{expected}'; valida la configuración con tu hoster antes de cambiarlo.");
    }
}
