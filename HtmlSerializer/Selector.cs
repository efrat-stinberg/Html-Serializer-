using System;
using System.Collections.Generic;

namespace HtmlSerializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public Selector()
        {
            Classes = new List<string>();
        }

        // Create a selector from a query string
        public static Selector FromQueryString(string queryString)
        {
            string[] parts = queryString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Selector root = new Selector();
            Selector current = root;

            foreach (var part in parts)
            {
                string[] subParts = part.Split(new[] { '#', '.' }, StringSplitOptions.None);

                if (subParts.Length > 0)
                {
                    // If the first part is an HTML tag name
                    if (HtmlHelper.Instance.IsValidHtmlTag(subParts[0]))
                    {
                        current.TagName = subParts[0];
                    }

                    // Check if there is an Id
                    if (subParts.Length > 1 && !string.IsNullOrEmpty(subParts[1]))
                    {
                        current.Id = subParts[1];
                    }

                    // Check if there are Classes
                    for (int i = 2; i < subParts.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(subParts[i]))
                        {
                            current.Classes.Add(subParts[i]);
                        }
                    }
                }

                // Create a new Selector object
                Selector child = new Selector();
                current.Child = child;
                child.Parent = current;
                current = child; // Update the current selector
            }

            return root;
        }
    }
}