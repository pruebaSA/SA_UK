namespace LinqToSqlShared.Mapping
{
    using System;
    using System.Data.Linq.Mapping;

    internal sealed class ColumnMapping : MemberMapping
    {
        private System.Data.Linq.Mapping.AutoSync autoSync;
        private bool? canBeNull = null;
        private string dbType;
        private string expression;
        private bool isDBGenerated;
        private bool isDiscriminator;
        private bool isPrimaryKey;
        private bool isVersion;
        private System.Data.Linq.Mapping.UpdateCheck updateCheck;

        internal ColumnMapping()
        {
        }

        internal System.Data.Linq.Mapping.AutoSync AutoSync
        {
            get => 
                this.autoSync;
            set
            {
                this.autoSync = value;
            }
        }

        internal bool? CanBeNull
        {
            get => 
                this.canBeNull;
            set
            {
                this.canBeNull = value;
            }
        }

        internal string DbType
        {
            get => 
                this.dbType;
            set
            {
                this.dbType = value;
            }
        }

        internal string Expression
        {
            get => 
                this.expression;
            set
            {
                this.expression = value;
            }
        }

        internal bool IsDbGenerated
        {
            get => 
                this.isDBGenerated;
            set
            {
                this.isDBGenerated = value;
            }
        }

        internal bool IsDiscriminator
        {
            get => 
                this.isDiscriminator;
            set
            {
                this.isDiscriminator = value;
            }
        }

        internal bool IsPrimaryKey
        {
            get => 
                this.isPrimaryKey;
            set
            {
                this.isPrimaryKey = value;
            }
        }

        internal bool IsVersion
        {
            get => 
                this.isVersion;
            set
            {
                this.isVersion = value;
            }
        }

        internal System.Data.Linq.Mapping.UpdateCheck UpdateCheck
        {
            get => 
                this.updateCheck;
            set
            {
                this.updateCheck = value;
            }
        }

        internal string XmlAutoSync
        {
            get
            {
                if (this.autoSync == System.Data.Linq.Mapping.AutoSync.Default)
                {
                    return null;
                }
                return this.autoSync.ToString();
            }
            set
            {
                this.autoSync = (value != null) ? ((System.Data.Linq.Mapping.AutoSync) Enum.Parse(typeof(System.Data.Linq.Mapping.AutoSync), value)) : System.Data.Linq.Mapping.AutoSync.Default;
            }
        }

        internal string XmlCanBeNull
        {
            get
            {
                if (this.canBeNull.HasValue && (this.canBeNull != true))
                {
                    return "false";
                }
                return null;
            }
            set
            {
                this.canBeNull = new bool?((value != null) ? bool.Parse(value) : true);
            }
        }

        internal string XmlIsDbGenerated
        {
            get
            {
                if (!this.isDBGenerated)
                {
                    return null;
                }
                return "true";
            }
            set
            {
                this.isDBGenerated = (value != null) ? bool.Parse(value) : false;
            }
        }

        internal string XmlIsDiscriminator
        {
            get
            {
                if (!this.isDiscriminator)
                {
                    return null;
                }
                return "true";
            }
            set
            {
                this.isDiscriminator = (value != null) ? bool.Parse(value) : false;
            }
        }

        internal string XmlIsPrimaryKey
        {
            get
            {
                if (!this.isPrimaryKey)
                {
                    return null;
                }
                return "true";
            }
            set
            {
                this.isPrimaryKey = (value != null) ? bool.Parse(value) : false;
            }
        }

        internal string XmlIsVersion
        {
            get
            {
                if (!this.isVersion)
                {
                    return null;
                }
                return "true";
            }
            set
            {
                this.isVersion = (value != null) ? bool.Parse(value) : false;
            }
        }

        internal string XmlUpdateCheck
        {
            get
            {
                if (this.updateCheck == System.Data.Linq.Mapping.UpdateCheck.Always)
                {
                    return null;
                }
                return this.updateCheck.ToString();
            }
            set
            {
                this.updateCheck = (value == null) ? System.Data.Linq.Mapping.UpdateCheck.Always : ((System.Data.Linq.Mapping.UpdateCheck) Enum.Parse(typeof(System.Data.Linq.Mapping.UpdateCheck), value));
            }
        }
    }
}

