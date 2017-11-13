﻿namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true), AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false, Inherited=false)]
    public sealed class ObfuscateAssemblyAttribute : Attribute
    {
        private bool m_assemblyIsPrivate;
        private bool m_strip = true;

        public ObfuscateAssemblyAttribute(bool assemblyIsPrivate)
        {
            this.m_assemblyIsPrivate = assemblyIsPrivate;
        }

        public bool AssemblyIsPrivate =>
            this.m_assemblyIsPrivate;

        public bool StripAfterObfuscation
        {
            get => 
                this.m_strip;
            set
            {
                this.m_strip = value;
            }
        }
    }
}

