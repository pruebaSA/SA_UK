namespace PdfSharp.Fonts
{
    using PdfSharp.Fonts.OpenType;
    using PdfSharp.Pdf.Internal;
    using System;
    using System.Collections.Generic;

    internal class CMapInfo
    {
        public Dictionary<char, int> CharacterToGlyphIndex = new Dictionary<char, int>();
        internal OpenTypeDescriptor descriptor;
        public Dictionary<int, object> GlyphIndices = new Dictionary<int, object>();
        public char MaxChar;
        public char MinChar = 0xffff;

        public CMapInfo(OpenTypeDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        internal void AddAnsiChars()
        {
            byte[] bytes = new byte[0xe0];
            for (int i = 0; i < 0xe0; i++)
            {
                bytes[i] = (byte) (i + 0x20);
            }
            string text = PdfEncoders.WinAnsiEncoding.GetString(bytes, 0, bytes.Length);
            this.AddChars(text);
        }

        public void AddChars(string text)
        {
            if (text != null)
            {
                bool symbol = this.descriptor.fontData.cmap.symbol;
                int length = text.Length;
                for (int i = 0; i < length; i++)
                {
                    char key = text[i];
                    if (!this.CharacterToGlyphIndex.ContainsKey(key))
                    {
                        int num3 = 0;
                        if (this.descriptor != null)
                        {
                            if (symbol)
                            {
                                num3 = key + (this.descriptor.fontData.os2.usFirstCharIndex & 0xff00);
                                num3 = this.descriptor.CharCodeToGlyphIndex((char) num3);
                            }
                            else
                            {
                                num3 = this.descriptor.CharCodeToGlyphIndex(key);
                            }
                        }
                        this.CharacterToGlyphIndex.Add(key, num3);
                        this.GlyphIndices[num3] = null;
                        this.MinChar = (char) Math.Min((ushort) this.MinChar, (ushort) key);
                        this.MaxChar = (char) Math.Max((ushort) this.MaxChar, (ushort) key);
                    }
                }
            }
        }

        public void AddGlyphIndices(string glyphIndices)
        {
            if (glyphIndices != null)
            {
                int length = glyphIndices.Length;
                for (int i = 0; i < length; i++)
                {
                    int num3 = glyphIndices[i];
                    this.GlyphIndices[num3] = null;
                }
            }
        }

        internal bool Contains(char ch) => 
            this.CharacterToGlyphIndex.ContainsKey(ch);

        public int[] GetGlyphIndices()
        {
            int[] array = new int[this.GlyphIndices.Count];
            this.GlyphIndices.Keys.CopyTo(array, 0);
            Array.Sort<int>(array);
            return array;
        }

        public char[] Chars
        {
            get
            {
                char[] array = new char[this.CharacterToGlyphIndex.Count];
                this.CharacterToGlyphIndex.Keys.CopyTo(array, 0);
                Array.Sort<char>(array);
                return array;
            }
        }
    }
}

