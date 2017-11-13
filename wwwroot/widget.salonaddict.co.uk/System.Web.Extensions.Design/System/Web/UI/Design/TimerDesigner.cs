namespace System.Web.UI.Design
{
    using System;

    public class TimerDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml() => 
            base.CreatePlaceHolderDesignTimeHtml();
    }
}

