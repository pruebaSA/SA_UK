namespace PdfSharp.Pdf.Filters
{
    using System;

    public class ASCIIHexDecode : PdfSharp.Pdf.Filters.Filter
    {
        public override byte[] Decode(byte[] data, FilterParms parms)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            data = base.RemoveWhiteSpace(data);
            int length = data.Length;
            if ((length % 2) == 1)
            {
                length++;
                byte[] buffer = data;
                data = new byte[length];
                buffer.CopyTo(data, 0);
            }
            length = length << 2;
            byte[] buffer2 = new byte[length];
            int index = 0;
            int num3 = 0;
            while (index < length)
            {
                byte num4 = data[num3++];
                byte num5 = data[num3++];
                buffer2[index] = (byte) ((((num4 > 0x39) ? (num4 - 0x41) : (num4 - 0x30)) * 0x10) + ((num5 > 0x39) ? (num5 - 0x41) : (num5 - 0x30)));
                index++;
            }
            return buffer2;
        }

        public override byte[] Encode(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            int length = data.Length;
            byte[] buffer = new byte[2 * length];
            int index = 0;
            int num3 = 0;
            while (index < length)
            {
                byte num4 = data[index];
                buffer[num3++] = (byte) ((num4 >> 4) + (((num4 >> 4) < 10) ? 0x30 : 0x37));
                buffer[num3++] = (byte) ((num4 & 15) + (((num4 & 15) < 10) ? 0x30 : 0x37));
                index++;
            }
            return buffer;
        }
    }
}

