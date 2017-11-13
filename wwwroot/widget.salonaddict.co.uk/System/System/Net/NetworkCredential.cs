namespace System.Net
{
    using Microsoft.Win32;
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    public class NetworkCredential : ICredentials, ICredentialsByHost
    {
        private static readonly object lockingObject = new object();
        private byte[] m_domain;
        private bool m_encrypt;
        private byte[] m_encryptionIV;
        private static EnvironmentPermission m_environmentDomainNamePermission;
        private static EnvironmentPermission m_environmentUserNamePermission;
        private byte[] m_password;
        private byte[] m_userName;
        private static RNGCryptoServiceProvider s_random;
        private static SymmetricAlgorithm s_symmetricAlgorithm;
        private static bool s_useTripleDES = false;

        public NetworkCredential()
        {
            this.m_encrypt = true;
        }

        public NetworkCredential(string userName, string password) : this(userName, password, string.Empty)
        {
        }

        public NetworkCredential(string userName, string password, string domain) : this(userName, password, domain, true)
        {
        }

        internal NetworkCredential(string userName, string password, string domain, bool encrypt)
        {
            this.m_encrypt = true;
            this.m_encrypt = encrypt;
            this.UserName = userName;
            this.Password = password;
            this.Domain = domain;
        }

        internal string Decrypt(byte[] ciphertext)
        {
            if (ciphertext == null)
            {
                return string.Empty;
            }
            if (!this.m_encrypt)
            {
                return Encoding.UTF8.GetString(ciphertext);
            }
            this.InitializePart2();
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, s_symmetricAlgorithm.CreateDecryptor(s_symmetricAlgorithm.Key, this.m_encryptionIV), CryptoStreamMode.Write);
            stream2.Write(ciphertext, 0, ciphertext.Length);
            stream2.FlushFinalBlock();
            byte[] bytes = stream.ToArray();
            stream2.Close();
            return Encoding.UTF8.GetString(bytes);
        }

        internal byte[] Encrypt(string text)
        {
            if ((text == null) || (text.Length == 0))
            {
                return null;
            }
            if (!this.m_encrypt)
            {
                return Encoding.UTF8.GetBytes(text);
            }
            this.InitializePart2();
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, s_symmetricAlgorithm.CreateEncryptor(s_symmetricAlgorithm.Key, this.m_encryptionIV), CryptoStreamMode.Write);
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            bytes = stream.ToArray();
            stream2.Close();
            return bytes;
        }

        public NetworkCredential GetCredential(Uri uri, string authType) => 
            this;

        public NetworkCredential GetCredential(string host, int port, string authenticationType) => 
            this;

        private void InitializePart1()
        {
            if (m_environmentUserNamePermission == null)
            {
                lock (lockingObject)
                {
                    if (m_environmentUserNamePermission == null)
                    {
                        m_environmentDomainNamePermission = new EnvironmentPermission(EnvironmentPermissionAccess.Read, "USERDOMAIN");
                        m_environmentUserNamePermission = new EnvironmentPermission(EnvironmentPermissionAccess.Read, "USERNAME");
                    }
                }
            }
        }

        private void InitializePart2()
        {
            if (this.m_encrypt)
            {
                if (s_symmetricAlgorithm == null)
                {
                    lock (lockingObject)
                    {
                        if (s_symmetricAlgorithm == null)
                        {
                            SymmetricAlgorithm algorithm;
                            s_useTripleDES = this.ReadRegFips();
                            if (s_useTripleDES)
                            {
                                algorithm = new TripleDESCryptoServiceProvider {
                                    KeySize = 0x80
                                };
                                algorithm.GenerateKey();
                            }
                            else
                            {
                                s_random = new RNGCryptoServiceProvider();
                                algorithm = Rijndael.Create();
                                byte[] data = new byte[0x10];
                                s_random.GetBytes(data);
                                algorithm.Key = data;
                            }
                            s_symmetricAlgorithm = algorithm;
                        }
                    }
                }
                if (this.m_encryptionIV == null)
                {
                    if (s_useTripleDES)
                    {
                        s_symmetricAlgorithm.GenerateIV();
                        byte[] iV = s_symmetricAlgorithm.IV;
                        Interlocked.CompareExchange<byte[]>(ref this.m_encryptionIV, iV, null);
                    }
                    else
                    {
                        byte[] buffer3 = new byte[0x10];
                        s_random.GetBytes(buffer3);
                        Interlocked.CompareExchange<byte[]>(ref this.m_encryptionIV, buffer3, null);
                    }
                }
            }
        }

        internal string InternalGetDomain() => 
            this.Decrypt(this.m_domain);

        internal string InternalGetDomainUserName()
        {
            string domain = this.InternalGetDomain();
            if (domain.Length != 0)
            {
                domain = domain + @"\";
            }
            return (domain + this.InternalGetUserName());
        }

        internal string InternalGetPassword() => 
            this.Decrypt(this.m_password);

        internal string InternalGetUserName() => 
            this.Decrypt(this.m_userName);

        internal bool IsEqualTo(object compObject)
        {
            if (compObject == null)
            {
                return false;
            }
            if (this == compObject)
            {
                return true;
            }
            NetworkCredential credential = compObject as NetworkCredential;
            if (credential == null)
            {
                return false;
            }
            return (((this.InternalGetUserName() == credential.InternalGetUserName()) && (this.InternalGetPassword() == credential.InternalGetPassword())) && (this.InternalGetDomain() == credential.InternalGetDomain()));
        }

        [RegistryPermission(SecurityAction.Assert, Read=@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa")]
        private bool ReadRegFips()
        {
            bool flag = false;
            bool pfEnabled = false;
            if (ComNetOS.IsVista)
            {
                uint num = UnsafeNclNativeMethods.BCryptGetFipsAlgorithmMode(out pfEnabled);
                flag = (num == 0) || (num == 0xc0000034);
            }
            else
            {
                RegistryKey key = null;
                object obj2 = null;
                try
                {
                    string name = @"SYSTEM\CurrentControlSet\Control\Lsa";
                    key = Registry.LocalMachine.OpenSubKey(name);
                    if (key != null)
                    {
                        obj2 = key.GetValue("fipsalgorithmpolicy");
                    }
                    flag = true;
                    if ((obj2 != null) && (((int) obj2) == 1))
                    {
                        pfEnabled = true;
                    }
                }
                catch
                {
                }
                finally
                {
                    if (key != null)
                    {
                        key.Close();
                    }
                }
            }
            if (flag && !pfEnabled)
            {
                return false;
            }
            return true;
        }

        public string Domain
        {
            get
            {
                this.InitializePart1();
                m_environmentDomainNamePermission.Demand();
                return this.InternalGetDomain();
            }
            set
            {
                this.m_domain = this.Encrypt(value);
            }
        }

        public string Password
        {
            get
            {
                ExceptionHelper.UnmanagedPermission.Demand();
                return this.InternalGetPassword();
            }
            set
            {
                this.m_password = this.Encrypt(value);
            }
        }

        public string UserName
        {
            get
            {
                this.InitializePart1();
                m_environmentUserNamePermission.Demand();
                return this.InternalGetUserName();
            }
            set
            {
                this.m_userName = this.Encrypt(value);
            }
        }
    }
}

