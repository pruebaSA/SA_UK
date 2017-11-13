namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.DraggableListItem", "ReorderList.DraggableListItemBehavior.js"), TargetControlType(typeof(ReorderListItem)), ToolboxItem(false)]
    public class DraggableListItemExtender : ExtenderControlBase
    {
        [ClientPropertyName("handle"), IDReferenceProperty(typeof(Control)), ExtenderControlProperty, ElementReference, DefaultValue("")]
        public string Handle
        {
            get => 
                base.GetPropertyValue<string>("handle", "");
            set
            {
                base.SetPropertyValue<string>("handle", value);
            }
        }
    }
}

