namespace System.Data.Linq.Mapping
{
    using System;

    public abstract class DataAttribute : Attribute
    {
        private string name;
        private string storage;

        protected DataAttribute()
        {
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

        public string Storage
        {
            get => 
                this.storage;
            set
            {
                this.storage = value;
            }
        }
    }
}

