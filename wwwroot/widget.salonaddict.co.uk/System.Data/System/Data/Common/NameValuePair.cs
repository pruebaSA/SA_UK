﻿namespace System.Data.Common
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal sealed class NameValuePair
    {
        [OptionalField(VersionAdded=2)]
        private readonly int _length;
        private readonly string _name;
        private NameValuePair _next;
        private readonly string _value;

        internal NameValuePair(string name, string value, int length)
        {
            this._name = name;
            this._value = value;
            this._length = length;
        }

        internal int Length =>
            this._length;

        internal string Name =>
            this._name;

        internal NameValuePair Next
        {
            get => 
                this._next;
            set
            {
                if ((this._next != null) || (value == null))
                {
                    throw ADP.InternalError(ADP.InternalErrorCode.NameValuePairNext);
                }
                this._next = value;
            }
        }

        internal string Value =>
            this._value;
    }
}

