namespace PdfSharp.Pdf.Filters
{
    using System;

    public class ASCII85Decode : PdfSharp.Pdf.Filters.Filter
    {
        public override byte[] Decode(byte[] data, FilterParms parms)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            int length = data.Length;
            int num3 = 0;
            int index = 0;
            int num = 0;
            while (num < length)
            {
                char ch = (char) data[num];
                if ((ch >= '!') && (ch <= 'u'))
                {
                    data[index++] = (byte) ch;
                }
                else if (ch == 'z')
                {
                    data[index++] = (byte) ch;
                    num3++;
                }
                else if (ch == '~')
                {
                    if (data[num + 1] != 0x3e)
                    {
                        throw new ArgumentException("Illegal character.", "data");
                    }
                    break;
                }
                num++;
            }
            if (num == length)
            {
                throw new ArgumentException("Illegal character.", "data");
            }
            length = index;
            int num5 = length - num3;
            int num6 = 4 * (num3 + (num5 / 5));
            int num7 = num5 % 5;
            if (num7 == 1)
            {
                throw new InvalidOperationException("Illegal character.");
            }
            if (num7 != 0)
            {
                num6 += num7 - 1;
            }
            byte[] buffer = new byte[num6];
            index = 0;
            num = 0;
            while ((num + 4) < length)
            {
                char ch2 = (char) data[num];
                if (ch2 == 'z')
                {
                    num++;
                    index += 4;
                }
                else
                {
                    uint num8 = (uint) ((((((data[num++] - 0x21) * 0x31c84b1) + ((data[num++] - 0x21) * 0x95eed)) + ((data[num++] - 0x21) * 0x1c39)) + ((data[num++] - 0x21) * 0x55)) + (data[num++] - 0x21));
                    buffer[index++] = (byte) (num8 >> 0x18);
                    buffer[index++] = (byte) (num8 >> 0x10);
                    buffer[index++] = (byte) (num8 >> 8);
                    buffer[index++] = (byte) num8;
                }
            }
            switch (num7)
            {
                case 2:
                {
                    uint num9 = (uint) (((data[num++] - 0x21) * 0x31c84b1) + ((data[num] - 0x21) * 0x95eed));
                    if (num9 != 0)
                    {
                        num9 += 0x1000000;
                    }
                    buffer[index] = (byte) (num9 >> 0x18);
                    return buffer;
                }
                case 3:
                {
                    int num10 = num;
                    uint num11 = (uint) ((((data[num++] - 0x21) * 0x31c84b1) + ((data[num++] - 0x21) * 0x95eed)) + ((data[num] - 0x21) * 0x1c39));
                    if (num11 != 0)
                    {
                        num11 &= 0xffff0000;
                        uint num12 = num11 / 0x1c39;
                        byte num13 = (byte) ((num12 % 0x55) + 0x21);
                        num12 /= 0x55;
                        byte num14 = (byte) ((num12 % 0x55) + 0x21);
                        num12 /= 0x55;
                        byte num15 = (byte) (num12 + 0x21);
                        if (((num15 != data[num10]) || (num14 != data[num10 + 1])) || (num13 != data[num10 + 2]))
                        {
                            num11 += 0x10000;
                        }
                    }
                    buffer[index++] = (byte) (num11 >> 0x18);
                    buffer[index] = (byte) (num11 >> 0x10);
                    return buffer;
                }
                case 4:
                {
                    int num16 = num;
                    uint num17 = (uint) (((((data[num++] - 0x21) * 0x31c84b1) + ((data[num++] - 0x21) * 0x95eed)) + ((data[num++] - 0x21) * 0x1c39)) + ((data[num] - 0x21) * 0x55));
                    if (num17 != 0)
                    {
                        num17 &= 0xffffff00;
                        uint num18 = num17 / 0x55;
                        byte num19 = (byte) ((num18 % 0x55) + 0x21);
                        num18 /= 0x55;
                        byte num20 = (byte) ((num18 % 0x55) + 0x21);
                        num18 /= 0x55;
                        byte num21 = (byte) ((num18 % 0x55) + 0x21);
                        num18 /= 0x55;
                        byte num22 = (byte) (num18 + 0x21);
                        if (((num22 != data[num16]) || (num21 != data[num16 + 1])) || ((num20 != data[num16 + 2]) || (num19 != data[num16 + 3])))
                        {
                            num17 += 0x100;
                        }
                    }
                    buffer[index++] = (byte) (num17 >> 0x18);
                    buffer[index++] = (byte) (num17 >> 0x10);
                    buffer[index] = (byte) (num17 >> 8);
                    break;
                }
            }
            return buffer;
        }

