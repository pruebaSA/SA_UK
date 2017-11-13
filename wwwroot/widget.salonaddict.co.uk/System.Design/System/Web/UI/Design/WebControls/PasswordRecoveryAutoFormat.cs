namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal sealed class PasswordRecoveryAutoFormat : DesignerAutoFormat
    {
        private string backColor;
        private string borderColor;
        private int borderPadding;
        private int borderStyle;
        private string borderWidth;
        private const int FONT_BOLD = 1;
        private const int FONT_ITALIC = 2;
        private string fontName;
        private string fontSize;
        private int instructionTextFont;
        private string instructionTextForeColor;
        private string submitButtonBackColor;
        private string submitButtonBorderColor;
        private int submitButtonBorderStyle;
        private string submitButtonBorderWidth;
        private string submitButtonFontName;
        private string submitButtonFontSize;
        private string submitButtonForeColor;
        private int successTextFont;
        private string successTextForeColor;
        private string textboxFontSize;
        private string titleTextBackColor;
        private int titleTextFont;
        private string titleTextFontSize;
        private string titleTextForeColor;

        public PasswordRecoveryAutoFormat(DataRow schemeData) : base(System.Design.SR.GetString(schemeData["SchemeName"].ToString()))
        {
            this.borderStyle = -1;
            this.borderPadding = 1;
            this.submitButtonBorderStyle = -1;
            this.Load(schemeData);
            base.Style.Width = 500;
            base.Style.Height = 300;
        }

        public override void Apply(Control control)
        {
            if (control is PasswordRecovery)
            {
                this.Apply(control as PasswordRecovery);
            }
        }

        private void Apply(PasswordRecovery passwordRecovery)
        {
            passwordRecovery.BackColor = ColorTranslator.FromHtml(this.backColor);
            passwordRecovery.BorderColor = ColorTranslator.FromHtml(this.borderColor);
            passwordRecovery.BorderWidth = new Unit(this.borderWidth, CultureInfo.InvariantCulture);
            if ((this.borderStyle >= 0) && (this.borderStyle <= 9))
            {
                passwordRecovery.BorderStyle = (BorderStyle) this.borderStyle;
            }
            else
            {
                passwordRecovery.BorderStyle = BorderStyle.NotSet;
            }
            passwordRecovery.Font.Size = new FontUnit(this.fontSize, CultureInfo.InvariantCulture);
            passwordRecovery.Font.Name = this.fontName;
            passwordRecovery.Font.ClearDefaults();
            passwordRecovery.TitleTextStyle.BackColor = ColorTranslator.FromHtml(this.titleTextBackColor);
            passwordRecovery.TitleTextStyle.ForeColor = ColorTranslator.FromHtml(this.titleTextForeColor);
            passwordRecovery.TitleTextStyle.Font.Bold = (this.titleTextFont & 1) != 0;
            passwordRecovery.TitleTextStyle.Font.Size = new FontUnit(this.titleTextFontSize, CultureInfo.InvariantCulture);
            passwordRecovery.TitleTextStyle.Font.ClearDefaults();
            passwordRecovery.BorderPadding = this.borderPadding;
            passwordRecovery.InstructionTextStyle.ForeColor = ColorTranslator.FromHtml(this.instructionTextForeColor);
            passwordRecovery.InstructionTextStyle.Font.Italic = (this.instructionTextFont & 2) != 0;
            passwordRecovery.InstructionTextStyle.Font.ClearDefaults();
            passwordRecovery.TextBoxStyle.Font.Size = new FontUnit(this.textboxFontSize, CultureInfo.InvariantCulture);
            passwordRecovery.TextBoxStyle.Font.ClearDefaults();
            passwordRecovery.SubmitButtonStyle.BackColor = ColorTranslator.FromHtml(this.submitButtonBackColor);
            passwordRecovery.SubmitButtonStyle.ForeColor = ColorTranslator.FromHtml(this.submitButtonForeColor);
            passwordRecovery.SubmitButtonStyle.Font.Size = new FontUnit(this.submitButtonFontSize, CultureInfo.InvariantCulture);
            passwordRecovery.SubmitButtonStyle.Font.Name = this.submitButtonFontName;
            passwordRecovery.SubmitButtonStyle.BorderColor = ColorTranslator.FromHtml(this.submitButtonBorderColor);
            passwordRecovery.SubmitButtonStyle.BorderWidth = new Unit(this.submitButtonBorderWidth, CultureInfo.InvariantCulture);
            if ((this.submitButtonBorderStyle >= 0) && (this.submitButtonBorderStyle <= 9))
            {
                passwordRecovery.SubmitButtonStyle.BorderStyle = (BorderStyle) this.submitButtonBorderStyle;
            }
            else
            {
                passwordRecovery.SubmitButtonStyle.BorderStyle = BorderStyle.NotSet;
            }
            passwordRecovery.SubmitButtonStyle.Font.ClearDefaults();
            passwordRecovery.SuccessTextStyle.ForeColor = ColorTranslator.FromHtml(this.successTextForeColor);
            passwordRecovery.SuccessTextStyle.Font.Bold = (this.successTextFont & 1) != 0;
            passwordRecovery.SuccessTextStyle.Font.ClearDefaults();
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
            this.titleTextFontSize = this.GetStringProperty("TitleTextFontSize", schemeData);
            this.instructionTextForeColor = this.GetStringProperty("InstructionTextForeColor", schemeData);
            this.instructionTextFont = this.GetIntProperty("InstructionTextFont", schemeData);
            this.borderPadding = this.GetIntProperty("BorderPadding", 1, schemeData);
            this.textboxFontSize = this.GetStringProperty("TextboxFontSize", schemeData);
            this.submitButtonBackColor = this.GetStringProperty("SubmitButtonBackColor", schemeData);
            this.submitButtonForeColor = this.GetStringProperty("SubmitButtonForeColor", schemeData);
            this.submitButtonFontSize = this.GetStringProperty("SubmitButtonFontSize", schemeData);
            this.submitButtonFontName = this.GetStringProperty("SubmitButtonFontName", schemeData);
            this.submitButtonBorderColor = this.GetStringProperty("SubmitButtonBorderColor", schemeData);
            this.submitButtonBorderWidth = this.GetStringProperty("SubmitButtonBorderWidth", schemeData);
            this.submitButtonBorderStyle = this.GetIntProperty("SubmitButtonBorderStyle", -1, schemeData);
            this.successTextForeColor = this.GetStringProperty("SuccessTextForeColor", schemeData);
            this.successTextFont = this.GetIntProperty("SuccessTextFont", schemeData);
        }
    }
}

