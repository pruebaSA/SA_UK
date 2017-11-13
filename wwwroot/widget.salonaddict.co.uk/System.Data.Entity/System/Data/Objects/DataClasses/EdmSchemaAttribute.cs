namespace System.Data.Objects.DataClasses
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class EdmSchemaAttribute : Attribute
    {
        public EdmSchemaAttribute()
        {
        }

        public EdmSchemaAttribute(string assemblyGuid)
        {
            if (assemblyGuid == null)
            {
                throw new ArgumentNullException("assemblyGuid");
            }
        }
    }
}

