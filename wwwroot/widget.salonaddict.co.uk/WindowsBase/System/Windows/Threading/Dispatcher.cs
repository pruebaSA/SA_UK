namespace System.Windows.Threading
{
    using MS.Internal;
    using MS.Internal.WindowsBase;
    using MS.Utility;
    using MS.Win32;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Interop;

    public sealed class Dispatcher
    {
        private static PriorityRange _backgroundPriorityRange = new PriorityRange(DispatcherPriority.Background, true, DispatcherPriority.Input, true);
        internal int _disableProcessingCount;
        private static List<WeakReference> _dispatchers = new List<WeakReference>();
        private System.Threading.Thread _dispatcherThread;
        private bool _dueTimeFound;
        private int _dueTimeInTicks;
        private DispatcherUnhandledExceptionFilterEventArgs _exceptionFilterEventArgs;
        private static ExceptionWrapper _exceptionWrapper = new ExceptionWrapper();
        internal bool _exitAllFrames;
        private static PriorityRange _foregroundPriorityRange = new PriorityRange(DispatcherPriority.Loaded, true, DispatcherPriority.Send, true);
        private int _frameDepth;
        private static object _globalLock = new object();
        private bool _hasShutdownFinished;
        internal bool _hasShutdownStarted;
        [SecurityCritical]
        private HwndWrapperHook _hook;
        [SecurityCritical]
        private DispatcherHooks _hooks;
        private static PriorityRange _idlePriorityRange = new PriorityRange(DispatcherPriority.SystemIdle, true, DispatcherPriority.ContextIdle, true);
        internal object _instanceLock = new object();
        private bool _isTSFMessagePumpEnabled;
        private bool _isWin32TimerSet;
        [SecurityCritical]
        private static int _msgProcessQueue = MS.Win32.UnsafeNativeMethods.RegisterWindowMessage("DispatcherProcessQueue");
        private static WeakReference _possibleDispatcher = new WeakReference(null);
        private int _postedProcessingType;
        private PriorityQueue<DispatcherOperation> _queue = new PriorityQueue<DispatcherOperation>();
        private object _reserved0;
        private object _reserved1;
        private object _reserved2;
        private object _reserved3;
        private object _reservedInputManager;
        private object _reservedInputMethod;
        private object _reservedPtsCache;
        private SecurityCriticalDataClass<ExecutionContext> _shutdownExecutionContext;
        private bool _startingShutdown;
        private List<DispatcherTimer> _timers = new List<DispatcherTimer>();
        private long _timersVersion;
        [ThreadStatic]
        private static Dispatcher _tlsDispatcher;
        private DispatcherUnhandledExceptionEventArgs _unhandledExceptionEventArgs;
        private SecurityCriticalData<MessageOnlyHwndWrapper> _window;
        private static readonly object ExceptionDataKey = new object();
        private const int PROCESS_BACKGROUND = 1;
        private const int PROCESS_FOREGROUND = 2;
        private const int PROCESS_NONE = 0;

        public event EventHandler ShutdownFinished;

        public event EventHandler ShutdownStarted;

        public event DispatcherUnhandledExceptionEventHandler UnhandledException;

        [field: SecurityCritical]
        public event DispatcherUnhandledExceptionFilterEventHandler UnhandledExceptionFilter;

        [SecurityCritical, SecurityTreatAsSafe]
        static Dispatcher()
        {
            _exceptionWrapper.Catch += new ExceptionWrapper.CatchHandler(Dispatcher.CatchExceptionStatic);
            _exceptionWrapper.Filter += new ExceptionWrapper.FilterHandler(Dispatcher.ExceptionFilterStatic);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private Dispatcher()
        {
            _tlsDispatcher = this;
            this._dispatcherThread = System.Threading.Thread.CurrentThread;
            lock (_globalLock)
            {
                _dispatchers.Add(new WeakReference(this));
            }
            this._unhandledExceptionEventArgs = new DispatcherUnhandledExceptionEventArgs(this);
            this._exceptionFilterEventArgs = new DispatcherUnhandledExceptionFilterEventArgs(this);
            MessageOnlyHwndWrapper wrapper = new MessageOnlyHwndWrapper();
            this._window = new SecurityCriticalData<MessageOnlyHwndWrapper>(wrapper);
            this._hook = new HwndWrapperHook(this.WndProcHook);
            this._window.Value.AddHook(this._hook);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal bool Abort(DispatcherOperation operation)
        {
            bool flag = false;
            DispatcherHooks hooks = null;
            lock (this._instanceLock)
            {
                if ((this._queue != null) && operation._item.IsQueued)
                {
                    this._queue.RemoveItem(operation._item);
                    operation._status = DispatcherOperationStatus.Aborted;
                    flag = true;
                    hooks = this._hooks;
                }
            }
            if (flag)
            {
                if (hooks != null)
                {
                    hooks.RaiseOperationAborted(this, operation);
                }
                if (EventTrace.IsEnabled(EventTrace.Flags.performance, EventTrace.Level.normal))
                {
                    EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.DISPATCHERABORTGUID), 0, operation.Priority, operation.Name);
                }
            }
            return flag;
        }

        internal void AddTimer(DispatcherTimer timer)
        {
            lock (this._instanceLock)
            {
                if (!this._hasShutdownFinished)
                {
                    this._timers.Add(timer);
                    this._timersVersion += 1L;
                }
            }
            this.UpdateWin32Timer();
        }

        public DispatcherOperation BeginInvoke(Delegate method, params object[] args) => 
            this.BeginInvokeImpl(DispatcherPriority.Normal, method, args, false);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method) => 
            this.BeginInvokeImpl(priority, method, null, false);

        public DispatcherOperation BeginInvoke(Delegate method, DispatcherPriority priority, params object[] args) => 
            this.BeginInvokeImpl(priority, method, args, false);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg) => 
            this.BeginInvokeImpl(priority, method, arg, true);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DispatcherOperation BeginInvoke(DispatcherPriority priority, Delegate method, object arg, params object[] args) => 
            this.BeginInvokeImpl(priority, method, this.CombineParameters(arg, args), false);

        [FriendAccessAllowed, SecurityTreatAsSafe, SecurityCritical]
        internal DispatcherOperation BeginInvokeImpl(DispatcherPriority priority, Delegate method, object args, bool isSingleParameter)
        {
            ValidatePriority(priority, "priority");
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            DispatcherOperation data = null;
            DispatcherHooks hooks = null;
            bool flag = false;
            lock (this._instanceLock)
            {
                if (!this._hasShutdownFinished && !Environment.HasShutdownStarted)
                {
                    data = new DispatcherOperation(this, method, priority, args, isSingleParameter);
                    data._item = this._queue.Enqueue(priority, data);
                    flag = this.RequestProcessing();
                    if (flag)
                    {
                        hooks = this._hooks;
                    }
                    else
                    {
                        this._queue.RemoveItem(data._item);
                        data._status = DispatcherOperationStatus.Aborted;
                    }
                }
                else
                {
                    data = new DispatcherOperation(this, method, priority);
                }
            }
            if ((data != null) && flag)
            {
                if (hooks != null)
                {
                    hooks.RaiseOperationPosted(this, data);
                }
                if (EventTrace.IsEnabled(EventTrace.Flags.performance, EventTrace.Level.normal))
                {
                    EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.DISPATCHERPOSTGUID), 0, priority, data.Name);
                }
            }
            return data;
        }

        [SecurityCritical]
        public void BeginInvokeShutdown(DispatcherPriority priority)
        {
            SecurityHelper.DemandUnrestrictedUIPermission();
            this.BeginInvoke(priority, new ShutdownCallback(this.ShutdownCallbackInternal));
        }

        private bool CatchException(Exception e)
        {
            bool handled = false;
            if (this.UnhandledException != null)
            {
                this._unhandledExceptionEventArgs.Initialize(e, false);
                bool flag2 = false;
                try
                {
                    this.UnhandledException(this, this._unhandledExceptionEventArgs);
                    handled = this._unhandledExceptionEventArgs.Handled;
                    flag2 = true;
                }
                finally
                {
                    if (!flag2)
                    {
                        handled = false;
                    }
                }
            }
            return handled;
        }

        private static bool CatchExceptionStatic(object source, Exception e)
        {
            Dispatcher dispatcher = (Dispatcher) source;
            return dispatcher.CatchException(e);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CheckAccess() => 
            (this.Thread == System.Threading.Thread.CurrentThread);

        private object[] CombineParameters(object arg, object[] args)
        {
            object[] destinationArray = new object[1 + ((args == null) ? 1 : args.Length)];
            destinationArray[0] = arg;
            if (args != null)
            {
                Array.Copy(args, 0, destinationArray, 1, args.Length);
                return destinationArray;
            }
            destinationArray[1] = null;
            return destinationArray;
        }

        [FriendAccessAllowed, SecurityCritical]
        internal void CriticalInvokeShutdown()
        {
            this.Invoke(DispatcherPriority.Send, new ShutdownCallback(this.ShutdownCallbackInternal));
        }

        public DispatcherProcessingDisabled DisableProcessing()
        {
            this.VerifyAccess();
            this._disableProcessingCount++;
            return new DispatcherProcessingDisabled { _dispatcher = this };
        }

        [SecurityCritical]
        private bool ExceptionFilter(Exception e)
        {
            if (!e.Data.Contains(ExceptionDataKey))
            {
                e.Data.Add(ExceptionDataKey, null);
            }
            else
            {
                return false;
            }
            bool hasUnhandledExceptionHandler = this.HasUnhandledExceptionHandler;
            if (this._unhandledExceptionFilter != null)
            {
                this._exceptionFilterEventArgs.Initialize(e, hasUnhandledExceptionHandler);
                bool flag2 = false;
                try
                {
                    this._unhandledExceptionFilter(this, this._exceptionFilterEventArgs);
                    flag2 = true;
                }
                finally
                {
                    if (flag2)
                    {
                        hasUnhandledExceptionHandler = this._exceptionFilterEventArgs.RequestCatch;
                    }
                }
            }
            return hasUnhandledExceptionHandler;
        }

        [SecurityCritical]
        private static bool ExceptionFilterStatic(object source, Exception e)
        {
            Dispatcher dispatcher = (Dispatcher) source;
            return dispatcher.ExceptionFilter(e);
        }

        [SecurityCritical]
        public static void ExitAllFrames()
        {
            SecurityHelper.DemandUnrestrictedUIPermission();
            Dispatcher currentDispatcher = CurrentDispatcher;
            if (currentDispatcher._frameDepth > 0)
            {
                currentDispatcher._exitAllFrames = true;
                currentDispatcher.BeginInvoke(DispatcherPriority.Send, unused => null, null);
            }
        }

        public static Dispatcher FromThread(System.Threading.Thread thread)
        {
            lock (_globalLock)
            {
                Dispatcher target = null;
                if (thread != null)
                {
                    target = _possibleDispatcher.Target as Dispatcher;
                    if ((target == null) || (target.Thread != thread))
                    {
                        target = null;
                        for (int i = 0; i < _dispatchers.Count; i++)
                        {
                            Dispatcher dispatcher2 = _dispatchers[i].Target as Dispatcher;
                            if (dispatcher2 != null)
                            {
                                if (dispatcher2.Thread == thread)
                                {
                                    target = dispatcher2;
                                }
                            }
                            else
                            {
                                _dispatchers.RemoveAt(i);
                                i--;
                            }
                        }
                        if (target != null)
                        {
                            _possibleDispatcher.Target = target;
                        }
                    }
                }
                return target;
            }
        }

        [SecurityCritical]
        private bool GetMessage(ref MSG msg, IntPtr hwnd, int minMessage, int maxMessage)
        {
            bool flag;
            MS.Win32.UnsafeNativeMethods.ITfMessagePump messagePump = this.GetMessagePump();
            try
            {
                if (messagePump == null)
                {
                    return MS.Win32.UnsafeNativeMethods.GetMessageW(ref msg, new HandleRef(this, hwnd), minMessage, maxMessage);
                }
                messagePump.GetMessageW(ref msg, (int) hwnd, minMessage, maxMessage, out flag);
            }
            finally
            {
                if (messagePump != null)
                {
                    Marshal.ReleaseComObject(messagePump);
                }
            }
            return flag;
        }

        [SecurityCritical]
        private MS.Win32.UnsafeNativeMethods.ITfMessagePump GetMessagePump()
        {
            MS.Win32.UnsafeNativeMethods.ITfMessagePump pump = null;
            if ((this._isTSFMessagePumpEnabled && (System.Threading.Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)) && TextServicesLoader.ServicesInstalled)
            {
                MS.Win32.UnsafeNativeMethods.ITfThreadMgr mgr = TextServicesLoader.Load();
                if (mgr != null)
                {
                    pump = mgr as MS.Win32.UnsafeNativeMethods.ITfMessagePump;
                }
            }
            return pump;
        }

        public object Invoke(Delegate method, params object[] args) => 
            this.InvokeImpl(DispatcherPriority.Normal, TimeSpan.FromMilliseconds(-1.0), method, args, false);

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public object Invoke(DispatcherPriority priority, Delegate method) => 
            this.InvokeImpl(priority, TimeSpan.FromMilliseconds(-1.0), method, null, false);

        public object Invoke(Delegate method, TimeSpan timeout, params object[] args) => 
            this.InvokeImpl(DispatcherPriority.Normal, timeout, method, args, false);

        public object Invoke(Delegate method, DispatcherPriority priority, params object[] args) => 
            this.InvokeImpl(priority, TimeSpan.FromMilliseconds(-1.0), method, args, false);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Invoke(DispatcherPriority priority, Delegate method, object arg) => 
            this.InvokeImpl(priority, TimeSpan.FromMilliseconds(-1.0), method, arg, true);

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public object Invoke(DispatcherPriority priority, TimeSpan timeout, Delegate method) => 
            this.InvokeImpl(priority, timeout, method, null, false);

        public object Invoke(Delegate method, TimeSpan timeout, DispatcherPriority priority, params object[] args) => 
            this.InvokeImpl(priority, timeout, method, args, false);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Invoke(DispatcherPriority priority, Delegate method, object arg, params object[] args) => 
            this.InvokeImpl(priority, TimeSpan.FromMilliseconds(-1.0), method, this.CombineParameters(arg, args), false);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Invoke(DispatcherPriority priority, TimeSpan timeout, Delegate method, object arg) => 
            this.InvokeImpl(priority, timeout, method, arg, true);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object Invoke(DispatcherPriority priority, TimeSpan timeout, Delegate method, object arg, params object[] args) => 
            this.InvokeImpl(priority, timeout, method, this.CombineParameters(arg, args), false);

        [FriendAccessAllowed, SecurityCritical, SecurityTreatAsSafe]
        internal object InvokeImpl(DispatcherPriority priority, TimeSpan timeout, Delegate method, object args, bool isSingleParameter)
        {
            ValidatePriority(priority, "priority");
            if (priority == DispatcherPriority.Inactive)
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidPriority"), "priority");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if ((priority == DispatcherPriority.Send) && this.CheckAccess())
            {
                SynchronizationContext current = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(this));
                    return this.WrappedInvoke(method, args, isSingleParameter);
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(current);
                }
            }
            DispatcherOperation operation = this.BeginInvokeImpl(priority, method, args, isSingleParameter);
            if (operation != null)
            {
                operation.Wait(timeout);
                if (operation.Status == DispatcherOperationStatus.Completed)
                {
                    return operation.Result;
                }
                if (operation.Status == DispatcherOperationStatus.Aborted)
                {
                    return null;
                }
                operation.Abort();
            }
            return null;
        }

        [SecurityCritical]
        public void InvokeShutdown()
        {
            SecurityHelper.DemandUnrestrictedUIPermission();
            this.CriticalInvokeShutdown();
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private bool IsInputPending() => 
            (MS.Win32.UnsafeNativeMethods.MsgWaitForMultipleObjectsEx(0, null, 0, 0x200f, 4) == 0);

        [SecurityTreatAsSafe, SecurityCritical]
        private bool IsWindowNull() => 
            (this._window.Value == null);

        [SecurityTreatAsSafe, SecurityCritical]
        private void KillWin32Timer()
        {
            if (!this.IsWindowNull())
            {
                SafeNativeMethods.KillTimer(new HandleRef(this, this._window.Value.Handle), 2);
                this._isWin32TimerSet = false;
            }
        }

        [SecurityCritical]
        private void ProcessQueue()
        {
            DispatcherPriority invalid = DispatcherPriority.Invalid;
            DispatcherOperation operation = null;
            DispatcherHooks hooks = null;
            lock (this._instanceLock)
            {
                this._postedProcessingType = 0;
                bool flag = !this.IsInputPending();
                invalid = this._queue.MaxPriority;
                if (((invalid != DispatcherPriority.Invalid) && (invalid != DispatcherPriority.Inactive)) && (_foregroundPriorityRange.Contains(invalid) || flag))
                {
                    operation = this._queue.Dequeue();
                    hooks = this._hooks;
                }
                invalid = this._queue.MaxPriority;
                this.RequestProcessing();
            }
            if (operation != null)
            {
                bool flag2 = false;
                if (EventTrace.IsEnabled(EventTrace.Flags.performance, EventTrace.Level.normal))
                {
                    EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.DISPATCHERDISPATCHGUID), 1, operation.Priority, operation.Name);
                    flag2 = true;
                }
                operation.Invoke();
                if (hooks != null)
                {
                    hooks.RaiseOperationCompleted(this, operation);
                }
                if (flag2)
                {
                    EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.DISPATCHERDISPATCHGUID), 2);
                    if (_idlePriorityRange.Contains(invalid))
                    {
                        EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.DISPATCHERIDLEGUID), 0);
                    }
                }
            }
        }

        private void PromoteTimers(int currentTimeInTicks)
        {
            try
            {
                object obj3;
                List<DispatcherTimer> list = null;
                long num = 0L;
                lock (this._instanceLock)
                {
                    if ((!this._hasShutdownFinished && this._dueTimeFound) && ((this._dueTimeInTicks - currentTimeInTicks) <= 0))
                    {
                        list = this._timers;
                        num = this._timersVersion;
                    }
                }
                if (list == null)
                {
                    return;
                }
                DispatcherTimer timer = null;
                int index = 0;
            Label_004D:
                Monitor.Enter(obj3 = this._instanceLock);
                try
                {
                    timer = null;
                    if (num != this._timersVersion)
                    {
                        num = this._timersVersion;
                        index = 0;
                    }
                    while (index < this._timers.Count)
                    {
                        if ((list[index]._dueTimeInTicks - currentTimeInTicks) <= 0)
                        {
                            timer = list[index];
                            list.RemoveAt(index);
                            goto Label_00AF;
                        }
                        index++;
                    }
                }
                finally
                {
                    Monitor.Exit(obj3);
                }
            Label_00AF:
                if (timer != null)
                {
                    timer.Promote();
                }
                if (timer != null)
                {
                    goto Label_004D;
                }
            }
            finally
            {
                this.UpdateWin32Timer();
            }
        }

        [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        public static void PushFrame(DispatcherFrame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }
            Dispatcher currentDispatcher = CurrentDispatcher;
            if (currentDispatcher._hasShutdownFinished)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("DispatcherHasShutdown"));
            }
            if (frame.Dispatcher != currentDispatcher)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("MismatchedDispatchers"));
            }
            if (currentDispatcher._disableProcessingCount > 0)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("DispatcherProcessingDisabled"));
            }
            currentDispatcher.PushFrameImpl(frame);
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private void PushFrameImpl(DispatcherFrame frame)
        {
            SynchronizationContext syncContext = null;
            SynchronizationContext context2 = null;
            MSG msg = new MSG();
            this._frameDepth++;
            try
            {
                syncContext = SynchronizationContext.Current;
                context2 = new DispatcherSynchronizationContext(this);
                SynchronizationContext.SetSynchronizationContext(context2);
                try
                {
                    while (frame.Continue)
                    {
                        if (!this.GetMessage(ref msg, IntPtr.Zero, 0, 0))
                        {
                            break;
                        }
                        this.TranslateAndDispatchMessage(ref msg);
                    }
                    if ((this._frameDepth == 1) && this._hasShutdownStarted)
                    {
                        this.ShutdownImpl();
                    }
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(syncContext);
                }
            }
            finally
            {
                this._frameDepth--;
                if (this._frameDepth == 0)
                {
                    this._exitAllFrames = false;
                }
            }
        }

        internal void RemoveTimer(DispatcherTimer timer)
        {
            lock (this._instanceLock)
            {
                if (!this._hasShutdownFinished)
                {
                    this._timers.Remove(timer);
                    this._timersVersion += 1L;
                }
            }
            this.UpdateWin32Timer();
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private bool RequestBackgroundProcessing()
        {
            bool flag = true;
            if (this._postedProcessingType >= 1)
            {
                return flag;
            }
            if (this.IsInputPending())
            {
                this._postedProcessingType = 1;
                return SafeNativeMethods.TrySetTimer(new HandleRef(this, this._window.Value.Handle), 1, 1);
            }
            return this.RequestForegroundProcessing();
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private bool RequestForegroundProcessing()
        {
            if (this._postedProcessingType >= 2)
            {
                return true;
            }
            if (this._postedProcessingType == 1)
            {
                SafeNativeMethods.KillTimer(new HandleRef(this, this._window.Value.Handle), 1);
            }
            this._postedProcessingType = 2;
            return MS.Win32.UnsafeNativeMethods.TryPostMessage(new HandleRef(this, this._window.Value.Handle), _msgProcessQueue, IntPtr.Zero, IntPtr.Zero);
        }

        private bool RequestProcessing()
        {
            bool flag = true;
            if (this.IsWindowNull())
            {
                return false;
            }
            DispatcherPriority maxPriority = this._queue.MaxPriority;
            switch (maxPriority)
            {
                case DispatcherPriority.Invalid:
                case DispatcherPriority.Inactive:
                    return flag;
            }
            if (_foregroundPriorityRange.Contains(maxPriority))
            {
                return this.RequestForegroundProcessing();
            }
            return this.RequestBackgroundProcessing();
        }

        [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        public static void Run()
        {
            PushFrame(new DispatcherFrame());
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal bool SetPriority(DispatcherOperation operation, DispatcherPriority priority)
        {
            bool flag = false;
            DispatcherHooks hooks = null;
            lock (this._instanceLock)
            {
                if ((this._queue != null) && operation._item.IsQueued)
                {
                    this._queue.ChangeItemPriority(operation._item, priority);
                    flag = true;
                    if (flag)
                    {
                        this.RequestProcessing();
                        hooks = this._hooks;
                    }
                }
            }
            if (flag)
            {
                if (hooks != null)
                {
                    hooks.RaiseOperationPriorityChanged(this, operation);
                }
                if (EventTrace.IsEnabled(EventTrace.Flags.performance, EventTrace.Level.normal))
                {
                    EventTrace.EventProvider.TraceEvent(EventTrace.GuidFromId(EventTraceGuidId.DISPATCHERPROMOTEGUID), 0, priority, operation.Name);
                }
            }
            return flag;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private void SetWin32Timer(int dueTimeInTicks)
        {
            if (!this.IsWindowNull())
            {
                int uElapse = dueTimeInTicks - Environment.TickCount;
                if (uElapse < 1)
                {
                    uElapse = 1;
                }
                SafeNativeMethods.SetTimer(new HandleRef(this, this._window.Value.Handle), 2, uElapse);
                this._isWin32TimerSet = true;
            }
        }

        [SecurityCritical]
        private void ShutdownCallbackInternal()
        {
            this.StartShutdownImpl();
        }

        [SecurityCritical]
        private void ShutdownImpl()
        {
            if (!this._hasShutdownFinished)
            {
                if ((this._shutdownExecutionContext != null) && (this._shutdownExecutionContext.Value != null))
                {
                    ExecutionContext.Run(this._shutdownExecutionContext.Value, new ContextCallback(this.ShutdownImplInSecurityContext), null);
                }
                else
                {
                    this.ShutdownImplInSecurityContext(null);
                }
                this._shutdownExecutionContext = null;
            }
        }

        [SecurityCritical]
        private void ShutdownImplInSecurityContext(object state)
        {
            if (this.ShutdownFinished != null)
            {
                this.ShutdownFinished(this, EventArgs.Empty);
            }
            MessageOnlyHwndWrapper wrapper = null;
            lock (this._instanceLock)
            {
                wrapper = this._window.Value;
                this._window = new SecurityCriticalData<MessageOnlyHwndWrapper>(null);
            }
            wrapper.Dispose();
            lock (this._instanceLock)
            {
                this._hasShutdownFinished = true;
            }
            DispatcherOperation operation = null;
            do
            {
                lock (this._instanceLock)
                {
                    if (this._queue.MaxPriority != DispatcherPriority.Invalid)
                    {
                        operation = this._queue.Peek();
                    }
                    else
                    {
                        operation = null;
                    }
                }
                if (operation != null)
                {
                    operation.Abort();
                }
            }
            while (operation != null);
            lock (this._instanceLock)
            {
                this._queue = null;
                this._timers = null;
                this._reserved0 = null;
                this._reserved1 = null;
                this._reserved2 = null;
                this._reserved3 = null;
                this._reservedInputMethod = null;
                this._reservedInputManager = null;
            }
        }

        [SecurityCritical]
        private void StartShutdownImpl()
        {
            if (!this._startingShutdown)
            {
                this._startingShutdown = true;
                if (this.ShutdownStarted != null)
                {
                    this.ShutdownStarted(this, EventArgs.Empty);
                }
                this._hasShutdownStarted = true;
                ExecutionContext context = ExecutionContext.Capture();
                this._shutdownExecutionContext = new SecurityCriticalDataClass<ExecutionContext>(context);
                if (this._frameDepth <= 0)
                {
                    this.ShutdownImpl();
                }
            }
        }

        [SecurityCritical]
        private void TranslateAndDispatchMessage(ref MSG msg)
        {
            if (!ComponentDispatcher.RaiseThreadMessage(ref msg))
            {
                MS.Win32.UnsafeNativeMethods.TranslateMessage(ref msg);
                MS.Win32.UnsafeNativeMethods.DispatchMessage(ref msg);
            }
        }

        internal void UpdateWin32Timer()
        {
            if (this.CheckAccess())
            {
                this.UpdateWin32TimerFromDispatcherThread(null);
            }
            else
            {
                this.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(this.UpdateWin32TimerFromDispatcherThread), null);
            }
        }

        private object UpdateWin32TimerFromDispatcherThread(object unused)
        {
            lock (this._instanceLock)
            {
                if (!this._hasShutdownFinished)
                {
                    bool flag = this._dueTimeFound;
                    int num = this._dueTimeInTicks;
                    this._dueTimeFound = false;
                    this._dueTimeInTicks = 0;
                    if (this._timers.Count > 0)
                    {
                        for (int i = 0; i < this._timers.Count; i++)
                        {
                            DispatcherTimer timer = this._timers[i];
                            if (!this._dueTimeFound || ((timer._dueTimeInTicks - this._dueTimeInTicks) < 0))
                            {
                                this._dueTimeFound = true;
                                this._dueTimeInTicks = timer._dueTimeInTicks;
                            }
                        }
                    }
                    if (this._dueTimeFound)
                    {
                        if ((!this._isWin32TimerSet || !flag) || (num != this._dueTimeInTicks))
                        {
                            this.SetWin32Timer(this._dueTimeInTicks);
                        }
                    }
                    else if (flag)
                    {
                        this.KillWin32Timer();
                    }
                }
            }
            return null;
        }

        public static void ValidatePriority(DispatcherPriority priority, string parameterName)
        {
            if ((!_foregroundPriorityRange.Contains(priority) && !_backgroundPriorityRange.Contains(priority)) && (!_idlePriorityRange.Contains(priority) && (priority != DispatcherPriority.Inactive)))
            {
                throw new InvalidEnumArgumentException(parameterName, (int) priority, typeof(DispatcherPriority));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void VerifyAccess()
        {
            if (!this.CheckAccess())
            {
                throw new InvalidOperationException(System.Windows.SR.Get("VerifyAccess"));
            }
        }

        [SecurityCritical]
        private IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (this._disableProcessingCount > 0)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("DispatcherProcessingDisabledButStillPumping"));
            }
            if (msg == 2)
            {
                if (!this._hasShutdownStarted && !this._hasShutdownFinished)
                {
                    this.ShutdownImpl();
                }
            }
            else if (msg == _msgProcessQueue)
            {
                this.ProcessQueue();
            }
            else if ((msg == 0x113) && (((int) wParam) == 1))
            {
                SafeNativeMethods.KillTimer(new HandleRef(this, hwnd), 1);
                this.ProcessQueue();
            }
            else if ((msg == 0x113) && (((int) wParam) == 2))
            {
                this.KillWin32Timer();
                this.PromoteTimers(Environment.TickCount);
            }
            DispatcherHooks hooks = null;
            bool flag = false;
            lock (this._instanceLock)
            {
                flag = this._postedProcessingType < 1;
                if (flag)
                {
                    hooks = this._hooks;
                }
            }
            if (flag)
            {
                if (hooks != null)
                {
                    hooks.RaiseDispatcherInactive(this);
                }
                ComponentDispatcher.RaiseIdle();
            }
            return IntPtr.Zero;
        }

        internal object WrappedInvoke(Delegate callback, object args, bool isSingleParameter) => 
            this.WrappedInvoke(callback, args, isSingleParameter, null);

        [FriendAccessAllowed]
        internal object WrappedInvoke(Delegate callback, object args, bool isSingleParameter, Delegate catchHandler)
        {
            this.PromoteTimers(Environment.TickCount);
            return _exceptionWrapper.TryCatchWhen(this, callback, args, isSingleParameter, catchHandler);
        }

        public static Dispatcher CurrentDispatcher
        {
            get
            {
                Dispatcher dispatcher = FromThread(System.Threading.Thread.CurrentThread);
                if (dispatcher == null)
                {
                    dispatcher = new Dispatcher();
                }
                return dispatcher;
            }
        }

        public bool HasShutdownFinished =>
            this._hasShutdownFinished;

        public bool HasShutdownStarted =>
            this._hasShutdownStarted;

        private bool HasUnhandledExceptionHandler =>
            (this.UnhandledException != null);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DispatcherHooks Hooks
        {
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
            get
            {
                lock (this._instanceLock)
                {
                    if (this._hooks == null)
                    {
                        this._hooks = new DispatcherHooks();
                    }
                    return this._hooks;
                }
            }
        }

        internal object InputManager
        {
            [FriendAccessAllowed, SecurityCritical]
            get => 
                this._reservedInputManager;
            [SecurityCritical, FriendAccessAllowed]
            set
            {
                this._reservedInputManager = value;
            }
        }

        internal object InputMethod
        {
            [FriendAccessAllowed]
            get => 
                this._reservedInputMethod;
            [FriendAccessAllowed]
            set
            {
                this._reservedInputMethod = value;
            }
        }

        [FriendAccessAllowed]
        internal bool IsTSFMessagePumpEnabled
        {
            set
            {
                this._isTSFMessagePumpEnabled = value;
            }
        }

        internal object PtsCache
        {
            [FriendAccessAllowed]
            get => 
                this._reservedPtsCache;
            [FriendAccessAllowed]
            set
            {
                this._reservedPtsCache = value;
            }
        }

        internal object Reserved0
        {
            [FriendAccessAllowed]
            get => 
                this._reserved0;
            [FriendAccessAllowed]
            set
            {
                this._reserved0 = value;
            }
        }

        internal object Reserved1
        {
            [FriendAccessAllowed]
            get => 
                this._reserved1;
            [FriendAccessAllowed]
            set
            {
                this._reserved1 = value;
            }
        }

        internal object Reserved2
        {
            [FriendAccessAllowed]
            get => 
                this._reserved2;
            [FriendAccessAllowed]
            set
            {
                this._reserved2 = value;
            }
        }

        internal object Reserved3
        {
            [FriendAccessAllowed]
            get => 
                this._reserved3;
            [FriendAccessAllowed]
            set
            {
                this._reserved3 = value;
            }
        }

        public System.Threading.Thread Thread =>
            this._dispatcherThread;

        internal delegate void ShutdownCallback();
    }
}

