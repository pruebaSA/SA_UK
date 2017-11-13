namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    [ComVisible(true)]
    public class FromBase64Transform : ICryptoTransform, IDisposable
    {
        private byte[] _inputBuffer;
        private int _inputIndex;
        private FromBase64TransformMode _whitespaces;

        public FromBase64Transform() : this(FromBase64TransformMode.IgnoreWhiteSpaces)
        {
        }

        public FromBase64Transform(FromBase64TransformMode whitespaces)
        {
            this._inputBuffer = new byte[4];
            this._whitespaces = whitespaces;
            this._inputIndex = 0;
        }

        public void Clear()
        {
            ((IDisposable) this).Dispose();
        }

        private byte[] DiscardWhiteSpaces(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            int num;
            int num2 = 0;
            for (num = 0; num < inputCount; num++)
            {
                if (char.IsWhiteSpace((char) inputBuffer[inputOffset + num]))
                {
                    num2++;
                }
            }
            byte[] buffer = new byte[inputCount - num2];
            num2 = 0;
            for (num = 0; num < inputCount; num++)
            {
                if (!char.IsWhiteSpace((char) inputBuffer[inputOffset + num]))
                {
                    buffer[num2++] = inputBuffer[inputOffset + num];
                }
            }
            return buffer;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._inputBuffer != null)
                {
                    Array.Clear(this._inputBuffer, 0, this._inputBuffer.Length);
                }
                this._inputBuffer = null;
                this._inputIndex = 0;
            }
        }

        ~FromBase64Transform()
        {
            this.Dispose(false);
        }

        private void Reset()
        {
            this._inputIndex = 0;
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            int length;
            byte[] dst = new byte[inputCount];
            if (inputBuffer == null)
            {
                throw new ArgumentNullException("inputBuffer");
            }
            if (inputOffset < 0)
            {
                throw new ArgumentOutOfRangeException("inputOffset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((inputCount < 0) || (inputCount > inputBuffer.Length))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidValue"));
            }
            if ((inputBuffer.Length - inputCount) < inputOffset)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            }
            if (this._inputBuffer == null)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
            }
            if (this._whitespaces == FromBase64TransformMode.IgnoreWhiteSpaces)
            {
                dst = this.DiscardWhiteSpaces(inputBuffer, inputOffset, inputCount);
                length = dst.Length;
            }
            else
            {
                Buffer.InternalBlockCopy(inputBuffer, inputOffset, dst, 0, inputCount);
                length = inputCount;
            }
            if ((length + this._inputIndex) < 4)
            {
                Buffer.InternalBlockCopy(dst, 0, this._inputBuffer, this._inputIndex, length);
                this._inputIndex += length;
                return 0;
            }
            int num2 = (length + this._inputIndex) / 4;
            byte[] buffer2 = new byte[this._inputIndex + length];
            Buffer.InternalBlockCopy(this._inputBuffer, 0, buffer2, 0, this._inputIndex);
            Buffer.InternalBlockCopy(dst, 0, buffer2, this._inputIndex, length);
            this._inputIndex = (length + this._inputIndex) % 4;
            Buffer.InternalBlockCopy(dst, length - this._inputIndex, this._inputBuffer, 0, this._inputIndex);
            byte[] src = Convert.FromBase64CharArray(Encoding.ASCII.GetChars(buffer2, 0, 4 * num2), 0, 4 * num2);
            Buffer.BlockCopy(src, 0, outputBuffer, outputOffset, src.Length);
            return src.Length;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            int length;
            byte[] dst = new byte[inputCount];
            if (inputBuffer == null)
            {
                throw new ArgumentNullException("inputBuffer");
            }
            if (inputOffset < 0)
            {
                throw new ArgumentOutOfRangeException("inputOffset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((inputCount < 0) || (inputCount > inputBuffer.Length))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidValue"));
            }
            if ((inputBuffer.Length - inputCount) < inputOffset)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            }
            if (this._inputBuffer == null)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
            }
            if (this._whitespaces == FromBase64TransformMode.IgnoreWhiteSpaces)
            {
                dst = this.DiscardWhiteSpaces(inputBuffer, inputOffset, inputCount);
                length = dst.Length;
            }
            else
            {
                Buffer.InternalBlockCopy(inputBuffer, inputOffset, dst, 0, inputCount);
                length = inputCount;
            }
            if ((length + this._inputIndex) < 4)
            {
                this.Reset();
                return new byte[0];
            }
            int num2 = (length + this._inputIndex) / 4;
            byte[] buffer2 = new byte[this._inputIndex + length];
            Buffer.InternalBlockCopy(this._inputBuffer, 0, buffer2, 0, this._inputIndex);
            Buffer.InternalBlockCopy(dst, 0, buffer2, this._inputIndex, length);
            this._inputIndex = (length + this._inputIndex) % 4;
            Buffer.InternalBlockCopy(dst, length - this._inputIndex, this._inputBuffer, 0, this._inputIndex);
            byte[] buffer3 = Convert.FromBase64CharArray(Encoding.ASCII.GetChars(buffer2, 0, 4 * num2), 0, 4 * num2);
            this.Reset();
            return buffer3;
        }

        public virtual bool CanReuseTransform =>
            true;

        public bool CanTransformMultipleBlocks =>
            false;

        public int InputBlockSize =>
            1;

        public int OutputBlockSize =>
            3;
    }
}

