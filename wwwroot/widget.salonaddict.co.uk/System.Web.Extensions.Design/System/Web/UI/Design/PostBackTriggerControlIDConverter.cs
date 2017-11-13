namespace System.Web.UI.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.UI;

    public class PostBackTriggerControlIDConverter : StringConverter
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                PostBackTrigger instance = context.Instance as PostBackTrigger;
                if ((instance != null) && (instance.Owner != null))
                {
                    List<string> controlIDs = new List<string>();
                    TriggerDesignUtil.WalkControlTree(instance.Owner, delegate (IComponent component) {
                        Control control = component as Control;
                        if (((control != null) && !string.IsNullOrEmpty(control.ID)) && !controlIDs.Contains(control.ID))
                        {
                            controlIDs.Add(control.ID);
                        }
                        return false;
                    }, false, null);
                    if (controlIDs.Count > 0)
                    {
                        controlIDs.Sort(StringComparer.OrdinalIgnoreCase);
                        return new TypeConverter.StandardValuesCollection(controlIDs);
                    }
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

