﻿namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;

    internal class SharedRuntimeState
    {
        private bool enableFaults = true;
        private bool isImmutable;
        private bool isOnServer;
        private bool manualAddressing;
        private bool validateMustUnderstand = true;

        internal SharedRuntimeState(bool isOnServer)
        {
            this.isOnServer = isOnServer;
        }

        internal void LockDownProperties()
        {
            this.isImmutable = true;
        }

        internal void ThrowIfImmutable()
        {
            if (this.isImmutable)
            {
                if (this.IsOnServer)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxImmutableServiceHostBehavior0")));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxImmutableChannelFactoryBehavior0")));
            }
        }

        internal bool EnableFaults
        {
            get => 
                this.enableFaults;
            set
            {
                this.enableFaults = value;
            }
        }

        internal bool IsOnServer =>
            this.isOnServer;

        internal bool ManualAddressing
        {
            get => 
                this.manualAddressing;
            set
            {
                this.manualAddressing = value;
            }
        }

        internal bool ValidateMustUnderstand
        {
            get => 
                this.validateMustUnderstand;
            set
            {
                this.validateMustUnderstand = value;
            }
        }
    }
}

