using AngleSharp;

namespace VaTool.Lib;

internal class SokmilParser : IParser
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
        if (titleElement is not null)
        {
            var title = titleElement.TextContent;
            if (string.IsNullOrEmpty(title))
            {
                throw new Exception($"Cannot get title from {url}");
            }

            product.Title = title;
        }

        var packageElement = document.QuerySelector("a.sokmil-lightbox-jacket");
        if (packageElement is not null)
        {
            var imageUrl = packageElement.GetAttribute("href");
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new Exception($"Cannot get large image from {url}");
            }

            product.LargeImage = imageUrl;
        }

        var thumbnailElement = document.QuerySelector("img.jacket-img");
        if (thumbnailElement is not null)
        {
            var imageUrl = thumbnailElement.GetAttribute("src");
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new Exception($"Cannot get small image from {url}");
            }

            product.SmallImage = imageUrl;
        }

        var productInfos = document.QuerySelectorAll("dl.product-info");
        foreach (var productInfo in productInfos)
        {
            var infoTitle = productInfo.QuerySelector("dt.info-title");
            if (infoTitle is not null && infoTitle.TextContent is "発売日" or "配信開始日")
            {
                var infoDate = productInfo.QuerySelector("dd.info-value");
                if (infoDate is not null)
                {
                    var dateStr = DateUtil.FormatDate(infoDate.TextContent.Trim());
                    if (string.IsNullOrEmpty(product.ReleaseDate) || (string.Compare(dateStr, product.ReleaseDate) < 0))
                    {
                        product.ReleaseDate = dateStr;
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(product.ReleaseDate))
        {
            var publishDateElement = document.QuerySelector("span.publish-at");
            if (publishDateElement is not null)
            {
                var text = publishDateElement.TextContent;
                if (string.IsNullOrEmpty(text))
                {
                    throw new Exception($"Cannot find releaseDate and publishDate from {url}");
                }

                var publishDate = text.Split(' ')[0];
                product.ReleaseDate = DateUtil.FormatDate(publishDate);
            }
        }

        product.Url = UrlUtil.SokmilAffiliateUrl(url, config);
        return product;
    }
}
