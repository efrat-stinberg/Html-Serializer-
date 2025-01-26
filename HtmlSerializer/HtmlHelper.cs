using System;
using System.Collections.Generic;
using System.IO;

namespace HtmlSerializer
{
    internal class HtmlHelper
    {
        private static readonly HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] AllTags { get; private set; }
        public string[] SelfClosingTags { get; private set; }

        public HtmlHelper()
        {
            var allTagsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "HtmlTags.json");
            if (!File.Exists(allTagsPath))
            {
                throw new FileNotFoundException("HtmlTags.json file not found.", allTagsPath);
            }
            AllTags = System.Text.Json.JsonSerializer.Deserialize<string[]>(File.ReadAllText(allTagsPath));

            var selfClosingTagsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "HtmlVoidTags.json");
            if (!File.Exists(selfClosingTagsPath))
            {
                throw new FileNotFoundException("HtmlVoidTags.json file not found.", selfClosingTagsPath);
            }
            SelfClosingTags = System.Text.Json.JsonSerializer.Deserialize<string[]>(File.ReadAllText(selfClosingTagsPath));
        }

        // Check if the tag name is a valid HTML tag
        public bool IsValidHtmlTag(string tagName)
        {
            return Array.Exists(AllTags, tag => tag.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        }
    }
}