        public override byte[] Encode(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            int length = data.Length;
            int num2 = length / 4;
            int num3 = length - (num2 * 4);
            byte[] array = new byte[((num2 * 5) + ((num3 == 0) ? 0 : (num3 + 1))) + 2];
            int index = 0;
            int newSize = 0;
            for (int i = 0; i < num2; i++)
            {
                uint num7 = (uint) ((((data[index++] << 0x18) + (data[index++] << 0x10)) + (data[index++] << 8)) + data[index++]);
                if (num7 == 0)
                {
                    array[newSize++] = 0x7a;
                }
                else
                {
                    byte num8 = (byte) ((num7 % 0x55) + 0x21);
                    num7 /= 0x55;
                    byte num9 = (byte) ((num7 % 0x55) + 0x21);
                    num7 /= 0x55;
                    byte num10 = (byte) ((num7 % 0x55) + 0x21);
                    num7 /= 0x55;
                    byte num11 = (byte) ((num7 % 0x55) + 0x21);
                    num7 /= 0x55;
                    byte num12 = (byte) (num7 + 0x21);
                    array[newSize++] = num12;
                    array[newSize++] = num11;
                    array[newSize++] = num10;
                    array[newSize++] = num9;
                    array[newSize++] = num8;
                }
            }
            switch (num3)
            {
                case 1:
                {
                    uint num13 = (uint) (data[index] << 0x18);
                    num13 /= 0x95eed;
                    byte num14 = (byte) ((num13 % 0x55) + 0x21);
                    num13 /= 0x55;
                    byte num15 = (byte) (num13 + 0x21);
                    array[newSize++] = num15;
                    array[newSize++] = num14;
                    break;
                }
                case 2:
                {
                    uint num16 = (uint) ((data[index++] << 0x18) + (data[index] << 0x10));
                    num16 /= 0x1c39;
                    byte num17 = (byte) ((num16 % 0x55) + 0x21);
                    num16 /= 0x55;
                    byte num18 = (byte) ((num16 % 0x55) + 0x21);
                    num16 /= 0x55;
                    byte num19 = (byte) (num16 + 0x21);
                    array[newSize++] = num19;
                    array[newSize++] = num18;
                    array[newSize++] = num17;
                    break;
                }
                case 3:
                {
                    uint num20 = (uint) (((data[index++] << 0x18) + (data[index++] << 0x10)) + (data[index] << 8));
                    num20 /= 0x55;
                    byte num21 = (byte) ((num20 % 0x55) + 0x21);
                    num20 /= 0x55;
                    byte num22 = (byte) ((num20 % 0x55) + 0x21);
                    num20 /= 0x55;
                    byte num23 = (byte) ((num20 % 0x55) + 0x21);
                    num20 /= 0x55;
                    byte num24 = (byte) (num20 + 0x21);
                    array[newSize++] = num24;
                    array[newSize++] = num23;
                    array[newSize++] = num22;
                    array[newSize++] = num21;
                    break;
                }
            }
            array[newSize++] = 0x7e;
            array[newSize++] = 0x3e;
            if (newSize < array.Length)
            {
                Array.Resize<byte>(ref array, newSize);
            }
            return array;
        }
    }
}

