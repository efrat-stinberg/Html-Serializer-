using System.Collections.Generic;

namespace HtmlSerializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<KeyValuePair<string, string>> Attributes { get; private set; }
        public List<string> Classes { get; private set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; private set; }
        public string OriginalLine { get; set; }
        public string InnerText { get; set; }

        public HtmlElement()
        {
            Attributes = new List<KeyValuePair<string, string>>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }

        // Get all descendants of the current element
        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        // Get all ancestors of the current element
        public IEnumerable<HtmlElement> Ancestors()
        {
            var current = this;
            while (current.Parent != null)
            {
                yield return current.Parent;
                current = current.Parent;
            }
        }
    }
}
