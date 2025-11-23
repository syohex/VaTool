using System.Text;
using System.Text.RegularExpressions;
using VaTool.Lib;

if (args.Length < 1)
{
    throw new Exception("Usage: wikigen <file> pattern...");
}

var pattern = CreatePattern(args.AsSpan()[1..]);
var shouldWriteHeder = pattern is null;

var config = await ConfigLoader.Load(Config.DefaultPath);
var pageFile = args[0];

var entries = new List<TableEntry>();
var page = await PageLoader.Load(pageFile);

foreach (var item in page.Items)
{
    var (parser, productUrl) = GetParser(item);
    if (pattern is not null && !pattern.IsMatch(item.Id))
    {
        continue;
    }

    var product = await parser.Parse(productUrl, config);
    entries.Add(new TableEntry(item, product));
}

var output = Render(page, entries, config, shouldWriteHeder);
Console.WriteLine(output);

await Clipboard.Copy(output);

(IParser, string) GetParser(PageItem item)
{
    if (!string.IsNullOrEmpty(item.SokmilUrl))
    {
        return (ParserFactory.Create(item.SokmilUrl), item.SokmilUrl);
    }

    if (!string.IsNullOrEmpty(item.FanzaUrl))
    {
        return (ParserFactory.Create(item.FanzaUrl), item.FanzaUrl);
    }

    throw new Exception("Please set sokmil or fanza URL");
}

Regex? CreatePattern(ReadOnlySpan<string> args)
{
    if (args.Length == 0)
    {
        return null;
    }

    var pattern = string.Join('|', args);
    return new Regex(pattern);
}

string Render(Page page, List<TableEntry> entries, Config config, bool shouldWriteHeder)
{
    const string tableHeader = "|~ID|Image|タイトル|出演者(出演順)|発売日|Note|";

    var sb = new StringBuilder();
    if (!string.IsNullOrEmpty(page.Summary))
    {
        sb.Append(page.Summary);
        sb.AppendLine();
        sb.AppendLine();
    }

    if (shouldWriteHeder)
    {
        sb.Append(tableHeader);
        sb.AppendLine();
    }

    foreach (var entry in entries)
    {
        RenderTableEntry(entry, config, sb);
    }

    return sb.ToString();
}

void RenderTableEntry(TableEntry entry, Config config, StringBuilder sb)
{
    const char columnSeparator = '|';
    const string newLine = "~~";
    const string actressSeparator = "／";

    var fanzaUrl = UrlUtil.FanzaAffiliateUrl(entry.item.FanzaUrl, config);

    // ID part
    sb.Append(columnSeparator);
    sb.Append($"[[{entry.item.Id}>{fanzaUrl}]]");

    // Image part
    sb.Append(columnSeparator);
    sb.Append($"center:[[&ref({entry.product.SmallImage},180)>{entry.product.LargeImage}]]");
    sb.Append(newLine);

    if (entry.item.HasSokmilUrl())
    {
        var sokmilUrl = UrlUtil.SokmilAffiliateUrl(entry.item.SokmilUrl, config);
        sb.Append($"[[ソクミル>{sokmilUrl}]]");
    }
    if (entry.item.HasSokmilUrl() && entry.item.HasFanzaUrl())
    {
        sb.Append(' ');
    }
    if (entry.item.HasFanzaUrl())
    {
        sb.Append($"[[FANZA>{fanzaUrl}]]");
    }

    // Title part
    sb.Append(columnSeparator);
    sb.Append(entry.product.Title);

    // Actresses part
    sb.Append(columnSeparator);

    var actressLinks = new List<string>();
    foreach (var actress in entry.item.Actresses)
    {
        if (actress.EndsWith('?'))
        {
            actressLinks.Add(actress.TrimEnd('?'));
        }
        else
        {
            actressLinks.Add($"[[{actress}]]");
        }
    }
    sb.Append(string.Join(actressSeparator, actressLinks));

    // Date part
    sb.Append(columnSeparator);
    sb.Append(entry.product.ReleaseDate);

    // Note part
    sb.Append(columnSeparator);
    sb.Append(entry.item.Note);
    sb.Append(columnSeparator);
}

record TableEntry(PageItem item, Product product);
