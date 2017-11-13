namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Diagnostics;
    using System.Runtime.ConstrainedExecution;
    using System.Security;
    using System.Security.Permissions;
    using System.ServiceModel;

    public abstract class ExceptionHandler
    {
        private static readonly ExceptionHandler alwaysHandle = new AlwaysHandleExceptionHandler();
        [SecurityCritical]
        private static ExceptionHandler asynchronousThreadExceptionHandler;
        private static ExceptionHandler transportExceptionHandler = alwaysHandle;

        protected ExceptionHandler()
        {
        }

        public abstract bool HandleException(Exception exception);
        internal static bool HandleTransportExceptionHelper(Exception exception)
        {
            if (exception == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            ExceptionHandler transportExceptionHandler = TransportExceptionHandler;
            if (transportExceptionHandler == null)
            {
                return false;
            }
            try
            {
                if (!transportExceptionHandler.HandleException(exception))
                {
                    return false;
                }
            }
            catch (Exception exception2)
            {
                if (DiagnosticUtility.IsFatal(exception2))
                {
                    throw;
                }
                if (DiagnosticUtility.ShouldTraceError)
                {
                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Error);
                }
                return false;
            }
            if (DiagnosticUtility.ShouldTraceError)
            {
                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
            }
            return true;
        }

        public static ExceptionHandler AlwaysHandle =>
            alwaysHandle;

        public static ExceptionHandler AsynchronousThreadExceptionHandler
        {
            [SecurityCritical, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SecurityTreatAsSafe]
            get => 
                asynchronousThreadExceptionHandler;
            [SecurityCritical, SecurityTreatAsSafe, SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
            set
            {
                asynchronousThreadExceptionHandler = value;
            }
        }

        public static ExceptionHandler TransportExceptionHandler
        {
            get => 
                transportExceptionHandler;
            set
            {
                transportExceptionHandler = value;
            }
        }

        private class AlwaysHandleExceptionHandler : ExceptionHandler
        {
            public override bool HandleException(Exception exception) => 
                true;
        }
    }
}

