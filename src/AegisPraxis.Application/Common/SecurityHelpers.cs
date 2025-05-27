using System.Text.RegularExpressions;

namespace AegisPraxis.Application.Common;

public static class SecurityHelpers
{
    public static string ExtractRealmFromIssuer(string issuer)
    {
        var match = Regex.Match(issuer, @"realms\/([^\/]+)");
        return match.Success
            ? match.Groups[1].Value
            : throw new InvalidOperationException("Cannot extract realm from issuer.");
    }
}
