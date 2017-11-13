namespace System.Data
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class EDesignResCategoryAttribute : CategoryAttribute
    {
        public EDesignResCategoryAttribute(string category) : base(category)
        {
        }

        protected override string GetLocalizedString(string value) => 
            EDesignRes.GetString(value);
    }
}

