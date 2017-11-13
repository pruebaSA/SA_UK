namespace System.Web.UI.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.UI;

    public class UpdateProgressAssociatedUpdatePanelIDConverter : StringConverter
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                List<string> values = new List<string>();
                IContainer container = context.Container;
                if (container != null)
                {
                    foreach (IComponent component in container.Components)
                    {
                        UpdatePanel panel = component as UpdatePanel;
                        if (((panel != null) && !string.IsNullOrEmpty(panel.ID)) && !values.Contains(panel.ID))
                        {
                            values.Add(panel.ID);
                        }
                    }
                }
                if (values.Count > 0)
                {
                    values.Sort(StringComparer.OrdinalIgnoreCase);
                    return new TypeConverter.StandardValuesCollection(values);
                }
            }
            return base.GetStandardValues(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            false;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => 
            true;
    }
}

