namespace System.Collections.Specialized
{
    using System;
    using System.Windows;

    public class CollectionChangedEventManager : WeakEventManager
    {
        private CollectionChangedEventManager()
        {
        }

        public static void AddListener(INotifyCollectionChanged source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            base.DeliverEvent(sender, args);
        }

        public static void RemoveListener(INotifyCollectionChanged source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        protected override void StartListening(object source)
        {
            INotifyCollectionChanged changed = (INotifyCollectionChanged) source;
            changed.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        }

        protected override void StopListening(object source)
        {
            INotifyCollectionChanged changed = (INotifyCollectionChanged) source;
            changed.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        }

        private static CollectionChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(CollectionChangedEventManager);
                CollectionChangedEventManager currentManager = (CollectionChangedEventManager) WeakEventManager.GetCurrentManager(managerType);
                if (currentManager == null)
                {
                    currentManager = new CollectionChangedEventManager();
                    WeakEventManager.SetCurrentManager(managerType, currentManager);
                }
                return currentManager;
            }
        }
    }
}

