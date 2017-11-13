namespace IFRAME.Controllers
{
    using System;
    using System.Web.UI;

    public class IFRMUserControl : UserControl
    {
        public string GetLocaleResourceString(string name) => 
            base.GetLocalResourceObject(name)?.ToString();
    }
}

