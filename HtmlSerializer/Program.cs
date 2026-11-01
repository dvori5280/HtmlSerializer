
using HtmlSerializer;

var serializer = new HtmlSerializerService();

var html = """
<!DOCTYPE html>
<html>
    <body>
        <div id="main" class="container wide">
            <input disabled required>
            <p class="text highlight">Hello world</p>
            <div class="inner">
                <p class="text">Another paragraph</p>
            </div>
            <br/>
            <img src="image.png">
            <script>if (a < b) { console.log("test"); }</script>
        </div>
    </body>
</html>
""";

var root = serializer.Serialize(html);

Console.WriteLine("===== HTML SERIALIZER TESTS =====");

var divs = root.FindElements(
    Selector.FromQueryString("div"));

Console.WriteLine($"1. Div elements: {divs.Count()}");

var main = root.FindElements(
    Selector.FromQueryString("#main"));

Console.WriteLine($"2. Main element: {main.Count()}");

var textElements = root.FindElements(
    Selector.FromQueryString(".text"));

Console.WriteLine($"3. Text elements: {textElements.Count()}");

var specificDiv = root.FindElements(
    Selector.FromQueryString("div#main.container"));

Console.WriteLine($"4. Specific div: {specificDiv.Count()}");

var paragraphs = root.FindElements(
    Selector.FromQueryString("div p"));

Console.WriteLine($"5. Paragraphs inside div: {paragraphs.Count()}");

var inputs = root.FindElements(
    Selector.FromQueryString("input"));

Console.WriteLine($"6. Input elements: {inputs.Count()}");

var input = inputs.FirstOrDefault();

Console.WriteLine(
    $"7. Input attributes: {string.Join(", ", input?.Attributes ?? new List<string>())}");

var paragraph = root.FindElements(
    Selector.FromQueryString("p.text.highlight")).FirstOrDefault();

Console.WriteLine(
    $"8. Paragraph classes: {string.Join(", ", paragraph?.Classes ?? new List<string>())}");

var innerParagraph = root.FindElements(
    Selector.FromQueryString(".inner p")).FirstOrDefault();

Console.WriteLine(
    $"9. Ancestors: {string.Join(" -> ", innerParagraph?.Ancestors().Select(x => x.Name) ?? Enumerable.Empty<string>())}");

Console.WriteLine(
    $"10. Total descendants: {root.Descendants().Count()}");

var script = root.FindElements(
    Selector.FromQueryString("script")).FirstOrDefault();

Console.WriteLine(
    $"11. Script content: {script?.InnerHtml}");

var images = root.FindElements(
    Selector.FromQueryString("img"));

Console.WriteLine($"12. Image elements: {images.Count()}");

Console.WriteLine("===== TESTS COMPLETED =====");


