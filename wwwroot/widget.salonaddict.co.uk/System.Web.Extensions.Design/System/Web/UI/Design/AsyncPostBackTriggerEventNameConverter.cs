namespace System.Web.UI.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.UI;

    public class AsyncPostBackTriggerEventNameConverter : StringConverter
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                Predicate<IComponent> visitor = null;
                AsyncPostBackTrigger trigger = context.Instance as AsyncPostBackTrigger;
                Control targetControl = null;
                if ((!string.IsNullOrEmpty(trigger.ControlID) && (trigger != null)) && ((trigger.Owner != null) && (trigger.Owner.Site != null)))
                {
                    IDesignerHost service = (IDesignerHost) trigger.Owner.Site.GetService(typeof(IDesignerHost));
                    Control rootComponent = service.RootComponent as Control;
                    if (rootComponent != null)
                    {
                        if (visitor == null)
                        {
                            visitor = delegate (IComponent component) {
                                Control control = component as Control;
                                if ((control != null) && string.Equals(control.ID, trigger.ControlID, StringComparison.OrdinalIgnoreCase))
                                {
                                    targetControl = control;
                                    return true;
                                }
                                return false;
                            };
                        }
                        TriggerDesignUtil.WalkControlTree(rootComponent, visitor, true, trigger.Owner);
                    }
                }
                if (targetControl != null)
                {
                    List<string> values = new List<string>();
                    foreach (EventDescriptor descriptor in TypeDescriptor.GetEvents(targetControl))
                    {
                        values.Add(descriptor.Name);
                    }
                    if (values.Count > 0)
                    {
                        values.Sort(StringComparer.OrdinalIgnoreCase);
                        return new TypeConverter.StandardValuesCollection(values);
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

