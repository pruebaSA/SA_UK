namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Runtime.InteropServices;
    using System.Web.UI.Design;

    public class LinqDesignerDataSourceView : DesignerDataSourceView
    {
        private ILinqDataSourceDesignerHelper _helper;
        internal const string DesignerStateDataSourceContextTypeNameKey = "DataSourceContextTypeName";
        internal const string DesignerStateDataSourceSchemaKey = "DataSourceSchema";
        internal const string DesignerStateDataSourceTableNameKey = "DataSourceTableName";

        internal LinqDesignerDataSourceView() : base(new LinqDataSourceDesigner(), string.Empty)
        {
        }

        public LinqDesignerDataSourceView(LinqDataSourceDesigner owner) : base(owner, "DefaultView")
        {
            this._helper = owner.Helper;
        }

        public override IEnumerable GetDesignTimeData(int minimumRows, out bool isSampleData)
        {
            DataTable table = this._helper.LoadSchema();
            if (table != null)
            {
                isSampleData = true;
                return DesignTimeData.GetDesignTimeDataSource(DesignTimeData.CreateSampleDataTable(new DataView(table), true), minimumRows);
            }
            return base.GetDesignTimeData(minimumRows, out isSampleData);
        }

        internal void SetHelper(ILinqDataSourceDesignerHelper helper)
        {
            this._helper = helper;
        }

        public override bool CanDelete =>
            (this.CanModify && this._helper.EnableDelete);

        public override bool CanInsert =>
            (this.CanModify && this._helper.EnableInsert);

        internal bool CanModify =>
            (((this.IsDataContext && this.IsTableTypeTable) && string.IsNullOrEmpty(this._helper.GroupBy)) && string.IsNullOrEmpty(this._helper.Select));

        public override bool CanPage =>
            this._helper.CanPage;

        public override bool CanSort =>
            this._helper.CanSort;

        public override bool CanUpdate =>
            (this.CanModify && this._helper.EnableUpdate);

        public bool IsDataContext =>
            this._helper.IsDataContext(this._helper);

        public bool IsTableTypeTable =>
            this._helper.IsTableTypeTable(this._helper);

        public override IDataSourceViewSchema Schema
        {
            get
            {
                DataTable dataTable = this._helper.LoadSchema();
                if (dataTable == null)
                {
                    return null;
                }
                return new DataSetViewSchema(dataTable);
            }
        }
    }
}

