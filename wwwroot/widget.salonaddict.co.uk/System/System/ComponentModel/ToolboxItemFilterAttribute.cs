namespace System.ComponentModel
{
    using System;

    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public sealed class ToolboxItemFilterAttribute : Attribute
    {
        private string filterString;
        private ToolboxItemFilterType filterType;
        private string typeId;

        public ToolboxItemFilterAttribute(string filterString) : this(filterString, ToolboxItemFilterType.Allow)
        {
        }

        public ToolboxItemFilterAttribute(string filterString, ToolboxItemFilterType filterType)
        {
            if (filterString == null)
            {
                filterString = string.Empty;
            }
            this.filterString = filterString;
            this.filterType = filterType;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            ToolboxItemFilterAttribute attribute = obj as ToolboxItemFilterAttribute;
            return (((attribute != null) && attribute.FilterType.Equals(this.FilterType)) && attribute.FilterString.Equals(this.FilterString));
        }

        public override int GetHashCode() => 
            this.filterString.GetHashCode();

        public override bool Match(object obj)
        {
            ToolboxItemFilterAttribute attribute = obj as ToolboxItemFilterAttribute;
            if (attribute == null)
            {
                return false;
            }
            if (!attribute.FilterString.Equals(this.FilterString))
            {
                return false;
            }
            return true;
        }

        public override string ToString() => 
            (this.filterString + "," + Enum.GetName(typeof(ToolboxItemFilterType), this.filterType));

        public string FilterString =>
            this.filterString;

        public ToolboxItemFilterType FilterType =>
            this.filterType;

        public override object TypeId
        {
            get
            {
                if (this.typeId == null)
                {
                    this.typeId = base.GetType().FullName + this.filterString;
                }
                return this.typeId;
            }
        }
    }
}

