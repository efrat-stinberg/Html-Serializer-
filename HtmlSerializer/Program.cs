using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlSerializer;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Load HTML from a URL
        var html = await LoadHtmlAsync("https://www.example.com");

        // Check if the HTML was loaded successfully
        if (string.IsNullOrWhiteSpace(html))
        {
            Console.WriteLine("Failed to load HTML.");
            return;
        }

        // Build the HTML tree
        HtmlTreeBuilder treeBuilder = new HtmlTreeBuilder();
        HtmlElement root = treeBuilder.BuildTree(html);

        // Check the selector
        Selector selector = Selector.FromQueryString("title");
        HashSet<HtmlElement> elements = new HashSet<HtmlElement>();
        GetElementsBySelector(selector, root, elements);

        // Print the results
        if (elements.Count == 0)
        {
            Console.WriteLine("No elements found.");
        }
        else
        {
            foreach (var element in elements)
            {
                Console.WriteLine(BuildElementString(element));
            }
        }
    }

    // Load HTML from a URL
    public static async Task<string> LoadHtmlAsync(string url)
    {
        using HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    // Get elements by selector
    static void GetElementsBySelector(Selector selector, HtmlElement root, HashSet<HtmlElement> result)
    {
        if (selector == null)
            return;
        foreach (HtmlElement element in root.Descendants())
        {
            if (IsSame(selector, element))
            {
                result.Add(element);
            }
        }
    }

    // Check if the element matches the selector
    static bool IsSame(Selector selector, HtmlElement element)
    {
        if (!string.IsNullOrEmpty(selector.TagName) && selector.TagName != element.Name)
            return false;
        if (!string.IsNullOrEmpty(selector.Id) && selector.Id != element.Id)
            return false;
        if (selector.Classes.Count > 0 && !selector.Classes.All(element.Classes.Contains))
            return false;
        return true;
    }

    // Build the element string in the desired format
    static string BuildElementString(HtmlElement element)
    {
        string attributes = string.Empty;
        if (!string.IsNullOrEmpty(element.Id))
        {
            attributes += $" id=\"{element.Id}\"";
        }
        if (element.Classes.Count > 0)
        {
            attributes += $" class=\"{string.Join(" ", element.Classes)}\"";
        }
        return $"<{element.Name}{attributes}>{element.InnerText}</{element.Name}>";
    }
}
