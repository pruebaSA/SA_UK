namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Reflection;

    internal sealed class MappedTable : MetaTable
    {
        private MethodInfo deleteMethod;
        private bool hasMethods;
        private MethodInfo insertMethod;
        private TableMapping mapping;
        private MappedMetaModel model;
        private MetaType rowType;
        private MethodInfo updateMethod;

        internal MappedTable(MappedMetaModel model, TableMapping mapping, Type rowType)
        {
            this.model = model;
            this.mapping = mapping;
            this.rowType = new MappedRootType(model, this, mapping.RowType, rowType);
        }

        private void InitMethods()
        {
            if (!this.hasMethods)
            {
                this.insertMethod = MethodFinder.FindMethod(this.model.ContextType, "Insert" + this.rowType.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, new Type[] { this.rowType.Type });
                this.updateMethod = MethodFinder.FindMethod(this.model.ContextType, "Update" + this.rowType.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, new Type[] { this.rowType.Type });
                this.deleteMethod = MethodFinder.FindMethod(this.model.ContextType, "Delete" + this.rowType.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, new Type[] { this.rowType.Type });
                this.hasMethods = true;
            }
        }

        public override MethodInfo DeleteMethod
        {
            get
            {
                this.InitMethods();
                return this.deleteMethod;
            }
        }

        public override MethodInfo InsertMethod
        {
            get
            {
                this.InitMethods();
                return this.insertMethod;
            }
        }

        public override MetaModel Model =>
            this.model;

        public override MetaType RowType =>
            this.rowType;

        public override string TableName =>
            this.mapping.TableName;

        public override MethodInfo UpdateMethod
        {
            get
            {
                this.InitMethods();
                return this.updateMethod;
            }
        }
    }
}

