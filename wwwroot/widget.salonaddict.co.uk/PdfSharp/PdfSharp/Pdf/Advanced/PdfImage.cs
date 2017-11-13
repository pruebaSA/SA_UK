namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Filters;
    using System;
    using System.Drawing.Imaging;
    using System.IO;

    public sealed class PdfImage : PdfXObject
    {
        internal static readonly uint[] BlackMakeUpCodes = new uint[] { 
            15, 10, 200, 12, 0xc9, 12, 0x5b, 12, 0x33, 12, 0x34, 12, 0x35, 12, 0x6c, 13,
            0x6d, 13, 0x4a, 13, 0x4b, 13, 0x4c, 13, 0x4d, 13, 0x72, 13, 0x73, 13, 0x74, 13,
            0x75, 13, 0x76, 13, 0x77, 13, 0x52, 13, 0x53, 13, 0x54, 13, 0x55, 13, 90, 13,
            0x5b, 13, 100, 13, 0x65, 13, 8, 11, 12, 11, 13, 11, 0x12, 12, 0x13, 12,
            20, 12, 0x15, 12, 0x16, 12, 0x17, 12, 0x1c, 12, 0x1d, 12, 30, 12, 0x1f, 12,
            1, 12
        };
        internal static readonly uint[] BlackTerminatingCodes = new uint[] { 
            0x37, 10, 2, 3, 3, 2, 2, 2, 3, 3, 3, 4, 2, 4, 3, 5,
            5, 6, 4, 6, 4, 7, 5, 7, 7, 7, 4, 8, 7, 8, 0x18, 9,
            0x17, 10, 0x18, 10, 8, 10, 0x67, 11, 0x68, 11, 0x6c, 11, 0x37, 11, 40, 11,
            0x17, 11, 0x18, 11, 0xca, 12, 0xcb, 12, 0xcc, 12, 0xcd, 12, 0x68, 12, 0x69, 12,
            0x6a, 12, 0x6b, 12, 210, 12, 0xd3, 12, 0xd4, 12, 0xd5, 12, 0xd6, 12, 0xd7, 12,
            0x6c, 12, 0x6d, 12, 0xda, 12, 0xdb, 12, 0x54, 12, 0x55, 12, 0x56, 12, 0x57, 12,
            100, 12, 0x65, 12, 0x52, 12, 0x53, 12, 0x24, 12, 0x37, 12, 0x38, 12, 0x27, 12,
            40, 12, 0x58, 12, 0x59, 12, 0x2b, 12, 0x2c, 12, 90, 12, 0x66, 12, 0x67, 12
        };
        internal static readonly uint[] HorizontalCodes = new uint[] { 1, 3 };
        private readonly XImage image;
        private static readonly uint[] OneRuns = new uint[] { 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 8
        };
        internal static readonly uint[] PassCodes = new uint[] { 1, 4 };
        internal static readonly uint[] VerticalCodes = new uint[] { 3, 7, 3, 6, 3, 3, 1, 1, 2, 3, 2, 6, 2, 7 };
        internal static readonly uint[] WhiteMakeUpCodes = new uint[] { 
            0x1b, 5, 0x12, 5, 0x17, 6, 0x37, 7, 0x36, 8, 0x37, 8, 100, 8, 0x65, 8,
            0x68, 8, 0x67, 8, 0xcc, 9, 0xcd, 9, 210, 9, 0xd3, 9, 0xd4, 9, 0xd5, 9,
            0xd6, 9, 0xd7, 9, 0xd8, 9, 0xd9, 9, 0xda, 9, 0xdb, 9, 0x98, 9, 0x99, 9,
            0x9a, 9, 0x18, 6, 0x9b, 9, 8, 11, 12, 11, 13, 11, 0x12, 12, 0x13, 12,
            20, 12, 0x15, 12, 0x16, 12, 0x17, 12, 0x1c, 12, 0x1d, 12, 30, 12, 0x1f, 12,
            1, 12
        };
        internal static readonly uint[] WhiteTerminatingCodes = new uint[] { 
            0x35, 8, 7, 6, 7, 4, 8, 4, 11, 4, 12, 4, 14, 4, 15, 4,
            0x13, 5, 20, 5, 7, 5, 8, 5, 8, 6, 3, 6, 0x34, 6, 0x35, 6,
            0x2a, 6, 0x2b, 6, 0x27, 7, 12, 7, 8, 7, 0x17, 7, 3, 7, 4, 7,
            40, 7, 0x2b, 7, 0x13, 7, 0x24, 7, 0x18, 7, 2, 8, 3, 8, 0x1a, 8,
            0x1b, 8, 0x12, 8, 0x13, 8, 20, 8, 0x15, 8, 0x16, 8, 0x17, 8, 40, 8,
            0x29, 8, 0x2a, 8, 0x2b, 8, 0x2c, 8, 0x2d, 8, 4, 8, 5, 8, 10, 8,
            11, 8, 0x52, 8, 0x53, 8, 0x54, 8, 0x55, 8, 0x24, 8, 0x25, 8, 0x58, 8,
            0x59, 8, 90, 8, 0x5b, 8, 0x4a, 8, 0x4b, 8, 50, 8, 0x33, 8, 0x34, 8
        };
        private static readonly uint[] ZeroRuns = new uint[] { 
            8, 7, 6, 6, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4,
            3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        public PdfImage(PdfDocument document, XImage image) : base(document)
        {
            base.Elements.SetName("/Type", "/XObject");
            base.Elements.SetName("/Subtype", "/Image");
            this.image = image;
            switch (this.image.Format.Guid.ToString("B").ToUpper())
            {
                case "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}":
                    this.InitializeJpeg();
                    return;

                case "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}":
                case "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}":
                case "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}":
                case "{B96B3CB5-0728-11D3-9D7B-0000F81EF32E}":
                    this.InitializeNonJpeg();
                    break;

                case "{84570158-DBF0-4C6B-8368-62D6A3CA76E0}":
                    break;

                default:
                    return;
            }
        }

        private static uint CountOneBits(BitReader reader, uint bitsLeft)
        {
            uint num = 0;
            while (true)
            {
                uint num2;
                int index = reader.PeekByte(out num2);
                uint bits = OneRuns[index];
                if (bits < num2)
                {
                    if (bits > 0)
                    {
                        reader.SkipBits(bits);
                    }
                    return (num + bits);
                }
                num += num2;
                if (num >= bitsLeft)
                {
                    return bitsLeft;
                }
                reader.NextByte();
            }
        }

        private static uint CountZeroBits(BitReader reader, uint bitsLeft)
        {
            uint num = 0;
            while (true)
            {
                uint num2;
                int index = reader.PeekByte(out num2);
                uint bits = ZeroRuns[index];
                if (bits < num2)
                {
                    if (bits > 0)
                    {
                        reader.SkipBits(bits);
                    }
                    return (num + bits);
                }
                num += num2;
                if (num >= bitsLeft)
                {
                    return bitsLeft;
                }
                reader.NextByte();
            }
        }

        private static int DoFaxEncoding(ref byte[] imageData, byte[] imageBits, uint bytesFileOffset, uint width, uint height)
        {
            try
            {
                uint num = ((width + 0x1f) / 0x20) * 4;
                BitWriter writer = new BitWriter(ref imageData);
                for (uint i = 0; i < height; i++)
                {
                    uint num3 = bytesFileOffset + (((height - 1) - i) * num);
                    BitReader reader = new BitReader(imageBits, num3, width);
                    uint num4 = 0;
                    while (num4 < width)
                    {
                        uint count = CountOneBits(reader, width - num4);
                        WriteSample(writer, count, true);
                        num4 += count;
                        if (num4 < width)
                        {
                            uint num6 = CountZeroBits(reader, width - num4);
                            WriteSample(writer, num6, false);
                            num4 += num6;
                        }
                    }
                }
                writer.FlushBuffer();
                return writer.BytesWritten();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static int DoFaxEncodingGroup4(ref byte[] imageData, byte[] imageBits, uint bytesFileOffset, uint width, uint height)
        {
            try
            {
                uint bytesPerLineBmp = ((width + 0x1f) / 0x20) * 4;
                BitWriter writer = new BitWriter(ref imageData);
                for (uint i = 0; i < height; i++)
                {
                    FaxEncode2DRow(writer, bytesFileOffset, imageBits, i, (i != 0) ? (i - 1) : uint.MaxValue, width, height, bytesPerLineBmp);
                }
                writer.FlushBuffer();
                return writer.BytesWritten();
            }
            catch (Exception exception)
            {
                exception.GetType();
                return 0;
            }
        }

        private static void FaxEncode2DRow(BitWriter writer, uint bytesFileOffset, byte[] imageBits, uint currentRow, uint referenceRow, uint width, uint height, uint bytesPerLineBmp)
        {
            BitReader reader2;
            uint num = bytesFileOffset + (((height - 1) - currentRow) * bytesPerLineBmp);
            BitReader reader = new BitReader(imageBits, num, width);
            if (referenceRow != uint.MaxValue)
            {
                uint num2 = bytesFileOffset + (((height - 1) - referenceRow) * bytesPerLineBmp);
                reader2 = new BitReader(imageBits, num2, width);
            }
            else
            {
                byte[] buffer = new byte[bytesPerLineBmp];
                for (int i = 0; i < bytesPerLineBmp; i++)
                {
                    buffer[i] = 0xff;
                }
                reader2 = new BitReader(buffer, 0, width);
            }
            uint position = 0;
            uint bitStart = !reader.GetBit(0) ? 0 : FindDifference(reader, 0, width, true);
            uint num6 = !reader2.GetBit(0) ? 0 : FindDifference(reader2, 0, width, true);
            while (true)
            {
                uint num8 = FindDifferenceWithCheck(reader2, num6, width, reader2.GetBit(num6));
                if (num8 >= bitStart)
                {
                    int num9 = (int) (num6 - bitStart);
                    if ((-3 > num9) || (num9 > 3))
                    {
                        uint num7 = FindDifferenceWithCheck(reader, bitStart, width, reader.GetBit(bitStart));
                        writer.WriteTableLine(HorizontalCodes, 0);
                        if (((position + bitStart) == 0) || reader.GetBit(position))
                        {
                            WriteSample(writer, bitStart - position, true);
                            WriteSample(writer, num7 - bitStart, false);
                        }
                        else
                        {
                            WriteSample(writer, bitStart - position, false);
                            WriteSample(writer, num7 - bitStart, true);
                        }
                        position = num7;
                    }
                    else
                    {
                        writer.WriteTableLine(VerticalCodes, (uint) (num9 + 3));
                        position = bitStart;
                    }
                }
                else
                {
                    writer.WriteTableLine(PassCodes, 0);
                    position = num8;
                }
                if (position >= width)
                {
                    return;
                }
                bool bit = reader.GetBit(position);
                bitStart = FindDifference(reader, position, width, bit);
                num6 = FindDifference(reader2, position, width, !bit);
                num6 = FindDifferenceWithCheck(reader2, num6, width, bit);
            }
        }

        private static uint FindDifference(BitReader reader, uint bitStart, uint bitEnd, bool searchOne)
        {
            reader.SetPosition(bitStart);
            return (bitStart + (searchOne ? CountOneBits(reader, bitEnd - bitStart) : CountZeroBits(reader, bitEnd - bitStart)));
        }

        private static uint FindDifferenceWithCheck(BitReader reader, uint bitStart, uint bitEnd, bool searchOne)
        {
            if (bitStart >= bitEnd)
            {
                return bitEnd;
            }
            return FindDifference(reader, bitStart, bitEnd, searchOne);
        }

        private void InitializeJpeg()
        {
            byte[] buffer = null;
            int count = 0;
            MemoryStream stream = new MemoryStream();
            this.image.gdiImage.Save(stream, ImageFormat.Jpeg);
            int length = (int) stream.Length;
            if (buffer == null)
            {
                count = (int) stream.Length;
                buffer = new byte[count];
                stream.Seek(0L, SeekOrigin.Begin);
                stream.Read(buffer, 0, count);
                stream.Close();
            }
            byte[] buffer2 = new FlateDecode().Encode(buffer);
            if (buffer2.Length < buffer.Length)
            {
                base.Stream = new PdfDictionary.PdfStream(buffer2, this);
                base.Elements["/Length"] = new PdfInteger(buffer2.Length);
                PdfArray array = new PdfArray(base.document) {
                    Elements = { 
                        new PdfName("/FlateDecode"),
                        new PdfName("/DCTDecode")
                    }
                };
                base.Elements["/Filter"] = array;
            }
            else
            {
                base.Stream = new PdfDictionary.PdfStream(buffer, this);
                base.Elements["/Length"] = new PdfInteger(count);
                base.Elements["/Filter"] = new PdfName("/DCTDecode");
            }
            base.Elements["/Width"] = new PdfInteger(this.image.PixelWidth);
            base.Elements["/Height"] = new PdfInteger(this.image.PixelHeight);
            base.Elements["/BitsPerComponent"] = new PdfInteger(8);
            if ((this.image.gdiImage.Flags & 0x120) != 0)
            {
                base.Elements["/ColorSpace"] = new PdfName("/DeviceCMYK");
                if ((this.image.gdiImage.Flags & 0x100) != 0)
                {
                    base.Elements["/Decode"] = new PdfLiteral("[1 0 1 0 1 0 1 0]");
                }
            }
            else if ((this.image.gdiImage.Flags & 0x40) != 0)
            {
                base.Elements["/ColorSpace"] = new PdfName("/DeviceGray");
            }
            else
            {
                base.Elements["/ColorSpace"] = new PdfName("/DeviceRGB");
            }
        }

        private void InitializeNonJpeg()
        {
            switch (this.image.gdiImage.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    this.ReadTrueColorMemoryBitmap(3, 8, false);
                    return;

                case PixelFormat.Format32bppRgb:
                    this.ReadTrueColorMemoryBitmap(4, 8, false);
                    return;

                case PixelFormat.Format1bppIndexed:
                    this.ReadIndexedMemoryBitmap(1);
                    return;

                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppArgb:
                    this.ReadTrueColorMemoryBitmap(3, 8, true);
                    return;

                case PixelFormat.Format4bppIndexed:
                    this.ReadIndexedMemoryBitmap(4);
                    return;

                case PixelFormat.Format8bppIndexed:
                    this.ReadIndexedMemoryBitmap(8);
                    return;
            }
            throw new NotImplementedException("Image format not supported.");
        }

        private static int ReadDWord(byte[] ab, int offset) => 
            (ReadWord(ab, offset) + (0x10000 * ReadWord(ab, offset + 2)));

        private void ReadIndexedMemoryBitmap(int bits)
        {
            int version = this.Owner.Version;
            int num2 = -1;
            int num3 = -1;
            bool flag = false;
            MemoryStream stream = new MemoryStream();
            this.image.gdiImage.Save(stream, ImageFormat.Bmp);
            int length = (int) stream.Length;
            if (length > 0)
            {
                byte[] buffer = new byte[length];
                stream.Seek(0L, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);
                stream.Close();
                int pixelHeight = this.image.PixelHeight;
                int pixelWidth = this.image.PixelWidth;
                if (((ReadWord(buffer, 0) != 0x4d42) || (ReadDWord(buffer, 2) != length)) || (((ReadDWord(buffer, 14) != 40) || (ReadDWord(buffer, 0x12) != pixelWidth)) || (ReadDWord(buffer, 0x16) != pixelHeight)))
                {
                    throw new NotImplementedException("ReadIndexedMemoryBitmap: unsupported format");
                }
                int num7 = ReadWord(buffer, 0x1c);
                if ((num7 != bits) && (((num7 == 1) || (num7 == 4)) || (num7 == 8)))
                {
                    bits = num7;
                }
                if (((ReadWord(buffer, 0x1a) != 1) || (ReadWord(buffer, 0x1c) != bits)) || (ReadDWord(buffer, 30) != 0))
                {
                    throw new NotImplementedException("ReadIndexedMemoryBitmap: unsupported format #2");
                }
                int num8 = ReadDWord(buffer, 10);
                int num9 = 0x36;
                int num10 = ReadDWord(buffer, 0x2e);
                if (((num8 - num9) / 4) != num10)
                {
                    throw new NotImplementedException("ReadIndexedMemoryBitmap: unsupported format #3");
                }
                MonochromeMask mask = new MonochromeMask(pixelWidth, pixelHeight);
                bool flag2 = (bits == 8) && ((num10 == 0x100) || (num10 == 0));
                int num11 = 0;
                byte[] data = new byte[3 * num10];
                for (int i = 0; i < num10; i++)
                {
                    data[3 * i] = buffer[(num9 + (4 * i)) + 2];
                    data[(3 * i) + 1] = buffer[(num9 + (4 * i)) + 1];
                    data[(3 * i) + 2] = buffer[num9 + (4 * i)];
                    if (flag2)
                    {
                        flag2 = (data[3 * i] == data[(3 * i) + 1]) && (data[3 * i] == data[(3 * i) + 2]);
                    }
                    if (buffer[(num9 + (4 * i)) + 3] < 0x80)
                    {
                        if (num2 == -1)
                        {
                            num2 = i;
                        }
                        if ((num3 == -1) || (num3 == (i - 1)))
                        {
                            num3 = i;
                        }
                        if (num3 != i)
                        {
                            flag = true;
                        }
                    }
                }
                if (bits == 1)
                {
                    switch (num10)
                    {
                        case 0:
                            num11 = 1;
                            break;

                        case 2:
                            if ((((data[0] == 0) && (data[1] == 0)) && ((data[2] == 0) && (data[3] == 0xff))) && ((data[4] == 0xff) && (data[5] == 0xff)))
                            {
                                num11 = 1;
                            }
                            if ((((data[5] == 0) && (data[4] == 0)) && ((data[3] == 0) && (data[2] == 0xff))) && ((data[1] == 0xff) && (data[0] == 0xff)))
                            {
                                num11 = -1;
                            }
                            break;
                    }
                }
                FlateDecode decode = new FlateDecode();
                PdfDictionary dictionary = null;
                if ((num11 == 0) && !flag2)
                {
                    dictionary = new PdfDictionary(base.document);
                    byte[] buffer3 = (data.Length >= 0x30) ? decode.Encode(data) : null;
                    if ((buffer3 != null) && ((buffer3.Length + 20) < data.Length))
                    {
                        dictionary.CreateStream(buffer3);
                        dictionary.Elements["/Length"] = new PdfInteger(buffer3.Length);
                        dictionary.Elements["/Filter"] = new PdfName("/FlateDecode");
                    }
                    else
                    {
                        dictionary.CreateStream(data);
                        dictionary.Elements["/Length"] = new PdfInteger(data.Length);
                    }
                    this.Owner.irefTable.Add(dictionary);
                }
                bool flag3 = false;
                byte[] buffer4 = new byte[(((pixelWidth * bits) + 7) / 8) * pixelHeight];
                byte[] buffer5 = null;
                int num13 = 0;
                if (bits == 1)
                {
                    byte[] imageData = new byte[buffer4.Length];
                    int newSize = DoFaxEncodingGroup4(ref imageData, buffer, (uint) num8, (uint) pixelWidth, (uint) pixelHeight);
                    flag3 = newSize > 0;
                    if (flag3)
                    {
                        if (newSize == 0)
                        {
                            newSize = 0x7fffffff;
                        }
                        Array.Resize<byte>(ref imageData, newSize);
                        buffer5 = imageData;
                        num13 = -1;
                    }
                }
                int num15 = 0;
                if (((bits != 8) && (bits != 4)) && (bits != 1))
                {
                    throw new NotImplementedException("ReadIndexedMemoryBitmap: unsupported format #3");
                }
                int num16 = ((pixelWidth * bits) + 7) / 8;
                for (int j = 0; j < pixelHeight; j++)
                {
                    mask.StartLine(j);
                    int index = ((pixelHeight - 1) - j) * (((pixelWidth * bits) + 7) / 8);
                    for (int k = 0; k < num16; k++)
                    {
                        if (flag2)
                        {
                            buffer4[index] = data[3 * buffer[num8 + num15]];
                        }
                        else
                        {
                            buffer4[index] = buffer[num8 + num15];
                        }
                        if (num2 != -1)
                        {
                            int num20 = buffer[num8 + num15];
                            if (bits == 8)
                            {
                                mask.AddPel((num20 >= num2) && (num20 <= num3));
                            }
                            else if (bits == 4)
                            {
                                int num21 = (num20 & 240) / 0x10;
                                int num22 = num20 & 15;
                                mask.AddPel((num21 >= num2) && (num21 <= num3));
                                mask.AddPel((num22 >= num2) && (num22 <= num3));
                            }
                            else if (bits == 1)
                            {
                                for (int m = 1; m <= 8; m++)
                                {
                                    int num24 = (num20 & 0x80) / 0x80;
                                    mask.AddPel((num24 >= num2) && (num24 <= num3));
                                    num20 *= 2;
                                }
                            }
                        }
                        num15++;
                        index++;
                    }
                    num15 = 4 * ((num15 + 3) / 4);
                }
                if ((num2 != -1) && (num3 != -1))
                {
                    if (!flag && (version >= 13))
                    {
                        PdfArray array = new PdfArray(base.document) {
                            Elements = { 
                                new PdfInteger(num2),
                                new PdfInteger(num3)
                            }
                        };
                        base.Elements["/Mask"] = array;
                    }
                    else
                    {
                        byte[] buffer7 = decode.Encode(mask.MaskData);
                        PdfDictionary dictionary2 = new PdfDictionary(base.document);
                        dictionary2.Elements.SetName("/Type", "/XObject");
                        dictionary2.Elements.SetName("/Subtype", "/Image");
                        this.Owner.irefTable.Add(dictionary2);
                        dictionary2.Stream = new PdfDictionary.PdfStream(buffer7, dictionary2);
                        dictionary2.Elements["/Length"] = new PdfInteger(buffer7.Length);
                        dictionary2.Elements["/Filter"] = new PdfName("/FlateDecode");
                        dictionary2.Elements["/Width"] = new PdfInteger(pixelWidth);
                        dictionary2.Elements["/Height"] = new PdfInteger(pixelHeight);
                        dictionary2.Elements["/BitsPerComponent"] = new PdfInteger(1);
                        dictionary2.Elements["/ImageMask"] = new PdfBoolean(true);
                        base.Elements["/Mask"] = dictionary2.Reference;
                    }
                }
                byte[] buffer8 = decode.Encode(buffer4);
                byte[] buffer9 = flag3 ? decode.Encode(buffer5) : null;
                bool flag4 = false;
                if (flag3 && ((buffer5.Length < buffer8.Length) || (buffer9.Length < buffer8.Length)))
                {
                    flag4 = true;
                    if (buffer5.Length < buffer8.Length)
                    {
                        base.Stream = new PdfDictionary.PdfStream(buffer5, this);
                        base.Elements["/Length"] = new PdfInteger(buffer5.Length);
                        base.Elements["/Filter"] = new PdfName("/CCITTFaxDecode");
                        PdfDictionary dictionary3 = new PdfDictionary();
                        if (num13 != 0)
                        {
                            dictionary3.Elements.Add("/K", new PdfInteger(num13));
                        }
                        if (num11 < 0)
                        {
                            dictionary3.Elements.Add("/BlackIs1", new PdfBoolean(true));
                        }
                        dictionary3.Elements.Add("/EndOfBlock", new PdfBoolean(false));
                        dictionary3.Elements.Add("/Columns", new PdfInteger(pixelWidth));
                        dictionary3.Elements.Add("/Rows", new PdfInteger(pixelHeight));
                        base.Elements["/DecodeParms"] = dictionary3;
                    }
                    else
                    {
                        base.Stream = new PdfDictionary.PdfStream(buffer9, this);
                        base.Elements["/Length"] = new PdfInteger(buffer9.Length);
                        PdfArray array2 = new PdfArray(base.document) {
                            Elements = { 
                                new PdfName("/FlateDecode"),
                                new PdfName("/CCITTFaxDecode")
                            }
                        };
                        base.Elements["/Filter"] = array2;
                        PdfArray array3 = new PdfArray(base.document);
                        PdfDictionary dictionary4 = new PdfDictionary();
                        PdfDictionary dictionary5 = new PdfDictionary();
                        if (num13 != 0)
                        {
                            dictionary5.Elements.Add("/K", new PdfInteger(num13));
                        }
                        if (num11 < 0)
                        {
                            dictionary5.Elements.Add("/BlackIs1", new PdfBoolean(true));
                        }
                        dictionary5.Elements.Add("/EndOfBlock", new PdfBoolean(false));
                        dictionary5.Elements.Add("/Columns", new PdfInteger(pixelWidth));
                        dictionary5.Elements.Add("/Rows", new PdfInteger(pixelHeight));
                        array3.Elements.Add(dictionary4);
                        array3.Elements.Add(dictionary5);
                        base.Elements["/DecodeParms"] = array3;
                    }
                }
                else
                {
                    base.Stream = new PdfDictionary.PdfStream(buffer8, this);
                    base.Elements["/Length"] = new PdfInteger(buffer8.Length);
                    base.Elements["/Filter"] = new PdfName("/FlateDecode");
                }
                base.Elements["/Width"] = new PdfInteger(pixelWidth);
                base.Elements["/Height"] = new PdfInteger(pixelHeight);
                base.Elements["/BitsPerComponent"] = new PdfInteger(bits);
                if ((flag4 && (num11 == 0)) || ((!flag4 && (num11 <= 0)) && !flag2))
                {
                    PdfArray array4 = new PdfArray(base.document) {
                        Elements = { 
                            new PdfName("/Indexed"),
                            new PdfName("/DeviceRGB"),
                            new PdfInteger(num10 - 1),
                            dictionary.Reference
                        }
                    };
                    base.Elements["/ColorSpace"] = array4;
                }
                else
                {
                    base.Elements["/ColorSpace"] = new PdfName("/DeviceGray");
                }
                if (this.image.Interpolate)
                {
                    base.Elements["/Interpolate"] = PdfBoolean.True;
                }
            }
        }

        private void ReadTrueColorMemoryBitmap(int components, int bits, bool hasAlpha)
        {
            int version = this.Owner.Version;
            MemoryStream stream = new MemoryStream();
            this.image.gdiImage.Save(stream, ImageFormat.Bmp);
            int length = (int) stream.Length;
            if (length > 0)
            {
                byte[] buffer = new byte[length];
                stream.Seek(0L, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);
                stream.Close();
                int pixelHeight = this.image.PixelHeight;
                int pixelWidth = this.image.PixelWidth;
                if (((ReadWord(buffer, 0) != 0x4d42) || (ReadDWord(buffer, 2) != length)) || (((ReadDWord(buffer, 14) != 40) || (ReadDWord(buffer, 0x12) != pixelWidth)) || (ReadDWord(buffer, 0x16) != pixelHeight)))
                {
                    throw new NotImplementedException("ReadTrueColorMemoryBitmap: unsupported format");
                }
                if (((ReadWord(buffer, 0x1a) != 1) || (!hasAlpha && (ReadWord(buffer, 0x1c) != (components * bits)))) || ((hasAlpha && (ReadWord(buffer, 0x1c) != ((components + 1) * bits))) || (ReadDWord(buffer, 30) != 0)))
                {
                    throw new NotImplementedException("ReadTrueColorMemoryBitmap: unsupported format #2");
                }
                int num5 = ReadDWord(buffer, 10);
                int num6 = components;
                if (components == 4)
                {
                    num6 = 3;
                }
                byte[] data = new byte[(components * pixelWidth) * pixelHeight];
                bool flag = false;
                bool flag2 = false;
                byte[] buffer3 = hasAlpha ? new byte[pixelWidth * pixelHeight] : null;
                MonochromeMask mask = hasAlpha ? new MonochromeMask(pixelWidth, pixelHeight) : null;
                int num7 = 0;
                if (num6 == 3)
                {
                    for (int i = 0; i < pixelHeight; i++)
                    {
                        int index = (3 * ((pixelHeight - 1) - i)) * pixelWidth;
                        int num10 = 0;
                        if (hasAlpha)
                        {
                            mask.StartLine(i);
                            num10 = ((pixelHeight - 1) - i) * pixelWidth;
                        }
                        for (int j = 0; j < pixelWidth; j++)
                        {
                            data[index] = buffer[(num5 + num7) + 2];
                            data[index + 1] = buffer[(num5 + num7) + 1];
                            data[index + 2] = buffer[num5 + num7];
                            if (hasAlpha)
                            {
                                mask.AddPel(buffer[(num5 + num7) + 3]);
                                buffer3[num10] = buffer[(num5 + num7) + 3];
                                if ((!flag || !flag2) && (buffer[(num5 + num7) + 3] != 0xff))
                                {
                                    flag = true;
                                    if (buffer[(num5 + num7) + 3] != 0)
                                    {
                                        flag2 = true;
                                    }
                                }
                                num10++;
                            }
                            num7 += hasAlpha ? 4 : components;
                            index += 3;
                        }
                        num7 = 4 * ((num7 + 3) / 4);
                    }
                }
                else if (components == 1)
                {
                    throw new NotImplementedException("Image format not supported (grayscales).");
                }
                FlateDecode decode = new FlateDecode();
                if (flag)
                {
                    byte[] buffer4 = decode.Encode(mask.MaskData);
                    PdfDictionary dictionary = new PdfDictionary(base.document);
                    dictionary.Elements.SetName("/Type", "/XObject");
                    dictionary.Elements.SetName("/Subtype", "/Image");
                    this.Owner.irefTable.Add(dictionary);
                    dictionary.Stream = new PdfDictionary.PdfStream(buffer4, dictionary);
                    dictionary.Elements["/Length"] = new PdfInteger(buffer4.Length);
                    dictionary.Elements["/Filter"] = new PdfName("/FlateDecode");
                    dictionary.Elements["/Width"] = new PdfInteger(pixelWidth);
                    dictionary.Elements["/Height"] = new PdfInteger(pixelHeight);
                    dictionary.Elements["/BitsPerComponent"] = new PdfInteger(1);
                    dictionary.Elements["/ImageMask"] = new PdfBoolean(true);
                    base.Elements["/Mask"] = dictionary.Reference;
                }
                if ((flag && flag2) && (version >= 14))
                {
                    byte[] buffer5 = decode.Encode(buffer3);
                    PdfDictionary dictionary2 = new PdfDictionary(base.document);
                    dictionary2.Elements.SetName("/Type", "/XObject");
                    dictionary2.Elements.SetName("/Subtype", "/Image");
                    this.Owner.irefTable.Add(dictionary2);
                    dictionary2.Stream = new PdfDictionary.PdfStream(buffer5, dictionary2);
                    dictionary2.Elements["/Length"] = new PdfInteger(buffer5.Length);
                    dictionary2.Elements["/Filter"] = new PdfName("/FlateDecode");
                    dictionary2.Elements["/Width"] = new PdfInteger(pixelWidth);
                    dictionary2.Elements["/Height"] = new PdfInteger(pixelHeight);
                    dictionary2.Elements["/BitsPerComponent"] = new PdfInteger(8);
                    dictionary2.Elements["/ColorSpace"] = new PdfName("/DeviceGray");
                    base.Elements["/SMask"] = dictionary2.Reference;
                }
                byte[] buffer6 = decode.Encode(data);
                base.Stream = new PdfDictionary.PdfStream(buffer6, this);
                base.Elements["/Length"] = new PdfInteger(buffer6.Length);
                base.Elements["/Filter"] = new PdfName("/FlateDecode");
                base.Elements["/Width"] = new PdfInteger(pixelWidth);
                base.Elements["/Height"] = new PdfInteger(pixelHeight);
                base.Elements["/BitsPerComponent"] = new PdfInteger(8);
                base.Elements["/ColorSpace"] = new PdfName("/DeviceRGB");
                if (this.image.Interpolate)
                {
                    base.Elements["/Interpolate"] = PdfBoolean.True;
                }
            }
        }

        private static int ReadWord(byte[] ab, int offset) => 
            (ab[offset] + (0x100 * ab[offset + 1]));

        public override string ToString() => 
            "Image";

        private static void WriteSample(BitWriter writer, uint count, bool white)
        {
            uint[] table = white ? WhiteTerminatingCodes : BlackTerminatingCodes;
            uint[] numArray2 = white ? WhiteMakeUpCodes : BlackMakeUpCodes;
            while (count >= 0xa40)
            {
                writer.WriteTableLine(numArray2, 0x27);
                count -= 0xa00;
            }
            if (count > 0x3f)
            {
                uint line = (count / 0x40) - 1;
                writer.WriteTableLine(numArray2, line);
                count -= (line + 1) * 0x40;
            }
            writer.WriteTableLine(table, count);
        }

        public XImage Image =>
            this.image;

        public sealed class Keys : PdfXObject.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Alternates = "/Alternates";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string BitsPerComponent = "/BitsPerComponent";
            [KeyInfo(KeyType.Required | KeyType.NameOrArray)]
            public const string ColorSpace = "/ColorSpace";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Decode = "/Decode";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string Height = "/Height";
            [KeyInfo(KeyType.Optional | KeyType.String)]
            public const string ID = "/ID";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string ImageMask = "/ImageMask";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Intent = "/Intent";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string Interpolate = "/Interpolate";
            [KeyInfo(KeyType.Optional | KeyType.StreamOrArray)]
            public const string Mask = "/Mask";
            [KeyInfo(KeyType.Optional | KeyType.Stream)]
            public const string Metadata = "/Metadata";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Name = "/Name";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string OC = "/OC";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string OPI = "/OPI";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string SMask = "/SMask";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string SMaskInData = "/SMaskInData";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string StructParent = "/StructParent";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Subtype = "/Subtype";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Type = "/Type";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string Width = "/Width";
        }
    }
}

