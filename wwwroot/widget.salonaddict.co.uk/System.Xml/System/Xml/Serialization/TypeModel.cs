﻿namespace System.Xml.Serialization
{
    using System;

    internal abstract class TypeModel
    {
        private System.Xml.Serialization.ModelScope scope;
        private System.Type type;
        private System.Xml.Serialization.TypeDesc typeDesc;

        protected TypeModel(System.Type type, System.Xml.Serialization.TypeDesc typeDesc, System.Xml.Serialization.ModelScope scope)
        {
            this.scope = scope;
            this.type = type;
            this.typeDesc = typeDesc;
        }

        internal System.Xml.Serialization.ModelScope ModelScope =>
            this.scope;

        internal System.Type Type =>
            this.type;

        internal System.Xml.Serialization.TypeDesc TypeDesc =>
            this.typeDesc;
    }
}

