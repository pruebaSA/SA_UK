﻿namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class ValueSerializerAttribute : Attribute
    {
        private Type _valueSerializerType;
        private string _valueSerializerTypeName;

        public ValueSerializerAttribute(string valueSerializerTypeName)
        {
            this._valueSerializerTypeName = valueSerializerTypeName;
        }

        public ValueSerializerAttribute(Type valueSerializerType)
        {
            this._valueSerializerType = valueSerializerType;
        }

        public Type ValueSerializerType
        {
            get
            {
                if ((this._valueSerializerType == null) && (this._valueSerializerTypeName != null))
                {
                    this._valueSerializerType = Type.GetType(this._valueSerializerTypeName);
                }
                return this._valueSerializerType;
            }
        }

        public string ValueSerializerTypeName
        {
            get
            {
                if (this._valueSerializerType != null)
                {
                    return this._valueSerializerType.AssemblyQualifiedName;
                }
                return this._valueSerializerTypeName;
            }
        }
    }
}

