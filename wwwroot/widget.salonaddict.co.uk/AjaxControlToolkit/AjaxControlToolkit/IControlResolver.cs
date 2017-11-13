namespace AjaxControlToolkit
{
    using System;
    using System.Web.UI;

    public interface IControlResolver
    {
        Control ResolveControl(string controlId);
    }
}

