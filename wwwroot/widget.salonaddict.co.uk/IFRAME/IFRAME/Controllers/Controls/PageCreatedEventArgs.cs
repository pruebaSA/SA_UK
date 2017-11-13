namespace IFRAME.Controllers.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.UI;

    public class PageCreatedEventArgs
    {
        public System.Web.UI.Control Control { get; set; }

        public int NewPageIndex { get; set; }
    }
}

