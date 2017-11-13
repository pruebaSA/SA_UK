namespace AjaxControlToolkit
{
    using System;
    using System.Web.UI.Design;

    public class NoBotDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml() => 
            base.CreatePlaceHolderDesignTimeHtml();
    }
}

