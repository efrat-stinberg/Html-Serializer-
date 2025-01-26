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

        // Print the tree structure
        PrintTree(root);

        // Check the selector
        Selector selector = Selector.FromQueryString("div");
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
                Console.WriteLine($"Found Element: {element.Name}, ID: {element.Id}");
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

    // Print the tree structure
     static void PrintTree(HtmlElement element, int indent = 0)
    {
        Console.WriteLine(new string(' ', indent) + element.Name);
        foreach (var child in element.Children)
        {
            PrintTree(child, indent + 2);
        }
    }

    // Get elements by selector
     static void GetElementsBySelector(Selector selector, HtmlElement root, HashSet<HtmlElement> result)
    {
        if (selector == null)
            return;

        List<HtmlElement> list = new List<HtmlElement>();
        foreach (HtmlElement element in root.Descendants())
        {
            if (IsSame(selector, element))
            {
                list.Add(element);
                Console.WriteLine($"Matched Element: {element.Name}, ID: {element.Id}");
            }
        }

        foreach (HtmlElement element in list)
        {
            if (selector.Child == null)
                result.Add(element);
            else
                foreach (HtmlElement child in element.Descendants())
                {
                    GetElementsBySelector(selector.Child, child, result);
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
}