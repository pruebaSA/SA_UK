namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Fonts;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Filters;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal sealed class PdfToUnicodeMap : PdfDictionary
    {
        private PdfSharp.Fonts.CMapInfo cmapInfo;

        public PdfToUnicodeMap(PdfDocument document) : base(document)
        {
        }

        public PdfToUnicodeMap(PdfDocument document, PdfSharp.Fonts.CMapInfo cmapInfo) : base(document)
        {
            this.cmapInfo = cmapInfo;
        }

        internal override void PrepareForSave()
        {
            base.PrepareForSave();
            string str = "/CIDInit /ProcSet findresource begin\n12 dict begin\nbegincmap\n/CIDSystemInfo << /Registry (Adobe)/Ordering (UCS)/Supplement 0>> def\n/CMapName /Adobe-Identity-UCS def /CMapType 2 def\n";
            string str2 = "endcmap CMapName currentdict /CMap defineresource pop end end";
            Dictionary<int, char> dictionary = new Dictionary<int, char>();
            int num = 0x10000;
            int num2 = -1;
            foreach (KeyValuePair<char, int> pair in this.cmapInfo.CharacterToGlyphIndex)
            {
                int num3 = pair.Value;
                num = Math.Min(num, num3);
                num2 = Math.Max(num2, num3);
                dictionary[num3] = pair.Key;
            }
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.ASCII);
            writer.Write(str);
            writer.WriteLine("1 begincodespacerange");
            writer.WriteLine($"<{num:X4}><{num2:X4}>");
            writer.WriteLine("endcodespacerange");
            writer.WriteLine($"{dictionary.Count} beginbfrange");
            foreach (KeyValuePair<int, char> pair2 in dictionary)
            {
                writer.WriteLine(string.Format("<{0:X4}><{0:X4}><{1:X4}>", pair2.Key, (int) pair2.Value));
            }
            writer.WriteLine("endbfrange");
            writer.Write(str2);
            writer.Close();
            byte[] data = stream.ToArray();
            stream.Close();
            if (this.Owner.Options.CompressContentStreams)
            {
                base.Elements.SetName("/Filter", "/FlateDecode");
                data = Filtering.FlateDecode.Encode(data);
            }
            base.CreateStream(data);
        }

        public PdfSharp.Fonts.CMapInfo CMapInfo
        {
            get => 
                this.cmapInfo;
            set
            {
                this.cmapInfo = value;
            }
        }

        public sealed class Keys : PdfDictionary.PdfStream.Keys
        {
        }
    }
}

