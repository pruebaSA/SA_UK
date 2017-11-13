namespace LinqToSqlShared.Mapping
{
    using System;
    using System.Collections.Generic;

    internal class DatabaseMapping
    {
        private string databaseName;
        private List<FunctionMapping> functions = new List<FunctionMapping>();
        private string provider;
        private List<TableMapping> tables = new List<TableMapping>();

        internal DatabaseMapping()
        {
        }

        internal FunctionMapping GetFunction(string functionName)
        {
            foreach (FunctionMapping mapping in this.functions)
            {
                if (string.Compare(mapping.Name, functionName, StringComparison.Ordinal) == 0)
                {
                    return mapping;
                }
            }
            return null;
        }

        internal TableMapping GetTable(string tableName)
        {
            foreach (TableMapping mapping in this.tables)
            {
                if (string.Compare(mapping.TableName, tableName, StringComparison.Ordinal) == 0)
                {
                    return mapping;
                }
            }
            return null;
        }

        internal TableMapping GetTable(Type rowType)
        {
            foreach (TableMapping mapping in this.tables)
            {
                if (this.IsType(mapping.RowType, rowType))
                {
                    return mapping;
                }
            }
            return null;
        }

        private bool IsType(TypeMapping map, Type type)
        {
            if (((string.Compare(map.Name, type.Name, StringComparison.Ordinal) == 0) || (string.Compare(map.Name, type.FullName, StringComparison.Ordinal) == 0)) || (string.Compare(map.Name, type.AssemblyQualifiedName, StringComparison.Ordinal) == 0))
            {
                return true;
            }
            foreach (TypeMapping mapping in map.DerivedTypes)
            {
                if (this.IsType(mapping, type))
                {
                    return true;
                }
            }
            return false;
        }

        internal string DatabaseName
        {
            get => 
                this.databaseName;
            set
            {
                this.databaseName = value;
            }
        }

        internal List<FunctionMapping> Functions =>
            this.functions;

        internal string Provider
        {
            get => 
                this.provider;
            set
            {
                this.provider = value;
            }
        }

        internal List<TableMapping> Tables =>
            this.tables;
    }
}

