namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal sealed class ChangePasswordAutoFormat : DesignerAutoFormat
    {
        private string _backColor;
        private string _borderColor;
        private int _borderPadding;
        private int _borderStyle;
        private string _borderWidth;
        private string _buttonBackColor;
        private string _buttonBorderColor;
        private int _buttonBorderStyle;
        private string _buttonBorderWidth;
        private string _buttonFontName;
        private string _buttonFontSize;
        private string _buttonForeColor;
        private string _fontName;
        private string _fontSize;
        private int _instructionTextFont;
        private string _instructionTextForeColor;
        private int _passwordHintFont;
        private string _passwordHintForeColor;
        private string _textboxFontSize;
        private string _titleTextBackColor;
        private int _titleTextFont;
        private string _titleTextFontSize;
        private string _titleTextForeColor;
        private const int FONT_BOLD = 1;
        private const int FONT_ITALIC = 2;

        public ChangePasswordAutoFormat(DataRow schemeData) : base(System.Design.SR.GetString(schemeData["SchemeName"].ToString()))
        {
            this._borderStyle = -1;
            this._borderPadding = 1;
            this._buttonBorderStyle = -1;
            this.Load(schemeData);
            base.Style.Width = 400;
            base.Style.Height = 250;
        }

        public override void Apply(Control control)
        {
            if (control is ChangePassword)
            {
                this.Apply(control as ChangePassword);
            }
        }

        private void Apply(ChangePassword changePassword)
        {
            changePassword.BackColor = ColorTranslator.FromHtml(this._backColor);
            changePassword.BorderColor = ColorTranslator.FromHtml(this._borderColor);
            changePassword.BorderWidth = new Unit(this._borderWidth, CultureInfo.InvariantCulture);
            if ((this._borderStyle >= 0) && (this._borderStyle <= 9))
            {
                changePassword.BorderStyle = (BorderStyle) this._borderStyle;
            }
            else
            {
                changePassword.BorderStyle = BorderStyle.NotSet;
            }
            changePassword.Font.Size = new FontUnit(this._fontSize, CultureInfo.InvariantCulture);
            changePassword.Font.Name = this._fontName;
            changePassword.Font.ClearDefaults();
            changePassword.TitleTextStyle.BackColor = ColorTranslator.FromHtml(this._titleTextBackColor);
            changePassword.TitleTextStyle.ForeColor = ColorTranslator.FromHtml(this._titleTextForeColor);
            changePassword.TitleTextStyle.Font.Bold = (this._titleTextFont & 1) != 0;
            changePassword.TitleTextStyle.Font.Size = new FontUnit(this._titleTextFontSize, CultureInfo.InvariantCulture);
            changePassword.TitleTextStyle.Font.ClearDefaults();
            changePassword.BorderPadding = this._borderPadding;
            changePassword.InstructionTextStyle.ForeColor = ColorTranslator.FromHtml(this._instructionTextForeColor);
            changePassword.InstructionTextStyle.Font.Italic = (this._instructionTextFont & 2) != 0;
            changePassword.InstructionTextStyle.Font.ClearDefaults();
            changePassword.TextBoxStyle.Font.Size = new FontUnit(this._textboxFontSize, CultureInfo.InvariantCulture);
            changePassword.TextBoxStyle.Font.ClearDefaults();
            changePassword.ChangePasswordButtonStyle.BackColor = ColorTranslator.FromHtml(this._buttonBackColor);
            changePassword.ChangePasswordButtonStyle.ForeColor = ColorTranslator.FromHtml(this._buttonForeColor);
            changePassword.ChangePasswordButtonStyle.Font.Size = new FontUnit(this._buttonFontSize, CultureInfo.InvariantCulture);
            changePassword.ChangePasswordButtonStyle.Font.Name = this._buttonFontName;
            changePassword.ChangePasswordButtonStyle.BorderColor = ColorTranslator.FromHtml(this._buttonBorderColor);
            changePassword.ChangePasswordButtonStyle.BorderWidth = new Unit(this._buttonBorderWidth, CultureInfo.InvariantCulture);
            if ((this._buttonBorderStyle >= 0) && (this._buttonBorderStyle <= 9))
            {
                changePassword.ChangePasswordButtonStyle.BorderStyle = (BorderStyle) this._buttonBorderStyle;
            }
            else
            {
                changePassword.ChangePasswordButtonStyle.BorderStyle = BorderStyle.NotSet;
            }
            changePassword.ChangePasswordButtonStyle.Font.ClearDefaults();
            changePassword.ContinueButtonStyle.BackColor = ColorTranslator.FromHtml(this._buttonBackColor);
            changePassword.ContinueButtonStyle.ForeColor = ColorTranslator.FromHtml(this._buttonForeColor);
            changePassword.ContinueButtonStyle.Font.Size = new FontUnit(this._buttonFontSize, CultureInfo.InvariantCulture);
            changePassword.ContinueButtonStyle.Font.Name = this._buttonFontName;
            changePassword.ContinueButtonStyle.BorderColor = ColorTranslator.FromHtml(this._buttonBorderColor);
            changePassword.ContinueButtonStyle.BorderWidth = new Unit(this._buttonBorderWidth, CultureInfo.InvariantCulture);
            if ((this._buttonBorderStyle >= 0) && (this._buttonBorderStyle <= 9))
            {
                changePassword.ContinueButtonStyle.BorderStyle = (BorderStyle) this._buttonBorderStyle;
            }
            else
            {
                changePassword.ContinueButtonStyle.BorderStyle = BorderStyle.NotSet;
            }
            changePassword.ContinueButtonStyle.Font.ClearDefaults();
            changePassword.CancelButtonStyle.BackColor = ColorTranslator.FromHtml(this._buttonBackColor);
            changePassword.CancelButtonStyle.ForeColor = ColorTranslator.FromHtml(this._buttonForeColor);
            changePassword.CancelButtonStyle.Font.Size = new FontUnit(this._buttonFontSize, CultureInfo.InvariantCulture);
            changePassword.CancelButtonStyle.Font.Name = this._buttonFontName;
            changePassword.CancelButtonStyle.BorderColor = ColorTranslator.FromHtml(this._buttonBorderColor);
            changePassword.CancelButtonStyle.BorderWidth = new Unit(this._buttonBorderWidth, CultureInfo.InvariantCulture);
            if ((this._buttonBorderStyle >= 0) && (this._buttonBorderStyle <= 9))
            {
                changePassword.CancelButtonStyle.BorderStyle = (BorderStyle) this._buttonBorderStyle;
            }
            else
            {
                changePassword.CancelButtonStyle.BorderStyle = BorderStyle.NotSet;
            }
            changePassword.CancelButtonStyle.Font.ClearDefaults();
            changePassword.PasswordHintStyle.ForeColor = ColorTranslator.FromHtml(this._passwordHintForeColor);
            changePassword.PasswordHintStyle.Font.Italic = (this._passwordHintFont & 2) != 0;
            changePassword.PasswordHintStyle.Font.ClearDefaults();
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
            this._backColor = this.GetStringProperty("BackColor", schemeData);
            this._borderColor = this.GetStringProperty("BorderColor", schemeData);
            this._borderWidth = this.GetStringProperty("BorderWidth", schemeData);
            this._borderStyle = this.GetIntProperty("BorderStyle", -1, schemeData);
            this._fontSize = this.GetStringProperty("FontSize", schemeData);
            this._fontName = this.GetStringProperty("FontName", schemeData);
            this._titleTextBackColor = this.GetStringProperty("TitleTextBackColor", schemeData);
            this._titleTextForeColor = this.GetStringProperty("TitleTextForeColor", schemeData);
            this._titleTextFont = this.GetIntProperty("TitleTextFont", schemeData);
            this._titleTextFontSize = this.GetStringProperty("TitleTextFontSize", schemeData);
            this._instructionTextForeColor = this.GetStringProperty("InstructionTextForeColor", schemeData);
            this._instructionTextFont = this.GetIntProperty("InstructionTextFont", schemeData);
            this._borderPadding = this.GetIntProperty("BorderPadding", 1, schemeData);
            this._textboxFontSize = this.GetStringProperty("TextboxFontSize", schemeData);
            this._buttonBackColor = this.GetStringProperty("ButtonBackColor", schemeData);
            this._buttonForeColor = this.GetStringProperty("ButtonForeColor", schemeData);
            this._buttonFontSize = this.GetStringProperty("ButtonFontSize", schemeData);
            this._buttonFontName = this.GetStringProperty("ButtonFontName", schemeData);
            this._buttonBorderColor = this.GetStringProperty("ButtonBorderColor", schemeData);
            this._buttonBorderWidth = this.GetStringProperty("ButtonBorderWidth", schemeData);
            this._buttonBorderStyle = this.GetIntProperty("ButtonBorderStyle", -1, schemeData);
            this._passwordHintForeColor = this.GetStringProperty("PasswordHintForeColor", schemeData);
            this._passwordHintFont = this.GetIntProperty("PasswordHintFont", schemeData);
        }
    }
}

