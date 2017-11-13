namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Reflection;

    public class SelectedProperty
    {
        private string key;
        private PropertyInfo property;

        public SelectedProperty(PropertyInfo property, string key)
        {
            this.property = property;
            this.key = key;
        }

        public string Key =>
            this.key;

        public PropertyInfo Property =>
            this.property;
    }
}

