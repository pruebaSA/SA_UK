namespace System.Xml
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class BufferBuilder
    {
        private Buffer[] buffers;
        private int buffersCount;
        private const int BufferSize = 0x10000;
        private const int DefaultSBCapacity = 0x10;
        private const int InitialBufferArrayLength = 4;
        private char[] lastBuffer;
        private int lastBufferIndex;
        private int length;
        private const int MaxStringBuilderLength = 0x10000;
        private StringBuilder stringBuilder;

        private void AddBuffer()
        {
            char[] target;
            if ((this.buffersCount + 1) == this.buffers.Length)
            {
                Buffer[] destinationArray = new Buffer[this.buffers.Length * 2];
                Array.Copy(this.buffers, 0, destinationArray, 0, this.buffers.Length);
                this.buffers = destinationArray;
            }
            if (this.buffers[this.buffersCount].recycledBuffer != null)
            {
                target = (char[]) this.buffers[this.buffersCount].recycledBuffer.Target;
                if (target != null)
                {
                    this.buffers[this.buffersCount].recycledBuffer.Target = null;
                    goto Label_00A4;
                }
            }
            target = new char[0x10000];
        Label_00A4:
            this.lastBuffer = target;
            this.buffers[this.buffersCount++].buffer = target;
            this.lastBufferIndex = 0;
        }

        public void Append(char value)
        {
            if ((this.length + 1) <= 0x10000)
            {
                if (this.stringBuilder == null)
                {
                    this.stringBuilder = new StringBuilder();
                }
                this.stringBuilder.Append(value);
            }
            else
            {
                if (this.lastBuffer == null)
                {
                    this.CreateBuffers();
                }
                if (this.lastBufferIndex == this.lastBuffer.Length)
                {
                    this.AddBuffer();
                }
                this.lastBuffer[this.lastBufferIndex++] = value;
            }
            this.length++;
        }

        public void Append(char[] value)
        {
            this.Append(value, 0, value.Length);
        }

        public void Append(string value)
        {
            this.Append(value, 0, value.Length);
        }

        public unsafe void Append(char[] value, int start, int count)
        {
            if (value == null)
            {
                if ((start != 0) || (count != 0))
                {
                    throw new ArgumentNullException("value");
                }
            }
            else if (count != 0)
            {
                if (start < 0)
                {
                    throw new ArgumentOutOfRangeException("start");
                }
                if ((count < 0) || ((start + count) > value.Length))
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                if ((this.length + count) <= 0x10000)
                {
                    if (this.stringBuilder == null)
                    {
                        this.stringBuilder = new StringBuilder((count < 0x10) ? 0x10 : count);
                    }
                    this.stringBuilder.Append(value, start, count);
                    this.length += count;
                }
                else
                {
                    fixed (char* chRef = &(value[start]))
                    {
                        this.AppendHelper(chRef, count);
                    }
                }
            }
        }

        public unsafe void Append(string value, int start, int count)
        {
            if (value == null)
            {
                if ((start != 0) || (count != 0))
                {
                    throw new ArgumentNullException("value");
                }
            }
            else if (count != 0)
            {
                if (start < 0)
                {
                    throw new ArgumentOutOfRangeException("start");
                }
                if ((count < 0) || ((start + count) > value.Length))
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                if ((this.length + count) <= 0x10000)
                {
                    if (this.stringBuilder == null)
                    {
                        this.stringBuilder = new StringBuilder(value, start, count, 0);
                    }
                    else
                    {
                        this.stringBuilder.Append(value, start, count);
                    }
                    this.length += count;
                }
                else
                {
                    fixed (char* str = ((char*) value))
                    {
                        char* chPtr = str;
                        this.AppendHelper(chPtr + start, count);
                    }
                }
            }
        }

        private unsafe void AppendHelper(char* pSource, int count)
        {
            if (this.lastBuffer == null)
            {
                this.CreateBuffers();
            }
            int charCount = 0;
            while (count > 0)
            {
                if (this.lastBufferIndex >= this.lastBuffer.Length)
                {
                    this.AddBuffer();
                }
                charCount = count;
                int num2 = this.lastBuffer.Length - this.lastBufferIndex;
                if (num2 < charCount)
                {
                    charCount = num2;
                }
                fixed (char* chRef = &(this.lastBuffer[this.lastBufferIndex]))
                {
                    wstrcpy(chRef, pSource, charCount);
                }
                pSource += charCount;
                this.length += charCount;
                this.lastBufferIndex += charCount;
                count -= charCount;
            }
        }

        public void Clear()
        {
            if (this.length <= 0x10000)
            {
                if (this.stringBuilder != null)
                {
                    this.stringBuilder.Length = 0;
                }
            }
            else
            {
                if (this.lastBuffer != null)
                {
                    this.ClearBuffers();
                }
                this.stringBuilder = null;
            }
            this.length = 0;
        }

        internal void ClearBuffers()
        {
            if (this.buffers != null)
            {
                for (int i = 0; i < this.buffersCount; i++)
                {
                    this.Recycle(this.buffers[i]);
                }
                this.lastBuffer = null;
            }
            this.lastBufferIndex = 0;
            this.buffersCount = 0;
        }

        private void CreateBuffers()
        {
            if (this.buffers == null)
            {
                this.lastBuffer = new char[0x10000];
                this.buffers = new Buffer[4];
                this.buffers[0].buffer = this.lastBuffer;
                this.buffersCount = 1;
            }
            else
            {
                this.AddBuffer();
            }
        }

        private void Recycle(Buffer buf)
        {
            if (buf.recycledBuffer == null)
            {
                buf.recycledBuffer = new WeakReference(buf.buffer);
            }
            else
            {
                buf.recycledBuffer.Target = buf.buffer;
            }
            buf.buffer = null;
        }

        private void SetLength(int newLength)
        {
            if (newLength != this.length)
            {
                if (this.length <= 0x10000)
                {
                    this.stringBuilder.Length = newLength;
                }
                else
                {
                    int num = newLength;
                    int index = 0;
                    while (index < this.buffersCount)
                    {
                        if (num < this.buffers[index].buffer.Length)
                        {
                            break;
                        }
                        num -= this.buffers[index].buffer.Length;
                        index++;
                    }
                    if (index < this.buffersCount)
                    {
                        this.lastBuffer = this.buffers[index].buffer;
                        this.lastBufferIndex = num;
                        index++;
                        int num3 = index;
                        while (index < this.buffersCount)
                        {
                            this.Recycle(this.buffers[index]);
                            index++;
                        }
                        this.buffersCount = num3;
                    }
                }
                this.length = newLength;
            }
        }

        public override string ToString()
        {
            if ((this.length <= 0x10000) || ((this.buffersCount == 1) && (this.lastBufferIndex == 0)))
            {
                return ((this.stringBuilder != null) ? this.stringBuilder.ToString() : string.Empty);
            }
            if (this.stringBuilder == null)
            {
                this.stringBuilder = new StringBuilder(this.length);
            }
            else
            {
                this.stringBuilder.Capacity = this.length;
            }
            int charCount = this.length - this.stringBuilder.Length;
            for (int i = 0; i < (this.buffersCount - 1); i++)
            {
                char[] buffer = this.buffers[i].buffer;
                this.stringBuilder.Append(buffer, 0, buffer.Length);
                charCount -= buffer.Length;
            }
            this.stringBuilder.Append(this.buffers[this.buffersCount - 1].buffer, 0, charCount);
            this.ClearBuffers();
            return this.stringBuilder.ToString();
        }

        public string ToString(int startIndex, int len)
        {
            if ((startIndex < 0) || (startIndex >= this.length))
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            if ((len < 0) || ((startIndex + len) > this.length))
            {
                throw new ArgumentOutOfRangeException("len");
            }
            if ((this.length <= 0x10000) || ((this.buffersCount == 1) && (this.lastBufferIndex == 0)))
            {
                return this.stringBuilder?.ToString(startIndex, len);
            }
            StringBuilder builder = new StringBuilder(len);
            if (this.stringBuilder != null)
            {
                if (startIndex < this.stringBuilder.Length)
                {
                    if (len < this.stringBuilder.Length)
                    {
                        return this.stringBuilder.ToString(startIndex, len);
                    }
                    builder.Append(this.stringBuilder.ToString(startIndex, this.stringBuilder.Length));
                    startIndex = 0;
                }
                else
                {
                    startIndex -= this.stringBuilder.Length;
                }
            }
            int index = 0;
            while (index < this.buffersCount)
            {
                if (startIndex < this.buffers[index].buffer.Length)
                {
                    break;
                }
                startIndex -= this.buffers[index].buffer.Length;
                index++;
            }
            if (index < this.buffersCount)
            {
                int num2 = len;
                while ((index < this.buffersCount) && (num2 > 0))
                {
                    char[] buffer = this.buffers[index].buffer;
                    int charCount = (buffer.Length < num2) ? buffer.Length : num2;
                    builder.Append(buffer, startIndex, charCount);
                    startIndex = 0;
                    num2 -= charCount;
                    index++;
                }
            }
            return builder.ToString();
        }

        internal static unsafe void wstrcpy(char* dmem, char* smem, int charCount)
        {
            if (charCount > 0)
            {
                if (((((int) dmem) ^ ((int) smem)) & 3) == 0)
                {
                    while (((((int) dmem) & 3) != 0) && (charCount > 0))
                    {
                        dmem[0] = smem[0];
                        dmem++;
                        smem++;
                        charCount--;
                    }
                    if (charCount >= 8)
                    {
                        charCount -= 8;
                        do
                        {
                            *((int*) dmem) = *((uint*) smem);
                            *((int*) (dmem + 2)) = *((uint*) (smem + 2));
                            *((int*) (dmem + 4)) = *((uint*) (smem + 4));
                            *((int*) (dmem + 6)) = *((uint*) (smem + 6));
                            dmem += 8;
                            smem += 8;
                            charCount -= 8;
                        }
                        while (charCount >= 0);
                    }
                    if ((charCount & 4) != 0)
                    {
                        *((int*) dmem) = *((uint*) smem);
                        *((int*) (dmem + 2)) = *((uint*) (smem + 2));
                        dmem += 4;
                        smem += 4;
                    }
                    if ((charCount & 2) != 0)
                    {
                        *((int*) dmem) = *((uint*) smem);
                        dmem += 2;
                        smem += 2;
                    }
                }
                else
                {
                    if (charCount >= 8)
                    {
                        charCount -= 8;
                        do
                        {
                            dmem[0] = smem[0];
                            dmem[1] = smem[1];
                            dmem[2] = smem[2];
                            dmem[3] = smem[3];
                            dmem[4] = smem[4];
                            dmem[5] = smem[5];
                            dmem[6] = smem[6];
                            dmem[7] = smem[7];
                            dmem += 8;
                            smem += 8;
                            charCount -= 8;
                        }
                        while (charCount >= 0);
                    }
                    if ((charCount & 4) != 0)
                    {
                        dmem[0] = smem[0];
                        dmem[1] = smem[1];
                        dmem[2] = smem[2];
                        dmem[3] = smem[3];
                        dmem += 4;
                        smem += 4;
                    }
                    if ((charCount & 2) != 0)
                    {
                        dmem[0] = smem[0];
                        dmem[1] = smem[1];
                        dmem += 2;
                        smem += 2;
                    }
                }
                if ((charCount & 1) != 0)
                {
                    dmem[0] = smem[0];
                }
            }
        }

        public int Length
        {
            get => 
                this.length;
            set
            {
                if ((value < 0) || (value > this.length))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value == 0)
                {
                    this.Clear();
                }
                else
                {
                    this.SetLength(value);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Buffer
        {
            internal char[] buffer;
            internal WeakReference recycledBuffer;
        }
    }
}

