namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;

    internal class ListViewViewsTypeConverter : TypeConverter
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ListViewActionList instance = (ListViewActionList) context.Instance;
            return new TypeConverter.StandardValuesCollection(instance.GetAllViewNames());
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            true;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => 
            true;
    }
}

