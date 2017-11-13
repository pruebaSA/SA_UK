﻿namespace System.Resources
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ResourceLocator
    {
        internal object _value;
        internal int _dataPos;
        internal ResourceLocator(int dataPos, object value)
        {
            this._dataPos = dataPos;
            this._value = value;
        }

        internal int DataPosition =>
            this._dataPos;
        internal object Value
        {
            get => 
                this._value;
            set
            {
                this._value = value;
            }
        }
        internal static bool CanCache(ResourceTypeCode value) => 
            (value <= ResourceTypeCode.TimeSpan);
    }
}

