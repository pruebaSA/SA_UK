namespace System.IO.Pipes
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class AnonymousPipeClientStream : PipeStream
    {
        [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        public AnonymousPipeClientStream(string pipeHandleAsString) : this(PipeDirection.In, pipeHandleAsString)
        {
        }

        [SecurityCritical, PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        public AnonymousPipeClientStream(PipeDirection direction, SafePipeHandle safePipeHandle) : base(direction, 0)
        {
            if (direction == PipeDirection.InOut)
            {
                throw new NotSupportedException(System.SR.GetString("NotSupported_AnonymousPipeUnidirectional"));
            }
            if (Microsoft.Win32.UnsafeNativeMethods.GetFileType(safePipeHandle) != 3)
            {
                throw new IOException(System.SR.GetString("IO_IO_InvalidPipeHandle"));
            }
            base.InitializeHandle(safePipeHandle, true, false);
            base.State = PipeState.Connected;
        }

        [SecurityCritical, PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        public AnonymousPipeClientStream(PipeDirection direction, string pipeHandleAsString) : this(direction, new SafePipeHandle((IntPtr) long.Parse(pipeHandleAsString), true))
        {
        }

        ~AnonymousPipeClientStream()
        {
            this.Dispose(false);
        }

        public override PipeTransmissionMode ReadMode
        {
            [SecurityCritical]
            set
            {
                this.CheckPipePropertyOperations();
                if ((value < PipeTransmissionMode.Byte) || (value > PipeTransmissionMode.Message))
                {
                    throw new ArgumentOutOfRangeException("value", System.SR.GetString("ArgumentOutOfRange_TransmissionModeByteOrMsg"));
                }
                if (value == PipeTransmissionMode.Message)
                {
                    throw new NotSupportedException(System.SR.GetString("NotSupported_AnonymousPipeMessagesNotSupported"));
                }
            }
        }

        public override PipeTransmissionMode TransmissionMode =>
            PipeTransmissionMode.Byte;
    }
}

