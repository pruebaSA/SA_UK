namespace System.Windows.Threading
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public class DispatcherTimer
    {
        private System.Windows.Threading.Dispatcher _dispatcher;
        internal int _dueTimeInTicks;
        private object _instanceLock;
        private TimeSpan _interval;
        private bool _isEnabled;
        private DispatcherOperation _operation;
        private DispatcherPriority _priority;
        private object _tag;

        public event EventHandler Tick;

        public DispatcherTimer() : this(DispatcherPriority.Background)
        {
        }

        public DispatcherTimer(DispatcherPriority priority)
        {
            this._instanceLock = new object();
            this.Initialize(System.Windows.Threading.Dispatcher.CurrentDispatcher, priority, TimeSpan.FromMilliseconds(0.0));
        }

        public DispatcherTimer(DispatcherPriority priority, System.Windows.Threading.Dispatcher dispatcher)
        {
            this._instanceLock = new object();
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            this.Initialize(dispatcher, priority, TimeSpan.FromMilliseconds(0.0));
        }

        public DispatcherTimer(TimeSpan interval, DispatcherPriority priority, EventHandler callback, System.Windows.Threading.Dispatcher dispatcher)
        {
            this._instanceLock = new object();
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            if (interval.TotalMilliseconds < 0.0)
            {
                throw new ArgumentOutOfRangeException("interval", System.Windows.SR.Get("TimeSpanPeriodOutOfRange_TooSmall"));
            }
            if (interval.TotalMilliseconds > 2147483647.0)
            {
                throw new ArgumentOutOfRangeException("interval", System.Windows.SR.Get("TimeSpanPeriodOutOfRange_TooLarge"));
            }
            this.Initialize(dispatcher, priority, interval);
            this.Tick = (EventHandler) Delegate.Combine(this.Tick, callback);
            this.Start();
        }

        private object FireTick(object unused)
        {
            this._operation = null;
            if (this.Tick != null)
            {
                this.Tick(this, EventArgs.Empty);
            }
            if (this._isEnabled)
            {
                this.Restart();
            }
            return null;
        }

        private void Initialize(System.Windows.Threading.Dispatcher dispatcher, DispatcherPriority priority, TimeSpan interval)
        {
            System.Windows.Threading.Dispatcher.ValidatePriority(priority, "priority");
            if (priority == DispatcherPriority.Inactive)
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPriority"), "priority");
            }
            this._dispatcher = dispatcher;
            this._priority = priority;
            this._interval = interval;
        }

        internal void Promote()
        {
            lock (this._instanceLock)
            {
                if (this._operation != null)
                {
                    this._operation.Priority = this._priority;
                }
            }
        }

        private void Restart()
        {
            lock (this._instanceLock)
            {
                if (this._operation == null)
                {
                    this._operation = this._dispatcher.BeginInvoke(DispatcherPriority.Inactive, new DispatcherOperationCallback(this.FireTick), null);
                    this._dueTimeInTicks = Environment.TickCount + ((int) this._interval.TotalMilliseconds);
                    if ((this._interval.TotalMilliseconds == 0.0) && this._dispatcher.CheckAccess())
                    {
                        this.Promote();
                    }
                    else
                    {
                        this._dispatcher.AddTimer(this);
                    }
                }
            }
        }

        public void Start()
        {
            lock (this._instanceLock)
            {
                if (!this._isEnabled)
                {
                    this._isEnabled = true;
                    this.Restart();
                }
            }
        }

        public void Stop()
        {
            bool flag = false;
            lock (this._instanceLock)
            {
                if (this._isEnabled)
                {
                    this._isEnabled = false;
                    flag = true;
                    if (this._operation != null)
                    {
                        this._operation.Abort();
                        this._operation = null;
                    }
                }
            }
            if (flag)
            {
                this._dispatcher.RemoveTimer(this);
            }
        }

        public System.Windows.Threading.Dispatcher Dispatcher =>
            this._dispatcher;

        public TimeSpan Interval
        {
            get => 
                this._interval;
            set
            {
                bool flag = false;
                if (value.TotalMilliseconds < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", System.Windows.SR.Get("TimeSpanPeriodOutOfRange_TooSmall"));
                }
                if (value.TotalMilliseconds > 2147483647.0)
                {
                    throw new ArgumentOutOfRangeException("value", System.Windows.SR.Get("TimeSpanPeriodOutOfRange_TooLarge"));
                }
                lock (this._instanceLock)
                {
                    this._interval = value;
                    if (this._isEnabled)
                    {
                        this._dueTimeInTicks = Environment.TickCount + ((int) this._interval.TotalMilliseconds);
                        flag = true;
                    }
                }
                if (flag)
                {
                    this._dispatcher.UpdateWin32Timer();
                }
            }
        }

        public bool IsEnabled
        {
            get => 
                this._isEnabled;
            set
            {
                lock (this._instanceLock)
                {
                    if (!value && this._isEnabled)
                    {
                        this.Stop();
                    }
                    else if (value && !this._isEnabled)
                    {
                        this.Start();
                    }
                }
            }
        }

        public object Tag
        {
            get => 
                this._tag;
            set
            {
                this._tag = value;
            }
        }
    }
}

