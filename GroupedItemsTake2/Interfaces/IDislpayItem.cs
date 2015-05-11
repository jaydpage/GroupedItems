using System;
using System.ComponentModel;

namespace GroupedItemsTake2.Interfaces
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