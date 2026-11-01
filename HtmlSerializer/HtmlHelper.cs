using System.Text.Json;

namespace HtmlSerializer;

public class HtmlHelper
{
    private static readonly Lazy<HtmlHelper> _instance = new(() => new HtmlHelper());

    public static HtmlHelper Instance => _instance.Value;

        public string[] HtmlTags { get; private set; }

        public string[] HtmlVoidTags { get; private set; }

    private HtmlHelper()
    {
        HtmlTags = LoadArray("Data/HtmlTags.json");
        HtmlVoidTags = LoadArray("Data/HtmlVoidTags.json");
    }

    private static string[] LoadArray(string relativePath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, relativePath);
        var content = File.ReadAllText(path);
        return JsonSerializer.Deserialize<string[]>(content) ?? Array.Empty<string>();
    }

    public bool IsHtmlTag(string name) =>
        HtmlTags.Contains(name, StringComparer.OrdinalIgnoreCase);

    public bool IsVoidTag(string name) =>
        HtmlVoidTags.Contains(name, StringComparer.OrdinalIgnoreCase);
}
