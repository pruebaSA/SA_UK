﻿namespace System.Runtime.InteropServices
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited=false), ComVisible(true)]
    public sealed class TypeLibVarAttribute : Attribute
    {
        internal TypeLibVarFlags _val;

        public TypeLibVarAttribute(short flags)
        {
            this._val = (TypeLibVarFlags) flags;
        }

        public TypeLibVarAttribute(TypeLibVarFlags flags)
        {
            this._val = flags;
        }

        public TypeLibVarFlags Value =>
            this._val;
    }
}

