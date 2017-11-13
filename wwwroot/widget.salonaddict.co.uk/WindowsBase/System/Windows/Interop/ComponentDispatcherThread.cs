namespace System.Windows.Interop
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security;

    internal class ComponentDispatcherThread
    {
        [SecurityCritical]
        private MSG _currentKeyboardMSG;
        private int _modalCount;

        [SecurityCritical]
        private event EventHandler _enterThreadModal;

        [SecurityCritical]
        private event EventHandler _leaveThreadModal;

        [SecurityCritical]
        private event ThreadMessageEventHandler _threadFilterMessage;

        [SecurityCritical]
        private event EventHandler _threadIdle;

        [SecurityCritical]
        private event ThreadMessageEventHandler _threadPreprocessMessage;

        public event EventHandler EnterThreadModal;

        public event EventHandler LeaveThreadModal;

        public event ThreadMessageEventHandler ThreadFilterMessage;

        public event EventHandler ThreadIdle;

        public event ThreadMessageEventHandler ThreadPreprocessMessage;

        [SecurityCritical]
        public void PopModal()
        {
            this._modalCount--;
            if ((this._modalCount == 0) && (this._leaveThreadModal != null))
            {
                this._leaveThreadModal(null, EventArgs.Empty);
            }
            if (this._modalCount < 0)
            {
                this._modalCount = 0;
            }
        }

        [SecurityCritical]
        public void PushModal()
        {
            this._modalCount++;
            if ((1 == this._modalCount) && (this._enterThreadModal != null))
            {
                this._enterThreadModal(null, EventArgs.Empty);
            }
        }

        [SecurityCritical]
        public void RaiseIdle()
        {
            if (this._threadIdle != null)
            {
                this._threadIdle(null, EventArgs.Empty);
            }
        }

        [SecurityCritical]
        public bool RaiseThreadMessage(ref MSG msg)
        {
            bool handled = false;
            if (this._threadFilterMessage != null)
            {
                this._threadFilterMessage(ref msg, ref handled);
            }
            if (!handled && (this._threadPreprocessMessage != null))
            {
                this._threadPreprocessMessage(ref msg, ref handled);
            }
            return handled;
        }

        public MSG CurrentKeyboardMessage
        {
            [SecurityCritical]
            get => 
                this._currentKeyboardMSG;
            [SecurityCritical]
            set
            {
                this._currentKeyboardMSG = value;
            }
        }

        public bool IsThreadModal =>
            (this._modalCount > 0);
    }
}

