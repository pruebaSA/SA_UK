namespace System.Windows.Input
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;

    public class ModifierKeysConverter : TypeConverter
    {
        private const char Modifier_Delimiter = '+';
        private static ModifierKeys ModifierKeysFlag = (ModifierKeys.Windows | ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt);

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            (sourceType == typeof(string));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => 
            ((((destinationType == typeof(string)) && (context != null)) && ((context.Instance != null) && (context.Instance is ModifierKeys))) && IsDefinedModifierKeys((ModifierKeys) context.Instance));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            if (!(source is string))
            {
                throw base.GetConvertFromException(source);
            }
            string modifiersToken = ((string) source).Trim();
            return this.GetModifierKeys(modifiersToken, CultureInfo.InvariantCulture);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (destinationType != typeof(string))
            {
                throw base.GetConvertToException(value, destinationType);
            }
            ModifierKeys modifierKeys = (ModifierKeys) value;
            if (!IsDefinedModifierKeys(modifierKeys))
            {
                throw new InvalidEnumArgumentException("modifiers", (int) modifierKeys, typeof(ModifierKeys));
            }
            string str = "";
            if ((modifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
            {
                str = str + MatchModifiers(ModifierKeys.Control);
            }
            if ((modifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                if (str.Length > 0)
                {
                    str = str + '+';
                }
                str = str + MatchModifiers(ModifierKeys.Alt);
            }
            if ((modifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                if (str.Length > 0)
                {
                    str = str + '+';
                }
                str = str + MatchModifiers(ModifierKeys.Windows);
            }
            if ((modifierKeys & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                return str;
            }
            if (str.Length > 0)
            {
                str = str + '+';
            }
            return (str + MatchModifiers(ModifierKeys.Shift));
        }

        private ModifierKeys GetModifierKeys(string modifiersToken, CultureInfo culture)
        {
            ModifierKeys none = ModifierKeys.None;
            if (modifiersToken.Length != 0)
            {
                int length = 0;
                do
                {
                    length = modifiersToken.IndexOf('+');
                    string str = (length < 0) ? modifiersToken : modifiersToken.Substring(0, length);
                    str = str.Trim().ToUpper(culture);
                    switch (str)
                    {
                        case "CONTROL":
                        case "CTRL":
                            none |= ModifierKeys.Control;
                            break;

                        case "SHIFT":
                            none |= ModifierKeys.Shift;
                            break;

                        case "ALT":
                            none |= ModifierKeys.Alt;
                            break;

                        case "WINDOWS":
                        case "WIN":
                            none |= ModifierKeys.Windows;
                            break;

                        case string.Empty:
                            return none;

                        default:
                            throw new NotSupportedException(System.Windows.SR.Get("Unsupported_Modifier", new object[] { str }));
                    }
                    modifiersToken = modifiersToken.Substring(length + 1);
                }
                while (length != -1);
            }
            return none;
        }

        public static bool IsDefinedModifierKeys(ModifierKeys modifierKeys)
        {
            if (modifierKeys != ModifierKeys.None)
            {
                return ((modifierKeys & ~ModifierKeysFlag) == ModifierKeys.None);
            }
            return true;
        }

        internal static string MatchModifiers(ModifierKeys modifierKeys)
        {
            string str = string.Empty;
            switch (modifierKeys)
            {
                case ModifierKeys.Alt:
                    return "Alt";

                case ModifierKeys.Control:
                    return "Ctrl";

                case (ModifierKeys.Control | ModifierKeys.Alt):
                    return str;

                case ModifierKeys.Shift:
                    return "Shift";

                case ModifierKeys.Windows:
                    return "Windows";
            }
            return str;
        }
    }
}

