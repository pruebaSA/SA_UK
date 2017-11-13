﻿namespace System.Runtime.Serialization
{
    using System;

    internal class TypeLoadExceptionHolder
    {
        private string m_typeName;

        internal TypeLoadExceptionHolder(string typeName)
        {
            this.m_typeName = typeName;
        }

        internal string TypeName =>
            this.m_typeName;
    }
}

