namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp.Drawing;
    using PdfSharp.Internal;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;

    internal class FontData
    {
        internal CMapTable cmap;
        internal ControlValueTable cvt;
        private byte[] data;
        private static readonly int[] entrySelectors = new int[] { 
            0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4
        };
        internal FontTechnology fontTechnology;
        internal FontProgram fpgm;
        internal GlyphDataTable glyf;
        internal GlyphSubstitutionTable gsub;
        internal FontHeaderTable head;
        internal HorizontalHeaderTable hhea;
        internal HorizontalMetricsTable hmtx;
        internal IndexToLocationTable loca;
        internal MaximumProfileTable maxp;
        internal NameTable name;
        internal OffsetTable offsetTable;
        internal OS2Table os2;
        private int pos;
        internal PostScriptTable post;
        internal ControlValueProgram prep;
        internal Dictionary<string, TableDirectoryEntry> tableDictionary;
        internal VerticalHeaderTable vhea;
        internal VerticalMetricsTable vmtx;

        private FontData(FontData fontData)
        {
            this.tableDictionary = new Dictionary<string, TableDirectoryEntry>();
            this.offsetTable = fontData.offsetTable;
        }

        public FontData(byte[] data)
        {
            this.tableDictionary = new Dictionary<string, TableDirectoryEntry>();
            int length = data.Length;
            byte[] destinationArray = new byte[length];
            Array.Copy(data, destinationArray, length);
            this.data = destinationArray;
            this.Read();
        }

        public FontData(XFont font, XPdfFontOptions options)
        {
            this.tableDictionary = new Dictionary<string, TableDirectoryEntry>();
            this.CreateGdiFontImage(font, options);
            if (this.data == null)
            {
                throw new InvalidOperationException("Cannot allocate font data.");
            }
            this.Read();
        }

        public void AddTable(OpenTypeFontTable fontTable)
        {
            if (!this.CanWrite)
            {
                throw new InvalidOperationException("Font image cannot be modified.");
            }
            if (fontTable == null)
            {
                throw new ArgumentNullException("fontTable");
            }
            if (fontTable.fontData == null)
            {
                fontTable.fontData = this;
            }
            else
            {
                fontTable = new IRefFontTable(this, fontTable);
            }
            this.tableDictionary[fontTable.DirectoryEntry.Tag] = fontTable.DirectoryEntry;
            switch (fontTable.DirectoryEntry.Tag)
            {
                case "cmap":
                    this.cmap = fontTable as CMapTable;
                    return;

                case "cvt ":
                    this.cvt = fontTable as ControlValueTable;
                    return;

                case "fpgm":
                    this.fpgm = fontTable as FontProgram;
                    return;

                case "maxp":
                    this.maxp = fontTable as MaximumProfileTable;
                    return;

                case "name":
                    this.name = fontTable as NameTable;
                    return;

                case "head":
                    this.head = fontTable as FontHeaderTable;
                    return;

                case "hhea":
                    this.hhea = fontTable as HorizontalHeaderTable;
                    return;

                case "hmtx":
                    this.hmtx = fontTable as HorizontalMetricsTable;
                    return;

                case "OS/2":
                    this.os2 = fontTable as OS2Table;
                    return;

                case "post":
                    this.post = fontTable as PostScriptTable;
                    return;

                case "glyf":
                    this.glyf = fontTable as GlyphDataTable;
                    return;

                case "loca":
                    this.loca = fontTable as IndexToLocationTable;
                    return;

                case "GSUB":
                    this.gsub = fontTable as GlyphSubstitutionTable;
                    return;

                case "prep":
                    this.prep = fontTable as ControlValueProgram;
                    return;
            }
        }

        private void Compile()
        {
            MemoryStream stream = new MemoryStream();
            OpenTypeFontWriter writer = new OpenTypeFontWriter(stream);
            int count = this.tableDictionary.Count;
            int num2 = entrySelectors[count];
            this.offsetTable.Version = 0x10000;
            this.offsetTable.TableCount = count;
            this.offsetTable.SearchRange = (ushort) ((((int) 1) << num2) * 0x10);
            this.offsetTable.EntrySelector = (ushort) num2;
            this.offsetTable.RangeShift = (ushort) ((count - (((int) 1) << num2)) * 0x10);
            this.offsetTable.Write(writer);
            string[] array = new string[count];
            this.tableDictionary.Keys.CopyTo(array, 0);
            Array.Sort<string>(array, StringComparer.Ordinal);
            int position = 12 + (0x10 * count);
            for (int i = 0; i < count; i++)
            {
                TableDirectoryEntry entry = this.tableDictionary[array[i]];
                entry.FontTable.PrepareForCompilation();
                entry.Offset = position;
                writer.Position = position;
                entry.FontTable.Write(writer);
                position = writer.Position;
                writer.Position = 12 + (0x10 * i);
                entry.Write(writer);
            }
            writer.Stream.Flush();
            ((int) writer.Stream.Length).GetType();
            this.data = stream.ToArray();
        }

        public FontData CreateFontSubSet(Dictionary<int, object> glyphs, bool cidFont)
        {
            FontData data = new FontData(this);
            IndexToLocationTable fontTable = new IndexToLocationTable {
                ShortIndex = this.loca.ShortIndex
            };
            GlyphDataTable table2 = new GlyphDataTable();
            if (!cidFont)
            {
                data.AddTable(this.cmap);
            }
            if (this.cvt != null)
            {
                data.AddTable(this.cvt);
            }
            if (this.fpgm != null)
            {
                data.AddTable(this.fpgm);
            }
            data.AddTable(table2);
            data.AddTable(this.head);
            data.AddTable(this.hhea);
            data.AddTable(this.hmtx);
            data.AddTable(fontTable);
            if (this.maxp != null)
            {
                data.AddTable(this.maxp);
            }
            if (this.prep != null)
            {
                data.AddTable(this.prep);
            }
            this.glyf.CompleteGlyphClosure(glyphs);
            int count = glyphs.Count;
            int[] array = new int[count];
            glyphs.Keys.CopyTo(array, 0);
            Array.Sort<int>(array);
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                num2 += this.glyf.GetGlyphSize(array[i]);
            }
            table2.DirectoryEntry.Length = num2;
            int numGlyphs = this.maxp.numGlyphs;
            fontTable.locaTable = new int[numGlyphs + 1];
            table2.glyphTable = new byte[table2.DirectoryEntry.PaddedLength];
            int dstOffset = 0;
            int index = 0;
            for (int j = 0; j < numGlyphs; j++)
            {
                fontTable.locaTable[j] = dstOffset;
                if ((index < count) && (array[index] == j))
                {
                    index++;
                    byte[] glyphData = this.glyf.GetGlyphData(j);
                    int length = glyphData.Length;
                    if (length > 0)
                    {
                        Buffer.BlockCopy(glyphData, 0, table2.glyphTable, dstOffset, length);
                        dstOffset += length;
                    }
                }
            }
            fontTable.locaTable[numGlyphs] = dstOffset;
            data.Compile();
            return data;
        }

        private void CreateGdiFontImage(XFont font, XPdfFontOptions options)
        {
            Font font2 = font.RealizeGdiFont();
            this.data = null;
            if (this.data == null)
            {
                IntPtr hgdiobj = font2.ToHfont();
                IntPtr dC = PdfSharp.Internal.NativeMethods.GetDC(IntPtr.Zero);
                int num = Marshal.GetLastWin32Error();
                IntPtr ptr3 = PdfSharp.Internal.NativeMethods.SelectObject(dC, hgdiobj);
                num = Marshal.GetLastWin32Error();
                int num2 = PdfSharp.Internal.NativeMethods.GetFontData(dC, 0, 0, null, 0);
                num = Marshal.GetLastWin32Error();
                if (num2 > 0)
                {
                    this.data = new byte[num2];
                    PdfSharp.Internal.NativeMethods.GetFontData(dC, 0, 0, this.data, this.data.Length);
                    PdfSharp.Internal.NativeMethods.SelectObject(dC, ptr3);
                    PdfSharp.Internal.NativeMethods.ReleaseDC(IntPtr.Zero, dC);
                    num.GetType();
                }
                else
                {
                    PdfSharp.Internal.NativeMethods.SelectObject(dC, ptr3);
                    PdfSharp.Internal.NativeMethods.ReleaseDC(IntPtr.Zero, dC);
                    PdfSharp.Internal.NativeMethods.LOGFONT logFont = new PdfSharp.Internal.NativeMethods.LOGFONT();
                    font2.ToLogFont(logFont);
                    logFont.lfHeight++;
                    IntPtr ptr4 = PdfSharp.Internal.NativeMethods.CreateFontIndirect(logFont);
                    dC = PdfSharp.Internal.NativeMethods.GetDC(IntPtr.Zero);
                    num = Marshal.GetLastWin32Error();
                    ptr3 = PdfSharp.Internal.NativeMethods.SelectObject(dC, ptr4);
                    num = Marshal.GetLastWin32Error();
                    num2 = PdfSharp.Internal.NativeMethods.GetFontData(dC, 0, 0, null, 0);
                    num = Marshal.GetLastWin32Error();
                    if (num2 > 0)
                    {
                        this.data = new byte[num2];
                        PdfSharp.Internal.NativeMethods.GetFontData(dC, 0, 0, this.data, this.data.Length);
                    }
                    PdfSharp.Internal.NativeMethods.SelectObject(dC, ptr3);
                    PdfSharp.Internal.NativeMethods.ReleaseDC(IntPtr.Zero, dC);
                    PdfSharp.Internal.NativeMethods.DeleteObject(ptr4);
                    num.GetType();
                }
            }
            if (this.data == null)
            {
                throw new InvalidOperationException("Internal error. Font data could not retrieved.");
            }
        }

        internal void Read()
        {
            try
            {
                this.offsetTable.Version = this.ReadULong();
                this.offsetTable.TableCount = this.ReadUShort();
                this.offsetTable.SearchRange = this.ReadUShort();
                this.offsetTable.EntrySelector = this.ReadUShort();
                this.offsetTable.RangeShift = this.ReadUShort();
                if (this.offsetTable.Version == 0x74746366)
                {
                    this.fontTechnology = FontTechnology.TrueTypeCollection;
                    throw new InvalidOperationException("TrueType collection fonts are not supported by PDFsharp.");
                }
                if (this.offsetTable.Version == 0x4f54544f)
                {
                    this.fontTechnology = FontTechnology.PostscriptOutlines;
                }
                else
                {
                    this.fontTechnology = FontTechnology.TrueTypeOutlines;
                }
                for (int i = 0; i < this.offsetTable.TableCount; i++)
                {
                    TableDirectoryEntry entry = TableDirectoryEntry.ReadFrom(this);
                    this.tableDictionary.Add(entry.Tag, entry);
                }
                if (this.tableDictionary.ContainsKey("bhed"))
                {
                    throw new NotSupportedException("Bitmap fonts are not supported by PDFsharp.");
                }
                if (this.Seek("cmap") != -1)
                {
                    this.cmap = new CMapTable(this);
                }
                if (this.Seek("cvt ") != -1)
                {
                    this.cvt = new ControlValueTable(this);
                }
                if (this.Seek("fpgm") != -1)
                {
                    this.fpgm = new FontProgram(this);
                }
                if (this.Seek("maxp") != -1)
                {
                    this.maxp = new MaximumProfileTable(this);
                }
                if (this.Seek("name") != -1)
                {
                    this.name = new NameTable(this);
                }
                if (this.Seek("head") != -1)
                {
                    this.head = new FontHeaderTable(this);
                }
                if (this.Seek("hhea") != -1)
                {
                    this.hhea = new HorizontalHeaderTable(this);
                }
                if (this.Seek("hmtx") != -1)
                {
                    this.hmtx = new HorizontalMetricsTable(this);
                }
                if (this.Seek("OS/2") != -1)
                {
                    this.os2 = new OS2Table(this);
                }
                if (this.Seek("post") != -1)
                {
                    this.post = new PostScriptTable(this);
                }
                if (this.Seek("glyf") != -1)
                {
                    this.glyf = new GlyphDataTable(this);
                }
                if (this.Seek("loca") != -1)
                {
                    this.loca = new IndexToLocationTable(this);
                }
                if (this.Seek("GSUB") != -1)
                {
                    this.gsub = new GlyphSubstitutionTable(this);
                }
                if (this.Seek("prep") != -1)
                {
                    this.prep = new ControlValueProgram(this);
                }
            }
            catch (Exception)
            {
                base.GetType();
                throw;
            }
        }

        public void Read(byte[] buffer)
        {
            this.Read(buffer, 0, buffer.Length);
        }

        public void Read(byte[] buffer, int offset, int length)
        {
            Buffer.BlockCopy(this.data, this.pos, buffer, offset, length);
            this.pos += length;
        }

        public byte ReadByte() => 
            this.data[this.pos++];

        public byte[] ReadBytes(int size)
        {
            byte[] buffer = new byte[size];
            for (int i = 0; i < size; i++)
            {
                buffer[i] = this.data[this.pos++];
            }
            return buffer;
        }

        public int ReadFixed()
        {
            int pos = this.pos;
            this.pos += 4;
            return ((((this.data[pos] << 0x18) | (this.data[pos + 1] << 0x10)) | (this.data[pos + 2] << 8)) | this.data[pos + 3]);
        }

        public short ReadFWord()
        {
            int pos = this.pos;
            this.pos += 2;
            return (short) ((this.data[pos] << 8) | this.data[pos + 1]);
        }

        public int ReadLong()
        {
            int pos = this.pos;
            this.pos += 4;
            return ((((this.data[pos] << 0x18) | (this.data[pos + 1] << 0x10)) | (this.data[pos + 2] << 8)) | this.data[pos + 3]);
        }

        public long ReadLongDate()
        {
            int pos = this.pos;
            this.pos += 8;
            return (long) ((((((((this.data[pos] << 0x38) | (this.data[pos + 1] << 0x30)) | (this.data[pos + 2] << 40)) | (this.data[pos + 3] << 0x20)) | (this.data[pos + 4] << 0x18)) | (this.data[pos + 5] << 0x10)) | (this.data[pos + 6] << 8)) | this.data[pos + 7]);
        }

        public short ReadShort()
        {
            int pos = this.pos;
            this.pos += 2;
            return (short) ((this.data[pos] << 8) | this.data[pos + 1]);
        }

        public string ReadString(int size)
        {
            char[] chArray = new char[size];
            for (int i = 0; i < size; i++)
            {
                chArray[i] = (char) this.data[this.pos++];
            }
            return new string(chArray);
        }

        public string ReadTag() => 
            this.ReadString(4);

        public ushort ReadUFWord()
        {
            int pos = this.pos;
            this.pos += 2;
            return (ushort) ((this.data[pos] << 8) | this.data[pos + 1]);
        }

        public uint ReadULong()
        {
            int pos = this.pos;
            this.pos += 4;
            return (uint) ((((this.data[pos] << 0x18) | (this.data[pos + 1] << 0x10)) | (this.data[pos + 2] << 8)) | this.data[pos + 3]);
        }

        public ushort ReadUShort()
        {
            int pos = this.pos;
            this.pos += 2;
            return (ushort) ((this.data[pos] << 8) | this.data[pos + 1]);
        }

        public int Seek(string tag)
        {
            if (this.tableDictionary.ContainsKey(tag))
            {
                this.pos = this.tableDictionary[tag].Offset;
                return this.pos;
            }
            return -1;
        }

        public int SeekOffset(int offset)
        {
            this.pos += offset;
            return this.pos;
        }

        public bool CanRead =>
            (this.data != null);

        public bool CanWrite =>
            (this.data == null);

        public byte[] Data =>
            this.data;

        public int Position
        {
            get => 
                this.pos;
            set
            {
                this.pos = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct OffsetTable
        {
            public uint Version;
            public int TableCount;
            public ushort SearchRange;
            public ushort EntrySelector;
            public ushort RangeShift;
            public void Write(OpenTypeFontWriter writer)
            {
                writer.WriteUInt(this.Version);
                writer.WriteShort(this.TableCount);
                writer.WriteUShort(this.SearchRange);
                writer.WriteUShort(this.EntrySelector);
                writer.WriteUShort(this.RangeShift);
            }
        }
    }
}

