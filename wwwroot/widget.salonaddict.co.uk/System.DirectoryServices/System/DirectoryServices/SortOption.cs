namespace System.DirectoryServices
{
    using System;
    using System.ComponentModel;

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SortOption
    {
        private string propertyName;
        private SortDirection sortDirection;

        public SortOption()
        {
        }

        public SortOption(string propertyName, SortDirection direction)
        {
            this.PropertyName = propertyName;
            this.Direction = this.sortDirection;
        }

        [DefaultValue(0), DSDescription("DSSortDirection")]
        public SortDirection Direction
        {
            get => 
                this.sortDirection;
            set
            {
                if ((value < SortDirection.Ascending) || (value > SortDirection.Descending))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(SortDirection));
                }
                this.sortDirection = value;
            }
        }

        [DefaultValue((string) null), DSDescription("DSSortName")]
        public string PropertyName
        {
            get => 
                this.propertyName;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.propertyName = value;
            }
        }
    }
}

