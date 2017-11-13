namespace System.Data.Services.Common
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class EntitySetAttribute : Attribute
    {
        private readonly string entitySet;

        public EntitySetAttribute(string entitySet)
        {
            this.entitySet = entitySet;
        }

        public string EntitySet =>
            this.entitySet;
    }
}

