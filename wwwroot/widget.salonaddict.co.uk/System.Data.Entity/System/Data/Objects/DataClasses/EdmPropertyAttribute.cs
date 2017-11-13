namespace System.Data.Objects.DataClasses
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class EdmPropertyAttribute : Attribute
    {
        internal EdmPropertyAttribute()
        {
        }
    }
}

