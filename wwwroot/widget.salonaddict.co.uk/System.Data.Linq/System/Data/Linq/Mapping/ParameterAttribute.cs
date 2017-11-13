namespace System.Data.Linq.Mapping
{
    using System;

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple=false)]
    public sealed class ParameterAttribute : Attribute
    {
        private string dbType;
        private string name;

        public string DbType
        {
            get => 
                this.dbType;
            set
            {
                this.dbType = value;
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
    }
}

