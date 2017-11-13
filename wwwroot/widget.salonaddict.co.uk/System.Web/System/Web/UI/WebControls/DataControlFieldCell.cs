namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataControlFieldCell : TableCell
    {
        private DataControlField _containingField;

        public DataControlFieldCell(DataControlField containingField)
        {
            this._containingField = containingField;
        }

        protected DataControlFieldCell(HtmlTextWriterTag tagKey, DataControlField containingField) : base(tagKey)
        {
            this._containingField = containingField;
        }

        public DataControlField ContainingField =>
            this._containingField;
    }
}

