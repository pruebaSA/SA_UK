namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Text;

    public static class SqlHelpers
    {
        private static string EscapeLikeText(string text, char escape)
        {
            if ((!text.Contains("%") && !text.Contains("_")) && (!text.Contains("[") && !text.Contains("^")))
            {
                return text;
            }
            StringBuilder builder = new StringBuilder(text.Length);
            foreach (char ch in text)
            {
                if (((ch == '%') || (ch == '_')) || (((ch == '[') || (ch == '^')) || (ch == escape)))
                {
                    builder.Append(escape);
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string GetStringContainsPattern(string text, char escape)
        {
            if (text == null)
            {
                throw Error.ArgumentNull("text");
            }
            return ("%" + EscapeLikeText(text, escape) + "%");
        }

        public static string GetStringEndsWithPattern(string text, char escape)
        {
            if (text == null)
            {
                throw Error.ArgumentNull("text");
            }
            return ("%" + EscapeLikeText(text, escape));
        }

        public static string GetStringStartsWithPattern(string text, char escape)
        {
            if (text == null)
            {
                throw Error.ArgumentNull("text");
            }
            return (EscapeLikeText(text, escape) + "%");
        }

        public static string TranslateVBLikePattern(string pattern, char escape)
        {
            if (pattern == null)
            {
                throw Error.ArgumentNull("pattern");
            }
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            int num = 0;
            foreach (char ch in pattern)
            {
                if (!flag)
                {
                    goto Label_010C;
                }
                num++;
                if (flag3)
                {
                    if (ch != ']')
                    {
                        builder.Append('^');
                        flag3 = false;
                    }
                    else
                    {
                        builder.Append('!');
                        flag3 = false;
                    }
                }
                char ch2 = ch;
                if (ch2 != '!')
                {
                    switch (ch2)
                    {
                        case ']':
                        {
                            flag = false;
                            flag3 = false;
                            builder.Append(']');
                            continue;
                        }
                        case '^':
                            goto Label_00C9;

                        case '-':
                            goto Label_00B0;
                    }
                    goto Label_00E4;
                }
                if (num == 1)
                {
                    flag3 = true;
                }
                else
                {
                    builder.Append(ch);
                }
                continue;
            Label_00B0:
                if (flag2)
                {
                    throw Error.VbLikeDoesNotSupportMultipleCharacterRanges();
                }
                flag2 = true;
                builder.Append('-');
                continue;
            Label_00C9:
                if (num == 1)
                {
                    builder.Append(escape);
                }
                builder.Append(ch);
                continue;
            Label_00E4:
                if (ch == escape)
                {
                    builder.Append(escape);
                    builder.Append(escape);
                }
                else
                {
                    builder.Append(ch);
                }
                continue;
            Label_010C:
                switch (ch)
                {
                    case '?':
                    {
                        builder.Append('_');
                        continue;
                    }
                    case '[':
                    {
                        flag = true;
                        flag2 = false;
                        num = 0;
                        builder.Append('[');
                        continue;
                    }
                    case '_':
                    case '%':
                    {
                        builder.Append(escape);
                        builder.Append(ch);
                        continue;
                    }
                    case '#':
                    {
                        builder.Append("[0-9]");
                        continue;
                    }
                    case '*':
                    {
                        builder.Append('%');
                        continue;
                    }
                }
                if (ch == escape)
                {
                    builder.Append(escape);
                    builder.Append(escape);
                }
                else
                {
                    builder.Append(ch);
                }
            }
            if (flag)
            {
                throw Error.VbLikeUnclosedBracket();
            }
            return builder.ToString();
        }
    }
}

