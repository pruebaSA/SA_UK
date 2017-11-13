namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Fields;
    using MigraDoc.RtfRendering.Resources;
    using System;
    using System.Diagnostics;
    using System.Globalization;

    internal class DateFieldRenderer : FieldRenderer
    {
        private DateField dateField;

        internal DateFieldRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.dateField = domObj as DateField;
        }

        protected override string GetFieldResult() => 
            DateTime.Now.ToString(this.dateField.Format);

        internal override void Render()
        {
            base.StartField();
            base.rtfWriter.WriteText("DATE ");
            this.TranslateFormat();
            base.EndField();
        }

        private string TranslateCustomFormatChar(char ch)
        {
            switch (ch)
            {
                case 'H':
                case 'M':
                case 'd':
                case 's':
                case 'y':
                case 'h':
                case 'm':
                    return ch.ToString();
            }
            return ("'" + ch + "'");
        }

        private void TranslateFormat()
        {
            string format = this.dateField.Format;
            string shortDatePattern = format;
            DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            if (format == "")
            {
                shortDatePattern = dateTimeFormat.ShortDatePattern + " " + dateTimeFormat.LongTimePattern;
            }
            else if (format.Length == 1)
            {
                switch (format)
                {
                    case "d":
                        shortDatePattern = dateTimeFormat.ShortDatePattern;
                        break;

                    case "D":
                        shortDatePattern = dateTimeFormat.LongDatePattern;
                        break;

                    case "T":
                        shortDatePattern = dateTimeFormat.LongTimePattern;
                        break;

                    case "t":
                        shortDatePattern = dateTimeFormat.ShortTimePattern;
                        break;

                    case "f":
                        shortDatePattern = dateTimeFormat.LongDatePattern + " " + dateTimeFormat.ShortTimePattern;
                        break;

                    case "F":
                        shortDatePattern = dateTimeFormat.FullDateTimePattern;
                        break;

                    case "G":
                        shortDatePattern = dateTimeFormat.ShortDatePattern + " " + dateTimeFormat.LongTimePattern;
                        break;

                    case "g":
                        shortDatePattern = dateTimeFormat.ShortDatePattern + " " + dateTimeFormat.ShortTimePattern;
                        break;

                    case "M":
                    case "m":
                        shortDatePattern = dateTimeFormat.MonthDayPattern;
                        break;

                    case "R":
                    case "r":
                        shortDatePattern = dateTimeFormat.RFC1123Pattern;
                        break;

                    case "s":
                        shortDatePattern = dateTimeFormat.SortableDateTimePattern;
                        break;

                    case "u":
                        shortDatePattern = dateTimeFormat.UniversalSortableDateTimePattern;
                        break;

                    case "U":
                        shortDatePattern = dateTimeFormat.FullDateTimePattern;
                        break;

                    case "Y":
                    case "y":
                        shortDatePattern = dateTimeFormat.YearMonthPattern;
                        break;
                }
            }
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            string str3 = "\\@ \"";
            foreach (char ch in shortDatePattern)
            {
                char ch2 = ch;
                if (ch2 <= '\'')
                {
                    switch (ch2)
                    {
                        case '"':
                            goto Label_0376;

                        case '\'':
                            goto Label_0305;
                    }
                    goto Label_044C;
                }
                switch (ch2)
                {
                    case '/':
                    {
                        if ((!flag && !flag2) && !flag3)
                        {
                            goto Label_0408;
                        }
                        flag = false;
                        str3 = str3 + ch;
                        continue;
                    }
                    case ':':
                    {
                        if ((!flag && !flag2) && !flag3)
                        {
                            goto Label_043B;
                        }
                        flag = false;
                        str3 = str3 + ch;
                        continue;
                    }
                    default:
                    {
                        if (ch2 != '\\')
                        {
                            goto Label_044C;
                        }
                        if (flag)
                        {
                            object obj2 = str3;
                            str3 = string.Concat(new object[] { obj2, "'", '\\', "'" });
                        }
                        flag = !flag;
                        continue;
                    }
                }
            Label_0305:
                if (flag)
                {
                    Trace.WriteLine(Messages.CharacterNotAllowedInDateFormat(ch), "warning");
                    flag = false;
                }
                else if (!flag3 && !flag2)
                {
                    flag3 = true;
                    str3 = str3 + ch;
                }
                else if (flag2)
                {
                    str3 = str3 + @"\'";
                }
                else if (flag3)
                {
                    flag3 = false;
                    str3 = str3 + ch;
                }
                continue;
            Label_0376:
                if (flag)
                {
                    str3 = str3 + ch;
                    flag = false;
                }
                else if (!flag2 && !flag3)
                {
                    flag2 = true;
                    str3 = str3 + '\'';
                }
                else if (flag2)
                {
                    flag2 = false;
                    str3 = str3 + '\'';
                }
                else if (flag3)
                {
                    str3 = str3 + "\\\"";
                }
                continue;
            Label_0408:
                str3 = str3 + dateTimeFormat.DateSeparator;
                continue;
            Label_043B:
                str3 = str3 + dateTimeFormat.TimeSeparator;
                continue;
            Label_044C:
                if (flag)
                {
                    object obj3 = str3;
                    str3 = string.Concat(new object[] { obj3, "'", ch, "'" });
                }
                else if (!flag2 && !flag3)
                {
                    str3 = str3 + this.TranslateCustomFormatChar(ch);
                }
                else
                {
                    str3 = str3 + ch;
                }
                flag = false;
            }
            base.rtfWriter.WriteText(str3 + "\" \\* MERGEFORMAT");
        }
    }
}

