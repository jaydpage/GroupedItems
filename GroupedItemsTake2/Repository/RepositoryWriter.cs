using System.Xml.Linq;
using GroupedItemsTake2.Domain;
using GroupedItemsTake2.Interfaces;

namespace GroupedItemsTake2.Repository
{
    public class RepositoryWriter
    {
        public void Write(DisplayCollection displayCollection, string path)
        {
            var items = new XElement("DisplayItems");

            foreach (var item in displayCollection)
            {
                items.Add(SerializeItem(item));
            }

            items.Save(path);
        }

        public XElement SerializeItem(IDisplayItem item)
        {
            var displayItem = new XElement("DisplayItem",
                new XElement("Name", item.Name ?? ""),
                new XElement("UID", item.UID ?? ""),
                new XElement("Level", item.Level));


            if (item.Level == Level.Parent || item.Level == Level.ParentChild)
            {
                var group = item as Group;
                var items = new XElement("Items");
                foreach (var child in group)
                {
                    items.Add(SerializeItem(child));                    
                }
                displayItem.Add(items);
            }

            return displayItem;
        }
    }
}