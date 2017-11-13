namespace System.ComponentModel
{
    using System;

    public interface IEditableCollectionView
    {
        object AddNew();
        void CancelEdit();
        void CancelNew();
        void CommitEdit();
        void CommitNew();
        void EditItem(object item);
        void Remove(object item);
        void RemoveAt(int index);

        bool CanAddNew { get; }

        bool CanCancelEdit { get; }

        bool CanRemove { get; }

        object CurrentAddItem { get; }

        object CurrentEditItem { get; }

        bool IsAddingNew { get; }

        bool IsEditingItem { get; }

        System.ComponentModel.NewItemPlaceholderPosition NewItemPlaceholderPosition { get; set; }
    }
}

