namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data.Objects.DataClasses;

    internal sealed class ObjectViewListener
    {
        private object _dataSource;
        private IList _list;
        private WeakReference _viewWeak;

        internal ObjectViewListener(IObjectView view, IList list, object dataSource)
        {
            this._viewWeak = new WeakReference(view);
            this._dataSource = dataSource;
            this._list = list;
            this.RegisterCollectionEvents();
            this.RegisterEntityEvents();
        }

        private void CleanUpListener()
        {
            this.UnregisterCollectionEvents();
            this.UnregisterEntityEvents();
        }

        private void CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            IObjectView target = (IObjectView) this._viewWeak.Target;
            if (target != null)
            {
                target.CollectionChanged(sender, e);
            }
            else
            {
                this.CleanUpListener();
            }
        }

        private void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IObjectView target = (IObjectView) this._viewWeak.Target;
            if (target != null)
            {
                target.EntityPropertyChanged(sender, e);
            }
            else
            {
                this.CleanUpListener();
            }
        }

        private void RegisterCollectionEvents()
        {
            ObjectStateManager manager = this._dataSource as ObjectStateManager;
            if (manager != null)
            {
                manager.EntityDeleted += new CollectionChangeEventHandler(this.CollectionChanged);
            }
            else if (this._dataSource != null)
            {
                ((RelatedEnd) this._dataSource).AssociationChangedForObjectView += new CollectionChangeEventHandler(this.CollectionChanged);
            }
        }

        private void RegisterEntityEvents()
        {
            if (this._list != null)
            {
                foreach (object obj2 in this._list)
                {
                    IEntityWithChangeTracker tracker = obj2 as IEntityWithChangeTracker;
                    if (tracker != null)
                    {
                        INotifyPropertyChanged changed = tracker as INotifyPropertyChanged;
                        if (changed != null)
                        {
                            changed.PropertyChanged += new PropertyChangedEventHandler(this.EntityPropertyChanged);
                        }
                    }
                }
            }
        }

        internal void RegisterEntityEvents(object entity)
        {
            INotifyPropertyChanged changed = entity as INotifyPropertyChanged;
            if (changed != null)
            {
                changed.PropertyChanged += new PropertyChangedEventHandler(this.EntityPropertyChanged);
            }
        }

        private void UnregisterCollectionEvents()
        {
            ObjectStateManager manager = this._dataSource as ObjectStateManager;
            if (manager != null)
            {
                manager.EntityDeleted -= new CollectionChangeEventHandler(this.CollectionChanged);
            }
            else if (this._dataSource != null)
            {
                ((RelatedEnd) this._dataSource).AssociationChangedForObjectView -= new CollectionChangeEventHandler(this.CollectionChanged);
            }
        }

        private void UnregisterEntityEvents()
        {
            if (this._list != null)
            {
                foreach (object obj2 in this._list)
                {
                    IEntityWithChangeTracker tracker = obj2 as IEntityWithChangeTracker;
                    if (tracker != null)
                    {
                        INotifyPropertyChanged changed = tracker as INotifyPropertyChanged;
                        if (changed != null)
                        {
                            changed.PropertyChanged -= new PropertyChangedEventHandler(this.EntityPropertyChanged);
                        }
                    }
                }
            }
        }

        internal void UnregisterEntityEvents(object entity)
        {
            INotifyPropertyChanged changed = entity as INotifyPropertyChanged;
            if (changed != null)
            {
                changed.PropertyChanged -= new PropertyChangedEventHandler(this.EntityPropertyChanged);
            }
        }
    }
}

