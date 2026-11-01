using System.Text.RegularExpressions;

namespace HtmlSerializer;

public class Selector
{
    public string? TagName { get; set; }
    public string? Id { get; set; }
    public List<string> Classes { get; set; } = new();
    public Selector? Parent { get; set; }
    public Selector? Child { get; set; }

                    public static Selector FromQueryString(string queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
            throw new ArgumentException("Query string must not be empty.", nameof(queryString));

        var levels = queryString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var root = new Selector();
        var current = root;

        for (int i = 0; i < levels.Length; i++)
        {
                        var parts = Regex.Split(levels[i], "(?=[#.])").Where(p => p.Length > 0);

            foreach (var part in parts)
            {
                if (part.StartsWith('#'))
                {
                    current.Id = part.Substring(1);
                }
                else if (part.StartsWith('.'))
                {
                    current.Classes.Add(part.Substring(1));
                }
                else if (HtmlHelper.Instance.IsHtmlTag(part))
                {
                    current.TagName = part.ToLowerInvariant();
                }
            }

                        if (i < levels.Length - 1)
            {
                var child = new Selector { Parent = current };
                current.Child = child;
                current = child;
            }
        }

        return root;
    }

                public bool Matches(HtmlElement element)
    {
        if (TagName != null &&
            !string.Equals(element.Name, TagName, StringComparison.OrdinalIgnoreCase))
            return false;

        if (Id != null && element.Id != Id)
            return false;

        foreach (var cls in Classes)
        {
            if (!element.Classes.Contains(cls))
                return false;
        }

        return true;
    }

    public override string ToString()
    {
        var s = (TagName ?? "") +
                (Id != null ? "#" + Id : "") +
                string.Concat(Classes.Select(c => "." + c));
        return Child == null ? s : s + " " + Child;
    }
}
