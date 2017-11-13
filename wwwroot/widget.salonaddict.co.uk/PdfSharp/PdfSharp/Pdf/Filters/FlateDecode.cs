namespace PdfSharp.Pdf.Filters
{
    using PdfSharp.SharpZipLib.Zip.Compression;
    using PdfSharp.SharpZipLib.Zip.Compression.Streams;
    using System;
    using System.IO;

    public class FlateDecode : PdfSharp.Pdf.Filters.Filter
    {
        public override byte[] Decode(byte[] data, FilterParms parms)
        {
            int num;
            MemoryStream baseInputStream = new MemoryStream(data);
            MemoryStream stream2 = new MemoryStream();
            InflaterInputStream stream3 = new InflaterInputStream(baseInputStream, new Inflater(false));
            byte[] buffer = new byte[0x8000];
            do
            {
                num = stream3.Read(buffer, 0, buffer.Length);
                if (num > 0)
                {
                    stream2.Write(buffer, 0, num);
                }
            }
            while (num > 0);
            stream3.Close();
            stream2.Flush();
            if (stream2.Length >= 0L)
            {
                stream2.Capacity = (int) stream2.Length;
                return stream2.GetBuffer();
            }
            return null;
        }

        public override byte[] Encode(byte[] data)
        {
            MemoryStream baseOutputStream = new MemoryStream();
            DeflaterOutputStream stream2 = new DeflaterOutputStream(baseOutputStream, new Deflater(Deflater.DEFAULT_COMPRESSION, false));
            stream2.Write(data, 0, data.Length);
            stream2.Finish();
            baseOutputStream.Capacity = (int) baseOutputStream.Length;
            return baseOutputStream.GetBuffer();
        }
    }
}

