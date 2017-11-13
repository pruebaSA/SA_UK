namespace System.Windows.Forms
{
    using System;
    using System.Globalization;

    public class InputLanguageChangedEventArgs : EventArgs
    {
        private readonly byte charSet;
        private readonly CultureInfo culture;
        private readonly System.Windows.Forms.InputLanguage inputLanguage;

        public InputLanguageChangedEventArgs(CultureInfo culture, byte charSet)
        {
            this.inputLanguage = System.Windows.Forms.InputLanguage.FromCulture(culture);
            this.culture = culture;
            this.charSet = charSet;
        }

        public InputLanguageChangedEventArgs(System.Windows.Forms.InputLanguage inputLanguage, byte charSet)
        {
            this.inputLanguage = inputLanguage;
            this.culture = inputLanguage.Culture;
            this.charSet = charSet;
        }

        public byte CharSet =>
            this.charSet;

        public CultureInfo Culture =>
            this.culture;

        public System.Windows.Forms.InputLanguage InputLanguage =>
            this.inputLanguage;
    }
}

