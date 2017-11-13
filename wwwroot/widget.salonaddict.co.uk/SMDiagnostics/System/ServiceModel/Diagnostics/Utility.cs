namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    internal class Utility
    {
        private System.ServiceModel.Diagnostics.ExceptionUtility exceptionUtility;

        [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.Utility instead")]
        internal Utility(System.ServiceModel.Diagnostics.ExceptionUtility exceptionUtility)
        {
            this.exceptionUtility = exceptionUtility;
        }

        internal byte[] AllocateByteArray(int size)
        {
            byte[] buffer;
            try
            {
                buffer = new byte[size];
            }
            catch (OutOfMemoryException exception)
            {
                throw this.ExceptionUtility.ThrowHelperError(new InsufficientMemoryException(TraceSR.GetString("BufferAllocationFailed", new object[] { size }), exception));
            }
            return buffer;
        }

        internal char[] AllocateCharArray(int size)
        {
            char[] chArray;
            try
            {
                chArray = new char[size];
            }
            catch (OutOfMemoryException exception)
            {
                throw this.ExceptionUtility.ThrowHelperError(new InsufficientMemoryException(TraceSR.GetString("BufferAllocationFailed", new object[] { size * 2 }), exception));
            }
            return chArray;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode=true)]
        internal virtual bool CallHandler(Exception exception) => 
            false;

        internal static void CloseInvalidOutCriticalHandle(CriticalHandle handle)
        {
            if (handle != null)
            {
                handle.SetHandleAsInvalid();
            }
        }

        internal static void CloseInvalidOutSafeHandle(SafeHandle handle)
        {
            if (handle != null)
            {
                handle.SetHandleAsInvalid();
            }
        }

        [SecurityCritical]
        internal static void CloseInvalidOutSafeHandleCritical(SafeHandle handle)
        {
            if (handle != null)
            {
                handle.SetHandleAsInvalid();
            }
        }

        internal Guid CreateGuid(string guidString)
        {
            bool flag = false;
            Guid empty = Guid.Empty;
            try
            {
                empty = new Guid(guidString);
                flag = true;
            }
            finally
            {
            }
            return empty;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private bool HandleAtThreadBase(Exception exception)
        {
            if (System.ServiceModel.Diagnostics.ExceptionUtility.IsInfrastructureException(exception))
            {
                this.TraceExceptionNoThrow(exception, TraceEventType.Warning);
                return false;
            }
            this.TraceExceptionNoThrow(exception, TraceEventType.Critical);
            try
            {
                return this.CallHandler(exception);
            }
            catch (Exception exception2)
            {
                this.TraceExceptionNoThrow(exception2, TraceEventType.Error);
            }
            return false;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal AsyncCallback ThunkCallback(AsyncCallback callback) => 
            new AsyncThunk(callback, this).ThunkFrame;

        [SecurityTreatAsSafe, SecurityCritical]
        internal IOCompletionCallback ThunkCallback(IOCompletionCallback callback) => 
            new IOCompletionThunk(callback, this).ThunkFrame;

        [SecurityTreatAsSafe, SecurityCritical]
        internal TimerCallback ThunkCallback(TimerCallback callback) => 
            new TimerThunk(callback, this).ThunkFrame;

        [SecurityCritical, SecurityTreatAsSafe]
        internal WaitCallback ThunkCallback(WaitCallback callback) => 
            new WaitThunk(callback, this).ThunkFrame;

        [SecurityTreatAsSafe, SecurityCritical]
        internal WaitOrTimerCallback ThunkCallback(WaitOrTimerCallback callback) => 
            new WaitOrTimerThunk(callback, this).ThunkFrame;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private void TraceExceptionNoThrow(Exception exception, TraceEventType eventType)
        {
            try
            {
                this.ExceptionUtility.TraceHandledException(exception, eventType);
            }
            catch
            {
            }
        }

        internal bool TryCreateGuid(string guidString, out Guid result)
        {
            bool flag = false;
            result = Guid.Empty;
            try
            {
                result = new Guid(guidString);
                flag = true;
            }
            catch (ArgumentException exception)
            {
                this.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
            }
            catch (FormatException exception2)
            {
                this.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Warning);
            }
            catch (OverflowException exception3)
            {
                this.ExceptionUtility.TraceHandledException(exception3, TraceEventType.Warning);
            }
            return flag;
        }

        private System.ServiceModel.Diagnostics.ExceptionUtility ExceptionUtility =>
            this.exceptionUtility;

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private sealed class AsyncThunk : Utility.Thunk<AsyncCallback>
        {
            public AsyncThunk(AsyncCallback callback, Utility utility) : base(callback, utility)
            {
            }

            private void UnhandledExceptionFrame(IAsyncResult result)
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    base.callback(result);
                }
                catch (Exception exception)
                {
                    if (!base.utility.HandleAtThreadBase(exception))
                    {
                        throw;
                    }
                }
            }

            public AsyncCallback ThunkFrame =>
                new AsyncCallback(this.UnhandledExceptionFrame);
        }

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private sealed class IOCompletionThunk
        {
            private IOCompletionCallback callback;
            private Utility utility;

            public IOCompletionThunk(IOCompletionCallback callback, Utility utility)
            {
                this.callback = callback;
                this.utility = utility;
            }

            private unsafe void UnhandledExceptionFrame(uint error, uint bytesRead, NativeOverlapped* nativeOverlapped)
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    this.callback(error, bytesRead, nativeOverlapped);
                }
                catch (Exception exception)
                {
                    if (!this.utility.HandleAtThreadBase(exception))
                    {
                        throw;
                    }
                }
            }

            public IOCompletionCallback ThunkFrame =>
                new IOCompletionCallback(this.UnhandledExceptionFrame);
        }

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private class Thunk<T> where T: class
        {
            protected T callback;
            protected Utility utility;

            public Thunk(T callback, Utility utility)
            {
                this.callback = callback;
                this.utility = utility;
            }
        }

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private sealed class TimerThunk : Utility.Thunk<TimerCallback>
        {
            public TimerThunk(TimerCallback callback, Utility utility) : base(callback, utility)
            {
            }

            private void UnhandledExceptionFrame(object state)
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    base.callback(state);
                }
                catch (Exception exception)
                {
                    if (!base.utility.HandleAtThreadBase(exception))
                    {
                        throw;
                    }
                }
            }

            public TimerCallback ThunkFrame =>
                new TimerCallback(this.UnhandledExceptionFrame);
        }

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private sealed class WaitOrTimerThunk : Utility.Thunk<WaitOrTimerCallback>
        {
            public WaitOrTimerThunk(WaitOrTimerCallback callback, Utility utility) : base(callback, utility)
            {
            }

            private void UnhandledExceptionFrame(object state, bool timedOut)
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    base.callback(state, timedOut);
                }
                catch (Exception exception)
                {
                    if (!base.utility.HandleAtThreadBase(exception))
                    {
                        throw;
                    }
                }
            }

            public WaitOrTimerCallback ThunkFrame =>
                new WaitOrTimerCallback(this.UnhandledExceptionFrame);
        }

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private sealed class WaitThunk : Utility.Thunk<WaitCallback>
        {
            public WaitThunk(WaitCallback callback, Utility utility) : base(callback, utility)
            {
            }

            private void UnhandledExceptionFrame(object state)
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    base.callback(state);
                }
                catch (Exception exception)
                {
                    if (!base.utility.HandleAtThreadBase(exception))
                    {
                        throw;
                    }
                }
            }

            public WaitCallback ThunkFrame =>
                new WaitCallback(this.UnhandledExceptionFrame);
        }
    }
}

