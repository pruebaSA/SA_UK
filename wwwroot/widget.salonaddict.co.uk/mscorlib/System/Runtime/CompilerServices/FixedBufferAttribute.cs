namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited=false)]
    public sealed class FixedBufferAttribute : Attribute
    {
        private Type elementType;
        private int length;

        public FixedBufferAttribute(Type elementType, int length)
        {
            this.elementType = elementType;
            this.length = length;
        }

        public Type ElementType =>
            this.elementType;

        public int Length =>
            this.length;
    }
}

