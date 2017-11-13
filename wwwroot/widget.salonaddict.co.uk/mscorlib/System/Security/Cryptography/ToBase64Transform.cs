namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    [ComVisible(true)]
    public class ToBase64Transform : ICryptoTransform, IDisposable
    {
        private ASCIIEncoding asciiEncoding = new ASCIIEncoding();

        public void Clear()
        {
            ((IDisposable) this).Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.asciiEncoding = null;
            }
        }

        ~ToBase64Transform()
        {
            this.Dispose(false);
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (this.asciiEncoding == null)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
            }
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
            char[] outArray = new char[4];
            Convert.ToBase64CharArray(inputBuffer, inputOffset, 3, outArray, 0);
            byte[] bytes = this.asciiEncoding.GetBytes(outArray);
            if (bytes.Length != 4)
            {
                throw new CryptographicException(Environment.GetResourceString("Cryptography_SSE_InvalidDataSize"));
            }
            Buffer.BlockCopy(bytes, 0, outputBuffer, outputOffset, bytes.Length);
            return bytes.Length;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (this.asciiEncoding == null)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
            }
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
            if (inputCount == 0)
            {
                return new byte[0];
            }
            char[] outArray = new char[4];
            Convert.ToBase64CharArray(inputBuffer, inputOffset, inputCount, outArray, 0);
            return this.asciiEncoding.GetBytes(outArray);
        }

        public virtual bool CanReuseTransform =>
            true;

        public bool CanTransformMultipleBlocks =>
            false;

        public int InputBlockSize =>
            3;

        public int OutputBlockSize =>
            4;
    }
}

