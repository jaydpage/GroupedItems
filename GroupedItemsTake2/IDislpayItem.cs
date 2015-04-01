using System;

namespace GroupedItemsTake2
{
    public interface IDisplayItem : ICloneable
    {
        string Name { get; set; }
        IDisplayItem Parent { get; }
        void SetParent(IDisplayItem parent);
        IDisplayItem Copy(); 
        Level Level { get; }
        string UID { get; }
    }

    public enum Level
    {
        Ungrouped,
        Child,
        Parent,
        ParentChild,
    }
}