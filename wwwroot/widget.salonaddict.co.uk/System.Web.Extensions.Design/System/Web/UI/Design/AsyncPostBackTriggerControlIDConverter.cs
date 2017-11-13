namespace System.Web.UI.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.UI;

    public class AsyncPostBackTriggerControlIDConverter : StringConverter
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                AsyncPostBackTrigger instance = context.Instance as AsyncPostBackTrigger;
                if (((instance != null) && (instance.Owner != null)) && (instance.Owner.Site != null))
                {
                    Predicate<IComponent> visitor = null;
                    List<string> controlIDs = new List<string>();
                    IDesignerHost service = (IDesignerHost) instance.Owner.Site.GetService(typeof(IDesignerHost));
                    Control rootComponent = service.RootComponent as Control;
                    if (rootComponent != null)
                    {
                        if (visitor == null)
                        {
                            visitor = delegate (IComponent component) {
                                Control control = component as Control;
                                if (((control != null) && !string.IsNullOrEmpty(control.ID)) && !controlIDs.Contains(control.ID))
                                {
                                    controlIDs.Add(control.ID);
                                }
                                return false;
                            };
                        }
                        TriggerDesignUtil.WalkControlTree(rootComponent, visitor, true, instance.Owner);
                    }
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

