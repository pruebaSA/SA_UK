namespace LinqToSqlShared.Mapping
{
    using System;

    internal class ReturnMapping
    {
        private string dbType;

        internal string DbType
        {
            get => 
                this.dbType;
            set
            {
                this.dbType = value;
            }
        }
    }
}

