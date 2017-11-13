﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [Designer("System.Web.UI.Design.WebControls.ContentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxItem(false), ControlBuilder(typeof(ContentBuilderInternal)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class Content : Control, INonBindingContainer, INamingContainer
    {
        private string _contentPlaceHolderID;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler DataBinding
        {
            add
            {
                base.DataBinding += value;
            }
            remove
            {
                base.DataBinding -= value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public event EventHandler Disposed
        {
            add
            {
                base.Disposed += value;
            }
            remove
            {
                base.Disposed -= value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler Init
        {
            add
            {
                base.Init += value;
            }
            remove
            {
                base.Init -= value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public event EventHandler Load
        {
            add
            {
                base.Load += value;
            }
            remove
            {
                base.Load -= value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public event EventHandler PreRender
        {
            add
            {
                base.PreRender += value;
            }
            remove
            {
                base.PreRender -= value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event EventHandler Unload
        {
            add
            {
                base.Unload += value;
            }
            remove
            {
                base.Unload -= value;
            }
        }

        [WebCategory("Behavior"), IDReferenceProperty(typeof(ContentPlaceHolder)), Themeable(false), WebSysDescription("Content_ContentPlaceHolderID"), DefaultValue("")]
        public string ContentPlaceHolderID
        {
            get
            {
                if (this._contentPlaceHolderID == null)
                {
                    return string.Empty;
                }
                return this._contentPlaceHolderID;
            }
            set
            {
                if (!base.DesignMode)
                {
                    throw new NotSupportedException(System.Web.SR.GetString("Property_Set_Not_Supported", new object[] { "ContentPlaceHolderID", base.GetType().ToString() }));
                }
                this._contentPlaceHolderID = value;
            }
        }
    }
}

