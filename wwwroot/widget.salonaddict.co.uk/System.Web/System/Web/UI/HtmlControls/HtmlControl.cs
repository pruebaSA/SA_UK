﻿namespace System.Web.UI.HtmlControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [Designer("System.Web.UI.Design.HtmlIntrinsicControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxItem(false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class HtmlControl : Control, IAttributeAccessor
    {
        private System.Web.UI.AttributeCollection _attributes;
        internal string _tagName;

        protected HtmlControl() : this("span")
        {
        }

        protected HtmlControl(string tag)
        {
            this._tagName = tag;
        }

        protected override ControlCollection CreateControlCollection() => 
            new EmptyControlCollection(this);

        protected virtual string GetAttribute(string name) => 
            this.Attributes[name];

        internal static string MapIntegerAttributeToString(int n)
        {
            if (n == -1)
            {
                return null;
            }
            return n.ToString(NumberFormatInfo.InvariantInfo);
        }

        internal static string MapStringAttributeToString(string s)
        {
            if ((s != null) && (s.Length == 0))
            {
                return null;
            }
            return s;
        }

        internal void PreProcessRelativeReferenceAttribute(HtmlTextWriter writer, string attribName)
        {
            string str = this.Attributes[attribName];
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    str = base.ResolveClientUrl(str);
                }
                catch (Exception exception)
                {
                    throw new HttpException(System.Web.SR.GetString("Property_Had_Malformed_Url", new object[] { attribName, exception.Message }));
                }
                writer.WriteAttribute(attribName, str);
                this.Attributes.Remove(attribName);
            }
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            this.RenderBeginTag(writer);
        }

        protected virtual void RenderAttributes(HtmlTextWriter writer)
        {
            if (this.ID != null)
            {
                writer.WriteAttribute("id", this.ClientID);
            }
            this.Attributes.Render(writer);
        }

        protected virtual void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.WriteBeginTag(this.TagName);
            this.RenderAttributes(writer);
            writer.Write('>');
        }

        protected virtual void SetAttribute(string name, string value)
        {
            this.Attributes[name] = value;
        }

        string IAttributeAccessor.GetAttribute(string name) => 
            this.GetAttribute(name);

        void IAttributeAccessor.SetAttribute(string name, string value)
        {
            this.SetAttribute(name, value);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Web.UI.AttributeCollection Attributes
        {
            get
            {
                if (this._attributes == null)
                {
                    this._attributes = new System.Web.UI.AttributeCollection(this.ViewState);
                }
                return this._attributes;
            }
        }

        [TypeConverter(typeof(MinimizableAttributeTypeConverter)), WebCategory("Behavior"), DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Disabled
        {
            get
            {
                string str = this.Attributes["disabled"];
                return str?.Equals("disabled");
            }
            set
            {
                if (value)
                {
                    this.Attributes["disabled"] = "disabled";
                }
                else
                {
                    this.Attributes["disabled"] = null;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public CssStyleCollection Style =>
            this.Attributes.CssStyle;

        [WebCategory("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue("")]
        public virtual string TagName =>
            this._tagName;

        protected override bool ViewStateIgnoresCase =>
            true;
    }
}

