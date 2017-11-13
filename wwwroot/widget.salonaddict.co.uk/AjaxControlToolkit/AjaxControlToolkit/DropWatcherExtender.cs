namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ToolboxItem(false), RequiredScript(typeof(DragDropScripts)), ClientScriptResource("Sys.Extended.UI.DragDropWatcher", "ReorderList.DropWatcherBehavior.js"), RequiredScript(typeof(CommonToolkitScripts)), TargetControlType(typeof(BulletedList))]
    public class DropWatcherExtender : ExtenderControlBase
    {
        [Browsable(false), ClientPropertyName("acceptedDataTypes"), ExtenderControlProperty]
        public string AcceptedDataTypes
        {
            get => 
                this.DataTypeName;
            set
            {
                ExtenderControlBase.SuppressUnusedParameterWarning(value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("argContextString")]
        public string ArgContextString
        {
            get => 
                base.GetPropertyValue<string>("ArgContextString", "");
            set
            {
                base.SetPropertyValue<string>("ArgContextString", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("argErrorString")]
        public string ArgErrorString
        {
            get => 
                base.GetPropertyValue<string>("ArgErrorString", "");
            set
            {
                base.SetPropertyValue<string>("ArgErrorString", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("argReplaceString")]
        public string ArgReplaceString
        {
            get => 
                base.GetPropertyValue<string>("ArgReplaceString", "");
            set
            {
                base.SetPropertyValue<string>("ArgReplaceString", value);
            }
        }

        [ClientPropertyName("argSuccessString"), ExtenderControlProperty]
        public string ArgSuccessString
        {
            get => 
                base.GetPropertyValue<string>("ArgSuccessString", "");
            set
            {
                base.SetPropertyValue<string>("ArgSuccessString", value);
            }
        }

        [ClientPropertyName("callbackCssStyle"), ExtenderControlProperty]
        public string CallbackCssStyle
        {
            get => 
                base.GetPropertyValue<string>("CallbackCssStyle", "");
            set
            {
                base.SetPropertyValue<string>("CallbackCssStyle", value);
            }
        }

        [ExtenderControlProperty, Browsable(false), ClientPropertyName("dragDataType")]
        public string DataType
        {
            get => 
                this.DataTypeName;
            set
            {
                ExtenderControlBase.SuppressUnusedParameterWarning(value);
            }
        }

        private string DataTypeName =>
            ("HTML_" + this.Parent.ID);

        [ClientPropertyName("dragMode"), ExtenderControlProperty, Browsable(false)]
        public int DragMode
        {
            get => 
                1;
            set
            {
                ExtenderControlBase.SuppressUnusedParameterWarning(value);
            }
        }

        [IDReferenceProperty(typeof(Control)), ExtenderControlProperty, ElementReference, ClientPropertyName("dropCueTemplate")]
        public string DropLayoutElement
        {
            get => 
                base.GetPropertyValue<string>("DropLayoutElement", "");
            set
            {
                base.SetPropertyValue<string>("DropLayoutElement", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("postbackCode")]
        public string PostBackCode
        {
            get => 
                base.GetPropertyValue<string>("PostbackCode", "");
            set
            {
                base.SetPropertyValue<string>("PostbackCode", value);
            }
        }
    }
}

