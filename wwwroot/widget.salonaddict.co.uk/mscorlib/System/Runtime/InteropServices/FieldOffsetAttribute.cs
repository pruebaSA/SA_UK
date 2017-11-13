namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;

    [ComVisible(true), AttributeUsage(AttributeTargets.Field, Inherited=false)]
    public sealed class FieldOffsetAttribute : Attribute
    {
        internal int _val;

        public FieldOffsetAttribute(int offset)
        {
            this._val = offset;
        }

        internal static Attribute GetCustomAttribute(RuntimeFieldInfo field)
        {
            int num;
            if ((field.DeclaringType != null) && field.Module.MetadataImport.GetFieldOffset(field.DeclaringType.MetadataToken, field.MetadataToken, out num))
            {
                return new FieldOffsetAttribute(num);
            }
            return null;
        }

        internal static bool IsDefined(RuntimeFieldInfo field) => 
            (GetCustomAttribute(field) != null);

        public int Value =>
            this._val;
    }
}

