using System.Collections.Generic;

namespace HtmlSerializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<KeyValuePair<string, string>> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement()
        {
            Attributes = new List<KeyValuePair<string, string>>();
            Classes = new List<string>();
            Children = new List<HtmlElement>();
        }

        // Get all descendants of the current element
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this); // Start with the current element

            while (queue.Count > 0)
            {
                HtmlElement current = queue.Dequeue(); // Get the element at the front of the queue
                yield return current; // Return the current element

                // Enqueue all children of the current element
                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
    }
}