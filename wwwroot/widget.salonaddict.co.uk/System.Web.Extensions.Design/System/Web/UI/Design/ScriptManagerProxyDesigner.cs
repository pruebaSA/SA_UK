namespace System.Web.UI.Design
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    public class ScriptManagerProxyDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml() => 
            base.CreatePlaceHolderDesignTimeHtml();

        public override void Initialize(IComponent component)
        {
            ControlDesigner.VerifyInitializeArgument(component, typeof(ScriptManagerProxy));
            base.Initialize(component);
        }
    }
}

