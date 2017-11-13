namespace System.ComponentModel
{
    using System.Collections.ObjectModel;

    public interface IItemProperties
    {
        ReadOnlyCollection<ItemPropertyInfo> ItemProperties { get; }
    }
}

