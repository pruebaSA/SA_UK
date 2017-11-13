namespace LinqToSqlShared.Mapping
{
    using System;

    internal class TableMapping
    {
        private string member;
        private TypeMapping rowType;
        private string tableName;

        internal TableMapping()
        {
        }

        internal string Member
        {
            get => 
                this.member;
            set
            {
                this.member = value;
            }
        }

        internal TypeMapping RowType
        {
            get => 
                this.rowType;
            set
            {
                this.rowType = value;
            }
        }

        internal string TableName
        {
            get => 
                this.tableName;
            set
            {
                this.tableName = value;
            }
        }
    }
}

