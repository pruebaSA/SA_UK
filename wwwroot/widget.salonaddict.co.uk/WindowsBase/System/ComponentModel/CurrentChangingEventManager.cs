namespace System.ComponentModel
{
    using System;
    using System.Windows;

    public class CurrentChangingEventManager : WeakEventManager
    {
        private CurrentChangingEventManager()
        {
        }

        public static void AddListener(ICollectionView source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        private void OnCurrentChanging(object sender, CurrentChangingEventArgs args)
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
            view.CurrentChanging += new CurrentChangingEventHandler(this.OnCurrentChanging);
        }

        protected override void StopListening(object source)
        {
            ICollectionView view = (ICollectionView) source;
            view.CurrentChanging -= new CurrentChangingEventHandler(this.OnCurrentChanging);
        }

        private static CurrentChangingEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(CurrentChangingEventManager);
                CurrentChangingEventManager currentManager = (CurrentChangingEventManager) WeakEventManager.GetCurrentManager(managerType);
                if (currentManager == null)
                {
                    currentManager = new CurrentChangingEventManager();
                    WeakEventManager.SetCurrentManager(managerType, currentManager);
                }
                return currentManager;
            }
        }
    }
}

