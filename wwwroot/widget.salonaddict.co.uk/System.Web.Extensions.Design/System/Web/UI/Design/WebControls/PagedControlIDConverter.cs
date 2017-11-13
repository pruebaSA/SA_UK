namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal class PagedControlIDConverter : StringConverter
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            string[] values = null;
            if (context != null)
            {
                WebFormsRootDesigner designer = null;
                IDesignerHost service = (IDesignerHost) context.GetService(typeof(IDesignerHost));
                if (service != null)
                {
                    IComponent rootComponent = service.RootComponent;
                    if (rootComponent != null)
                    {
                        designer = service.GetDesigner(rootComponent) as WebFormsRootDesigner;
                    }
                }
                if ((designer != null) && !designer.IsDesignerViewLocked)
                {
                    IComponent instance = context.Instance as IComponent;
                    if (instance == null)
                    {
                        DesignerActionList list = context.Instance as DesignerActionList;
                        if (list != null)
                        {
                            instance = list.Component;
                        }
                    }
                    IList<IComponent> allComponents = ControlHelper.GetAllComponents(instance, new ControlHelper.IsValidComponentDelegate(this.IsValidPagedControl));
                    List<string> list3 = new List<string>();
                    foreach (IComponent component3 in allComponents)
                    {
                        Control control = component3 as Control;
                        if (((control != null) && !string.IsNullOrEmpty(control.ID)) && !list3.Contains(control.ID))
                        {
                            list3.Add(control.ID);
                        }
                    }
                    list3.Sort(StringComparer.OrdinalIgnoreCase);
                    values = list3.ToArray();
                }
            }
            return new TypeConverter.StandardValuesCollection(values);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            false;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => 
            true;

        protected virtual bool IsValidPagedControl(IComponent component)
        {
            Control control = component as Control;
            if (control == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(control.ID))
            {
                return false;
            }
            return (component is IPageableItemContainer);
        }
    }
}

