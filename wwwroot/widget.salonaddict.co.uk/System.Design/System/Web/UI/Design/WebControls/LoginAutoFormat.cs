namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal sealed class LoginAutoFormat : DesignerAutoFormat
    {
        private string _loginButtonBackColor;
        private string _loginButtonBorderColor;
        private int _loginButtonBorderStyle;
        private string _loginButtonBorderWidth;
        private string _loginButtonFontName;
        private string _loginButtonFontSize;
        private string _loginButtonForeColor;
        private string backColor;
        private string borderColor;
        private int borderPadding;
        private int borderStyle;
        private string borderWidth;
        private const int FONT_BOLD = 1;
        private const int FONT_ITALIC = 2;
        private string fontName;
        private string fontSize;
        private string foreColor;
        private int instructionTextFont;
        private string instructionTextForeColor;
        private string textboxFontSize;
        private int textLayout;
        private string titleTextBackColor;
        private int titleTextFont;
        private string titleTextFontSize;
        private string titleTextForeColor;

        public LoginAutoFormat(DataRow schemeData) : base(System.Design.SR.GetString(schemeData["SchemeName"].ToString()))
        {
            this.borderStyle = -1;
            this._loginButtonBorderStyle = -1;
            this.Load(schemeData);
            base.Style.Width = 300;
            base.Style.Height = 200;
        }

        public override void Apply(Control control)
        {
            if (control is Login)
            {
                this.Apply(control as Login);
            }
        }

        private void Apply(Login login)
        {
            login.BackColor = ColorTranslator.FromHtml(this.backColor);
            login.ForeColor = ColorTranslator.FromHtml(this.foreColor);
            login.BorderColor = ColorTranslator.FromHtml(this.borderColor);
            login.BorderWidth = new Unit(this.borderWidth, CultureInfo.InvariantCulture);
            if ((this.borderStyle >= 0) && (this.borderStyle <= 9))
            {
                login.BorderStyle = (BorderStyle) this.borderStyle;
            }
            else
            {
                login.BorderStyle = BorderStyle.NotSet;
            }
            login.Font.Size = new FontUnit(this.fontSize, CultureInfo.InvariantCulture);
            login.Font.Name = this.fontName;
            login.Font.ClearDefaults();
            login.TitleTextStyle.BackColor = ColorTranslator.FromHtml(this.titleTextBackColor);
            login.TitleTextStyle.ForeColor = ColorTranslator.FromHtml(this.titleTextForeColor);
            login.TitleTextStyle.Font.Bold = (this.titleTextFont & 1) != 0;
            login.TitleTextStyle.Font.Size = new FontUnit(this.titleTextFontSize, CultureInfo.InvariantCulture);
            login.TitleTextStyle.Font.ClearDefaults();
            login.BorderPadding = this.borderPadding;
            if (this.textLayout > 0)
            {
                login.TextLayout = LoginTextLayout.TextOnTop;
            }
            else
            {
                login.TextLayout = LoginTextLayout.TextOnLeft;
            }
            login.InstructionTextStyle.ForeColor = ColorTranslator.FromHtml(this.instructionTextForeColor);
            login.InstructionTextStyle.Font.Italic = (this.instructionTextFont & 2) != 0;
            login.InstructionTextStyle.Font.ClearDefaults();
            login.TextBoxStyle.Font.Size = new FontUnit(this.textboxFontSize, CultureInfo.InvariantCulture);
            login.TextBoxStyle.Font.ClearDefaults();
            login.LoginButtonStyle.BackColor = ColorTranslator.FromHtml(this._loginButtonBackColor);
            login.LoginButtonStyle.ForeColor = ColorTranslator.FromHtml(this._loginButtonForeColor);
            login.LoginButtonStyle.Font.Size = new FontUnit(this._loginButtonFontSize, CultureInfo.InvariantCulture);
            login.LoginButtonStyle.Font.Name = this._loginButtonFontName;
            login.LoginButtonStyle.BorderColor = ColorTranslator.FromHtml(this._loginButtonBorderColor);
            login.LoginButtonStyle.BorderWidth = new Unit(this._loginButtonBorderWidth, CultureInfo.InvariantCulture);
            if ((this._loginButtonBorderStyle >= 0) && (this._loginButtonBorderStyle <= 9))
            {
                login.LoginButtonStyle.BorderStyle = (BorderStyle) this._loginButtonBorderStyle;
            }
            else
            {
                login.LoginButtonStyle.BorderStyle = BorderStyle.NotSet;
            }
            login.LoginButtonStyle.Font.ClearDefaults();
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
            this.foreColor = this.GetStringProperty("ForeColor", schemeData);
            this.borderColor = this.GetStringProperty("BorderColor", schemeData);
            this.borderWidth = this.GetStringProperty("BorderWidth", schemeData);
            this.borderStyle = this.GetIntProperty("BorderStyle", -1, schemeData);
            this.fontSize = this.GetStringProperty("FontSize", schemeData);
            this.fontName = this.GetStringProperty("FontName", schemeData);
            this.instructionTextForeColor = this.GetStringProperty("InstructionTextForeColor", schemeData);
            this.instructionTextFont = this.GetIntProperty("InstructionTextFont", schemeData);
            this.titleTextBackColor = this.GetStringProperty("TitleTextBackColor", schemeData);
            this.titleTextForeColor = this.GetStringProperty("TitleTextForeColor", schemeData);
            this.titleTextFont = this.GetIntProperty("TitleTextFont", schemeData);
            this.titleTextFontSize = this.GetStringProperty("TitleTextFontSize", schemeData);
            this.borderPadding = this.GetIntProperty("BorderPadding", 1, schemeData);
            this.textLayout = this.GetIntProperty("TextLayout", schemeData);
            this.textboxFontSize = this.GetStringProperty("TextboxFontSize", schemeData);
            this._loginButtonBackColor = this.GetStringProperty("SubmitButtonBackColor", schemeData);
            this._loginButtonForeColor = this.GetStringProperty("SubmitButtonForeColor", schemeData);
            this._loginButtonFontSize = this.GetStringProperty("SubmitButtonFontSize", schemeData);
            this._loginButtonFontName = this.GetStringProperty("SubmitButtonFontName", schemeData);
            this._loginButtonBorderColor = this.GetStringProperty("SubmitButtonBorderColor", schemeData);
            this._loginButtonBorderWidth = this.GetStringProperty("SubmitButtonBorderWidth", schemeData);
            this._loginButtonBorderStyle = this.GetIntProperty("SubmitButtonBorderStyle", -1, schemeData);
        }
    }
}

