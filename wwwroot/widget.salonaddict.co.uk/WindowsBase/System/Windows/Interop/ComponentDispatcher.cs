namespace System.Windows.Interop
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    public static class ComponentDispatcher
    {
        private static LocalDataStoreSlot _threadSlot = Thread.AllocateDataSlot();

        public static  event EventHandler EnterThreadModal
        {
            [SecurityCritical] add
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                CurrentThreadData.EnterThreadModal += value;
            }
            [SecurityCritical] remove
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                CurrentThreadData.EnterThreadModal -= value;
            }
        }

        public static  event EventHandler LeaveThreadModal
        {
            [SecurityCritical] add
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                CurrentThreadData.LeaveThreadModal += value;
            }
            [SecurityCritical] remove
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                CurrentThreadData.LeaveThreadModal -= value;
            }
        }

        public static  event ThreadMessageEventHandler ThreadFilterMessage
        {
            [SecurityCritical] add
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                CurrentThreadData.ThreadFilterMessage += value;
            }
            [SecurityCritical] remove
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                CurrentThreadData.ThreadFilterMessage -= value;
            }
        }

        public static  event EventHandler ThreadIdle
        {
            [SecurityCritical] add
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                CurrentThreadData.ThreadIdle += value;
            }
            [SecurityCritical] remove
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                CurrentThreadData.ThreadIdle -= value;
            }
        }

        public static  event ThreadMessageEventHandler ThreadPreprocessMessage
        {
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] add
            {
                CurrentThreadData.ThreadPreprocessMessage += value;
            }
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] remove
            {
                CurrentThreadData.ThreadPreprocessMessage -= value;
            }
        }

        [SecurityCritical]
        public static void PopModal()
        {
            SecurityHelper.DemandUnrestrictedUIPermission();
            CurrentThreadData.PopModal();
        }

        [SecurityCritical]
        public static void PushModal()
        {
            SecurityHelper.DemandUnrestrictedUIPermission();
            CurrentThreadData.PushModal();
        }

        [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        public static void RaiseIdle()
        {
            CurrentThreadData.RaiseIdle();
        }

        [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        public static bool RaiseThreadMessage(ref MSG msg) => 
            CurrentThreadData.RaiseThreadMessage(ref msg);

        public static MSG CurrentKeyboardMessage
        {
            [SecurityCritical]
            get
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                return CurrentThreadData.CurrentKeyboardMessage;
            }
        }

        private static ComponentDispatcherThread CurrentThreadData
        {
            get
            {
                object data = Thread.GetData(_threadSlot);
                if (data == null)
                {
                    ComponentDispatcherThread thread = new ComponentDispatcherThread();
                    Thread.SetData(_threadSlot, thread);
                    return thread;
                }
                return (ComponentDispatcherThread) data;
            }
        }

        public static bool IsThreadModal
        {
            [SecurityCritical]
            get
            {
                SecurityHelper.DemandUnrestrictedUIPermission();
                return CurrentThreadData.IsThreadModal;
            }
        }

        internal static MSG UnsecureCurrentKeyboardMessage
        {
            [SecurityCritical, FriendAccessAllowed]
            get => 
                CurrentThreadData.CurrentKeyboardMessage;
            [FriendAccessAllowed, SecurityCritical]
            set
            {
                CurrentThreadData.CurrentKeyboardMessage = value;
            }
        }
    }
}

