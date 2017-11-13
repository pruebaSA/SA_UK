namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Design;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [TargetControlType(typeof(WebControl))]
    public class ModalPopupDesigner : ExtenderControlBaseDesigner<ModalPopupExtender>
    {
        [PageMethodSignature("Dynamic Populate", "DynamicServicePath", "DynamicServiceMethod")]
        private delegate string GetDynamicContent(string contextKey);
    }
}

