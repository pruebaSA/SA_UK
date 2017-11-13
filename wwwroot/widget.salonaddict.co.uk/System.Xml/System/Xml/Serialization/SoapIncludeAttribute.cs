﻿namespace System.Xml.Serialization
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=true)]
    public class SoapIncludeAttribute : Attribute
    {
        private System.Type type;

        public SoapIncludeAttribute(System.Type type)
        {
            this.type = type;
        }

        public System.Type Type
        {
            get => 
                this.type;
            set
            {
                this.type = value;
            }
        }
    }
}

