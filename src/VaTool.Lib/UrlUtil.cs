using System.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace VaTool.Lib;

public static class UrlUtil
{
    private const string AffiliateUrl = "https://al.dmm.co.jp";

    public static string SokmilAffiliateUrl(string baseUrl, Config config)
    {
        var queries = new Dictionary<string, string?>
        {
            ["affi"] = config.Sokmil.Id,
            ["utm_source"] = "sokmil_ad",
            ["utm_medium"] = "affiliate",
            ["utm_campaign"] = config.Sokmil.Id
        };

        var builder = new UriBuilder(baseUrl)
        {
            Query = string.Empty,
            Fragment = string.Empty
        };

        return QueryHelpers.AddQueryString(builder.Uri.ToString(), queries);
    }

    public static string FanzaAffiliateUrl(string url, Config config)
    {
        // strip unnecessary query parameters
        var uri = new Uri(url);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var keys = query.AllKeys;
        foreach (var key in keys)
        {
            if (key != "id")
            {
                query.Remove(key);
            }
        }

        var strippedUri = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}?{query}";

        var queries = new Dictionary<string, string?>
        {
            ["lurl"] = strippedUri,
            ["af_id"] = config.Dmm.Id,
            ["ch"] = "link_tool",
            ["ch_id"] = "link",
        };

        return QueryHelpers.AddQueryString(AffiliateUrl, queries);
    }
}
