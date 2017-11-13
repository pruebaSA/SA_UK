namespace System.ComponentModel
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    public interface ICollectionView : IEnumerable, INotifyCollectionChanged
    {
        event EventHandler CurrentChanged;

        event CurrentChangingEventHandler CurrentChanging;

        bool Contains(object item);
        IDisposable DeferRefresh();
        bool MoveCurrentTo(object item);
        bool MoveCurrentToFirst();
        bool MoveCurrentToLast();
        bool MoveCurrentToNext();
        bool MoveCurrentToPosition(int position);
        bool MoveCurrentToPrevious();
        void Refresh();

        bool CanFilter { get; }

        bool CanGroup { get; }

        bool CanSort { get; }

        CultureInfo Culture { get; set; }

        object CurrentItem { get; }

        int CurrentPosition { get; }

        Predicate<object> Filter { get; set; }

        ObservableCollection<GroupDescription> GroupDescriptions { get; }

        ReadOnlyObservableCollection<object> Groups { get; }

        bool IsCurrentAfterLast { get; }

        bool IsCurrentBeforeFirst { get; }

        bool IsEmpty { get; }

        SortDescriptionCollection SortDescriptions { get; }

        IEnumerable SourceCollection { get; }
    }
}

