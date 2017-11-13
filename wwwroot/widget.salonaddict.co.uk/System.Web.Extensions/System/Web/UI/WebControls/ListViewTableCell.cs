namespace System.Web.UI.WebControls
{
    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    internal class ListViewTableCell : HtmlTableCell
    {
        protected override ControlCollection CreateControlCollection() => 
            new ControlCollection(this);

        protected internal override void Render(HtmlTextWriter writer)
        {
            this.RenderChildren(writer);
        }
    }
}

