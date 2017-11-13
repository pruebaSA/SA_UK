namespace System.Windows.Threading
{
    using System;
    using System.Security;

    public class DispatcherFrame : DispatcherObject
    {
        private bool _continue;
        private bool _exitWhenRequested;

        public DispatcherFrame() : this(true)
        {
        }

        public DispatcherFrame(bool exitWhenRequested)
        {
            this._exitWhenRequested = exitWhenRequested;
            this._continue = true;
        }

        public bool Continue
        {
            get
            {
                bool flag = this._continue;
                if (!flag || !this._exitWhenRequested)
                {
                    return flag;
                }
                Dispatcher dispatcher = base.Dispatcher;
                return ((!dispatcher._exitAllFrames && !dispatcher._hasShutdownStarted) && flag);
            }
            [SecurityCritical]
            set
            {
                this._continue = value;
                base.Dispatcher.BeginInvoke(DispatcherPriority.Send, unused => null, null);
            }
        }
    }
}

