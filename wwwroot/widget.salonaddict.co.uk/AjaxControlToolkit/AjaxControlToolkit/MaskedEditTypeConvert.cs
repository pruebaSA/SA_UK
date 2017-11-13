namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Web.UI;

    public class MaskedEditTypeConvert : StringConverter
    {
        private static object[] GetControls(IContainer container)
        {
            ArrayList list = new ArrayList();
            foreach (IComponent component in container.Components)
            {
                Control serverControl = component as Control;
                if ((((serverControl != null) && !(serverControl is Page)) && ((serverControl.ID != null) && (serverControl.ID.Length != 0))) && IncludeControl(serverControl))
                {
                    list.Add(serverControl.ID);
                }
            }
            list.Sort(Comparer.Default);
            return list.ToArray();
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if ((context != null) && (context.Container != null))
            {
                object[] controls = GetControls(context.Container);
                if (controls != null)
                {
                    return new TypeConverter.StandardValuesCollection(controls);
                }
            }
            return null;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            false;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => 
            true;

        private static bool IncludeControl(Control serverControl)
        {
            bool flag = false;
            if (serverControl.GetType().ToString().IndexOf("Sys.Extended.UI.maskededitextender", StringComparison.OrdinalIgnoreCase) != -1)
            {
                flag = true;
            }
            return flag;
        }
    }
}

