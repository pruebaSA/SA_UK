namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal sealed class DetailsViewAutoFormat : DesignerAutoFormat
    {
        private string alternatingRowBackColor;
        private int alternatingRowFont;
        private string alternatingRowForeColor;
        private string backColor;
        private string borderColor;
        private int borderStyle;
        private string borderWidth;
        private int cellPadding;
        private int cellSpacing;
        private string commandRowBackColor;
        private int commandRowFont;
        private string commandRowForeColor;
        private string editRowBackColor;
        private int editRowFont;
        private string editRowForeColor;
        private string fieldHeaderBackColor;
        private int fieldHeaderFont;
        private string fieldHeaderForeColor;
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

        public DetailsViewAutoFormat(DataRow schemeData) : base(System.Design.SR.GetString(schemeData["SchemeName"].ToString()))
        {
            this.borderStyle = -1;
            this.gridLines = -1;
            this.cellPadding = -1;
            this.Load(schemeData);
        }

        public override void Apply(Control control)
        {
            if (control is DetailsView)
            {
                this.Apply(control as DetailsView);
            }
        }

        private void Apply(DetailsView view)
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
                    view.GridLines = GridLines.Both;
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
            view.AlternatingRowStyle.ForeColor = ColorTranslator.FromHtml(this.alternatingRowForeColor);
            view.AlternatingRowStyle.BackColor = ColorTranslator.FromHtml(this.alternatingRowBackColor);
            view.AlternatingRowStyle.Font.Bold = (this.alternatingRowFont & 1) != 0;
            view.AlternatingRowStyle.Font.Italic = (this.alternatingRowFont & 2) != 0;
            view.AlternatingRowStyle.Font.ClearDefaults();
            view.CommandRowStyle.ForeColor = ColorTranslator.FromHtml(this.commandRowForeColor);
            view.CommandRowStyle.BackColor = ColorTranslator.FromHtml(this.commandRowBackColor);
            view.CommandRowStyle.Font.Bold = (this.commandRowFont & 1) != 0;
            view.CommandRowStyle.Font.Italic = (this.commandRowFont & 2) != 0;
            view.CommandRowStyle.Font.ClearDefaults();
            view.FieldHeaderStyle.ForeColor = ColorTranslator.FromHtml(this.fieldHeaderForeColor);
            view.FieldHeaderStyle.BackColor = ColorTranslator.FromHtml(this.fieldHeaderBackColor);
            view.FieldHeaderStyle.Font.Bold = (this.fieldHeaderFont & 1) != 0;
            view.FieldHeaderStyle.Font.Italic = (this.fieldHeaderFont & 2) != 0;
            view.FieldHeaderStyle.Font.ClearDefaults();
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
            this.alternatingRowForeColor = this.GetStringProperty("AltRowForeColor", schemeData);
            this.alternatingRowBackColor = this.GetStringProperty("AltRowBackColor", schemeData);
            this.alternatingRowFont = this.GetIntProperty("AltRowFont", schemeData);
            this.commandRowForeColor = this.GetStringProperty("CommandRowForeColor", schemeData);
            this.commandRowBackColor = this.GetStringProperty("CommandRowBackColor", schemeData);
            this.commandRowFont = this.GetIntProperty("CommandRowFont", schemeData);
            this.fieldHeaderForeColor = this.GetStringProperty("FieldHeaderForeColor", schemeData);
            this.fieldHeaderBackColor = this.GetStringProperty("FieldHeaderBackColor", schemeData);
            this.fieldHeaderFont = this.GetIntProperty("FieldHeaderFont", schemeData);
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

