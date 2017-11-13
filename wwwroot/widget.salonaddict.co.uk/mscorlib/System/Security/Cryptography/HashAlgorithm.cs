namespace System.Security.Cryptography
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class HashAlgorithm : ICryptoTransform, IDisposable
    {
        protected int HashSizeValue;
        protected internal byte[] HashValue;
        private bool m_bDisposed;
        protected int State;

        protected HashAlgorithm()
        {
        }

        public void Clear()
        {
            ((IDisposable) this).Dispose();
        }

        public byte[] ComputeHash(Stream inputStream)
        {
            int num;
            if (this.m_bDisposed)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
            }
            byte[] buffer = new byte[0x1000];
            do
            {
                num = inputStream.Read(buffer, 0, 0x1000);
                if (num > 0)
                {
                    this.HashCore(buffer, 0, num);
                }
            }
            while (num > 0);
            this.HashValue = this.HashFinal();
            byte[] buffer2 = (byte[]) this.HashValue.Clone();
            this.Initialize();
            return buffer2;
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            if (this.m_bDisposed)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            this.HashCore(buffer, 0, buffer.Length);
            this.HashValue = this.HashFinal();
            byte[] buffer2 = (byte[]) this.HashValue.Clone();
            this.Initialize();
            return buffer2;
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            if (this.m_bDisposed)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if ((count < 0) || (count > buffer.Length))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidValue"));
            }
            if ((buffer.Length - count) < offset)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            }
            this.HashCore(buffer, offset, count);
            this.HashValue = this.HashFinal();
            byte[] buffer2 = (byte[]) this.HashValue.Clone();
            this.Initialize();
            return buffer2;
        }

        public static HashAlgorithm Create() => 
            Create("System.Security.Cryptography.HashAlgorithm");

        public static HashAlgorithm Create(string hashName) => 
            ((HashAlgorithm) CryptoConfig.CreateFromName(hashName));

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.HashValue != null)
                {
                    Array.Clear(this.HashValue, 0, this.HashValue.Length);
                }
                this.HashValue = null;
                this.m_bDisposed = true;
            }
        }

        protected abstract void HashCore(byte[] array, int ibStart, int cbSize);
        protected abstract byte[] HashFinal();
        public abstract void Initialize();
        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (this.m_bDisposed)
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
            this.State = 1;
            this.HashCore(inputBuffer, inputOffset, inputCount);
            if ((outputBuffer != null) && ((inputBuffer != outputBuffer) || (inputOffset != outputOffset)))
            {
                Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
            }
            return inputCount;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (this.m_bDisposed)
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
            this.HashCore(inputBuffer, inputOffset, inputCount);
            this.HashValue = this.HashFinal();
            byte[] dst = new byte[inputCount];
            if (inputCount != 0)
            {
                Buffer.InternalBlockCopy(inputBuffer, inputOffset, dst, 0, inputCount);
            }
            this.State = 0;
            return dst;
        }

        public virtual bool CanReuseTransform =>
            true;

        public virtual bool CanTransformMultipleBlocks =>
            true;

        public virtual byte[] Hash
        {
            get
            {
                if (this.m_bDisposed)
                {
                    throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
                }
                if (this.State != 0)
                {
                    throw new CryptographicUnexpectedOperationException(Environment.GetResourceString("Cryptography_HashNotYetFinalized"));
                }
                return (byte[]) this.HashValue.Clone();
            }
        }

        public virtual int HashSize =>
            this.HashSizeValue;

        public virtual int InputBlockSize =>
            1;

        public virtual int OutputBlockSize =>
            1;
    }
}

