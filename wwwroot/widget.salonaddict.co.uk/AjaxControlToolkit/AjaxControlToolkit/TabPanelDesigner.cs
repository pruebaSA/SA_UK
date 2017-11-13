namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.Design;

    internal class TabPanelDesigner : ControlDesigner
    {
        protected override void PreFilterProperties(IDictionary properties)
        {
            PropertyDescriptor[] descriptorArray = new PropertyDescriptor[] { (PropertyDescriptor) properties["HeaderTemplate"], (PropertyDescriptor) properties["ContentTemplate"] };
            foreach (PropertyDescriptor descriptor in descriptorArray)
            {
                if (descriptor != null)
                {
                    properties[descriptor.Name] = TypeDescriptor.CreateProperty(typeof(TabPanel), descriptor, new Attribute[] { new TemplateContainerAttribute(typeof(TabPanel)) });
                }
            }
            base.PreFilterProperties(properties);
        }
    }
}

