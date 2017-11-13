namespace System.Data.Linq.Mapping
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false)]
    public sealed class ColumnAttribute : DataAttribute
    {
        private System.Data.Linq.Mapping.AutoSync autoSync;
        private bool canBeNull = true;
        private bool canBeNullSet;
        private System.Data.Linq.Mapping.UpdateCheck check = System.Data.Linq.Mapping.UpdateCheck.Always;
        private string dbtype;
        private string expression;
        private bool isDBGenerated;
        private bool isDiscriminator;
        private bool isPrimaryKey;
        private bool isVersion;

        public System.Data.Linq.Mapping.AutoSync AutoSync
        {
            get => 
                this.autoSync;
            set
            {
                this.autoSync = value;
            }
        }

        public bool CanBeNull
        {
            get => 
                this.canBeNull;
            set
            {
                this.canBeNullSet = true;
                this.canBeNull = value;
            }
        }

        internal bool CanBeNullSet =>
            this.canBeNullSet;

        public string DbType
        {
            get => 
                this.dbtype;
            set
            {
                this.dbtype = value;
            }
        }

        public string Expression
        {
            get => 
                this.expression;
            set
            {
                this.expression = value;
            }
        }

        public bool IsDbGenerated
        {
            get => 
                this.isDBGenerated;
            set
            {
                this.isDBGenerated = value;
            }
        }

        public bool IsDiscriminator
        {
            get => 
                this.isDiscriminator;
            set
            {
                this.isDiscriminator = value;
            }
        }

        public bool IsPrimaryKey
        {
            get => 
                this.isPrimaryKey;
            set
            {
                this.isPrimaryKey = value;
            }
        }

        public bool IsVersion
        {
            get => 
                this.isVersion;
            set
            {
                this.isVersion = value;
            }
        }

        public System.Data.Linq.Mapping.UpdateCheck UpdateCheck
        {
            get => 
                this.check;
            set
            {
                this.check = value;
            }
        }
    }
}

