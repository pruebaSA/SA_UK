namespace System.Data.Linq.Mapping
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public sealed class TableAttribute : Attribute
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

