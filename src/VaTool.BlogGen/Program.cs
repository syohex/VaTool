using Scriban;
using VaTool.Lib;

if (args.Length < 1)
{
    throw new Exception("Usage: BlogGen <url>");
}

var config = await ConfigLoader.Load(Config.DefaultPath);

bool hasHeader = false;
string url;
if (args[0] == "-h")
{
    url = args[1];
    hasHeader = true;
}
else
{
    url = args[0];
}

var parser = ParserFactory.Create(url);
var product = await parser.Parse(url, config);

string template = string.Empty;
if (hasHeader)
{
    template = "<h2>{{ product.title }}</h2>\n\n";
}

template += """
<a href="{{ product.url }}" target="_blank">
<img src="{{ product.largeImage }}" alt="{{ product.title }}" />
</a>

<p>
</p>
""";

var t = Template.Parse(template);
var result = t.Render(new { product });
Console.WriteLine(result);

if (hasHeader)
{
    await Clipboard.Copy(result);
}
else
{
    await Clipboard.Copy(product.Title);
}
