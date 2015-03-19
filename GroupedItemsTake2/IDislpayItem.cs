using System;

namespace GroupedItemsTake2
{
    public interface IDislpayItem : ICloneable
    {
        string Name { get; set; }
        IDislpayItem Parent { get; }
        void SetParent(IDislpayItem parent);
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