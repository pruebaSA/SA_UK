namespace System.Data.Linq.Mapping
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class DatabaseAttribute : Attribute
    {
        private string name;

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }
    }
}

