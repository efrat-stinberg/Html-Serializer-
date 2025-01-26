using System;
using System.Collections.Generic;

namespace HtmlSerializer
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; private set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public Selector()
        {
            Classes = new List<string>();
        }

        public static Selector FromQueryString(string queryString)
        {
            var parts = queryString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var root = new Selector();
            var current = root;

            foreach (var part in parts)
            {
                var subParts = part.Split(new[] { '#', '.' }, StringSplitOptions.None);
                if (subParts.Length > 0)
                {
                    if (HtmlHelper.Instance.IsValidHtmlTag(subParts[0]))
                    {
                        current.TagName = subParts[0];
                    }
                    if (subParts.Length > 1 && !string.IsNullOrEmpty(subParts[1]))
                    {
                        current.Id = subParts[1];
                    }
                    for (int i = 2; i < subParts.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(subParts[i]))
                        {
                            current.Classes.Add(subParts[i]);
                        }
                    }
                }
                var child = new Selector();
                current.Child = child;
                child.Parent = current;
                current = child;
            }
            return root;
        }
    }
}
