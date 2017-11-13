namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel.Design;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal sealed class FormViewAutoFormat : DesignerAutoFormat
    {
        private string backColor;
        private string borderColor;
        private int borderStyle;
        private string borderWidth;
        private int cellPadding;
        private int cellSpacing;
        private string editRowBackColor;
        private int editRowFont;
        private string editRowForeColor;
        private const int FONT_BOLD = 1;
        private const int FONT_ITALIC = 2;
        private string footerBackColor;
        private int footerFont;
        private string footerForeColor;
        private string foreColor;
        private int gridLines;
        private string headerBackColor;
        private int headerFont;
        private string headerForeColor;
        private int itemFont;
        private int pagerAlign;
        private string pagerBackColor;
        private int pagerButtons;
        private int pagerFont;
        private string pagerForeColor;
        private string rowBackColor;
        private string rowForeColor;

        public FormViewAutoFormat(DataRow schemeData) : base(System.Design.SR.GetString(schemeData["SchemeName"].ToString()))
        {
            this.borderStyle = -1;
            this.gridLines = -1;
            this.cellPadding = -1;
            this.Load(schemeData);
        }

        public override void Apply(Control control)
        {
            if (control is FormView)
            {
                this.Apply(control as FormView);
            }
        }

        private void Apply(FormView view)
        {
            view.HeaderStyle.ForeColor = ColorTranslator.FromHtml(this.headerForeColor);
            view.HeaderStyle.BackColor = ColorTranslator.FromHtml(this.headerBackColor);
            view.HeaderStyle.Font.Bold = (this.headerFont & 1) != 0;
            view.HeaderStyle.Font.Italic = (this.headerFont & 2) != 0;
            view.HeaderStyle.Font.ClearDefaults();
            view.FooterStyle.ForeColor = ColorTranslator.FromHtml(this.footerForeColor);
            view.FooterStyle.BackColor = ColorTranslator.FromHtml(this.footerBackColor);
            view.FooterStyle.Font.Bold = (this.footerFont & 1) != 0;
            view.FooterStyle.Font.Italic = (this.footerFont & 2) != 0;
            view.FooterStyle.Font.ClearDefaults();
            view.BorderWidth = new Unit(this.borderWidth, CultureInfo.InvariantCulture);
            switch (this.gridLines)
            {
                case 0:
                    view.GridLines = GridLines.None;
                    break;

                case 1:
                    view.GridLines = GridLines.Horizontal;
                    break;

                case 2:
                    view.GridLines = GridLines.Vertical;
                    break;

                case 3:
                    view.GridLines = GridLines.Both;
                    break;

                default:
                    view.GridLines = GridLines.None;
                    break;
            }
            if ((this.borderStyle >= 0) && (this.borderStyle <= 9))
            {
                view.BorderStyle = (BorderStyle) this.borderStyle;
            }
            else
            {
                view.BorderStyle = BorderStyle.NotSet;
            }
            view.BorderColor = ColorTranslator.FromHtml(this.borderColor);
            view.CellPadding = this.cellPadding;
            view.CellSpacing = this.cellSpacing;
            view.ForeColor = ColorTranslator.FromHtml(this.foreColor);
            view.BackColor = ColorTranslator.FromHtml(this.backColor);
            view.RowStyle.ForeColor = ColorTranslator.FromHtml(this.rowForeColor);
            view.RowStyle.BackColor = ColorTranslator.FromHtml(this.rowBackColor);
            view.RowStyle.Font.Bold = (this.itemFont & 1) != 0;
            view.RowStyle.Font.Italic = (this.itemFont & 2) != 0;
            view.RowStyle.Font.ClearDefaults();
            view.EditRowStyle.ForeColor = ColorTranslator.FromHtml(this.editRowForeColor);
            view.EditRowStyle.BackColor = ColorTranslator.FromHtml(this.editRowBackColor);
            view.EditRowStyle.Font.Bold = (this.editRowFont & 1) != 0;
            view.EditRowStyle.Font.Italic = (this.editRowFont & 2) != 0;
            view.EditRowStyle.Font.ClearDefaults();
            view.PagerStyle.ForeColor = ColorTranslator.FromHtml(this.pagerForeColor);
            view.PagerStyle.BackColor = ColorTranslator.FromHtml(this.pagerBackColor);
            view.PagerStyle.Font.Bold = (this.pagerFont & 1) != 0;
            view.PagerStyle.Font.Italic = (this.pagerFont & 2) != 0;
            view.PagerStyle.HorizontalAlign = (HorizontalAlign) this.pagerAlign;
            view.PagerStyle.Font.ClearDefaults();
            view.PagerSettings.Mode = (PagerButtons) this.pagerButtons;
        }

        private int GetIntProperty(string propertyTag, DataRow schemeData)
        {
            object obj2 = schemeData[propertyTag];
            if ((obj2 != null) && !obj2.Equals(DBNull.Value))
            {
                return int.Parse(obj2.ToString(), CultureInfo.InvariantCulture);
            }
            return 0;
        }

        private int GetIntProperty(string propertyTag, int defaultValue, DataRow schemeData)
        {
            object obj2 = schemeData[propertyTag];
            if ((obj2 != null) && !obj2.Equals(DBNull.Value))
            {
                return int.Parse(obj2.ToString(), CultureInfo.InvariantCulture);
            }
            return defaultValue;
        }

        public override Control GetPreviewControl(Control runtimeControl)
        {
            Control previewControl = base.GetPreviewControl(runtimeControl);
            if (previewControl != null)
            {
                IDesignerHost service = (IDesignerHost) runtimeControl.Site.GetService(typeof(IDesignerHost));
                FormView view = previewControl as FormView;
                if ((view == null) || (service == null))
                {
                    return previewControl;
                }
                TemplateBuilder itemTemplate = view.ItemTemplate as TemplateBuilder;
                if (((itemTemplate != null) && (itemTemplate.Text.Length == 0)) || (view.ItemTemplate == null))
                {
                    string templateText = "####&nbsp;&nbsp;####<br/>####&nbsp;&nbsp;####<br/>####&nbsp;&nbsp;####<br/>####&nbsp;&nbsp;####";
                    view.ItemTemplate = ControlParser.ParseTemplate(service, templateText);
                    view.RowStyle.HorizontalAlign = HorizontalAlign.Center;
                }
                view.HorizontalAlign = HorizontalAlign.Center;
                view.Width = new Unit(80.0, UnitType.Percentage);
            }
            return previewControl;
        }

        private string GetStringProperty(string propertyTag, DataRow schemeData)
        {
            object obj2 = schemeData[propertyTag];
            if ((obj2 != null) && !obj2.Equals(DBNull.Value))
            {
                return obj2.ToString();
            }
            return string.Empty;
        }

        private void Load(DataRow schemeData)
        {
            this.foreColor = this.GetStringProperty("ForeColor", schemeData);
            this.backColor = this.GetStringProperty("BackColor", schemeData);
            this.borderColor = this.GetStringProperty("BorderColor", schemeData);
            this.borderWidth = this.GetStringProperty("BorderWidth", schemeData);
            this.borderStyle = this.GetIntProperty("BorderStyle", -1, schemeData);
            this.cellSpacing = this.GetIntProperty("CellSpacing", schemeData);
            this.cellPadding = this.GetIntProperty("CellPadding", -1, schemeData);
            this.gridLines = this.GetIntProperty("GridLines", -1, schemeData);
            this.rowForeColor = this.GetStringProperty("RowForeColor", schemeData);
            this.rowBackColor = this.GetStringProperty("RowBackColor", schemeData);
            this.itemFont = this.GetIntProperty("RowFont", schemeData);
            this.editRowForeColor = this.GetStringProperty("EditRowForeColor", schemeData);
            this.editRowBackColor = this.GetStringProperty("EditRowBackColor", schemeData);
            this.editRowFont = this.GetIntProperty("EditRowFont", schemeData);
            this.headerForeColor = this.GetStringProperty("HeaderForeColor", schemeData);
            this.headerBackColor = this.GetStringProperty("HeaderBackColor", schemeData);
            this.headerFont = this.GetIntProperty("HeaderFont", schemeData);
            this.footerForeColor = this.GetStringProperty("FooterForeColor", schemeData);
            this.footerBackColor = this.GetStringProperty("FooterBackColor", schemeData);
            this.footerFont = this.GetIntProperty("FooterFont", schemeData);
            this.pagerForeColor = this.GetStringProperty("PagerForeColor", schemeData);
            this.pagerBackColor = this.GetStringProperty("PagerBackColor", schemeData);
            this.pagerFont = this.GetIntProperty("PagerFont", schemeData);
            this.pagerAlign = this.GetIntProperty("PagerAlign", schemeData);
            this.pagerButtons = this.GetIntProperty("PagerButtons", 1, schemeData);
        }
    }
}

