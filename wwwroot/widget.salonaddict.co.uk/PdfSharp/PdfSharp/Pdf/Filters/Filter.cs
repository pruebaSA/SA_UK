namespace PdfSharp.Pdf.Filters
{
    using PdfSharp.Pdf.Internal;
    using System;

    public abstract class Filter
    {
        protected Filter()
        {
        }

        public byte[] Decode(byte[] data) => 
            this.Decode(data, null);

        public abstract byte[] Decode(byte[] data, FilterParms parms);
        public string DecodeToString(byte[] data) => 
            this.DecodeToString(data, null);

        public virtual string DecodeToString(byte[] data, FilterParms parms)
        {
            byte[] bytes = this.Decode(data, parms);
            return PdfEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);
        }

        public abstract byte[] Encode(byte[] data);
        public virtual byte[] Encode(string rawString)
        {
            byte[] bytes = PdfEncoders.RawEncoding.GetBytes(rawString);
            return this.Encode(bytes);
        }

        protected byte[] RemoveWhiteSpace(byte[] data)
        {
            int length = data.Length;
            int index = 0;
            int num3 = 0;
            while (num3 < length)
            {
                switch (data[num3])
                {
                    case 9:
                    case 10:
                    case 12:
                    case 13:
                    case 0x20:
                    case 0:
                        index--;
                        break;

                    default:
                        if (num3 != index)
                        {
                            data[index] = data[num3];
                        }
                        break;
                }
                if (index < length)
                {
                    byte[] buffer = data;
                    data = new byte[index];
                    for (int i = 0; i < index; i++)
                    {
                        data[i] = buffer[i];
                    }
                }
                num3++;
                index++;
            }
            return data;
        }
    }
}

