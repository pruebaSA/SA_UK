namespace MigraDoc.DocumentObjectModel
{
    using System;
    using System.Text;

    public sealed class DdlEncoder
    {
        private DdlEncoder()
        {
        }

        internal static bool IsDdeIdentifier(string name)
        {
            if ((name == null) || (name == string.Empty))
            {
                return false;
            }
            int length = name.Length;
            if (length > 0x40)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                char c = name[i];
                if (c == ' ')
                {
                    return false;
                }
                if (i == 0)
                {
                    if (!char.IsLetter(c) && (c != '_'))
                    {
                        return false;
                    }
                }
                else if (!char.IsLetterOrDigit(c) && (c != '_'))
                {
                    return false;
                }
            }
            return true;
        }

        internal static string QuoteIfNameContainsBlanks(string name)
        {
            if (IsDdeIdentifier(name))
            {
                return name;
            }
            return ("\"" + name + "\"");
        }

        public static string StringToLiteral(string str)
        {
            int num = 0;
            if ((str == null) || ((num = str.Length) == 0))
            {
                return "\"\"";
            }
            StringBuilder builder = new StringBuilder(num + (num >> 2));
            builder.Append("\"");
            for (int i = 0; i < num; i++)
            {
                char ch = str[i];
                char ch2 = ch;
                if (ch2 != '"')
                {
                    if (ch2 != '\\')
                    {
                        goto Label_0063;
                    }
                    builder.Append(@"\\");
                }
                else
                {
                    builder.Append("\\\"");
                }
                continue;
            Label_0063:
                builder.Append(ch);
            }
            builder.Append("\"");
            return builder.ToString();
        }

        public static string StringToText(string str)
        {
            if (str == null)
            {
                return null;
            }
            int length = str.Length;
            StringBuilder builder = new StringBuilder(length + (length >> 2));
            for (int i = 0; i < length; i++)
            {
                char ch = str[i];
                switch (ch)
                {
                    case '{':
                    {
                        builder.Append(@"\{");
                        continue;
                    }
                    case '}':
                    {
                        builder.Append(@"\}");
                        continue;
                    }
                    case '\\':
                    {
                        builder.Append(@"\\");
                        continue;
                    }
                    case '/':
                    {
                        if ((i < (length - 1)) && (str[i + 1] == '/'))
                        {
                            builder.Append(@"\//");
                            i++;
                        }
                        else
                        {
                            builder.Append("/");
                        }
                        continue;
                    }
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}

