namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [TypeConverter(typeof(EmbeddedMailObject.EmbeddedMailObjectTypeConverter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class EmbeddedMailObject
    {
        private string _name;
        private string _path;

        public EmbeddedMailObject()
        {
        }

        public EmbeddedMailObject(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        [NotifyParentProperty(true), WebCategory("Behavior"), DefaultValue(""), WebSysDescription("EmbeddedMailObject_Name")]
        public string Name
        {
            get
            {
                if (this._name == null)
                {
                    return string.Empty;
                }
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        [DefaultValue(""), WebCategory("Behavior"), NotifyParentProperty(true), UrlProperty, Editor("System.Web.UI.Design.MailFileEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebSysDescription("EmbeddedMailObject_Path")]
        public string Path
        {
            get
            {
                if (this._path != null)
                {
                    return this._path;
                }
                return string.Empty;
            }
            set
            {
                this._path = value;
            }
        }

        private sealed class EmbeddedMailObjectTypeConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return "EmbeddedMailObject";
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}

