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

            // Clean up the HTML string and split into lines based on tags
            var cleanHtml = Regex.Replace(html, @"\s+", " ");
            var htmlLines = Regex.Split(cleanHtml, @"(<[^>]+>)").Where(s => !string.IsNullOrEmpty(s)).ToArray();

            foreach (var line in htmlLines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                var firstWord = trimmedLine.Split(' ')[0].Replace("<", "").Replace(">", "");

                if (firstWord.Equals("html/", StringComparison.OrdinalIgnoreCase))
                {
                    // End of HTML
                    break;
                }

                if (firstWord.StartsWith("/"))
                {
                    // Closing tag
                    currentElement = currentElement.Parent; // Move up in the tree
                }
                else
                {
                    // Opening tag
                    var isSelfClosing = trimmedLine.EndsWith("/>") || HtmlHelper.Instance.SelfClosingTags.Contains(firstWord);
                    var newElement = new HtmlElement { Name = firstWord, Parent = currentElement };
                    currentElement.Children.Add(newElement);

                    // Parse attributes
                    var attributesString = trimmedLine.Substring(firstWord.Length + 1).Trim().TrimEnd('>');
                    ParseAttributes(attributesString, newElement);

                    if (!isSelfClosing)
                    {
                        currentElement = newElement; // Move to the new element
                    }
                }
            }

            return root;
        }

        private void ParseAttributes(string attributesString, HtmlElement element)
        {
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
                        element.Id = value; // Set the ID of the element
                    }
                    else if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
                    {
                        element.Classes.AddRange(value.Split(' ')); // Add classes to the element
                    }
                    else
                    {
                        element.Attributes.Add(new KeyValuePair<string, string>(key, value)); // Add other attributes to the element
                    }
                }
            }
        }
    }
}