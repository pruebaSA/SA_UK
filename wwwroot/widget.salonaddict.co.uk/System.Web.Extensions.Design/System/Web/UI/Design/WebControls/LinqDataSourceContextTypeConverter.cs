namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Data.Linq;
    using System.Web.UI.WebControls;

    internal class LinqDataSourceContextTypeConverter : StringConverter
    {
        private List<ILinqDataSourceContextTypeItem> _contextTypes;
        private IServiceProvider _serviceProvider;

        public LinqDataSourceContextTypeConverter()
        {
        }

        internal LinqDataSourceContextTypeConverter(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this._contextTypes.Count <= 0)
            {
                return null;
            }
            string[] values = new string[this._contextTypes.Count];
            for (int i = 0; i < this._contextTypes.Count; i++)
            {
                values[i] = this._contextTypes[i].Type.FullName;
            }
            return new TypeConverter.StandardValuesCollection(values);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            false;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            IServiceProvider site;
            LinqDataSource instance = (LinqDataSource) context.Instance;
            if (this._serviceProvider != null)
            {
                site = this._serviceProvider;
            }
            else
            {
                site = instance.Site;
            }
            IDesignerHost service = (IDesignerHost) site.GetService(typeof(IDesignerHost));
            LinqDataSourceDesigner designer = (LinqDataSourceDesigner) service.GetDesigner(instance);
            this._contextTypes = new LinqDataSourceDesignerHelper().GetContextTypes(typeof(DataContext), site);
            return (this._contextTypes.Count > 0);
        }
    }
}

