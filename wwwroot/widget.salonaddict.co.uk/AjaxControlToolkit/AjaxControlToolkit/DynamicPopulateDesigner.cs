namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Design;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [TargetControlType(typeof(WebControl))]
    public class DynamicPopulateDesigner : ExtenderControlBaseDesigner<DynamicPopulateExtender>
    {
        [PageMethodSignature("Dynamic Populate", "ServicePath", "ServiceMethod")]
        private delegate string GetDynamicContent(string contextKey);
    }
}

