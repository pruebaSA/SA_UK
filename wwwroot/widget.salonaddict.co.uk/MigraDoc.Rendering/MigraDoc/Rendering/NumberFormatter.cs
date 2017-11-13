namespace MigraDoc.Rendering
{
    using MigraDoc.Rendering.Resources;
    using System;
    using System.Diagnostics;

    internal class NumberFormatter
    {
        private static string AsLetters(int number, bool lowercase)
        {
            char ch;
            if (Math.Abs(number) > 0x8000)
            {
                Trace.WriteLine(Messages.NumberTooLargeForLetters(number));
                return number.ToString();
            }
            if (number == 0)
            {
                return "0";
            }
            string str = "";
            if (number < 0)
            {
                str = str + "-";
            }
            number = Math.Abs(number);
            if (lowercase)
            {
                ch = (char) (0x61 + ((number - 1) % 0x1a));
            }
            else
            {
                ch = (char) (0x41 + ((number - 1) % 0x1a));
            }
            for (int i = 0; i <= ((number - 1) / 0x1a); i++)
            {
                str = str + ch;
            }
            return str;
        }

        private static string AsRoman(int number, bool lowercase)
        {
            string[] strArray;
            if (Math.Abs(number) > 0x8000)
            {
                Trace.WriteLine(Messages.NumberTooLargeForRoman(number), "warning");
                return number.ToString();
            }
            if (number == 0)
            {
                return "0";
            }
            string str = "";
            if (number < 0)
            {
                str = str + "-";
            }
            number = Math.Abs(number);
            if (lowercase)
            {
                strArray = new string[] { "m", "cm", "d", "cd", "c", "xc", "l", "xl", "x", "ix", "v", "iv", "i" };
            }
            else
            {
                strArray = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            }
            int[] numArray = new int[] { 0x3e8, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            for (int i = 0; i < numArray.Length; i++)
            {
                while (number >= numArray[i])
                {
                    str = str + strArray[i];
                    number -= numArray[i];
                }
            }
            return str;
        }

        internal static string Format(int number, string format)
        {
            switch (format)
            {
                case "ROMAN":
                    return AsRoman(number, false);

                case "roman":
                    return AsRoman(number, true);

                case "ALPHABETIC":
                    return AsLetters(number, false);

                case "alphabetic":
                    return AsLetters(number, true);
            }
            return number.ToString();
        }
    }
}

