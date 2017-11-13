namespace System.Web.UI.Design
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public abstract class DesignerDataSourceView
    {
        private string _name;
        private IDataSourceDesigner _owner;

        protected DesignerDataSourceView(IDataSourceDesigner owner, string viewName)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            if (viewName == null)
            {
                throw new ArgumentNullException("viewName");
            }
            this._owner = owner;
            this._name = viewName;
        }

        public virtual IEnumerable GetDesignTimeData(int minimumRows, out bool isSampleData)
        {
            isSampleData = true;
            return DesignTimeData.GetDesignTimeDataSource(DesignTimeData.CreateDummyDataBoundDataTable(), minimumRows);
        }

        public virtual bool CanDelete =>
            false;

        public virtual bool CanInsert =>
            false;

        public virtual bool CanPage =>
            false;

        public virtual bool CanRetrieveTotalRowCount =>
            false;

        public virtual bool CanSort =>
            false;

        public virtual bool CanUpdate =>
            false;

        public IDataSourceDesigner DataSourceDesigner =>
            this._owner;

        public string Name =>
            this._name;

        public virtual IDataSourceViewSchema Schema =>
            null;
    }
}

