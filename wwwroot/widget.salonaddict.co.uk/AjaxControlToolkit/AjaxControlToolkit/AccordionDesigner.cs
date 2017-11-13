namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    public class AccordionDesigner : ControlDesigner
    {
        private Accordion _accordion;

        public override string GetDesignTimeHtml()
        {
            if (this._accordion.Height == Unit.Empty)
            {
                this._accordion.Height = new Unit(0xaf);
            }
            if (this._accordion.Width == Unit.Empty)
            {
                this._accordion.Width = new Unit(300);
            }
            ControlCollection controls = this._accordion.Controls;
            string designTimeHtml = base.GetDesignTimeHtml();
            if (designTimeHtml.ToString().IndexOf("<div", 1) > 0)
            {
                designTimeHtml = designTimeHtml.ToString().Substring(0, designTimeHtml.ToString().IndexOf("<div", 1));
            }
            else
            {
                designTimeHtml = designTimeHtml.Remove(designTimeHtml.Length - 6, 6);
            }
            designTimeHtml = designTimeHtml.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            if (!designTimeHtml.Contains("overflow"))
            {
                designTimeHtml = designTimeHtml.Replace("style=\"", "style=\"overflow:scroll;");
            }
            StringBuilder builder = new StringBuilder(designTimeHtml);
            foreach (AccordionPane pane in (AccordionPane[]) this._accordion.Panes.ToArray<AccordionPane>().Clone())
            {
                builder.Append("<span>");
                string str2 = !string.IsNullOrEmpty(pane.HeaderCssClass) ? pane.HeaderCssClass : this._accordion.HeaderCssClass;
                builder.AppendFormat("<div class=\"{0}\">", str2);
                TemplateBuilder header = pane.Header as TemplateBuilder;
                if (header != null)
                {
                    builder.Append(header.Text);
                }
                else
                {
                    builder.Append("AccordionPane Header ");
                    builder.Append(pane.ID);
                }
                builder.Append("</div>");
                string str3 = !string.IsNullOrEmpty(pane.ContentCssClass) ? pane.ContentCssClass : this._accordion.ContentCssClass;
                builder.AppendFormat("<div class=\"{0}\">", str3);
                header = pane.Content as TemplateBuilder;
                if (header != null)
                {
                    builder.Append(header.Text);
                }
                else
                {
                    builder.Append("AccordionPane Content ");
                    builder.Append(pane.ID);
                }
                builder.Append("</div>");
                builder.Append("</span>");
            }
            builder.Append("</div>");
            return builder.ToString();
        }

        public override void Initialize(IComponent component)
        {
            this._accordion = component as Accordion;
            if (this._accordion == null)
            {
                throw new ArgumentException("Component must be an Accordion control", "component");
            }
            base.Initialize(component);
        }
    }
}

