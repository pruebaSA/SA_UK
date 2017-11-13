namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    public abstract class CommunicationObject : ICommunicationObject
    {
        private bool aborted;
        private bool closeCalled;
        private object eventSender;
        private ExceptionQueue exceptionQueue;
        private object mutex;
        private bool onClosedCalled;
        private bool onClosingCalled;
        private bool onOpenedCalled;
        private bool onOpeningCalled;
        private bool raisedClosed;
        private bool raisedClosing;
        private bool raisedFaulted;
        private CommunicationState state;
        private bool traceOpenAndClose;

        public event EventHandler Closed;

        public event EventHandler Closing;

        public event EventHandler Faulted;

        public event EventHandler Opened;

        public event EventHandler Opening;

        protected CommunicationObject() : this(new object())
        {
        }

        protected CommunicationObject(object mutex)
        {
            this.mutex = mutex;
            this.eventSender = this;
            this.state = CommunicationState.Created;
        }

        internal CommunicationObject(object mutex, object eventSender)
        {
            this.mutex = mutex;
            this.eventSender = eventSender;
            this.state = CommunicationState.Created;
        }

        public void Abort()
        {
            lock (this.ThisLock)
            {
                if (this.aborted || (this.state == CommunicationState.Closed))
                {
                    return;
                }
                this.aborted = true;
                this.state = CommunicationState.Closing;
            }
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.CommunicationObjectAborted, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectAborted", new object[] { DiagnosticTrace.CreateSourceString(this) }), null, null, this);
            }
            bool flag = true;
            try
            {
                this.OnClosing();
                if (!this.onClosingCalled)
                {
                    throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnClosing"), Guid.Empty, this);
                }
                this.OnAbort();
                this.OnClosed();
                if (!this.onClosedCalled)
                {
                    throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnClosed"), Guid.Empty, this);
                }
                flag = false;
            }
            finally
            {
                if (flag && DiagnosticUtility.ShouldTraceWarning)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.CommunicationObjectAbortFailed, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectAbortFailed", new object[] { this.GetCommunicationObjectType().ToString() }), null, null, this);
                }
            }
        }

        internal void AddPendingException(Exception exception)
        {
            lock (this.ThisLock)
            {
                if (this.exceptionQueue == null)
                {
                    this.exceptionQueue = new ExceptionQueue(this.ThisLock);
                }
            }
            this.exceptionQueue.AddException(exception);
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state) => 
            this.BeginClose(this.DefaultCloseTimeout, callback, state);

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("timeout", System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
            }
            using ((DiagnosticUtility.ShouldUseActivity && this.TraceOpenAndClose) ? this.CreateCloseActivity() : null)
            {
                CommunicationState state2;
                lock (this.ThisLock)
                {
                    state2 = this.state;
                    if (state2 != CommunicationState.Closed)
                    {
                        this.state = CommunicationState.Closing;
                    }
                    this.closeCalled = true;
                }
                switch (state2)
                {
                    case CommunicationState.Created:
                    case CommunicationState.Opening:
                    case CommunicationState.Faulted:
                        this.Abort();
                        if (state2 == CommunicationState.Faulted)
                        {
                            throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
                        }
                        return new AlreadyClosedAsyncResult(callback, state);

                    case CommunicationState.Opened:
                    {
                        bool flag = true;
                        try
                        {
                            this.OnClosing();
                            if (!this.onClosingCalled)
                            {
                                throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnClosing"), Guid.Empty, this);
                            }
                            IAsyncResult result = new CloseAsyncResult(this, timeout, callback, state);
                            flag = false;
                            return result;
                        }
                        finally
                        {
                            if (flag)
                            {
                                if (DiagnosticUtility.ShouldTraceWarning)
                                {
                                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.CommunicationObjectCloseFailed, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectCloseFailed", new object[] { this.GetCommunicationObjectType().ToString() }), null, null, this);
                                }
                                this.Abort();
                            }
                        }
                        break;
                    }
                    case CommunicationState.Closing:
                    case CommunicationState.Closed:
                        break;

                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                return new AlreadyClosedAsyncResult(callback, state);
            }
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state) => 
            this.BeginOpen(this.DefaultOpenTimeout, callback, state);

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            IAsyncResult result2;
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("timeout", System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
            }
            lock (this.ThisLock)
            {
                this.ThrowIfDisposedOrImmutable();
                this.state = CommunicationState.Opening;
            }
            bool flag = true;
            try
            {
                this.OnOpening();
                if (!this.onOpeningCalled)
                {
                    throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnOpening"), Guid.Empty, this);
                }
                IAsyncResult result = new OpenAsyncResult(this, timeout, callback, state);
                flag = false;
                result2 = result;
            }
            finally
            {
                if (flag)
                {
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.CommunicationObjectOpenFailed, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectOpenFailed", new object[] { this.GetCommunicationObjectType().ToString() }), null, null, this);
                    }
                    this.Fault();
                }
            }
            return result2;
        }

        public void Close()
        {
            this.Close(this.DefaultCloseTimeout);
        }

        public void Close(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("timeout", System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
            }
            using ((DiagnosticUtility.ShouldUseActivity && this.TraceOpenAndClose) ? this.CreateCloseActivity() : null)
            {
                CommunicationState state;
                lock (this.ThisLock)
                {
                    state = this.state;
                    if (state != CommunicationState.Closed)
                    {
                        this.state = CommunicationState.Closing;
                    }
                    this.closeCalled = true;
                }
                switch (state)
                {
                    case CommunicationState.Created:
                    case CommunicationState.Opening:
                    case CommunicationState.Faulted:
                        this.Abort();
                        if (state == CommunicationState.Faulted)
                        {
                            throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
                        }
                        return;

                    case CommunicationState.Opened:
                    {
                        bool flag = true;
                        try
                        {
                            this.OnClosing();
                            if (!this.onClosingCalled)
                            {
                                throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnClosing"), Guid.Empty, this);
                            }
                            this.OnClose(timeout);
                            this.OnClosed();
                            if (!this.onClosedCalled)
                            {
                                throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnClosed"), Guid.Empty, this);
                            }
                            flag = false;
                            return;
                        }
                        finally
                        {
                            if (flag)
                            {
                                if (DiagnosticUtility.ShouldTraceWarning)
                                {
                                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.CommunicationObjectCloseFailed, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectCloseFailed", new object[] { this.GetCommunicationObjectType().ToString() }), null, null, this);
                                }
                                this.Abort();
                            }
                        }
                        break;
                    }
                    case CommunicationState.Closing:
                    case CommunicationState.Closed:
                        return;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
        }

        internal Exception CreateAbortedException() => 
            new CommunicationObjectAbortedException(System.ServiceModel.SR.GetString("CommunicationObjectAborted1", new object[] { this.GetCommunicationObjectType().ToString() }));

        private Exception CreateBaseClassMethodNotCalledException(string method) => 
            new InvalidOperationException(System.ServiceModel.SR.GetString("CommunicationObjectBaseClassMethodNotCalled", new object[] { this.GetCommunicationObjectType().ToString(), method }));

        private ServiceModelActivity CreateCloseActivity()
        {
            ServiceModelActivity activity = null;
            activity = ServiceModelActivity.CreateBoundedActivity();
            if (DiagnosticUtility.ShouldUseActivity)
            {
                ServiceModelActivity.Start(activity, this.CloseActivityName, ActivityType.Close);
            }
            return activity;
        }

        internal Exception CreateClosedException()
        {
            if (!this.closeCalled)
            {
                return this.CreateAbortedException();
            }
            return new ObjectDisposedException(this.GetCommunicationObjectType().ToString());
        }

        private Exception CreateFaultedException() => 
            new CommunicationObjectFaultedException(System.ServiceModel.SR.GetString("CommunicationObjectFaulted1", new object[] { this.GetCommunicationObjectType().ToString() }));

        private Exception CreateImmutableException() => 
            new InvalidOperationException(System.ServiceModel.SR.GetString("CommunicationObjectCannotBeModifiedInState", new object[] { this.GetCommunicationObjectType().ToString(), this.state.ToString() }));

        private Exception CreateNotOpenException() => 
            new InvalidOperationException(System.ServiceModel.SR.GetString("CommunicationObjectCannotBeUsed", new object[] { this.GetCommunicationObjectType().ToString(), this.state.ToString() }));

        internal bool DoneReceivingInCurrentState()
        {
            this.ThrowPending();
            switch (this.state)
            {
                case CommunicationState.Created:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    return false;

                case CommunicationState.Closing:
                    return true;

                case CommunicationState.Closed:
                    return true;

                case CommunicationState.Faulted:
                    return true;
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        public void EndClose(IAsyncResult result)
        {
            if (result is AlreadyClosedAsyncResult)
            {
                CompletedAsyncResult.End(result);
            }
            else
            {
                CloseAsyncResult.End(result);
            }
        }

        public void EndOpen(IAsyncResult result)
        {
            OpenAsyncResult.End(result);
        }

        protected void Fault()
        {
            lock (this.ThisLock)
            {
                if (((this.state == CommunicationState.Closed) || (this.state == CommunicationState.Closing)) || (this.state == CommunicationState.Faulted))
                {
                    return;
                }
                this.state = CommunicationState.Faulted;
            }
            this.OnFaulted();
        }

        internal void Fault(Exception exception)
        {
            lock (this.ThisLock)
            {
                if (this.exceptionQueue == null)
                {
                    this.exceptionQueue = new ExceptionQueue(this.ThisLock);
                }
            }
            if ((exception != null) && DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.CommunicationObjectFaultReason, exception, null);
            }
            this.exceptionQueue.AddException(exception);
            this.Fault();
        }

        protected virtual Type GetCommunicationObjectType() => 
            base.GetType();

        internal Exception GetPendingException()
        {
            ExceptionQueue exceptionQueue = this.exceptionQueue;
            if (exceptionQueue != null)
            {
                return exceptionQueue.GetException();
            }
            return null;
        }

        internal Exception GetTerminalException()
        {
            Exception pendingException = this.GetPendingException();
            if (pendingException == null)
            {
                pendingException = this.CreateFaultedException();
            }
            return pendingException;
        }

        protected abstract void OnAbort();
        protected abstract IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state);
        protected abstract IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state);
        protected abstract void OnClose(TimeSpan timeout);
        protected virtual void OnClosed()
        {
            this.onClosedCalled = true;
            lock (this.ThisLock)
            {
                if (this.raisedClosed)
                {
                    return;
                }
                this.raisedClosed = true;
                this.state = CommunicationState.Closed;
            }
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Verbose, TraceCode.CommunicationObjectClosed, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectClosed", new object[] { DiagnosticTrace.CreateSourceString(this) }), null, null, this);
            }
            EventHandler closed = this.Closed;
            if (closed != null)
            {
                try
                {
                    closed(this.eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected virtual void OnClosing()
        {
            this.onClosingCalled = true;
            lock (this.ThisLock)
            {
                if (this.raisedClosing)
                {
                    return;
                }
                this.raisedClosing = true;
            }
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Verbose, TraceCode.CommunicationObjectClosing, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectClosing", new object[] { DiagnosticTrace.CreateSourceString(this) }), null, null, this);
            }
            EventHandler closing = this.Closing;
            if (closing != null)
            {
                try
                {
                    closing(this.eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected abstract void OnEndClose(IAsyncResult result);
        protected abstract void OnEndOpen(IAsyncResult result);
        protected virtual void OnFaulted()
        {
            lock (this.ThisLock)
            {
                if (this.raisedFaulted)
                {
                    return;
                }
                this.raisedFaulted = true;
            }
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.CommunicationObjectFaulted, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectFaulted", new object[] { this.GetCommunicationObjectType().ToString() }), null, null, this);
            }
            EventHandler faulted = this.Faulted;
            if (faulted != null)
            {
                try
                {
                    faulted(this.eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected abstract void OnOpen(TimeSpan timeout);
        protected virtual void OnOpened()
        {
            this.onOpenedCalled = true;
            lock (this.ThisLock)
            {
                if (this.aborted || (this.state != CommunicationState.Opening))
                {
                    return;
                }
                this.state = CommunicationState.Opened;
            }
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Verbose, TraceCode.CommunicationObjectOpened, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectOpened", new object[] { DiagnosticTrace.CreateSourceString(this) }), null, null, this);
            }
            EventHandler opened = this.Opened;
            if (opened != null)
            {
                try
                {
                    opened(this.eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected virtual void OnOpening()
        {
            this.onOpeningCalled = true;
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Verbose, TraceCode.CommunicationObjectOpening, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectOpening", new object[] { DiagnosticTrace.CreateSourceString(this) }), null, null, this);
            }
            EventHandler opening = this.Opening;
            if (opening != null)
            {
                try
                {
                    opening(this.eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        public void Open()
        {
            this.Open(this.DefaultOpenTimeout);
        }

        public void Open(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("timeout", System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
            }
            using (ServiceModelActivity activity = (DiagnosticUtility.ShouldUseActivity && this.TraceOpenAndClose) ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, this.OpenActivityName, this.OpenActivityType);
                }
                lock (this.ThisLock)
                {
                    this.ThrowIfDisposedOrImmutable();
                    this.state = CommunicationState.Opening;
                }
                bool flag = true;
                try
                {
                    this.OnOpening();
                    if (!this.onOpeningCalled)
                    {
                        throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnOpening"), Guid.Empty, this);
                    }
                    this.OnOpen(timeout);
                    this.OnOpened();
                    if (!this.onOpenedCalled)
                    {
                        throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnOpened"), Guid.Empty, this);
                    }
                    flag = false;
                }
                finally
                {
                    if (flag)
                    {
                        if (DiagnosticUtility.ShouldTraceWarning)
                        {
                            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.CommunicationObjectOpenFailed, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectOpenFailed", new object[] { this.GetCommunicationObjectType().ToString() }), null, null, this);
                        }
                        this.Fault();
                    }
                }
            }
        }

        internal void ThrowIfAborted()
        {
            if (this.aborted && !this.closeCalled)
            {
                throw TraceUtility.ThrowHelperError(this.CreateAbortedException(), Guid.Empty, this);
            }
        }

        internal void ThrowIfClosed()
        {
            this.ThrowPending();
            switch (this.state)
            {
                case CommunicationState.Created:
                case CommunicationState.Opening:
                case CommunicationState.Opened:
                case CommunicationState.Closing:
                    return;

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        internal void ThrowIfClosedOrNotOpen()
        {
            this.ThrowPending();
            switch (this.state)
            {
                case CommunicationState.Created:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opened:
                case CommunicationState.Closing:
                    return;

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        internal void ThrowIfClosedOrOpened()
        {
            this.ThrowPending();
            switch (this.state)
            {
                case CommunicationState.Created:
                case CommunicationState.Opening:
                    return;

                case CommunicationState.Opened:
                    throw TraceUtility.ThrowHelperError(this.CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(this.CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        protected internal void ThrowIfDisposed()
        {
            this.ThrowPending();
            switch (this.state)
            {
                case CommunicationState.Created:
                case CommunicationState.Opening:
                case CommunicationState.Opened:
                    return;

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        protected internal void ThrowIfDisposedOrImmutable()
        {
            this.ThrowPending();
            switch (this.state)
            {
                case CommunicationState.Created:
                    return;

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(this.CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    throw TraceUtility.ThrowHelperError(this.CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        protected internal void ThrowIfDisposedOrNotOpen()
        {
            this.ThrowPending();
            switch (this.state)
            {
                case CommunicationState.Created:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    return;

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        internal void ThrowIfFaulted()
        {
            this.ThrowPending();
            switch (this.state)
            {
                case CommunicationState.Created:
                case CommunicationState.Opening:
                case CommunicationState.Opened:
                case CommunicationState.Closing:
                case CommunicationState.Closed:
                    return;

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        internal void ThrowIfNotOpened()
        {
            if ((this.state == CommunicationState.Created) || (this.state == CommunicationState.Opening))
            {
                throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);
            }
        }

        internal void ThrowPending()
        {
            ExceptionQueue exceptionQueue = this.exceptionQueue;
            if (exceptionQueue != null)
            {
                Exception exception = exceptionQueue.GetException();
                if (exception != null)
                {
                    throw TraceUtility.ThrowHelperError(exception, Guid.Empty, this);
                }
            }
        }

        internal bool Aborted =>
            this.aborted;

        internal virtual string CloseActivityName =>
            System.ServiceModel.SR.GetString("ActivityClose", new object[] { base.GetType().FullName });

        protected abstract TimeSpan DefaultCloseTimeout { get; }

        protected abstract TimeSpan DefaultOpenTimeout { get; }

        internal object EventSender
        {
            get => 
                this.eventSender;
            set
            {
                this.eventSender = value;
            }
        }

        internal TimeSpan InternalCloseTimeout =>
            this.DefaultCloseTimeout;

        internal TimeSpan InternalOpenTimeout =>
            this.DefaultOpenTimeout;

        protected bool IsDisposed =>
            (this.state == CommunicationState.Closed);

        internal virtual string OpenActivityName =>
            System.ServiceModel.SR.GetString("ActivityOpen", new object[] { base.GetType().FullName });

        internal virtual ActivityType OpenActivityType =>
            ActivityType.Open;

        public CommunicationState State =>
            this.state;

        protected object ThisLock =>
            this.mutex;

        internal bool TraceOpenAndClose
        {
            get => 
                this.traceOpenAndClose;
            set
            {
                this.traceOpenAndClose = value && DiagnosticUtility.ShouldUseActivity;
            }
        }

        private class AlreadyClosedAsyncResult : CompletedAsyncResult
        {
            public AlreadyClosedAsyncResult(AsyncCallback callback, object state) : base(callback, state)
            {
            }
        }

        private class CloseAsyncResult : TraceAsyncResult
        {
            private CommunicationObject communicationObject;
            private static AsyncCallback onCloseComplete = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(CommunicationObject.CloseAsyncResult.OnCloseComplete));

            public CloseAsyncResult(CommunicationObject communicationObject, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                this.communicationObject = communicationObject;
                IAsyncResult result = this.communicationObject.OnBeginClose(timeout, onCloseComplete, this);
                if (result.CompletedSynchronously)
                {
                    this.HandleCloseComplete(result);
                    base.Complete(true);
                }
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<CommunicationObject.CloseAsyncResult>(result);
            }

            private void HandleCloseComplete(IAsyncResult result)
            {
                this.communicationObject.OnEndClose(result);
                this.communicationObject.OnClosed();
                if (!this.communicationObject.onClosedCalled)
                {
                    throw TraceUtility.ThrowHelperError(this.communicationObject.CreateBaseClassMethodNotCalledException("OnClosed"), Guid.Empty, this.communicationObject);
                }
            }

            private static void OnCloseComplete(IAsyncResult result)
            {
                if (!result.CompletedSynchronously)
                {
                    CommunicationObject.CloseAsyncResult asyncState = (CommunicationObject.CloseAsyncResult) result.AsyncState;
                    using (ServiceModelActivity.BoundOperation(asyncState.CallbackActivity))
                    {
                        Exception exception = null;
                        try
                        {
                            asyncState.HandleCloseComplete(result);
                        }
                        catch (Exception exception2)
                        {
                            if (DiagnosticUtility.IsFatal(exception2))
                            {
                                throw;
                            }
                            exception = exception2;
                            if (DiagnosticUtility.ShouldTraceWarning)
                            {
                                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.CommunicationObjectCloseFailed, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectCloseFailed", new object[] { asyncState.communicationObject.GetCommunicationObjectType().ToString() }), null, null, asyncState);
                            }
                            asyncState.communicationObject.Abort();
                        }
                        asyncState.Complete(false, exception);
                    }
                }
            }
        }

        private class ExceptionQueue
        {
            private Queue<Exception> exceptions = new Queue<Exception>();
            private object thisLock;

            internal ExceptionQueue(object thisLock)
            {
                this.thisLock = thisLock;
            }

            public void AddException(Exception exception)
            {
                if (exception != null)
                {
                    lock (this.ThisLock)
                    {
                        this.exceptions.Enqueue(exception);
                    }
                }
            }

            public Exception GetException()
            {
                lock (this.ThisLock)
                {
                    if (this.exceptions.Count > 0)
                    {
                        return this.exceptions.Dequeue();
                    }
                }
                return null;
            }

            private object ThisLock =>
                this.thisLock;
        }

        private class OpenAsyncResult : AsyncResult
        {
            private CommunicationObject communicationObject;
            private static AsyncCallback onOpenComplete = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(CommunicationObject.OpenAsyncResult.OnOpenComplete));

            public OpenAsyncResult(CommunicationObject communicationObject, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                this.communicationObject = communicationObject;
                IAsyncResult result = this.communicationObject.OnBeginOpen(timeout, onOpenComplete, this);
                if (result.CompletedSynchronously)
                {
                    this.HandleOpenComplete(result);
                    base.Complete(true);
                }
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<CommunicationObject.OpenAsyncResult>(result);
            }

            private void HandleOpenComplete(IAsyncResult result)
            {
                this.communicationObject.OnEndOpen(result);
                this.communicationObject.OnOpened();
                if (!this.communicationObject.onOpenedCalled)
                {
                    throw TraceUtility.ThrowHelperError(this.communicationObject.CreateBaseClassMethodNotCalledException("OnOpened"), Guid.Empty, this.communicationObject);
                }
            }

            private static void OnOpenComplete(IAsyncResult result)
            {
                if (!result.CompletedSynchronously)
                {
                    Exception exception = null;
                    CommunicationObject.OpenAsyncResult asyncState = (CommunicationObject.OpenAsyncResult) result.AsyncState;
                    try
                    {
                        asyncState.HandleOpenComplete(result);
                    }
                    catch (Exception exception2)
                    {
                        if (DiagnosticUtility.IsFatal(exception2))
                        {
                            throw;
                        }
                        exception = exception2;
                        if (DiagnosticUtility.ShouldTraceWarning)
                        {
                            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.CommunicationObjectOpenFailed, System.ServiceModel.SR.GetString("TraceCodeCommunicationObjectOpenFailed", new object[] { asyncState.communicationObject.GetCommunicationObjectType().ToString() }), null, null, asyncState);
                        }
                        asyncState.communicationObject.Fault();
                    }
                    asyncState.Complete(false, exception);
                }
            }
        }
    }
}

