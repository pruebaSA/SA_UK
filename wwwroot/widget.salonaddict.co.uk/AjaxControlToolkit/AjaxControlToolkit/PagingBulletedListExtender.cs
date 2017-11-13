namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [Designer("AjaxControlToolkit.PagingBulletedListDesigner, AjaxControlToolkit"), TargetControlType(typeof(System.Web.UI.WebControls.BulletedList)), ToolboxBitmap(typeof(PagingBulletedListExtender), "PagingBulletedList.PagingBulletedList.ico"), ClientScriptResource("Sys.Extended.UI.PagingBulletedListBehavior", "PagingBulletedList.PagingBulletedListBehavior.js")]
    public class PagingBulletedListExtender : ExtenderControlBase
    {
        public PagingBulletedListExtender()
        {
            base.EnableClientState = true;
        }

        [DefaultValue(false), ExtenderControlProperty]
        public bool ClientSort
        {
            get => 
                base.GetPropertyValue<bool>("ClientSort", false);
            set
            {
                base.SetPropertyValue<bool>("ClientSort", value);
            }
        }

        [ExtenderControlProperty]
        public int? Height
        {
            get => 
                base.GetPropertyValue<int?>("Height", null);
            set
            {
                base.SetPropertyValue<int?>("Height", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(1)]
        public int IndexSize
        {
            get => 
                base.GetPropertyValue<int>("IndexSize", 1);
            set
            {
                base.SetPropertyValue<int>("IndexSize", value);
            }
        }

        [ExtenderControlProperty]
        public int? MaxItemPerPage
        {
            get => 
                base.GetPropertyValue<int?>("MaxItemPerPage", null);
            set
            {
                base.SetPropertyValue<int?>("MaxItemPerPage", value);
            }
        }

        [ExtenderControlProperty]
        public string SelectIndexCssClass
        {
            get => 
                base.GetPropertyValue<string>("SelectIndexCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("SelectIndexCssClass", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(" - ")]
        public string Separator
        {
            get => 
                base.GetPropertyValue<string>("Separator", " - ");
            set
            {
                base.SetPropertyValue<string>("Separator", value);
            }
        }

        [ExtenderControlProperty]
        public string UnselectIndexCssClass
        {
            get => 
                base.GetPropertyValue<string>("UnselectIndexCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("UnselectIndexCssClass", value);
            }
        }
    }
}

