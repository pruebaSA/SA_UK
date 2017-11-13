namespace System.Data.Linq.Mapping
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
    public sealed class ResultTypeAttribute : Attribute
    {
        private System.Type type;

        public ResultTypeAttribute(System.Type type)
        {
            this.type = type;
        }

        public System.Type Type =>
            this.type;
    }
}

