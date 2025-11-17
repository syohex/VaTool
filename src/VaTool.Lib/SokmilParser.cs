using AngleSharp;
using Microsoft.AspNetCore.WebUtilities;

namespace VaTool.Lib;

public class SokmilParser : IParser
{
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36";

    public async Task<Product> Parse(string url, Config config)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        client.DefaultRequestHeaders.Add("Cookie", "AGEAUTH=ok");

        var response = await client.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();

        var context = BrowsingContext.New();
        var document = await context.OpenAsync(req => req.Content(responseBody).Address(url));

        var product = new Product();

        var titleElement = document.QuerySelector("h1.page-title");
        if (titleElement != null)
        {
            var title = titleElement.TextContent;
            if (title == null)
            {
                throw new Exception($"Cannot get title from {url}");
            }

            product.Title = title;
        }

        var packageElement = document.QuerySelector("a.sokmil-lightbox-jacket");
        if (packageElement != null)
        {
            var title = packageElement.GetAttribute("href");
            if (title == null)
            {
                throw new Exception($"Cannot get package from {url}");
            }

            product.Image = title;
        }

        product.Url = ConstructAffiliateUrl(url, config);
        return product;
    }

    private string ConstructAffiliateUrl(string url, Config config)
    {
        var queries = new Dictionary<string, string?>
        {
            ["affi"] = config.Sokmil.Id,
            ["utm_source"] = "sokmil_ad",
            ["utm_medium"] = "affiliate",
            ["utm_campaign"] = config.Sokmil.Id
        };

        var builder = new UriBuilder(url)
        {
            Query = string.Empty,
            Fragment = string.Empty
        };

        return QueryHelpers.AddQueryString(builder.Uri.ToString(), queries);
    }
}
