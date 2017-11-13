﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.Threading;

    internal class SignalGate
    {
        private int state;

        public bool Signal()
        {
            int state = this.state;
            switch (state)
            {
                case 0:
                    state = Interlocked.CompareExchange(ref this.state, 1, 0);
                    break;

                case 2:
                    this.state = 3;
                    return true;
            }
            if (state != 0)
            {
                this.ThrowInvalidSignalGateState();
            }
            return false;
        }

        private void ThrowInvalidSignalGateState()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("InvalidLockOperation")));
        }

        public bool Unlock()
        {
            int state = this.state;
            switch (state)
            {
                case 0:
                    state = Interlocked.CompareExchange(ref this.state, 2, 0);
                    break;

                case 1:
                    this.state = 3;
                    return true;
            }
            if (state != 0)
            {
                this.ThrowInvalidSignalGateState();
            }
            return false;
        }

        internal bool IsLocked =>
            (this.state == 0);

        internal bool IsSignalled =>
            (this.state == 3);

        private static class GateState
        {
            public const int Locked = 0;
            public const int Signalled = 3;
            public const int SignalPending = 1;
            public const int Unlocked = 2;
        }
    }
}

