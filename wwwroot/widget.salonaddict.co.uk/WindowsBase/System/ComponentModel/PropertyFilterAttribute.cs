namespace System.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class PropertyFilterAttribute : Attribute
    {
        private PropertyFilterOptions _filter;
        public static readonly PropertyFilterAttribute Default = new PropertyFilterAttribute(PropertyFilterOptions.All);

        public PropertyFilterAttribute(PropertyFilterOptions filter)
        {
            this._filter = filter;
        }

        public override bool Equals(object value)
        {
            PropertyFilterAttribute attribute = value as PropertyFilterAttribute;
            return ((attribute != null) && attribute._filter.Equals(this._filter));
        }

        public override int GetHashCode() => 
            this._filter.GetHashCode();

        public override bool Match(object value)
        {
            PropertyFilterAttribute attribute = value as PropertyFilterAttribute;
            if (attribute == null)
            {
                return false;
            }
            return ((this._filter & attribute._filter) == this._filter);
        }

        public PropertyFilterOptions Filter =>
            this._filter;
    }
}

