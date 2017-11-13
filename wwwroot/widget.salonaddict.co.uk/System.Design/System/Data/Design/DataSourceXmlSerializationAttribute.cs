namespace System.Data.Design
{
    using System;

    internal abstract class DataSourceXmlSerializationAttribute : Attribute
    {
        private Type itemType;
        private string name;
        private bool specialWay = false;

        internal DataSourceXmlSerializationAttribute()
        {
        }

        public Type ItemType
        {
            get => 
                this.itemType;
            set
            {
                this.itemType = value;
            }
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        public bool SpecialWay
        {
            get => 
                this.specialWay;
            set
            {
                this.specialWay = value;
            }
        }
    }
}

