namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal sealed class WizardAutoFormat : DesignerAutoFormat
    {
        private Color BackColor;
        private Color BorderColor;
        private System.Web.UI.WebControls.BorderStyle BorderStyle;
        private Unit BorderWidth;
        private string FontName;
        private FontUnit FontSize;
        private Color HeaderStyleBackColor;
        private Color HeaderStyleBorderColor;
        private System.Web.UI.WebControls.BorderStyle HeaderStyleBorderStyle;
        private Unit HeaderStyleBorderWidth;
        private bool HeaderStyleFontBold;
        private FontUnit HeaderStyleFontSize;
        private Color HeaderStyleForeColor;
        private HorizontalAlign HeaderStyleHorizontalAlign;
        private Color NavigationButtonStyleBackColor;
        private Color NavigationButtonStyleBorderColor;
        private System.Web.UI.WebControls.BorderStyle NavigationButtonStyleBorderStyle;
        private Unit NavigationButtonStyleBorderWidth;
        private string NavigationButtonStyleFontName;
        private FontUnit NavigationButtonStyleFontSize;
        private Color NavigationButtonStyleForeColor;
        private Color SideBarButtonStyleBackColor;
        private Unit SideBarButtonStyleBorderWidth;
        private string SideBarButtonStyleFontName;
        private bool SideBarButtonStyleFontUnderline;
        private Color SideBarButtonStyleForeColor;
        private Color SideBarStyleBackColor;
        private Unit SideBarStyleBorderWidth;
        private FontUnit SideBarStyleFontSize;
        private bool SideBarStyleFontStrikeout;
        private bool SideBarStyleFontUnderline;
        private VerticalAlign SideBarStyleVerticalAlign;
        private Color StepStyleBackColor;
        private Color StepStyleBorderColor;
        private System.Web.UI.WebControls.BorderStyle StepStyleBorderStyle;
        private Unit StepStyleBorderWidth;
        private FontUnit StepStyleFontSize;
        private Color StepStyleForeColor;

        public WizardAutoFormat(DataRow schemeData) : base(System.Design.SR.GetString(schemeData["SchemeName"].ToString()))
        {
            this.Load(schemeData);
            base.Style.Width = 350;
            base.Style.Height = 200;
        }

        public override void Apply(Control control)
        {
            if (control is Wizard)
            {
                this.Apply(control as Wizard);
            }
        }

        private void Apply(Wizard wizard)
        {
            wizard.Font.Name = this.FontName;
            wizard.Font.Size = this.FontSize;
            wizard.BackColor = this.BackColor;
            wizard.BorderColor = this.BorderColor;
            wizard.BorderWidth = this.BorderWidth;
            wizard.BorderStyle = this.BorderStyle;
            wizard.Font.ClearDefaults();
            wizard.NavigationButtonStyle.BorderWidth = this.NavigationButtonStyleBorderWidth;
            wizard.NavigationButtonStyle.Font.Name = this.NavigationButtonStyleFontName;
            wizard.NavigationButtonStyle.Font.Size = this.NavigationButtonStyleFontSize;
            wizard.NavigationButtonStyle.BorderStyle = this.NavigationButtonStyleBorderStyle;
            wizard.NavigationButtonStyle.BorderColor = this.NavigationButtonStyleBorderColor;
            wizard.NavigationButtonStyle.ForeColor = this.NavigationButtonStyleForeColor;
            wizard.NavigationButtonStyle.BackColor = this.NavigationButtonStyleBackColor;
            wizard.NavigationButtonStyle.Font.ClearDefaults();
            wizard.StepStyle.BorderWidth = this.StepStyleBorderWidth;
            wizard.StepStyle.BorderStyle = this.StepStyleBorderStyle;
            wizard.StepStyle.BorderColor = this.StepStyleBorderColor;
            wizard.StepStyle.ForeColor = this.StepStyleForeColor;
            wizard.StepStyle.BackColor = this.StepStyleBackColor;
            wizard.StepStyle.Font.Size = this.StepStyleFontSize;
            wizard.StepStyle.Font.ClearDefaults();
            wizard.SideBarButtonStyle.Font.Underline = this.SideBarButtonStyleFontUnderline;
            wizard.SideBarButtonStyle.Font.Name = this.SideBarButtonStyleFontName;
            wizard.SideBarButtonStyle.ForeColor = this.SideBarButtonStyleForeColor;
            wizard.SideBarButtonStyle.BorderWidth = this.SideBarButtonStyleBorderWidth;
            wizard.SideBarButtonStyle.BackColor = this.SideBarButtonStyleBackColor;
            wizard.SideBarButtonStyle.Font.ClearDefaults();
            wizard.HeaderStyle.ForeColor = this.HeaderStyleForeColor;
            wizard.HeaderStyle.BorderColor = this.HeaderStyleBorderColor;
            wizard.HeaderStyle.BackColor = this.HeaderStyleBackColor;
            wizard.HeaderStyle.Font.Size = this.HeaderStyleFontSize;
            wizard.HeaderStyle.Font.Bold = this.HeaderStyleFontBold;
            wizard.HeaderStyle.BorderWidth = this.HeaderStyleBorderWidth;
            wizard.HeaderStyle.HorizontalAlign = this.HeaderStyleHorizontalAlign;
            wizard.HeaderStyle.BorderStyle = this.HeaderStyleBorderStyle;
            wizard.HeaderStyle.Font.ClearDefaults();
            wizard.SideBarStyle.BackColor = this.SideBarStyleBackColor;
            wizard.SideBarStyle.VerticalAlign = this.SideBarStyleVerticalAlign;
            wizard.SideBarStyle.Font.Size = this.SideBarStyleFontSize;
            wizard.SideBarStyle.Font.Underline = this.SideBarStyleFontUnderline;
            wizard.SideBarStyle.Font.Strikeout = this.SideBarStyleFontStrikeout;
            wizard.SideBarStyle.BorderWidth = this.SideBarStyleBorderWidth;
            wizard.SideBarStyle.Font.ClearDefaults();
        }

        private bool GetBooleanProperty(string propertyTag, DataRow schemeData)
        {
            object obj2 = schemeData[propertyTag];
            return (((obj2 != null) && !obj2.Equals(DBNull.Value)) && bool.Parse(obj2.ToString()));
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
            if (schemeData != null)
            {
                this.FontName = this.GetStringProperty("FontName", schemeData);
                this.FontSize = new FontUnit(this.GetStringProperty("FontSize", schemeData), CultureInfo.InvariantCulture);
                this.BackColor = ColorTranslator.FromHtml(this.GetStringProperty("BackColor", schemeData));
                this.BorderColor = ColorTranslator.FromHtml(this.GetStringProperty("BorderColor", schemeData));
                this.BorderWidth = new Unit(this.GetStringProperty("BorderWidth", schemeData), CultureInfo.InvariantCulture);
                this.SideBarStyleBackColor = ColorTranslator.FromHtml(this.GetStringProperty("SideBarStyleBackColor", schemeData));
                this.SideBarStyleVerticalAlign = (VerticalAlign) this.GetIntProperty("SideBarStyleVerticalAlign", schemeData);
                this.BorderStyle = (System.Web.UI.WebControls.BorderStyle) this.GetIntProperty("BorderStyle", schemeData);
                this.NavigationButtonStyleBorderWidth = new Unit(this.GetStringProperty("NavigationButtonStyleBorderWidth", schemeData), CultureInfo.InvariantCulture);
                this.NavigationButtonStyleFontName = this.GetStringProperty("NavigationButtonStyleFontName", schemeData);
                this.NavigationButtonStyleFontSize = new FontUnit(this.GetStringProperty("NavigationButtonStyleFontSize", schemeData), CultureInfo.InvariantCulture);
                this.NavigationButtonStyleBorderStyle = (System.Web.UI.WebControls.BorderStyle) this.GetIntProperty("NavigationButtonStyleBorderStyle", schemeData);
                this.NavigationButtonStyleBorderColor = ColorTranslator.FromHtml(this.GetStringProperty("NavigationButtonStyleBorderColor", schemeData));
                this.NavigationButtonStyleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("NavigationButtonStyleForeColor", schemeData));
                this.NavigationButtonStyleBackColor = ColorTranslator.FromHtml(this.GetStringProperty("NavigationButtonStyleBackColor", schemeData));
                this.StepStyleBorderWidth = new Unit(this.GetStringProperty("StepStyleBorderWidth", schemeData), CultureInfo.InvariantCulture);
                this.StepStyleBorderStyle = (System.Web.UI.WebControls.BorderStyle) this.GetIntProperty("StepStyleBorderStyle", schemeData);
                this.StepStyleBorderColor = ColorTranslator.FromHtml(this.GetStringProperty("StepStyleBorderColor", schemeData));
                this.StepStyleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("StepStyleForeColor", schemeData));
                this.StepStyleBackColor = ColorTranslator.FromHtml(this.GetStringProperty("StepStyleBackColor", schemeData));
                this.StepStyleFontSize = new FontUnit(this.GetStringProperty("StepStyleFontSize", schemeData), CultureInfo.InvariantCulture);
                this.SideBarButtonStyleFontUnderline = this.GetBooleanProperty("SideBarButtonStyleFontUnderline", schemeData);
                this.SideBarButtonStyleFontName = this.GetStringProperty("SideBarButtonStyleFontName", schemeData);
                this.SideBarButtonStyleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("SideBarButtonStyleForeColor", schemeData));
                this.SideBarButtonStyleBorderWidth = new Unit(this.GetStringProperty("SideBarButtonStyleBorderWidth", schemeData), CultureInfo.InvariantCulture);
                this.SideBarButtonStyleBackColor = ColorTranslator.FromHtml(this.GetStringProperty("SideBarButtonStyleBackColor", schemeData));
                this.HeaderStyleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("HeaderStyleForeColor", schemeData));
                this.HeaderStyleBorderColor = ColorTranslator.FromHtml(this.GetStringProperty("HeaderStyleBorderColor", schemeData));
                this.HeaderStyleBackColor = ColorTranslator.FromHtml(this.GetStringProperty("HeaderStyleBackColor", schemeData));
                this.HeaderStyleFontSize = new FontUnit(this.GetStringProperty("HeaderStyleFontSize", schemeData), CultureInfo.InvariantCulture);
                this.HeaderStyleFontBold = this.GetBooleanProperty("HeaderStyleFontBold", schemeData);
                this.HeaderStyleBorderWidth = new Unit(this.GetStringProperty("HeaderStyleBorderWidth", schemeData), CultureInfo.InvariantCulture);
                this.HeaderStyleHorizontalAlign = (HorizontalAlign) this.GetIntProperty("HeaderStyleHorizontalAlign", schemeData);
                this.HeaderStyleBorderStyle = (System.Web.UI.WebControls.BorderStyle) this.GetIntProperty("HeaderStyleBorderStyle", schemeData);
                this.SideBarStyleBackColor = ColorTranslator.FromHtml(this.GetStringProperty("SideBarStyleBackColor", schemeData));
                this.SideBarStyleVerticalAlign = (VerticalAlign) this.GetIntProperty("SideBarStyleVerticalAlign", schemeData);
                this.SideBarStyleFontSize = new FontUnit(this.GetStringProperty("SideBarStyleFontSize", schemeData), CultureInfo.InvariantCulture);
                this.SideBarStyleFontUnderline = this.GetBooleanProperty("SideBarStyleFontUnderline", schemeData);
                this.SideBarStyleFontStrikeout = this.GetBooleanProperty("SideBarStyleFontStrikeout", schemeData);
                this.SideBarStyleBorderWidth = new Unit(this.GetStringProperty("SideBarStyleBorderWidth", schemeData), CultureInfo.InvariantCulture);
            }
        }
    }
}

