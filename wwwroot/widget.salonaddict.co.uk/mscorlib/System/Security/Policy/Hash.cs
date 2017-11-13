namespace System.Security.Policy
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class Hash : ISerializable, IBuiltInEvidence
    {
        private byte[] m_md5;
        private SafePEFileHandle m_peFile;
        private byte[] m_rawData;
        private byte[] m_sha1;

        internal Hash()
        {
            this.m_peFile = SafePEFileHandle.InvalidHandle;
        }

        public Hash(Assembly assembly)
        {
            this.m_peFile = SafePEFileHandle.InvalidHandle;
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            _GetPEFileFromAssembly(assembly.InternalAssembly, ref this.m_peFile);
        }

        internal Hash(SerializationInfo info, StreamingContext context)
        {
            this.m_peFile = SafePEFileHandle.InvalidHandle;
            this.m_md5 = (byte[]) info.GetValueNoThrow("Md5", typeof(byte[]));
            this.m_sha1 = (byte[]) info.GetValueNoThrow("Sha1", typeof(byte[]));
            this.m_peFile = SafePEFileHandle.InvalidHandle;
            this.m_rawData = (byte[]) info.GetValue("RawData", typeof(byte[]));
            if (this.m_rawData == null)
            {
                IntPtr inHandle = (IntPtr) info.GetValue("PEFile", typeof(IntPtr));
                if (inHandle != IntPtr.Zero)
                {
                    _SetPEFileHandle(inHandle, ref this.m_peFile);
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void _GetPEFileFromAssembly(Assembly assembly, ref SafePEFileHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern byte[] _GetRawData(SafePEFileHandle handle);
        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern void _ReleasePEFile(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void _SetPEFileHandle(IntPtr inHandle, ref SafePEFileHandle outHandle);
        public static Hash CreateMD5(byte[] md5)
        {
            if (md5 == null)
            {
                throw new ArgumentNullException("md5");
            }
            Hash hash = new Hash {
                m_md5 = new byte[md5.Length]
            };
            Array.Copy(md5, hash.m_md5, md5.Length);
            return hash;
        }

        public static Hash CreateSHA1(byte[] sha1)
        {
            if (sha1 == null)
            {
                throw new ArgumentNullException("sha1");
            }
            Hash hash = new Hash {
                m_sha1 = new byte[sha1.Length]
            };
            Array.Copy(sha1, hash.m_sha1, sha1.Length);
            return hash;
        }

        public byte[] GenerateHash(HashAlgorithm hashAlg)
        {
            if (hashAlg == null)
            {
                throw new ArgumentNullException("hashAlg");
            }
            if (hashAlg is System.Security.Cryptography.SHA1)
            {
                return this.SHA1;
            }
            if (hashAlg is System.Security.Cryptography.MD5)
            {
                return this.MD5;
            }
            return hashAlg.ComputeHash(this.RawData);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            info.AddValue("Md5", this.m_md5);
            info.AddValue("Sha1", this.m_sha1);
            if ((context.State == StreamingContextStates.Clone) || (context.State == StreamingContextStates.CrossAppDomain))
            {
                info.AddValue("PEFile", this.m_peFile.DangerousGetHandle());
                if (this.m_peFile.IsInvalid)
                {
                    info.AddValue("RawData", this.m_rawData);
                }
                else
                {
                    info.AddValue("RawData", null);
                }
            }
            else
            {
                if (!this.m_peFile.IsInvalid)
                {
                    this.m_rawData = this.RawData;
                }
                info.AddValue("PEFile", IntPtr.Zero);
                info.AddValue("RawData", this.m_rawData);
            }
        }

        int IBuiltInEvidence.GetRequiredSize(bool verbose)
        {
            if (verbose)
            {
                return 5;
            }
            return 0;
        }

        int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
        {
            this.m_peFile = SafePEFileHandle.InvalidHandle;
            IntPtr longFromCharArray = (IntPtr) BuiltInEvidenceHelper.GetLongFromCharArray(buffer, position);
            _SetPEFileHandle(longFromCharArray, ref this.m_peFile);
            return (position + 4);
        }

        int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
        {
            if (!verbose)
            {
                return position;
            }
            buffer[position++] = '\b';
            IntPtr zero = IntPtr.Zero;
            if (!this.m_peFile.IsInvalid)
            {
                zero = this.m_peFile.DangerousGetHandle();
            }
            BuiltInEvidenceHelper.CopyLongToCharArray((long) zero, buffer, position);
            return (position + 4);
        }

        public override string ToString() => 
            this.ToXml().ToString();

        private SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("System.Security.Policy.Hash");
            element.AddAttribute("version", "1");
            element.AddChild(new SecurityElement("RawData", Hex.EncodeHexString(this.RawData)));
            return element;
        }

        public byte[] MD5
        {
            get
            {
                if (this.m_md5 == null)
                {
                    this.m_md5 = new MD5CryptoServiceProvider().ComputeHash(this.RawData);
                }
                byte[] destinationArray = new byte[this.m_md5.Length];
                Array.Copy(this.m_md5, destinationArray, this.m_md5.Length);
                return destinationArray;
            }
        }

        internal byte[] RawData
        {
            get
            {
                if (this.m_rawData == null)
                {
                    if (this.m_peFile.IsInvalid)
                    {
                        throw new SecurityException(Environment.GetResourceString("Security_CannotGetRawData"));
                    }
                    byte[] buffer = _GetRawData(this.m_peFile);
                    if (buffer == null)
                    {
                        throw new SecurityException(Environment.GetResourceString("Security_CannotGenerateHash"));
                    }
                    this.m_rawData = buffer;
                }
                return this.m_rawData;
            }
        }

        public byte[] SHA1
        {
            get
            {
                if (this.m_sha1 == null)
                {
                    this.m_sha1 = new SHA1Managed().ComputeHash(this.RawData);
                }
                byte[] destinationArray = new byte[this.m_sha1.Length];
                Array.Copy(this.m_sha1, destinationArray, this.m_sha1.Length);
                return destinationArray;
            }
        }
    }
}

