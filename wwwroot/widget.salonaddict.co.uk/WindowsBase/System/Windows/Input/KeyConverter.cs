namespace System.Windows.Input
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;

    public class KeyConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            (sourceType == typeof(string));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (((destinationType != typeof(string)) || (context == null)) || (context.Instance == null))
            {
                return false;
            }
            Key instance = (Key) context.Instance;
            return ((instance >= Key.None) && (instance <= Key.DeadCharProcessed));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            if (!(source is string))
            {
                throw base.GetConvertFromException(source);
            }
            string keyToken = ((string) source).Trim();
            object key = this.GetKey(keyToken, CultureInfo.InvariantCulture);
            if (key == null)
            {
                throw new NotSupportedException(System.Windows.SR.Get("Unsupported_Key", new object[] { keyToken }));
            }
            return (Key) key;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if ((destinationType == typeof(string)) && (value != null))
            {
                Key key = (Key) value;
                if (key == Key.None)
                {
                    return string.Empty;
                }
                if ((key >= Key.D0) && (key <= Key.D9))
                {
                    return char.ToString((char) ((ushort) ((key - 0x22) + 0x30)));
                }
                if ((key >= Key.A) && (key <= Key.Z))
                {
                    return char.ToString((char) ((ushort) ((key - 0x2c) + 0x41)));
                }
                string str = MatchKey(key, culture);
                if ((str != null) && ((str.Length != 0) || (str == string.Empty)))
                {
                    return str;
                }
            }
            throw base.GetConvertToException(value, destinationType);
        }

        private object GetKey(string keyToken, CultureInfo culture)
        {
            if (keyToken == string.Empty)
            {
                return Key.None;
            }
            keyToken = keyToken.ToUpper(culture);
            if ((keyToken.Length == 1) && char.IsLetterOrDigit(keyToken[0]))
            {
                if ((char.IsDigit(keyToken[0]) && (keyToken[0] >= '0')) && (keyToken[0] <= '9'))
                {
                    return (('"' + keyToken[0]) - 0x30);
                }
                if ((!char.IsLetter(keyToken[0]) || (keyToken[0] < 'A')) || (keyToken[0] > 'Z'))
                {
                    throw new ArgumentException(System.Windows.SR.Get("CannotConvertStringToType", new object[] { keyToken, typeof(Key) }));
                }
                return ((',' + keyToken[0]) - 0x41);
            }
            Key escape = ~Key.None;
            switch (keyToken)
            {
                case "ENTER":
                    escape = Key.Return;
                    break;

                case "ESC":
                    escape = Key.Escape;
                    break;

                case "PGUP":
                    escape = Key.Prior;
                    break;

                case "PGDN":
                    escape = Key.Next;
                    break;

                case "PRTSC":
                    escape = Key.Snapshot;
                    break;

                case "INS":
                    escape = Key.Insert;
                    break;

                case "DEL":
                    escape = Key.Delete;
                    break;

                case "WINDOWS":
                    escape = Key.LWin;
                    break;

                case "WIN":
                    escape = Key.LWin;
                    break;

                case "LEFTWINDOWS":
                    escape = Key.LWin;
                    break;

                case "RIGHTWINDOWS":
                    escape = Key.RWin;
                    break;

                case "APPS":
                    escape = Key.Apps;
                    break;

                case "APPLICATION":
                    escape = Key.Apps;
                    break;

                case "BREAK":
                    escape = Key.Cancel;
                    break;

                case "BACKSPACE":
                    escape = Key.Back;
                    break;

                case "BKSP":
                    escape = Key.Back;
                    break;

                case "BS":
                    escape = Key.Back;
                    break;

                case "SHIFT":
                    escape = Key.LeftShift;
                    break;

                case "LEFTSHIFT":
                    escape = Key.LeftShift;
                    break;

                case "RIGHTSHIFT":
                    escape = Key.RightShift;
                    break;

                case "CONTROL":
                    escape = Key.LeftCtrl;
                    break;

                case "CTRL":
                    escape = Key.LeftCtrl;
                    break;

                case "LEFTCTRL":
                    escape = Key.LeftCtrl;
                    break;

                case "RIGHTCTRL":
                    escape = Key.RightCtrl;
                    break;

                case "ALT":
                    escape = Key.LeftAlt;
                    break;

                case "LEFTALT":
                    escape = Key.LeftAlt;
                    break;

                case "RIGHTALT":
                    escape = Key.RightAlt;
                    break;

                case "SEMICOLON":
                    escape = Key.Oem1;
                    break;

                case "PLUS":
                    escape = Key.OemPlus;
                    break;

                case "COMMA":
                    escape = Key.OemComma;
                    break;

                case "MINUS":
                    escape = Key.OemMinus;
                    break;

                case "PERIOD":
                    escape = Key.OemPeriod;
                    break;

                case "QUESTION":
                    escape = Key.Oem2;
                    break;

                case "TILDE":
                    escape = Key.Oem3;
                    break;

                case "OPENBRACKETS":
                    escape = Key.Oem4;
                    break;

                case "PIPE":
                    escape = Key.Oem5;
                    break;

                case "CLOSEBRACKETS":
                    escape = Key.Oem6;
                    break;

                case "QUOTES":
                    escape = Key.Oem7;
                    break;

                case "BACKSLASH":
                    escape = Key.Oem102;
                    break;

                case "FINISH":
                    escape = Key.OemFinish;
                    break;

                case "ATTN":
                    escape = Key.Attn;
                    break;

                case "CRSEL":
                    escape = Key.CrSel;
                    break;

                case "EXSEL":
                    escape = Key.ExSel;
                    break;

                case "ERASEEOF":
                    escape = Key.EraseEof;
                    break;

                case "PLAY":
                    escape = Key.Play;
                    break;

                case "ZOOM":
                    escape = Key.Zoom;
                    break;

                case "PA1":
                    escape = Key.Pa1;
                    break;

                default:
                    escape = (Key) Enum.Parse(typeof(Key), keyToken, true);
                    break;
            }
            if (escape != ~Key.None)
            {
                return escape;
            }
            return null;
        }

        private static string MatchKey(Key key, CultureInfo culture)
        {
            switch (key)
            {
                case Key.Back:
                    return "Backspace";

                case Key.LineFeed:
                    return "Clear";

                case Key.Escape:
                    return "Esc";

                case Key.None:
                    return string.Empty;
            }
            if ((key >= Key.None) && (key <= Key.DeadCharProcessed))
            {
                return key.ToString();
            }
            return null;
        }
    }
}

