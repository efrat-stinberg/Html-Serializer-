using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace HtmlSerializer
{
    internal class HtmlTreeBuilder
    {
        public HtmlElement BuildTree(string html)
        {
            var root = new HtmlElement { Name = "root" };
            var currentElement = root;

            var cleanHtml = Regex.Replace(html, @"\s+", " ");
            var htmlLines = Regex.Split(cleanHtml, @"(<[^>]+>)").Where(s => !string.IsNullOrEmpty(s)).ToArray();

            for (int i = 0; i < htmlLines.Length; i++)
            {
                var line = htmlLines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var firstWord = line.Split(' ')[0].Trim('<', '>');
                if (firstWord.Equals("html/", StringComparison.OrdinalIgnoreCase)) break;

                if (firstWord.StartsWith("/"))
                {
                    currentElement = currentElement.Parent; // Move up in the tree
                }
                else
                {
                    var isSelfClosing = line.EndsWith("/>") || HtmlHelper.Instance.SelfClosingTags.Contains(firstWord);
                    var newElement = CreateNewElement(firstWord, line, currentElement);
                    currentElement.Children.Add(newElement);

                    if (!isSelfClosing && i + 1 < htmlLines.Length && !htmlLines[i + 1].StartsWith("<"))
                    {
                        newElement.InnerText = htmlLines[i + 1].Trim();
                        i++;
                    }
                    if (!isSelfClosing)
                    {
                        currentElement = newElement;
                    }
                }
            }
            return root;
        }

        private HtmlElement CreateNewElement(string tagName, string originalLine, HtmlElement parent)
        {
            var newElement = new HtmlElement
            {
                Name = tagName,
                Parent = parent,
                OriginalLine = originalLine
            };
            ParseAttributes(originalLine, newElement);
            return newElement;
        }

        private void ParseAttributes(string line, HtmlElement element)
        {
            var attributesString = line.Substring(line.IndexOf(' ') + 1).Trim().TrimEnd('>');
            var attributePattern = @"(\w+)=""([^""]*)""";
            var matches = Regex.Matches(attributesString, attributePattern);
            foreach (Match match in matches)
            {
                if (match.Groups.Count == 3)
                {
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;
                    if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        element.Id = value;
                    }
                    else if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
                    {
                        element.Classes.AddRange(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                    }
                    else
                    {
                        element.Attributes.Add(new KeyValuePair<string, string>(key, value));
                    }
                }
            }
        }
    }
}
