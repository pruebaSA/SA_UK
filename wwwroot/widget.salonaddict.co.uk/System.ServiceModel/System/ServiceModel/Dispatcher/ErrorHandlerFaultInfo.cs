﻿namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ErrorHandlerFaultInfo
    {
        private Message fault;
        private bool isConsideredUnhandled;
        private string defaultFaultAction;
        public ErrorHandlerFaultInfo(string defaultFaultAction)
        {
            this.defaultFaultAction = defaultFaultAction;
            this.fault = null;
            this.isConsideredUnhandled = false;
        }

        public Message Fault
        {
            get => 
                this.fault;
            set
            {
                this.fault = value;
            }
        }
        public string DefaultFaultAction
        {
            get => 
                this.defaultFaultAction;
            set
            {
                this.defaultFaultAction = value;
            }
        }
        public bool IsConsideredUnhandled
        {
            get => 
                this.isConsideredUnhandled;
            set
            {
                this.isConsideredUnhandled = value;
            }
        }
    }
}

