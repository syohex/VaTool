using VaTool.Lib;

namespace VaTool.Tests;

public class FanzaTest
{
    [Fact]
    public async Task FetchAndParseTest()
    {
        var testUrl = "https://video.dmm.co.jp/av/content/?id=1start00369";
        var parser = ParserFactory.Create(testUrl);
        var res = await parser.Parse(testUrl, TestData.DummyConfig);
        Assert.NotEmpty(res.Title);
        Assert.NotEmpty(res.LargeImage);
        Assert.NotEmpty(res.SmallImage);
        Assert.NotEmpty(res.ReleaseDate);
    }
}
