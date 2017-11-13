namespace AjaxControlToolkit.Design
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class TypedControlIDConverter<T> : ControlIDConverter
    {
        protected override bool FilterControl(Control control) => 
            typeof(T).IsInstanceOfType(control);
    }
}

