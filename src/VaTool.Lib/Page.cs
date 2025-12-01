using YamlDotNet.Serialization;

namespace VaTool.Lib;

public struct Page
{
    public string Name { get; set; }
    public string Summary { get; set; }
    public List<string>? RelatedLinks { get; set; }
    public List<PageItem> Items { get; set; }
}

public struct PageItem
{
    public string Id { get; set; }
    [YamlMember(Alias = "sokmil")]
    public string SokmilUrl { get; set; }
    [YamlMember(Alias = "fanza")]
    public string FanzaUrl { get; set; }
    public List<string>? Actresses { get; set; }
    public string Note { get; set; }

    public bool HasSokmilUrl() => !string.IsNullOrEmpty(SokmilUrl);
    public bool HasFanzaUrl() => !string.IsNullOrEmpty(FanzaUrl);
}
