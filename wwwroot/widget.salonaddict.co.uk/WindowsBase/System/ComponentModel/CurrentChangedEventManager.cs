namespace System.ComponentModel
{
    using System;
    using System.Windows;

    public class CurrentChangedEventManager : WeakEventManager
    {
        private CurrentChangedEventManager()
        {
        }

        public static void AddListener(ICollectionView source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        private void OnCurrentChanged(object sender, EventArgs args)
        {
            base.DeliverEvent(sender, args);
        }

        public static void RemoveListener(ICollectionView source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        protected override void StartListening(object source)
        {
            ICollectionView view = (ICollectionView) source;
            view.CurrentChanged += new EventHandler(this.OnCurrentChanged);
        }

        protected override void StopListening(object source)
        {
            ICollectionView view = (ICollectionView) source;
            view.CurrentChanged -= new EventHandler(this.OnCurrentChanged);
        }

        private static CurrentChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(CurrentChangedEventManager);
                CurrentChangedEventManager currentManager = (CurrentChangedEventManager) WeakEventManager.GetCurrentManager(managerType);
                if (currentManager == null)
                {
                    currentManager = new CurrentChangedEventManager();
                    WeakEventManager.SetCurrentManager(managerType, currentManager);
                }
                return currentManager;
            }
        }
    }
}

