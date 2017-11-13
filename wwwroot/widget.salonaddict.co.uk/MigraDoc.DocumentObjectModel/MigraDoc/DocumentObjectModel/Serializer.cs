namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal class Serializer
    {
        private bool[] commitTextStack;
        private bool fWriteStamp;
        protected int indent;
        private char lastChar;
        private int lineBreakBeyond;
        private int linePos;
        private int stackIdx;
        protected TextWriter textWriter;
        protected int writeIndent;

        internal Serializer(TextWriter textWriter) : this(textWriter, 2, 0)
        {
        }

        internal Serializer(TextWriter textWriter, int indent) : this(textWriter, indent, 0)
        {
        }

        internal Serializer(TextWriter textWriter, int indent, int initialIndent)
        {
            this.indent = 2;
            this.commitTextStack = new bool[0x20];
            this.lineBreakBeyond = 200;
            if (textWriter == null)
            {
                throw new ArgumentNullException("textWriter");
            }
            this.textWriter = textWriter;
            this.indent = indent;
            this.writeIndent = initialIndent;
            if (textWriter is StreamWriter)
            {
                this.WriteStamp();
            }
        }

        internal int BeginAttributes()
        {
            int position = this.Position;
            this.WriteLineNoCommit("[");
            this.IncreaseIndent();
            this.BeginBlock();
            return position;
        }

        internal int BeginAttributes(string str)
        {
            int position = this.Position;
            this.WriteLineNoCommit(str);
            this.WriteLineNoCommit("[");
            this.IncreaseIndent();
            this.BeginBlock();
            return position;
        }

        internal int BeginBlock()
        {
            int position = this.Position;
            if ((this.stackIdx + 1) >= this.commitTextStack.Length)
            {
                throw new ArgumentException("Block nesting level exhausted.");
            }
            this.stackIdx++;
            this.commitTextStack[this.stackIdx] = false;
            return position;
        }

        internal int BeginContent()
        {
            int position = this.Position;
            this.WriteLineNoCommit("{");
            this.IncreaseIndent();
            this.BeginBlock();
            return position;
        }

        internal int BeginContent(string str)
        {
            int position = this.Position;
            this.WriteLineNoCommit(str);
            this.WriteLineNoCommit("{");
            this.IncreaseIndent();
            this.BeginBlock();
            return position;
        }

        internal void CloseUpLine()
        {
            if (this.linePos > 0)
            {
                this.WriteLine();
            }
        }

        private void CommitText()
        {
            this.commitTextStack[this.stackIdx] = true;
        }

        private void DecreaseIndent()
        {
            this.writeIndent -= this.indent;
        }

        private string DoWordWrap(string str)
        {
            if ((str.Length + this.writeIndent) < this.lineBreakBeyond)
            {
                return str;
            }
            int index = str.IndexOf("\r\n");
            if ((index > 0) && ((index + this.writeIndent) <= this.lineBreakBeyond))
            {
                return str.Substring(0, index + 1);
            }
            int num2 = str.Substring(0, this.lineBreakBeyond - this.writeIndent).LastIndexOf(" ");
            int num3 = str.Substring(0, this.lineBreakBeyond - this.writeIndent).LastIndexOf("\r\n");
            int length = Math.Max(num2, num3);
            if (length == -1)
            {
                length = Math.Min(str.IndexOf(" ", (int) ((this.lineBreakBeyond - this.writeIndent) + 1)), str.IndexOf("\r\n", (int) ((this.lineBreakBeyond - this.writeIndent) + 1)));
            }
            if (length <= 0)
            {
                return str;
            }
            return str.Substring(0, length);
        }

        internal bool EndAttributes()
        {
            this.DecreaseIndent();
            this.WriteLineNoCommit("]");
            return this.EndBlock();
        }

        internal bool EndAttributes(int pos)
        {
            bool flag = this.EndAttributes();
            if (!flag)
            {
                this.Position = pos;
            }
            return flag;
        }

        internal bool EndBlock()
        {
            if (this.stackIdx <= 0)
            {
                throw new ArgumentException("Block nesting level underflow.");
            }
            this.stackIdx--;
            if (this.commitTextStack[this.stackIdx + 1])
            {
                this.commitTextStack[this.stackIdx] = this.commitTextStack[this.stackIdx + 1];
            }
            return this.commitTextStack[this.stackIdx + 1];
        }

        internal bool EndBlock(int pos)
        {
            bool flag = this.EndBlock();
            if (!flag)
            {
                this.Position = pos;
            }
            return flag;
        }

        internal bool EndContent()
        {
            this.DecreaseIndent();
            this.WriteLineNoCommit("}");
            return this.EndBlock();
        }

        internal bool EndContent(int pos)
        {
            bool flag = this.EndContent();
            if (!flag)
            {
                this.Position = pos;
            }
            return flag;
        }

        internal void Flush()
        {
            this.textWriter.Flush();
        }

        private void IncreaseIndent()
        {
            this.writeIndent += this.indent;
        }

        private static string Ind(int indent) => 
            new string(' ', indent);

        private bool IsBlankRequired(char left, char right)
        {
            if ((left == ' ') || (right == ' '))
            {
                return false;
            }
            bool flag = char.IsLetterOrDigit(left);
            bool flag2 = char.IsLetterOrDigit(right);
            return (flag && flag2);
        }

        internal void Write(string str)
        {
            string text = this.DoWordWrap(str);
            if ((text.Length < str.Length) && (text != ""))
            {
                this.WriteLineToStream(text);
                this.Write(str.Substring(text.Length));
            }
            else
            {
                this.WriteToStream(str);
            }
            this.CommitText();
        }

        internal void WriteComment(string comment)
        {
            if ((comment != null) && (comment != string.Empty))
            {
                int index = comment.IndexOf("\r\n");
                if (index != -1)
                {
                    this.WriteComment(comment.Substring(0, index));
                    this.WriteComment(comment.Substring(index + 2));
                }
                else
                {
                    int num2;
                    this.CloseUpLine();
                    int startIndex = (this.lineBreakBeyond - this.indent) - "// ".Length;
                    while ((num2 = comment.Length) > 0)
                    {
                        string str;
                        if (num2 <= startIndex)
                        {
                            str = "// " + comment;
                            comment = string.Empty;
                        }
                        else
                        {
                            int num4;
                            if (((num4 = comment.LastIndexOf(' ', startIndex)) == -1) && ((num4 = comment.IndexOf(' ', startIndex)) == -1))
                            {
                                str = "// " + comment;
                                comment = string.Empty;
                            }
                            else
                            {
                                str = "// " + comment.Substring(0, num4);
                                comment = comment.Substring(num4 + 1);
                            }
                        }
                        this.WriteLineToStream(str);
                        this.CommitText();
                    }
                }
            }
        }

        internal void WriteLine()
        {
            this.WriteLine(string.Empty);
        }

        internal void WriteLine(string str)
        {
            string text = this.DoWordWrap(str);
            if (text.Length < str.Length)
            {
                this.WriteLineToStream(text);
                this.WriteLine(str.Substring(text.Length));
            }
            else
            {
                this.WriteLineToStream(text);
            }
            this.CommitText();
        }

        internal void WriteLineNoCommit()
        {
            this.WriteLineNoCommit(string.Empty);
        }

        internal void WriteLineNoCommit(string str)
        {
            this.WriteLineToStream(str);
        }

        private void WriteLineToStream(string text)
        {
            this.WriteToStream(text, true, true);
        }

        internal void WriteSimpleAttribute(string valueName, object value)
        {
            INullableValue value2 = value as INullableValue;
            if (value2 != null)
            {
                value = value2.GetValue();
            }
            Type type = value.GetType();
            if (type == typeof(Unit))
            {
                string str = value.ToString();
                Unit unit = (Unit) value;
                if (unit.Type == UnitType.Point)
                {
                    this.WriteLine(valueName + " = " + str);
                }
                else
                {
                    this.WriteLine(valueName + " = \"" + str + "\"");
                }
            }
            else if (type == typeof(float))
            {
                this.WriteLine(valueName + " = " + ((float) value).ToString(CultureInfo.InvariantCulture));
            }
            else if (type == typeof(double))
            {
                this.WriteLine(valueName + " = " + ((double) value).ToString(CultureInfo.InvariantCulture));
            }
            else if (type == typeof(bool))
            {
                this.WriteLine(valueName + " = " + value.ToString().ToLower());
            }
            else if (type == typeof(string))
            {
                StringBuilder builder = new StringBuilder(value.ToString());
                builder.Replace(@"\", @"\\");
                builder.Replace("\"", "\\\"");
                this.WriteLine(valueName + " = \"" + builder.ToString() + "\"");
            }
            else if (((type == typeof(int)) || (type.BaseType == typeof(Enum))) || (type == typeof(Color)))
            {
                this.WriteLine(valueName + " = " + value.ToString());
            }
            else
            {
                $"Type '{type.ToString()}' of value '{valueName}' not supported";
            }
        }

        internal void WriteStamp()
        {
            if (this.fWriteStamp)
            {
                this.WriteComment("Created by empira MigraDoc Document Object Model");
                this.WriteComment(string.Format("generated file created {0:d} at {0:t}", DateTime.Now));
            }
        }

        private void WriteToStream(string text)
        {
            this.WriteToStream(text, false, true);
        }

        private void WriteToStream(string text, bool fLineBreak, bool fAutoIndent)
        {
            int index = text.IndexOf("\r\n");
            if (index != -1)
            {
                this.WriteToStream(text.Substring(0, index), true, fAutoIndent);
                this.WriteToStream(text.Substring(index + 2), fLineBreak, fAutoIndent);
            }
            else
            {
                int length = text.Length;
                if (length > 0)
                {
                    if ((this.linePos <= 0) && fAutoIndent)
                    {
                        text = this.Indentation + text;
                        length += this.writeIndent;
                    }
                    this.textWriter.Write(text);
                    this.linePos += length;
                    if (this.linePos > this.lineBreakBeyond)
                    {
                        fLineBreak = true;
                    }
                    else
                    {
                        this.lastChar = text[length - 1];
                    }
                }
                if (fLineBreak)
                {
                    this.textWriter.WriteLine(string.Empty);
                    this.linePos = 0;
                    this.lastChar = '\n';
                }
            }
        }

        internal int Indent
        {
            get => 
                this.indent;
            set
            {
                this.indent = value;
            }
        }

        private string Indentation =>
            Ind(this.writeIndent);

        internal int InitialIndent
        {
            get => 
                this.writeIndent;
            set
            {
                this.writeIndent = value;
            }
        }

        private int Position
        {
            get
            {
                this.textWriter.Flush();
                if (this.textWriter is StreamWriter)
                {
                    return (int) ((StreamWriter) this.textWriter).BaseStream.Position;
                }
                if (this.textWriter is StringWriter)
                {
                    return ((StringWriter) this.textWriter).GetStringBuilder().Length;
                }
                return 0;
            }
            set
            {
                this.textWriter.Flush();
                if (this.textWriter is StreamWriter)
                {
                    ((StreamWriter) this.textWriter).BaseStream.SetLength((long) value);
                }
                else if (this.textWriter is StringWriter)
                {
                    ((StringWriter) this.textWriter).GetStringBuilder().Length = value;
                }
            }
        }
    }
}

