namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Text;

    internal class NumberFormatterBase
    {
        private const string cjkIdeographic = "〇一二三四五六七八九";
        private const string hiraganaAiueo = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん";
        private const string hiraganaIroha = "いろはにほへとちりぬるをわかよたれそつねならむうゐのおくやまけふこえてあさきゆめみしゑひもせす";
        private const string katakanaAiueo = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン";
        private const string katakanaAiueoHw = "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｦﾝ";
        private const string katakanaIroha = "イロハニホヘトチリヌルヲワカヨタレソツネナラムウヰノオクヤマケフコエテアサキユメミシヱヒモセスン";
        private const string katakanaIrohaHw = "ｲﾛﾊﾆﾎﾍﾄﾁﾘﾇﾙｦﾜｶﾖﾀﾚｿﾂﾈﾅﾗﾑｳヰﾉｵｸﾔﾏｹﾌｺｴﾃｱｻｷﾕﾒﾐｼヱﾋﾓｾｽﾝ";
        private const int MaxAlphabeticLength = 7;
        protected const int MaxAlphabeticValue = 0x7fffffff;
        protected const int MaxRomanValue = 0x7fff;
        private const string RomanDigitsLC = "iivixxlxccdcm";
        private const string RomanDigitsUC = "IIVIXXLXCCDCM";
        private static readonly int[] RomanDigitValue = new int[] { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 0x3e8 };

        public static void ConvertToAlphabetic(StringBuilder sb, double val, char firstChar, int totalChars)
        {
            char[] chArray = new char[7];
            int startIndex = 7;
            int num2 = (int) val;
            while (num2 > totalChars)
            {
                int num3 = --num2 / totalChars;
                chArray[--startIndex] = (char) (firstChar + (num2 - (num3 * totalChars)));
                num2 = num3;
            }
            chArray[--startIndex] = (char) (firstChar + --num2);
            sb.Append(chArray, startIndex, 7 - startIndex);
        }

        public static void ConvertToRoman(StringBuilder sb, double val, bool upperCase)
        {
            int num = (int) val;
            string str = upperCase ? "IIVIXXLXCCDCM" : "iivixxlxccdcm";
            int length = RomanDigitValue.Length;
            while (length-- != 0)
            {
                while (num >= RomanDigitValue[length])
                {
                    num -= RomanDigitValue[length];
                    sb.Append(str, length, 1 + (length & 1));
                }
            }
        }
    }
}

