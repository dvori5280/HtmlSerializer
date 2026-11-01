using System.Text;

namespace HtmlSerializer;

public class HtmlElement
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public List<string> Attributes { get; set; } = new();
    public List<string> Classes { get; set; } = new();
    public string InnerHtml { get; set; } = string.Empty;
    public HtmlElement? Parent { get; set; }
    public List<HtmlElement> Children { get; set; } = new();

                    public IEnumerable<HtmlElement> Descendants()
    {
        var queue = new Queue<HtmlElement>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

                public IEnumerable<HtmlElement> Ancestors()
    {
        var current = Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('<').Append(Name);
        if (!string.IsNullOrEmpty(Id))
            sb.Append(" id=\"").Append(Id).Append('"');
        if (Classes.Count > 0)
            sb.Append(" class=\"").Append(string.Join(' ', Classes)).Append('"');
        sb.Append('>');
        return sb.ToString();
    }
}
