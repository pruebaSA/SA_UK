namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.UI.WebControls;

    internal class LinqDataSourceTableConverter : StringConverter
    {
        private LinqDataSource _linqDataSource;
        private IServiceProvider _serviceProvider;

        public LinqDataSourceTableConverter()
        {
        }

        internal LinqDataSourceTableConverter(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this._linqDataSource != null)
            {
                List<ILinqDataSourcePropertyItem> tables = new LinqDataSourceDesignerHelper().GetTables(new LinqDataSourceContextTypeItem(Type.GetType(this._linqDataSource.ContextTypeName)), true);
                if ((tables != null) && (tables.Count > 0))
                {
                    string[] values = new string[tables.Count];
                    for (int i = 0; i < tables.Count; i++)
                    {
                        values[i] = tables[i].Name;
                    }
                    return new TypeConverter.StandardValuesCollection(values);
                }
            }
            return null;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            false;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            IServiceProvider site;
            this._linqDataSource = (LinqDataSource) context.Instance;
            if (this._serviceProvider != null)
            {
                site = this._serviceProvider;
            }
            else
            {
                site = this._linqDataSource.Site;
            }
            IDesignerHost service = (IDesignerHost) site.GetService(typeof(IDesignerHost));
            LinqDataSourceDesigner designer = (LinqDataSourceDesigner) service.GetDesigner(this._linqDataSource);
            if (string.IsNullOrEmpty(this._linqDataSource.ContextTypeName))
            {
                return false;
            }
            return true;
        }
    }
}

