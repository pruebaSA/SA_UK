namespace System.Runtime.Serialization
{
    using System;

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited=true, AllowMultiple=true)]
    public sealed class KnownTypeAttribute : Attribute
    {
        private string methodName;
        private System.Type type;

        private KnownTypeAttribute()
        {
        }

        public KnownTypeAttribute(string methodName)
        {
            this.methodName = methodName;
        }

        public KnownTypeAttribute(System.Type type)
        {
            this.type = type;
        }

        public string MethodName =>
            this.methodName;

        public System.Type Type =>
            this.type;
    }
}

