namespace AjaxControlToolkit
{
    using System;
    using System.Globalization;
    using System.Text;

    public static class MaskedEditCommon
    {
        private const string _charEscape = @"\";
        private const string _charNumbers = "0123456789";
        private const string _CharsEditMask = "9L$CAN?";

        public static string ConvertMask(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            string str = "";
            int startIndex = 0;
            for (startIndex = 0; startIndex < text.Length; startIndex++)
            {
                if ("9L$CAN?".IndexOf(text.Substring(startIndex, 1), StringComparison.Ordinal) != -1)
                {
                    if (builder2.Length == 0)
                    {
                        builder.Append(text.Substring(startIndex, 1));
                        builder2.Length = 0;
                        str = text.Substring(startIndex, 1);
                    }
                    else if (text.Substring(startIndex, 1) == "9")
                    {
                        builder2.Append("9");
                    }
                    else if (text.Substring(startIndex, 1) == "0")
                    {
                        builder2.Append("0");
                    }
                }
                else if ((("9L$CAN?".IndexOf(text.Substring(startIndex, 1), StringComparison.Ordinal) == -1) && (text.Substring(startIndex, 1) != "{")) && (text.Substring(startIndex, 1) != "}"))
                {
                    if (builder2.Length == 0)
                    {
                        builder.Append(text.Substring(startIndex, 1));
                        builder2.Length = 0;
                        str = "";
                    }
                    else if ("0123456789".IndexOf(text.Substring(startIndex, 1), StringComparison.Ordinal) != -1)
                    {
                        builder2.Append(text.Substring(startIndex, 1));
                    }
                }
                else if ((text.Substring(startIndex, 1) == "{") && (builder2.Length == 0))
                {
                    builder2.Length = 0;
                    builder2.Append("0");
                }
                else if ((text.Substring(startIndex, 1) == "}") && (builder2.Length != 0))
                {
                    int num2 = int.Parse(builder2.ToString(), CultureInfo.InvariantCulture) - 1;
                    if (num2 > 0)
                    {
                        int num3 = 0;
                        for (num3 = 0; num3 < num2; num3++)
                        {
                            builder.Append(str);
                        }
                    }
                    builder2.Length = 0;
                    builder2.Append("0");
                    str = "";
                }
            }
            return builder.ToString();
        }

        public static int GetFirstMaskPosition(string text)
        {
            bool flag = false;
            text = ConvertMask(text);
            for (int i = 0; i < text.Length; i++)
            {
                if ((text.Substring(i, 1) == @"\") && !flag)
                {
                    flag = true;
                }
                else
                {
                    if (("9L$CAN?".IndexOf(text.Substring(i, 1), StringComparison.Ordinal) != -1) && !flag)
                    {
                        return i;
                    }
                    if (flag)
                    {
                        flag = false;
                    }
                }
            }
            return -1;
        }

        public static int GetLastMaskPosition(string text)
        {
            bool flag = false;
            text = ConvertMask(text);
            int num = -1;
            for (int i = 0; i < text.Length; i++)
            {
                if ((text.Substring(i, 1) == @"\") && !flag)
                {
                    flag = true;
                }
                else if (("9L$CAN?".IndexOf(text.Substring(i, 1), StringComparison.Ordinal) != -1) && !flag)
                {
                    num = i;
                }
                else if (flag)
                {
                    flag = false;
                }
            }
            return num;
        }

        public static string GetValidMask(string text)
        {
            int firstMaskPosition = GetFirstMaskPosition(text);
            int lastMaskPosition = GetLastMaskPosition(text);
            text = ConvertMask(text);
            return text.Substring(firstMaskPosition, (lastMaskPosition - firstMaskPosition) + 1);
        }
    }
}

