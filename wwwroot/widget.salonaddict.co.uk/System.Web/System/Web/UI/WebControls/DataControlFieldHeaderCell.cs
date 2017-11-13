namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataControlFieldHeaderCell : DataControlFieldCell
    {
        public DataControlFieldHeaderCell(DataControlField containingField) : base(HtmlTextWriterTag.Th, containingField)
        {
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            switch (this.Scope)
            {
                case TableHeaderScope.NotSet:
                    break;

                case TableHeaderScope.Column:
                    writer.AddAttribute(HtmlTextWriterAttribute.Scope, "col");
                    break;

                default:
                    writer.AddAttribute(HtmlTextWriterAttribute.Scope, "row");
                    break;
            }
            string abbreviatedText = this.AbbreviatedText;
            if (!string.IsNullOrEmpty(abbreviatedText))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Abbr, abbreviatedText);
            }
        }

        public virtual string AbbreviatedText
        {
            get
            {
                object obj2 = this.ViewState["AbbrText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["AbbrText"] = value;
            }
        }

        public virtual TableHeaderScope Scope
        {
            get
            {
                object obj2 = this.ViewState["Scope"];
                if (obj2 != null)
                {
                    return (TableHeaderScope) obj2;
                }
                return TableHeaderScope.NotSet;
            }
            set
            {
                this.ViewState["Scope"] = value;
            }
        }
    }
}

