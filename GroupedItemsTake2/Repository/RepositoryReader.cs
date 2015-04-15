using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GroupedItemsTake2.Domain;
using GroupedItemsTake2.Interfaces;

namespace GroupedItemsTake2.Repository
{
    public class RepositoryReader
    {
        public IEnumerable<IDisplayItem> Read(string path)
        {
            var xmlDoc = XDocument.Load(path);
            var items = LoadItemsFromXmlData(xmlDoc.Element("DisplayItems"), null);
            return items;
        }

        private IEnumerable<IDisplayItem> LoadItemsFromXmlData(XElement element, IDisplayItem parent)
        {
            var xmlItems = element.Elements("DisplayItem");
            var items = xmlItems.Select(x => CreateFromXmlString(x, parent)).ToList();
            return items;
        }

        public IDisplayItem CreateFromXmlString(XElement xElement, IDisplayItem parent)
        {
            var item = XElement.Parse(xElement.ToString());
            var level = item.Element("Level").Value;
            if (IsOfTypeItem(level)) return CreateItem(item, null);                
            return CreateGroup(item, parent);                                        
        }

        private static bool IsOfTypeItem(string level)
        {
            return level == Level.Ungrouped.ToString() || level == Level.Child.ToString();
        }

        private IDisplayItem CreateItem(XElement item, IDisplayItem parent)
        {
            return new Item(item.Element("Name").Value, parent);
        }

        private IDisplayItem CreateGroup(XElement item, IDisplayItem parent)
        {
           var group = new Group(item.Element("Name").Value, parent);
           var itemsToAdd = LoadItemsFromXmlData(item.Element("Items"), group);
           group.AddRange(itemsToAdd);
           return group;
        }

    }
}