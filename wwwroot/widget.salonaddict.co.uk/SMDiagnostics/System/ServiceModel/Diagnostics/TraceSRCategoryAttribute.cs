namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class TraceSRCategoryAttribute : CategoryAttribute
    {
        public TraceSRCategoryAttribute(string category) : base(category)
        {
        }

        protected override string GetLocalizedString(string value) => 
            TraceSR.GetString(value);
    }
}

