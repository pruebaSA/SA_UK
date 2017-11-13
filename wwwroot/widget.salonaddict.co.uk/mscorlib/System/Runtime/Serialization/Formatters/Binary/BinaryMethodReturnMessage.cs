﻿namespace System.Runtime.Serialization.Formatters.Binary
{
    using System;
    using System.Collections;
    using System.Runtime.Remoting.Messaging;

    [Serializable]
    internal class BinaryMethodReturnMessage
    {
        private object[] _args;
        private System.Exception _exception;
        private System.Runtime.Remoting.Messaging.LogicalCallContext _logicalCallContext;
        private object[] _outargs;
        private object[] _properties;
        private object _returnValue;

        internal BinaryMethodReturnMessage(object returnValue, object[] args, System.Exception e, System.Runtime.Remoting.Messaging.LogicalCallContext callContext, object[] properties)
        {
            this._returnValue = returnValue;
            if (args == null)
            {
                args = new object[0];
            }
            this._outargs = args;
            this._args = args;
            this._exception = e;
            if (callContext == null)
            {
                this._logicalCallContext = new System.Runtime.Remoting.Messaging.LogicalCallContext();
            }
            else
            {
                this._logicalCallContext = callContext;
            }
            this._properties = properties;
        }

        internal void PopulateMessageProperties(IDictionary dict)
        {
            foreach (DictionaryEntry entry in this._properties)
            {
                dict[entry.Key] = entry.Value;
            }
        }

        public object[] Args =>
            this._args;

        public System.Exception Exception =>
            this._exception;

        public bool HasProperties =>
            (this._properties != null);

        public System.Runtime.Remoting.Messaging.LogicalCallContext LogicalCallContext =>
            this._logicalCallContext;

        public object ReturnValue =>
            this._returnValue;
    }
}

