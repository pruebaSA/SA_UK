namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Windows;

    [FriendAccessAllowed]
    internal class InheritanceContextChangedEventManager : WeakEventManager
    {
        private InheritanceContextChangedEventManager()
        {
        }

        public static void AddListener(DependencyObject source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        private void OnInheritanceContextChanged(object sender, EventArgs args)
        {
            base.DeliverEvent(sender, args);
        }

        public static void RemoveListener(DependencyObject source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        protected override void StartListening(object source)
        {
            DependencyObject obj2 = (DependencyObject) source;
            obj2.InheritanceContextChanged += new EventHandler(this.OnInheritanceContextChanged);
        }

        protected override void StopListening(object source)
        {
            DependencyObject obj2 = (DependencyObject) source;
            obj2.InheritanceContextChanged -= new EventHandler(this.OnInheritanceContextChanged);
        }

        private static InheritanceContextChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(InheritanceContextChangedEventManager);
                InheritanceContextChangedEventManager currentManager = (InheritanceContextChangedEventManager) WeakEventManager.GetCurrentManager(managerType);
                if (currentManager == null)
                {
                    currentManager = new InheritanceContextChangedEventManager();
                    WeakEventManager.SetCurrentManager(managerType, currentManager);
                }
                return currentManager;
            }
        }
    }
}

