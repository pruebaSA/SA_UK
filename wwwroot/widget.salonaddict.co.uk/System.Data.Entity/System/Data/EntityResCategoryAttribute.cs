namespace System.Data
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class EntityResCategoryAttribute : CategoryAttribute
    {
        public EntityResCategoryAttribute(string category) : base(category)
        {
        }

        protected override string GetLocalizedString(string value) => 
            EntityRes.GetString(value);
    }
}

