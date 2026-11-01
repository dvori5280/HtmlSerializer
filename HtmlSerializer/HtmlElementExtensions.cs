namespace HtmlSerializer;

public static class HtmlElementExtensions
{
                public static IEnumerable<HtmlElement> FindElements(this HtmlElement element, Selector selector)
    {
                var result = new HashSet<HtmlElement>();
        FindElements(element, selector, result, includeSelf: true);
        return result;
    }

    private static void FindElements(HtmlElement element, Selector selector,
                                     HashSet<HtmlElement> result, bool includeSelf)
    {
                        var descendants = element.Descendants();
        if (!includeSelf)
            descendants = descendants.Skip(1);

        var filtered = descendants.Where(selector.Matches).ToList();

                if (selector.Child == null)
        {
            foreach (var match in filtered)
                result.Add(match);
            return;
        }

        foreach (var match in filtered)
        {
            FindElements(match, selector.Child, result, includeSelf: false);
        }
    }
}
