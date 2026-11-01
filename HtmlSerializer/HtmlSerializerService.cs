
using System.Text.RegularExpressions;

namespace HtmlSerializer;

public class HtmlSerializerService
{
            private static readonly Regex TokenRegex = new(
        @"(?<raw><(?<rawName>script|style)\b[^>]*>.*?</\k<rawName>\s*>)|(?<tag><\s*/?\s*[A-Za-z][A-Za-z0-9:-]*\b[^>]*>)|(?<declaration><[!?][^>]*>)|(?<text>[^<]+|<)",
        RegexOptions.Compiled |
        RegexOptions.Singleline |
        RegexOptions.IgnoreCase);

        private static readonly Regex HtmlTagRegex = new(
        @"^<\s*(?<closing>/)?\s*(?<name>[A-Za-z][A-Za-z0-9:-]*)\b(?<attributes>.*?)\s*(?<selfClosing>/)?\s*>$",
        RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex AttributeRegex = new(
        @"(?<name>[^\s=/>]+)(?:\s*=\s*(?:""(?<double>[^""]*)""|'(?<single>[^']*)'|(?<unquoted>[^\s>]+)))?",
        RegexOptions.Compiled);

    private static readonly Regex CommentRegex = new(
        @"<!--.*?-->",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex OpeningRawTagRegex = new(
        @"^<(?<name>script|style)\b[^>]*>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ClosingRawTagRegex = new(
        @"</(?:script|style)\s*>$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly HttpClient Client = new();

                public async Task<string> Load(string url)
    {
        var response = await Client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        return html;
    }

                public async Task<HtmlElement> LoadAndSerialize(string url)
    {
        var html = await Load(url);

        return Serialize(html);
    }

                    public List<string> SplitToTags(string html)
    {
        var cleaned = CommentRegex.Replace(html, string.Empty);

        var result = new List<string>();

        foreach (Match match in TokenRegex.Matches(cleaned))
        {
                        if (match.Groups["raw"].Success)
            {
                var raw = match.Groups["raw"].Value;

                var openingTag = OpeningRawTagRegex.Match(raw);
                var closingTag = ClosingRawTagRegex.Match(raw);

                if (openingTag.Success && closingTag.Success)
                {
                                        result.Add(openingTag.Value);

                                        var contentStart = openingTag.Length;

                    var contentLength =
                        raw.Length - openingTag.Length - closingTag.Length;

                    result.Add(raw.Substring(contentStart, contentLength));

                                        result.Add(closingTag.Value);
                }
            }
            else
            {
                                result.Add(match.Value);
            }
        }

        return result;
    }

                public HtmlElement Serialize(string html)
    {
        var lines = SplitToTags(html);

        var helper = HtmlHelper.Instance;

        var root = new HtmlElement { Name = "root" };

        var current = root;

        foreach (var line in lines)
        {
                        if (line.StartsWith("<!") || line.StartsWith("<?"))
                continue;

                        var match = HtmlTagRegex.Match(line);

            if (!match.Success)
            {
                                current.InnerHtml += line;
                continue;
            }

            var tagName = match.Groups["name"].Value.ToLowerInvariant();

                        if (match.Groups["closing"].Success)
            {
                if (tagName == "html")
                    break;

                if (current.Parent != null)
                    current = current.Parent;

                continue;
            }

                        if (!helper.IsHtmlTag(tagName))
            {
                current.InnerHtml += line;
                continue;
            }

                        var element = new HtmlElement
            {
                Name = tagName,
                Parent = current
            };

                        current.Children.Add(element);

                        var attributesText = match.Groups["attributes"].Value;

            ParseAttributes(attributesText, element);

                        bool selfClosing =
                match.Groups["selfClosing"].Success ||
                helper.IsVoidTag(tagName);

                                    if (!selfClosing)
            {
                current = element;
            }
        }

                if (root.Children.Count == 1)
        {
            var top = root.Children[0];

            top.Parent = null;

            return top;
        }

        return root;
    }

                private static void ParseAttributes(string text, HtmlElement element)
    {
        foreach (Match match in AttributeRegex.Matches(text))
        {
            var name = match.Groups["name"].Value;

            if (string.IsNullOrWhiteSpace(name))
                continue;

            string? value = null;

            if (match.Groups["double"].Success)
            {
                value = match.Groups["double"].Value;
            }
            else if (match.Groups["single"].Success)
            {
                value = match.Groups["single"].Value;
            }
            else if (match.Groups["unquoted"].Success)
            {
                value = match.Groups["unquoted"].Value;
            }

                        if (value == null)
            {
                element.Attributes.Add(name);
                continue;
            }

            element.Attributes.Add($"{name}=\"{value}\"");

                        if (name.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                element.Id = value;
            }

                        else if (name.Equals("class", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    element.Classes.AddRange(
                        Regex.Split(value.Trim(), @"\s+"));
                }
            }
        }
    }
}
