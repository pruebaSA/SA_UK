namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal sealed class CalendarAutoFormat : DesignerAutoFormat
    {
        private Color BackColor;
        private Color BorderColor;
        private System.Web.UI.WebControls.BorderStyle BorderStyle;
        private Unit BorderWidth;
        private int CellPadding;
        private int CellSpacing;
        private Color DayBackColor;
        private int DayFont;
        private FontUnit DayFontSize;
        private Color DayForeColor;
        private Color DayHeaderBackColor;
        private int DayHeaderFont;
        private FontUnit DayHeaderFontSize;
        private Color DayHeaderForeColor;
        private Unit DayHeaderHeight;
        private System.Web.UI.WebControls.DayNameFormat DayNameFormat;
        private Unit DayWidth;
        private const int FONT_BOLD = 1;
        private const int FONT_ITALIC = 2;
        private const int FONT_UNDERLINE = 4;
        private string FontName;
        private FontUnit FontSize;
        private Color ForeColor;
        private Unit Height;
        private Color NextPrevBackColor;
        private int NextPrevFont;
        private FontUnit NextPrevFontSize;
        private Color NextPrevForeColor;
        private System.Web.UI.WebControls.NextPrevFormat NextPrevFormat;
        private VerticalAlign NextPrevVerticalAlign;
        private Color OtherMonthDayBackColor;
        private int OtherMonthDayFont;
        private FontUnit OtherMonthDayFontSize;
        private Color OtherMonthDayForeColor;
        private Color SelectedDayBackColor;
        private int SelectedDayFont;
        private FontUnit SelectedDayFontSize;
        private Color SelectedDayForeColor;
        private Color SelectorBackColor;
        private int SelectorFont;
        private string SelectorFontName;
        private FontUnit SelectorFontSize;
        private Color SelectorForeColor;
        private Unit SelectorWidth;
        private bool ShowGridLines;
        private Color TitleBackColor;
        private Color TitleBorderColor;
        private System.Web.UI.WebControls.BorderStyle TitleBorderStyle;
        private Unit TitleBorderWidth;
        private int TitleFont;
        private FontUnit TitleFontSize;
        private Color TitleForeColor;
        private System.Web.UI.WebControls.TitleFormat TitleFormat;
        private Unit TitleHeight;
        private Color TodayDayBackColor;
        private int TodayDayFont;
        private FontUnit TodayDayFontSize;
        private Color TodayDayForeColor;
        private Color WeekendDayBackColor;
        private int WeekendDayFont;
        private FontUnit WeekendDayFontSize;
        private Color WeekendDayForeColor;
        private Unit Width;

        public CalendarAutoFormat(DataRow schemeData) : base(System.Design.SR.GetString(schemeData["SchemeName"].ToString()))
        {
            this.Load(schemeData);
            base.Style.Width = 430;
            base.Style.Height = 280;
        }

        public override void Apply(Control control)
        {
            if (control is System.Web.UI.WebControls.Calendar)
            {
                this.Apply(control as System.Web.UI.WebControls.Calendar);
            }
        }

        private void Apply(System.Web.UI.WebControls.Calendar calendar)
        {
            calendar.Width = this.Width;
            calendar.Height = this.Height;
            calendar.Font.Name = this.FontName;
            calendar.Font.Size = this.FontSize;
            calendar.ForeColor = this.ForeColor;
            calendar.BackColor = this.BackColor;
            calendar.BorderColor = this.BorderColor;
            calendar.BorderWidth = this.BorderWidth;
            calendar.BorderStyle = this.BorderStyle;
            calendar.ShowGridLines = this.ShowGridLines;
            calendar.CellPadding = this.CellPadding;
            calendar.CellSpacing = this.CellSpacing;
            calendar.DayNameFormat = this.DayNameFormat;
            calendar.TitleFormat = this.TitleFormat;
            calendar.NextPrevFormat = this.NextPrevFormat;
            calendar.Font.ClearDefaults();
            calendar.NextPrevStyle.BackColor = this.NextPrevBackColor;
            calendar.NextPrevStyle.Font.Bold = (this.NextPrevFont & 1) != 0;
            calendar.NextPrevStyle.Font.Italic = (this.NextPrevFont & 2) != 0;
            calendar.NextPrevStyle.Font.Underline = (this.NextPrevFont & 4) != 0;
            calendar.NextPrevStyle.Font.Size = this.NextPrevFontSize;
            calendar.NextPrevStyle.ForeColor = this.NextPrevForeColor;
            calendar.NextPrevStyle.VerticalAlign = this.NextPrevVerticalAlign;
            calendar.NextPrevStyle.Font.ClearDefaults();
            calendar.TitleStyle.BackColor = this.TitleBackColor;
            calendar.TitleStyle.BorderColor = this.TitleBorderColor;
            calendar.TitleStyle.BorderStyle = this.TitleBorderStyle;
            calendar.TitleStyle.BorderWidth = this.TitleBorderWidth;
            calendar.TitleStyle.Font.Bold = (this.TitleFont & 1) != 0;
            calendar.TitleStyle.Font.Italic = (this.TitleFont & 2) != 0;
            calendar.TitleStyle.Font.Underline = (this.TitleFont & 4) != 0;
            calendar.TitleStyle.Font.Size = this.TitleFontSize;
            calendar.TitleStyle.ForeColor = this.TitleForeColor;
            calendar.TitleStyle.Height = this.TitleHeight;
            calendar.TitleStyle.Font.ClearDefaults();
            calendar.DayStyle.BackColor = this.DayBackColor;
            calendar.DayStyle.Font.Bold = (this.DayFont & 1) != 0;
            calendar.DayStyle.Font.Italic = (this.DayFont & 2) != 0;
            calendar.DayStyle.Font.Underline = (this.DayFont & 4) != 0;
            calendar.DayStyle.Font.Size = this.DayFontSize;
            calendar.DayStyle.ForeColor = this.DayForeColor;
            calendar.DayStyle.Width = this.DayWidth;
            calendar.DayStyle.Font.ClearDefaults();
            calendar.DayHeaderStyle.BackColor = this.DayHeaderBackColor;
            calendar.DayHeaderStyle.Font.Bold = (this.DayHeaderFont & 1) != 0;
            calendar.DayHeaderStyle.Font.Italic = (this.DayHeaderFont & 2) != 0;
            calendar.DayHeaderStyle.Font.Underline = (this.DayHeaderFont & 4) != 0;
            calendar.DayHeaderStyle.Font.Size = this.DayHeaderFontSize;
            calendar.DayHeaderStyle.ForeColor = this.DayHeaderForeColor;
            calendar.DayHeaderStyle.Height = this.DayHeaderHeight;
            calendar.DayHeaderStyle.Font.ClearDefaults();
            calendar.TodayDayStyle.BackColor = this.TodayDayBackColor;
            calendar.TodayDayStyle.Font.Bold = (this.TodayDayFont & 1) != 0;
            calendar.TodayDayStyle.Font.Italic = (this.TodayDayFont & 2) != 0;
            calendar.TodayDayStyle.Font.Underline = (this.TodayDayFont & 4) != 0;
            calendar.TodayDayStyle.Font.Size = this.TodayDayFontSize;
            calendar.TodayDayStyle.ForeColor = this.TodayDayForeColor;
            calendar.TodayDayStyle.Font.ClearDefaults();
            calendar.SelectedDayStyle.BackColor = this.SelectedDayBackColor;
            calendar.SelectedDayStyle.Font.Bold = (this.SelectedDayFont & 1) != 0;
            calendar.SelectedDayStyle.Font.Italic = (this.SelectedDayFont & 2) != 0;
            calendar.SelectedDayStyle.Font.Underline = (this.SelectedDayFont & 4) != 0;
            calendar.SelectedDayStyle.Font.Size = this.SelectedDayFontSize;
            calendar.SelectedDayStyle.ForeColor = this.SelectedDayForeColor;
            calendar.SelectedDayStyle.Font.ClearDefaults();
            calendar.OtherMonthDayStyle.BackColor = this.OtherMonthDayBackColor;
            calendar.OtherMonthDayStyle.Font.Bold = (this.OtherMonthDayFont & 1) != 0;
            calendar.OtherMonthDayStyle.Font.Italic = (this.OtherMonthDayFont & 2) != 0;
            calendar.OtherMonthDayStyle.Font.Underline = (this.OtherMonthDayFont & 4) != 0;
            calendar.OtherMonthDayStyle.Font.Size = this.OtherMonthDayFontSize;
            calendar.OtherMonthDayStyle.ForeColor = this.OtherMonthDayForeColor;
            calendar.OtherMonthDayStyle.Font.ClearDefaults();
            calendar.WeekendDayStyle.BackColor = this.WeekendDayBackColor;
            calendar.WeekendDayStyle.Font.Bold = (this.WeekendDayFont & 1) != 0;
            calendar.WeekendDayStyle.Font.Italic = (this.WeekendDayFont & 2) != 0;
            calendar.WeekendDayStyle.Font.Underline = (this.WeekendDayFont & 4) != 0;
            calendar.WeekendDayStyle.Font.Size = this.WeekendDayFontSize;
            calendar.WeekendDayStyle.ForeColor = this.WeekendDayForeColor;
            calendar.WeekendDayStyle.Font.ClearDefaults();
            calendar.SelectorStyle.BackColor = this.SelectorBackColor;
            calendar.SelectorStyle.Font.Bold = (this.SelectorFont & 1) != 0;
            calendar.SelectorStyle.Font.Italic = (this.SelectorFont & 2) != 0;
            calendar.SelectorStyle.Font.Underline = (this.SelectorFont & 4) != 0;
            calendar.SelectorStyle.Font.Name = this.SelectorFontName;
            calendar.SelectorStyle.Font.Size = this.SelectorFontSize;
            calendar.SelectorStyle.ForeColor = this.SelectorForeColor;
            calendar.SelectorStyle.Width = this.SelectorWidth;
            calendar.SelectorStyle.Font.ClearDefaults();
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

        private int GetIntProperty(string propertyTag, DataRow schemeData, int defaultValue)
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

        private string GetStringProperty(string propertyTag, DataRow schemeData, string defaultValue)
        {
            object obj2 = schemeData[propertyTag];
            if ((obj2 != null) && !obj2.Equals(DBNull.Value))
            {
                return obj2.ToString();
            }
            return defaultValue;
        }

        private void Load(DataRow schemeData)
        {
            if (schemeData != null)
            {
                this.Width = new Unit(this.GetStringProperty("Width", schemeData), CultureInfo.InvariantCulture);
                this.Height = new Unit(this.GetStringProperty("Height", schemeData), CultureInfo.InvariantCulture);
                this.FontName = this.GetStringProperty("FontName", schemeData);
                this.FontSize = new FontUnit(this.GetStringProperty("FontSize", schemeData), CultureInfo.InvariantCulture);
                this.ForeColor = ColorTranslator.FromHtml(this.GetStringProperty("ForeColor", schemeData));
                this.BackColor = ColorTranslator.FromHtml(this.GetStringProperty("BackColor", schemeData));
                this.BorderColor = ColorTranslator.FromHtml(this.GetStringProperty("BorderColor", schemeData));
                this.BorderWidth = new Unit(this.GetStringProperty("BorderWidth", schemeData), CultureInfo.InvariantCulture);
                this.BorderStyle = (System.Web.UI.WebControls.BorderStyle) Enum.Parse(typeof(System.Web.UI.WebControls.BorderStyle), this.GetStringProperty("BorderStyle", schemeData, "NotSet"));
                this.ShowGridLines = bool.Parse(this.GetStringProperty("ShowGridLines", schemeData, "false"));
                this.CellPadding = this.GetIntProperty("CellPadding", schemeData, 2);
                this.CellSpacing = this.GetIntProperty("CellSpacing", schemeData);
                this.DayNameFormat = (System.Web.UI.WebControls.DayNameFormat) Enum.Parse(typeof(System.Web.UI.WebControls.DayNameFormat), this.GetStringProperty("DayNameFormat", schemeData, "Short"));
                this.NextPrevBackColor = ColorTranslator.FromHtml(this.GetStringProperty("NextPrevBackColor", schemeData));
                this.NextPrevFont = this.GetIntProperty("NextPrevFont", schemeData);
                this.NextPrevFontSize = new FontUnit(this.GetStringProperty("NextPrevFontSize", schemeData), CultureInfo.InvariantCulture);
                this.NextPrevForeColor = ColorTranslator.FromHtml(this.GetStringProperty("NextPrevForeColor", schemeData));
                this.NextPrevFormat = (System.Web.UI.WebControls.NextPrevFormat) Enum.Parse(typeof(System.Web.UI.WebControls.NextPrevFormat), this.GetStringProperty("NextPrevFormat", schemeData, "CustomText"));
                this.NextPrevVerticalAlign = (VerticalAlign) Enum.Parse(typeof(VerticalAlign), this.GetStringProperty("NextPrevVerticalAlign", schemeData, "NotSet"));
                this.TitleFormat = (System.Web.UI.WebControls.TitleFormat) Enum.Parse(typeof(System.Web.UI.WebControls.TitleFormat), this.GetStringProperty("TitleFormat", schemeData, "MonthYear"));
                this.TitleBackColor = ColorTranslator.FromHtml(this.GetStringProperty("TitleBackColor", schemeData));
                this.TitleBorderColor = ColorTranslator.FromHtml(this.GetStringProperty("TitleBorderColor", schemeData));
                this.TitleBorderStyle = (System.Web.UI.WebControls.BorderStyle) Enum.Parse(typeof(System.Web.UI.WebControls.BorderStyle), this.GetStringProperty("BorderStyle", schemeData, "NotSet"));
                this.TitleBorderWidth = new Unit(this.GetStringProperty("TitleBorderWidth", schemeData), CultureInfo.InvariantCulture);
                this.TitleFont = this.GetIntProperty("TitleFont", schemeData);
                this.TitleFontSize = new FontUnit(this.GetStringProperty("TitleFontSize", schemeData), CultureInfo.InvariantCulture);
                this.TitleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("TitleForeColor", schemeData));
                this.TitleHeight = new Unit(this.GetStringProperty("TitleHeight", schemeData), CultureInfo.InvariantCulture);
                this.DayBackColor = ColorTranslator.FromHtml(this.GetStringProperty("DayBackColor", schemeData));
                this.DayFont = this.GetIntProperty("DayFont", schemeData);
                this.DayFontSize = new FontUnit(this.GetStringProperty("DayFontSize", schemeData), CultureInfo.InvariantCulture);
                this.DayForeColor = ColorTranslator.FromHtml(this.GetStringProperty("DayForeColor", schemeData));
                this.DayWidth = new Unit(this.GetStringProperty("DayWidth", schemeData), CultureInfo.InvariantCulture);
                this.DayHeaderBackColor = ColorTranslator.FromHtml(this.GetStringProperty("DayHeaderBackColor", schemeData));
                this.DayHeaderFont = this.GetIntProperty("DayHeaderFont", schemeData);
                this.DayHeaderFontSize = new FontUnit(this.GetStringProperty("DayHeaderFontSize", schemeData), CultureInfo.InvariantCulture);
                this.DayHeaderForeColor = ColorTranslator.FromHtml(this.GetStringProperty("DayHeaderForeColor", schemeData));
                this.DayHeaderHeight = new Unit(this.GetStringProperty("DayHeaderHeight", schemeData), CultureInfo.InvariantCulture);
                this.TodayDayBackColor = ColorTranslator.FromHtml(this.GetStringProperty("TodayDayBackColor", schemeData));
                this.TodayDayFont = this.GetIntProperty("TodayDayFont", schemeData);
                this.TodayDayFontSize = new FontUnit(this.GetStringProperty("TodayDayFontSize", schemeData), CultureInfo.InvariantCulture);
                this.TodayDayForeColor = ColorTranslator.FromHtml(this.GetStringProperty("TodayDayForeColor", schemeData));
                this.SelectedDayBackColor = ColorTranslator.FromHtml(this.GetStringProperty("SelectedDayBackColor", schemeData));
                this.SelectedDayFont = this.GetIntProperty("SelectedDayFont", schemeData);
                this.SelectedDayFontSize = new FontUnit(this.GetStringProperty("SelectedDayFontSize", schemeData), CultureInfo.InvariantCulture);
                this.SelectedDayForeColor = ColorTranslator.FromHtml(this.GetStringProperty("SelectedDayForeColor", schemeData));
                this.OtherMonthDayBackColor = ColorTranslator.FromHtml(this.GetStringProperty("OtherMonthDayBackColor", schemeData));
                this.OtherMonthDayFont = this.GetIntProperty("OtherMonthDayFont", schemeData);
                this.OtherMonthDayFontSize = new FontUnit(this.GetStringProperty("OtherMonthDayFontSize", schemeData), CultureInfo.InvariantCulture);
                this.OtherMonthDayForeColor = ColorTranslator.FromHtml(this.GetStringProperty("OtherMonthDayForeColor", schemeData));
                this.WeekendDayBackColor = ColorTranslator.FromHtml(this.GetStringProperty("WeekendDayBackColor", schemeData));
                this.WeekendDayFont = this.GetIntProperty("WeekendDayFont", schemeData);
                this.WeekendDayFontSize = new FontUnit(this.GetStringProperty("WeekendDayFontSize", schemeData), CultureInfo.InvariantCulture);
                this.WeekendDayForeColor = ColorTranslator.FromHtml(this.GetStringProperty("WeekendDayForeColor", schemeData));
                this.SelectorBackColor = ColorTranslator.FromHtml(this.GetStringProperty("SelectorBackColor", schemeData));
                this.SelectorFont = this.GetIntProperty("SelectorFont", schemeData);
                this.SelectorFontName = this.GetStringProperty("SelectorFontName", schemeData);
                this.SelectorFontSize = new FontUnit(this.GetStringProperty("SelectorFontSize", schemeData), CultureInfo.InvariantCulture);
                this.SelectorForeColor = ColorTranslator.FromHtml(this.GetStringProperty("SelectorForeColor", schemeData));
                this.SelectorWidth = new Unit(this.GetStringProperty("SelectorWidth", schemeData), CultureInfo.InvariantCulture);
            }
        }
    }
}

