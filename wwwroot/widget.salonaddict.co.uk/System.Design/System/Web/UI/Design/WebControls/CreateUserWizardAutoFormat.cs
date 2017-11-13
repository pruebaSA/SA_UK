namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal sealed class CreateUserWizardAutoFormat : DesignerAutoFormat
    {
        private string backColor;
        private string borderColor;
        private int borderStyle;
        private string borderWidth;
        private const int FONT_BOLD = 1;
        private string fontName;
        private string fontSize;
        private Color HeaderStyleBackColor;
        private Color HeaderStyleBorderColor;
        private BorderStyle HeaderStyleBorderStyle;
        private Unit HeaderStyleBorderWidth;
        private bool HeaderStyleFontBold;
        private FontUnit HeaderStyleFontSize;
        private Color HeaderStyleForeColor;
        private HorizontalAlign HeaderStyleHorizontalAlign;
        private Color NavigationButtonStyleBackColor;
        private Color NavigationButtonStyleBorderColor;
        private BorderStyle NavigationButtonStyleBorderStyle;
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
        private BorderStyle StepStyleBorderStyle;
        private Unit StepStyleBorderWidth;
        private FontUnit StepStyleFontSize;
        private Color StepStyleForeColor;
        private string titleTextBackColor;
        private int titleTextFont;
        private string titleTextForeColor;

        public CreateUserWizardAutoFormat(DataRow schemeData) : base(System.Design.SR.GetString(schemeData["SchemeName"].ToString()))
        {
            this.borderStyle = -1;
            this.Load(schemeData);
            base.Style.Width = 500;
            base.Style.Height = 400;
        }

        public override void Apply(Control control)
        {
            if (control is CreateUserWizard)
            {
                this.Apply(control as CreateUserWizard);
            }
        }

        private void Apply(CreateUserWizard createUserWizard)
        {
            createUserWizard.StepStyle.Reset();
            createUserWizard.BackColor = ColorTranslator.FromHtml(this.backColor);
            createUserWizard.BorderColor = ColorTranslator.FromHtml(this.borderColor);
            createUserWizard.BorderWidth = new Unit(this.borderWidth, CultureInfo.InvariantCulture);
            if ((this.borderStyle >= 0) && (this.borderStyle <= 9))
            {
                createUserWizard.BorderStyle = (BorderStyle) this.borderStyle;
            }
            else
            {
                createUserWizard.BorderStyle = BorderStyle.NotSet;
            }
            createUserWizard.Font.Size = new FontUnit(this.fontSize, CultureInfo.InvariantCulture);
            createUserWizard.Font.Name = this.fontName;
            createUserWizard.Font.ClearDefaults();
            createUserWizard.TitleTextStyle.BackColor = ColorTranslator.FromHtml(this.titleTextBackColor);
            createUserWizard.TitleTextStyle.ForeColor = ColorTranslator.FromHtml(this.titleTextForeColor);
            createUserWizard.TitleTextStyle.Font.Bold = (this.titleTextFont & 1) != 0;
            createUserWizard.TitleTextStyle.Font.ClearDefaults();
            createUserWizard.StepStyle.BorderWidth = this.StepStyleBorderWidth;
            createUserWizard.StepStyle.BorderStyle = this.StepStyleBorderStyle;
            createUserWizard.StepStyle.BorderColor = this.StepStyleBorderColor;
            createUserWizard.StepStyle.ForeColor = this.StepStyleForeColor;
            createUserWizard.StepStyle.BackColor = this.StepStyleBackColor;
            createUserWizard.StepStyle.Font.Size = this.StepStyleFontSize;
            createUserWizard.StepStyle.Font.ClearDefaults();
            createUserWizard.SideBarButtonStyle.Font.Underline = this.SideBarButtonStyleFontUnderline;
            createUserWizard.SideBarButtonStyle.Font.Name = this.SideBarButtonStyleFontName;
            createUserWizard.SideBarButtonStyle.ForeColor = this.SideBarButtonStyleForeColor;
            createUserWizard.SideBarButtonStyle.BorderWidth = this.SideBarButtonStyleBorderWidth;
            createUserWizard.SideBarButtonStyle.BackColor = this.SideBarButtonStyleBackColor;
            createUserWizard.SideBarButtonStyle.Font.ClearDefaults();
            createUserWizard.NavigationButtonStyle.BorderWidth = this.NavigationButtonStyleBorderWidth;
            createUserWizard.NavigationButtonStyle.Font.Name = this.NavigationButtonStyleFontName;
            createUserWizard.NavigationButtonStyle.Font.Size = this.NavigationButtonStyleFontSize;
            createUserWizard.NavigationButtonStyle.BorderStyle = this.NavigationButtonStyleBorderStyle;
            createUserWizard.NavigationButtonStyle.BorderColor = this.NavigationButtonStyleBorderColor;
            createUserWizard.NavigationButtonStyle.ForeColor = this.NavigationButtonStyleForeColor;
            createUserWizard.NavigationButtonStyle.BackColor = this.NavigationButtonStyleBackColor;
            createUserWizard.NavigationButtonStyle.Font.ClearDefaults();
            createUserWizard.ContinueButtonStyle.BorderWidth = this.NavigationButtonStyleBorderWidth;
            createUserWizard.ContinueButtonStyle.Font.Name = this.NavigationButtonStyleFontName;
            createUserWizard.ContinueButtonStyle.Font.Size = this.NavigationButtonStyleFontSize;
            createUserWizard.ContinueButtonStyle.BorderStyle = this.NavigationButtonStyleBorderStyle;
            createUserWizard.ContinueButtonStyle.BorderColor = this.NavigationButtonStyleBorderColor;
            createUserWizard.ContinueButtonStyle.ForeColor = this.NavigationButtonStyleForeColor;
            createUserWizard.ContinueButtonStyle.BackColor = this.NavigationButtonStyleBackColor;
            createUserWizard.ContinueButtonStyle.Font.ClearDefaults();
            createUserWizard.CreateUserButtonStyle.BorderWidth = this.NavigationButtonStyleBorderWidth;
            createUserWizard.CreateUserButtonStyle.Font.Name = this.NavigationButtonStyleFontName;
            createUserWizard.CreateUserButtonStyle.Font.Size = this.NavigationButtonStyleFontSize;
            createUserWizard.CreateUserButtonStyle.BorderStyle = this.NavigationButtonStyleBorderStyle;
            createUserWizard.CreateUserButtonStyle.BorderColor = this.NavigationButtonStyleBorderColor;
            createUserWizard.CreateUserButtonStyle.ForeColor = this.NavigationButtonStyleForeColor;
            createUserWizard.CreateUserButtonStyle.BackColor = this.NavigationButtonStyleBackColor;
            createUserWizard.CreateUserButtonStyle.Font.ClearDefaults();
            createUserWizard.HeaderStyle.ForeColor = this.HeaderStyleForeColor;
            createUserWizard.HeaderStyle.BorderColor = this.HeaderStyleBorderColor;
            createUserWizard.HeaderStyle.BackColor = this.HeaderStyleBackColor;
            createUserWizard.HeaderStyle.Font.Size = this.HeaderStyleFontSize;
            createUserWizard.HeaderStyle.Font.Bold = this.HeaderStyleFontBold;
            createUserWizard.HeaderStyle.BorderWidth = this.HeaderStyleBorderWidth;
            createUserWizard.HeaderStyle.HorizontalAlign = this.HeaderStyleHorizontalAlign;
            createUserWizard.HeaderStyle.BorderStyle = this.HeaderStyleBorderStyle;
            createUserWizard.HeaderStyle.Font.ClearDefaults();
            createUserWizard.SideBarStyle.BackColor = this.SideBarStyleBackColor;
            createUserWizard.SideBarStyle.VerticalAlign = this.SideBarStyleVerticalAlign;
            createUserWizard.SideBarStyle.Font.Size = this.SideBarStyleFontSize;
            createUserWizard.SideBarStyle.Font.Underline = this.SideBarStyleFontUnderline;
            createUserWizard.SideBarStyle.Font.Strikeout = this.SideBarStyleFontStrikeout;
            createUserWizard.SideBarStyle.BorderWidth = this.SideBarStyleBorderWidth;
            createUserWizard.SideBarStyle.Font.ClearDefaults();
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
            this.backColor = this.GetStringProperty("BackColor", schemeData);
            this.borderColor = this.GetStringProperty("BorderColor", schemeData);
            this.borderWidth = this.GetStringProperty("BorderWidth", schemeData);
            this.borderStyle = this.GetIntProperty("BorderStyle", -1, schemeData);
            this.fontSize = this.GetStringProperty("FontSize", schemeData);
            this.fontName = this.GetStringProperty("FontName", schemeData);
            this.titleTextBackColor = this.GetStringProperty("TitleTextBackColor", schemeData);
            this.titleTextForeColor = this.GetStringProperty("TitleTextForeColor", schemeData);
            this.titleTextFont = this.GetIntProperty("TitleTextFont", schemeData);
            this.NavigationButtonStyleBorderWidth = new Unit(this.GetStringProperty("NavigationButtonStyleBorderWidth", schemeData), CultureInfo.InvariantCulture);
            this.NavigationButtonStyleFontName = this.GetStringProperty("NavigationButtonStyleFontName", schemeData);
            this.NavigationButtonStyleFontSize = new FontUnit(this.GetStringProperty("NavigationButtonStyleFontSize", schemeData), CultureInfo.InvariantCulture);
            this.NavigationButtonStyleBorderStyle = (BorderStyle) this.GetIntProperty("NavigationButtonStyleBorderStyle", schemeData);
            this.NavigationButtonStyleBorderColor = ColorTranslator.FromHtml(this.GetStringProperty("NavigationButtonStyleBorderColor", schemeData));
            this.NavigationButtonStyleForeColor = ColorTranslator.FromHtml(this.GetStringProperty("NavigationButtonStyleForeColor", schemeData));
            this.NavigationButtonStyleBackColor = ColorTranslator.FromHtml(this.GetStringProperty("NavigationButtonStyleBackColor", schemeData));
            this.StepStyleBorderWidth = new Unit(this.GetStringProperty("StepStyleBorderWidth", schemeData), CultureInfo.InvariantCulture);
            this.StepStyleBorderStyle = (BorderStyle) this.GetIntProperty("StepStyleBorderStyle", schemeData);
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
            this.HeaderStyleBorderStyle = (BorderStyle) this.GetIntProperty("HeaderStyleBorderStyle", schemeData);
            this.SideBarStyleBackColor = ColorTranslator.FromHtml(this.GetStringProperty("SideBarStyleBackColor", schemeData));
            this.SideBarStyleVerticalAlign = (VerticalAlign) this.GetIntProperty("SideBarStyleVerticalAlign", schemeData);
            this.SideBarStyleFontSize = new FontUnit(this.GetStringProperty("SideBarStyleFontSize", schemeData), CultureInfo.InvariantCulture);
            this.SideBarStyleFontUnderline = this.GetBooleanProperty("SideBarStyleFontUnderline", schemeData);
            this.SideBarStyleFontStrikeout = this.GetBooleanProperty("SideBarStyleFontStrikeout", schemeData);
            this.SideBarStyleBorderWidth = new Unit(this.GetStringProperty("SideBarStyleBorderWidth", schemeData), CultureInfo.InvariantCulture);
        }
    }
}

