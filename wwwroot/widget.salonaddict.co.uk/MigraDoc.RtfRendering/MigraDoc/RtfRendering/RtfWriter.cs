namespace MigraDoc.RtfRendering
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal class RtfWriter
    {
        private bool lastWasControl;
        private TextWriter textWriter;

        public RtfWriter(TextWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        public void EndContent()
        {
            this.textWriter.Write("}");
            this.lastWasControl = false;
        }

        private static bool IsCp1252Char(char ch)
        {
            if (ch < '\x00ff')
            {
                return true;
            }
            switch (ch)
            {
                case '\x008d':
                case '\x008f':
                case '\x0090':
                case '\x009d':
                case '\x0081':
                case 'Ž':
                case 'ž':
                case 'Ÿ':
                case 'Œ':
                case 'œ':
                case 'Š':
                case 'š':
                case '–':
                case '—':
                case '‘':
                case '’':
                case '‚':
                case '“':
                case '”':
                case '„':
                case '†':
                case '‡':
                case '•':
                case '…':
                case '˜':
                case 'ƒ':
                case 'ˆ':
                case '€':
                case '™':
                case '‹':
                case '›':
                case '‰':
                    return true;
            }
            return false;
        }

        public void StartContent()
        {
            this.textWriter.Write("{");
            this.lastWasControl = false;
        }

        public void WriteBlank()
        {
            this.textWriter.Write(" ");
        }

        public void WriteControl(string ctrl)
        {
            this.WriteControl(ctrl, "");
        }

        public void WriteControl(string ctrl, bool withStar)
        {
            if (!withStar)
            {
                this.WriteControl(ctrl);
            }
            else
            {
                this.WriteControlWithStar(ctrl);
            }
        }

        public void WriteControl(string ctrl, int value)
        {
            this.WriteControl(ctrl, value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteControl(string ctrl, string value)
        {
            this.textWriter.Write(@"\" + ctrl + value);
            this.lastWasControl = true;
        }

        public void WriteControl(string ctrl, int value, bool withStar)
        {
            this.WriteControl(ctrl, value.ToString(CultureInfo.InvariantCulture), withStar);
        }

        public void WriteControl(string ctrl, string value, bool withStar)
        {
            if (withStar)
            {
                this.WriteControlWithStar(ctrl, value);
            }
            else
            {
                this.WriteControl(ctrl, value);
            }
        }

        public void WriteControlWithStar(string ctrl)
        {
            this.WriteControlWithStar(ctrl, "");
        }

        public void WriteControlWithStar(string ctrl, int value)
        {
            this.WriteControlWithStar(ctrl, value.ToString(CultureInfo.InvariantCulture));
        }

        public void WriteControlWithStar(string ctrl, string value)
        {
            this.textWriter.Write(@"\*\" + ctrl + value);
            this.lastWasControl = true;
        }

        public void WriteHex(uint hex)
        {
            if (hex <= 0xff)
            {
                this.textWriter.Write(@"\'" + hex.ToString("x"));
                this.lastWasControl = false;
            }
        }

        public void WriteSeparator()
        {
            this.textWriter.Write(";");
            this.lastWasControl = false;
        }

        public void WriteText(string text)
        {
            StringBuilder builder = new StringBuilder(text.Length);
            if (this.lastWasControl)
            {
                builder.Append(" ");
            }
            int length = text.Length;
            for (int i = 0; i < length; i++)
            {
                char ch = text[i];
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
                    case '\x00ad':
                    {
                        builder.Append(@"\-");
                        continue;
                    }
                    case '\\':
                    {
                        builder.Append(@"\\");
                        continue;
                    }
                }
                if (IsCp1252Char(ch))
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append(@"\u");
                    builder.Append(((int) ch).ToString(CultureInfo.InvariantCulture));
                    builder.Append('?');
                }
            }
            this.textWriter.Write(builder.ToString());
            this.lastWasControl = false;
        }
    }
}

