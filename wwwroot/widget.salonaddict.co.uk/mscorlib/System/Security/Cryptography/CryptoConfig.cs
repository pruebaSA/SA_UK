namespace System.Security.Cryptography
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;
    using System.Threading;

    [ComVisible(true)]
    public class CryptoConfig
    {
        private static string _Version = null;
        private static Hashtable defaultNameHT = null;
        private static Hashtable defaultOidHT = null;
        private static bool isInitialized = false;
        private static string machineConfigDir = Config.MachineDirectory;
        private static string machineConfigFilename = "machine.config";
        private static Hashtable machineNameHT = null;
        private static Hashtable machineOidHT = null;
        private static object s_InternalSyncObject;

        public static object CreateFromName(string name) => 
            CreateFromName(name, null);

        public static object CreateFromName(string name, params object[] args)
        {
            object obj4;
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            Type type = null;
            if (!isInitialized)
            {
                InitializeConfigInfo();
            }
            if (machineNameHT != null)
            {
                string typeName = (string) machineNameHT[name];
                if (typeName != null)
                {
                    type = Type.GetType(typeName, false, false);
                    if ((type != null) && !type.IsVisible)
                    {
                        type = null;
                    }
                }
            }
            if (type == null)
            {
                object obj3 = DefaultNameHT[name];
                if (obj3 != null)
                {
                    if (obj3 is Type)
                    {
                        type = (Type) obj3;
                    }
                    else if (obj3 is string)
                    {
                        type = Type.GetType((string) obj3, false, false);
                        if ((type != null) && !type.IsVisible)
                        {
                            type = null;
                        }
                    }
                }
            }
            if (type == null)
            {
                type = Type.GetType(name, false, false);
                if ((type != null) && !type.IsVisible)
                {
                    type = null;
                }
            }
            if (type == null)
            {
                return null;
            }
            RuntimeType type2 = type as RuntimeType;
            if (type2 == null)
            {
                return null;
            }
            if (args == null)
            {
                args = new object[0];
            }
            MethodBase[] constructors = type2.GetConstructors(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance);
            if (constructors == null)
            {
                return null;
            }
            ArrayList list = new ArrayList();
            for (int i = 0; i < constructors.Length; i++)
            {
                MethodBase base2 = constructors[i];
                if (base2.GetParameters().Length == args.Length)
                {
                    list.Add(base2);
                }
            }
            if (list.Count == 0)
            {
                return null;
            }
            constructors = list.ToArray(typeof(MethodBase)) as MethodBase[];
            RuntimeConstructorInfo info = Type.DefaultBinder.BindToMethod(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, constructors, ref args, null, null, null, out obj4) as RuntimeConstructorInfo;
            if ((info == null) || typeof(Delegate).IsAssignableFrom(info.DeclaringType))
            {
                return null;
            }
            object obj2 = info.Invoke(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, args, null);
            if (obj4 != null)
            {
                Type.DefaultBinder.ReorderArgumentArray(ref args, obj4);
            }
            return obj2;
        }

        public static byte[] EncodeOID(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            char[] separator = new char[] { '.' };
            string[] strArray = str.Split(separator);
            uint[] numArray = new uint[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                numArray[i] = (uint) int.Parse(strArray[i], CultureInfo.InvariantCulture);
            }
            byte[] destinationArray = new byte[numArray.Length * 5];
            int destinationIndex = 0;
            if (numArray.Length < 2)
            {
                throw new CryptographicUnexpectedOperationException(Environment.GetResourceString("Cryptography_InvalidOID"));
            }
            uint dwValue = (numArray[0] * 40) + numArray[1];
            byte[] sourceArray = EncodeSingleOIDNum(dwValue);
            Array.Copy(sourceArray, 0, destinationArray, destinationIndex, sourceArray.Length);
            destinationIndex += sourceArray.Length;
            for (int j = 2; j < numArray.Length; j++)
            {
                sourceArray = EncodeSingleOIDNum(numArray[j]);
                Buffer.InternalBlockCopy(sourceArray, 0, destinationArray, destinationIndex, sourceArray.Length);
                destinationIndex += sourceArray.Length;
            }
            if (destinationIndex > 0x7f)
            {
                throw new CryptographicUnexpectedOperationException(Environment.GetResourceString("Cryptography_Config_EncodedOIDError"));
            }
            sourceArray = new byte[destinationIndex + 2];
            sourceArray[0] = 6;
            sourceArray[1] = (byte) destinationIndex;
            Buffer.InternalBlockCopy(destinationArray, 0, sourceArray, 2, destinationIndex);
            return sourceArray;
        }

        private static byte[] EncodeSingleOIDNum(uint dwValue)
        {
            if (dwValue < 0x80)
            {
                return new byte[] { ((byte) dwValue) };
            }
            if (dwValue < 0x4000)
            {
                return new byte[] { ((byte) ((dwValue >> 7) | 0x80)), ((byte) (dwValue & 0x7f)) };
            }
            if (dwValue < 0x200000)
            {
                return new byte[] { ((byte) ((dwValue >> 14) | 0x80)), ((byte) ((dwValue >> 7) | 0x80)), ((byte) (dwValue & 0x7f)) };
            }
            if (dwValue < 0x10000000)
            {
                return new byte[] { ((byte) ((dwValue >> 0x15) | 0x80)), ((byte) ((dwValue >> 14) | 0x80)), ((byte) ((dwValue >> 7) | 0x80)), ((byte) (dwValue & 0x7f)) };
            }
            return new byte[] { ((byte) ((dwValue >> 0x1c) | 0x80)), ((byte) ((dwValue >> 0x15) | 0x80)), ((byte) ((dwValue >> 14) | 0x80)), ((byte) ((dwValue >> 7) | 0x80)), ((byte) (dwValue & 0x7f)) };
        }

        private static void InitializeConfigInfo()
        {
            Type type = typeof(CryptoConfig);
            _Version = type.Assembly.GetVersion().ToString();
            if ((machineNameHT == null) && (machineOidHT == null))
            {
                lock (InternalSyncObject)
                {
                    string path = machineConfigDir + machineConfigFilename;
                    new FileIOPermission(FileIOPermissionAccess.Read, path).Assert();
                    if (File.Exists(path))
                    {
                        ConfigNode node = new ConfigTreeParser().Parse(path, "configuration");
                        if (node != null)
                        {
                            ArrayList children = node.Children;
                            ConfigNode node2 = null;
                            foreach (ConfigNode node3 in children)
                            {
                                if (node3.Name.Equals("mscorlib"))
                                {
                                    if (node3.Attributes.Count > 0)
                                    {
                                        DictionaryEntry entry = (DictionaryEntry) node3.Attributes[0];
                                        if (!entry.Key.Equals("version") || !entry.Value.Equals(_Version))
                                        {
                                            continue;
                                        }
                                        node2 = node3;
                                        break;
                                    }
                                    node2 = node3;
                                }
                            }
                            if (node2 != null)
                            {
                                ArrayList list3 = node2.Children;
                                ConfigNode node4 = null;
                                foreach (ConfigNode node5 in list3)
                                {
                                    if (node5.Name.Equals("cryptographySettings"))
                                    {
                                        node4 = node5;
                                        break;
                                    }
                                }
                                if (node4 != null)
                                {
                                    ConfigNode node6 = null;
                                    foreach (ConfigNode node7 in node4.Children)
                                    {
                                        if (node7.Name.Equals("cryptoNameMapping"))
                                        {
                                            node6 = node7;
                                            break;
                                        }
                                    }
                                    if (node6 != null)
                                    {
                                        ArrayList list4 = node6.Children;
                                        ConfigNode node8 = null;
                                        foreach (ConfigNode node9 in list4)
                                        {
                                            if (node9.Name.Equals("cryptoClasses"))
                                            {
                                                node8 = node9;
                                                break;
                                            }
                                        }
                                        if (node8 != null)
                                        {
                                            Hashtable hashtable = new Hashtable();
                                            Hashtable hashtable2 = new Hashtable();
                                            foreach (ConfigNode node10 in node8.Children)
                                            {
                                                if (node10.Name.Equals("cryptoClass") && (node10.Attributes.Count > 0))
                                                {
                                                    DictionaryEntry entry2 = (DictionaryEntry) node10.Attributes[0];
                                                    hashtable.Add(entry2.Key, entry2.Value);
                                                }
                                            }
                                            foreach (ConfigNode node11 in list4)
                                            {
                                                if (node11.Name.Equals("nameEntry"))
                                                {
                                                    string key = null;
                                                    string str3 = null;
                                                    foreach (DictionaryEntry entry3 in node11.Attributes)
                                                    {
                                                        if (((string) entry3.Key).Equals("name"))
                                                        {
                                                            key = (string) entry3.Value;
                                                        }
                                                        else if (((string) entry3.Key).Equals("class"))
                                                        {
                                                            str3 = (string) entry3.Value;
                                                        }
                                                    }
                                                    if ((key != null) && (str3 != null))
                                                    {
                                                        string str4 = (string) hashtable[str3];
                                                        if (str4 != null)
                                                        {
                                                            hashtable2.Add(key, str4);
                                                        }
                                                    }
                                                }
                                            }
                                            machineNameHT = hashtable2;
                                        }
                                    }
                                    ConfigNode node12 = null;
                                    foreach (ConfigNode node13 in node4.Children)
                                    {
                                        if (node13.Name.Equals("oidMap"))
                                        {
                                            node12 = node13;
                                            break;
                                        }
                                    }
                                    if (node12 != null)
                                    {
                                        Hashtable hashtable3 = new Hashtable();
                                        foreach (ConfigNode node14 in node12.Children)
                                        {
                                            if (node14.Name.Equals("oidEntry"))
                                            {
                                                string str5 = null;
                                                string str6 = null;
                                                foreach (DictionaryEntry entry4 in node14.Attributes)
                                                {
                                                    if (((string) entry4.Key).Equals("OID"))
                                                    {
                                                        str5 = (string) entry4.Value;
                                                    }
                                                    else if (((string) entry4.Key).Equals("name"))
                                                    {
                                                        str6 = (string) entry4.Value;
                                                    }
                                                }
                                                if ((str6 != null) && (str5 != null))
                                                {
                                                    hashtable3.Add(str6, str5);
                                                }
                                            }
                                        }
                                        machineOidHT = hashtable3;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            isInitialized = true;
        }

        public static string MapNameToOID(string name) => 
            MapNameToOID(name, OidGroup.AllGroups);

        internal static string MapNameToOID(string name, OidGroup group)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (!isInitialized)
            {
                InitializeConfigInfo();
            }
            string str = null;
            if (machineOidHT != null)
            {
                str = machineOidHT[name] as string;
            }
            if (str == null)
            {
                str = DefaultOidHT[name] as string;
            }
            if (str == null)
            {
                str = X509Utils._GetOidFromFriendlyName(name, group);
            }
            return str;
        }

        private static Hashtable DefaultNameHT
        {
            get
            {
                if (defaultNameHT == null)
                {
                    Hashtable hashtable = new Hashtable(StringComparer.OrdinalIgnoreCase);
                    Type type = typeof(SHA1CryptoServiceProvider);
                    Type type2 = typeof(MD5CryptoServiceProvider);
                    Type type3 = typeof(SHA256Managed);
                    Type type4 = typeof(SHA384Managed);
                    Type type5 = typeof(SHA512Managed);
                    Type type6 = typeof(RIPEMD160Managed);
                    Type type7 = typeof(HMACMD5);
                    Type type8 = typeof(HMACRIPEMD160);
                    Type type9 = typeof(HMACSHA1);
                    Type type10 = typeof(HMACSHA256);
                    Type type11 = typeof(HMACSHA384);
                    Type type12 = typeof(HMACSHA512);
                    Type type13 = typeof(MACTripleDES);
                    Type type14 = typeof(RSACryptoServiceProvider);
                    Type type15 = typeof(DSACryptoServiceProvider);
                    Type type16 = typeof(DESCryptoServiceProvider);
                    Type type17 = typeof(TripleDESCryptoServiceProvider);
                    Type type18 = typeof(RC2CryptoServiceProvider);
                    Type type19 = typeof(RijndaelManaged);
                    Type type20 = typeof(DSASignatureDescription);
                    Type type21 = typeof(RSAPKCS1SHA1SignatureDescription);
                    Type type22 = typeof(RNGCryptoServiceProvider);
                    hashtable.Add("RandomNumberGenerator", type22);
                    hashtable.Add("System.Security.Cryptography.RandomNumberGenerator", type22);
                    hashtable.Add("SHA", type);
                    hashtable.Add("SHA1", type);
                    hashtable.Add("System.Security.Cryptography.SHA1", type);
                    hashtable.Add("System.Security.Cryptography.HashAlgorithm", type);
                    hashtable.Add("MD5", type2);
                    hashtable.Add("System.Security.Cryptography.MD5", type2);
                    hashtable.Add("SHA256", type3);
                    hashtable.Add("SHA-256", type3);
                    hashtable.Add("System.Security.Cryptography.SHA256", type3);
                    hashtable.Add("SHA384", type4);
                    hashtable.Add("SHA-384", type4);
                    hashtable.Add("System.Security.Cryptography.SHA384", type4);
                    hashtable.Add("SHA512", type5);
                    hashtable.Add("SHA-512", type5);
                    hashtable.Add("System.Security.Cryptography.SHA512", type5);
                    hashtable.Add("RIPEMD160", type6);
                    hashtable.Add("RIPEMD-160", type6);
                    hashtable.Add("System.Security.Cryptography.RIPEMD160", type6);
                    hashtable.Add("System.Security.Cryptography.RIPEMD160Managed", type6);
                    hashtable.Add("System.Security.Cryptography.HMAC", type9);
                    hashtable.Add("System.Security.Cryptography.KeyedHashAlgorithm", type9);
                    hashtable.Add("HMACMD5", type7);
                    hashtable.Add("System.Security.Cryptography.HMACMD5", type7);
                    hashtable.Add("HMACRIPEMD160", type8);
                    hashtable.Add("System.Security.Cryptography.HMACRIPEMD160", type8);
                    hashtable.Add("HMACSHA1", type9);
                    hashtable.Add("System.Security.Cryptography.HMACSHA1", type9);
                    hashtable.Add("HMACSHA256", type10);
                    hashtable.Add("System.Security.Cryptography.HMACSHA256", type10);
                    hashtable.Add("HMACSHA384", type11);
                    hashtable.Add("System.Security.Cryptography.HMACSHA384", type11);
                    hashtable.Add("HMACSHA512", type12);
                    hashtable.Add("System.Security.Cryptography.HMACSHA512", type12);
                    hashtable.Add("MACTripleDES", type13);
                    hashtable.Add("System.Security.Cryptography.MACTripleDES", type13);
                    hashtable.Add("RSA", type14);
                    hashtable.Add("System.Security.Cryptography.RSA", type14);
                    hashtable.Add("System.Security.Cryptography.AsymmetricAlgorithm", type14);
                    hashtable.Add("DSA", type15);
                    hashtable.Add("System.Security.Cryptography.DSA", type15);
                    hashtable.Add("DES", type16);
                    hashtable.Add("System.Security.Cryptography.DES", type16);
                    hashtable.Add("3DES", type17);
                    hashtable.Add("TripleDES", type17);
                    hashtable.Add("Triple DES", type17);
                    hashtable.Add("System.Security.Cryptography.TripleDES", type17);
                    hashtable.Add("RC2", type18);
                    hashtable.Add("System.Security.Cryptography.RC2", type18);
                    hashtable.Add("Rijndael", type19);
                    hashtable.Add("System.Security.Cryptography.Rijndael", type19);
                    hashtable.Add("System.Security.Cryptography.SymmetricAlgorithm", type19);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig#dsa-sha1", type20);
                    hashtable.Add("System.Security.Cryptography.DSASignatureDescription", type20);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig#rsa-sha1", type21);
                    hashtable.Add("System.Security.Cryptography.RSASignatureDescription", type21);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig#sha1", type);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#sha256", type3);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#sha512", type5);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#ripemd160", type6);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#des-cbc", type16);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#tripledes-cbc", type17);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#kw-tripledes", type17);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#aes128-cbc", type19);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#kw-aes128", type19);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#aes192-cbc", type19);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#kw-aes192", type19);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#aes256-cbc", type19);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc#kw-aes256", type19);
                    hashtable.Add("http://www.w3.org/TR/2001/REC-xml-c14n-20010315", "System.Security.Cryptography.Xml.XmlDsigC14NTransform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments", "System.Security.Cryptography.Xml.XmlDsigC14NWithCommentsTransform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2001/10/xml-exc-c14n#", "System.Security.Cryptography.Xml.XmlDsigExcC14NTransform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2001/10/xml-exc-c14n#WithComments", "System.Security.Cryptography.Xml.XmlDsigExcC14NWithCommentsTransform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig#base64", "System.Security.Cryptography.Xml.XmlDsigBase64Transform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/TR/1999/REC-xpath-19991116", "System.Security.Cryptography.Xml.XmlDsigXPathTransform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/TR/1999/REC-xslt-19991116", "System.Security.Cryptography.Xml.XmlDsigXsltTransform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig#enveloped-signature", "System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2002/07/decrypt#XML", "System.Security.Cryptography.Xml.XmlDecryptionTransform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform", "System.Security.Cryptography.Xml.XmlLicenseTransform, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig# X509Data", "System.Security.Cryptography.Xml.KeyInfoX509Data, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig# KeyName", "System.Security.Cryptography.Xml.KeyInfoName, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig# KeyValue/DSAKeyValue", "System.Security.Cryptography.Xml.DSAKeyValue, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig# KeyValue/RSAKeyValue", "System.Security.Cryptography.Xml.RSAKeyValue, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2000/09/xmldsig# RetrievalMethod", "System.Security.Cryptography.Xml.KeyInfoRetrievalMethod, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2001/04/xmlenc# EncryptedKey", "System.Security.Cryptography.Xml.KeyInfoEncryptedKey, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("http://www.w3.org/2001/04/xmldsig-more#md5", type2);
                    hashtable.Add("http://www.w3.org/2001/04/xmldsig-more#sha384", type4);
                    hashtable.Add("http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160", type8);
                    hashtable.Add("http://www.w3.org/2001/04/xmldsig-more#hmac-sha256", type10);
                    hashtable.Add("http://www.w3.org/2001/04/xmldsig-more#hmac-sha384", type11);
                    hashtable.Add("http://www.w3.org/2001/04/xmldsig-more#hmac-sha512", type12);
                    hashtable.Add("2.5.29.10", "System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension, System, Culture=neutral, PublicKeyToken=b77a5c561934e089, Version=" + _Version);
                    hashtable.Add("2.5.29.19", "System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension, System, Culture=neutral, PublicKeyToken=b77a5c561934e089, Version=" + _Version);
                    hashtable.Add("2.5.29.14", "System.Security.Cryptography.X509Certificates.X509SubjectKeyIdentifierExtension, System, Culture=neutral, PublicKeyToken=b77a5c561934e089, Version=" + _Version);
                    hashtable.Add("2.5.29.15", "System.Security.Cryptography.X509Certificates.X509KeyUsageExtension, System, Culture=neutral, PublicKeyToken=b77a5c561934e089, Version=" + _Version);
                    hashtable.Add("2.5.29.37", "System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension, System, Culture=neutral, PublicKeyToken=b77a5c561934e089, Version=" + _Version);
                    hashtable.Add("X509Chain", "System.Security.Cryptography.X509Certificates.X509Chain, System, Culture=neutral, PublicKeyToken=b77a5c561934e089, Version=" + _Version);
                    hashtable.Add("1.2.840.113549.1.9.3", "System.Security.Cryptography.Pkcs.Pkcs9ContentType, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("1.2.840.113549.1.9.4", "System.Security.Cryptography.Pkcs.Pkcs9MessageDigest, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("1.2.840.113549.1.9.5", "System.Security.Cryptography.Pkcs.Pkcs9SigningTime, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("1.3.6.1.4.1.311.88.2.1", "System.Security.Cryptography.Pkcs.Pkcs9DocumentName, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    hashtable.Add("1.3.6.1.4.1.311.88.2.2", "System.Security.Cryptography.Pkcs.Pkcs9DocumentDescription, System.Security, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, Version=" + _Version);
                    defaultNameHT = hashtable;
                }
                return defaultNameHT;
            }
        }

        private static Hashtable DefaultOidHT
        {
            get
            {
                if (defaultOidHT == null)
                {
                    Hashtable hashtable = new Hashtable(StringComparer.OrdinalIgnoreCase) {
                        { 
                            "SHA",
                            "1.3.14.3.2.26"
                        },
                        { 
                            "SHA1",
                            "1.3.14.3.2.26"
                        },
                        { 
                            "System.Security.Cryptography.SHA1",
                            "1.3.14.3.2.26"
                        },
                        { 
                            "System.Security.Cryptography.SHA1CryptoServiceProvider",
                            "1.3.14.3.2.26"
                        },
                        { 
                            "System.Security.Cryptography.SHA1Managed",
                            "1.3.14.3.2.26"
                        },
                        { 
                            "SHA256",
                            "2.16.840.1.101.3.4.2.1"
                        },
                        { 
                            "System.Security.Cryptography.SHA256",
                            "2.16.840.1.101.3.4.2.1"
                        },
                        { 
                            "System.Security.Cryptography.SHA256Managed",
                            "2.16.840.1.101.3.4.2.1"
                        },
                        { 
                            "SHA384",
                            "2.16.840.1.101.3.4.2.2"
                        },
                        { 
                            "System.Security.Cryptography.SHA384",
                            "2.16.840.1.101.3.4.2.2"
                        },
                        { 
                            "System.Security.Cryptography.SHA384Managed",
                            "2.16.840.1.101.3.4.2.2"
                        },
                        { 
                            "SHA512",
                            "2.16.840.1.101.3.4.2.3"
                        },
                        { 
                            "System.Security.Cryptography.SHA512",
                            "2.16.840.1.101.3.4.2.3"
                        },
                        { 
                            "System.Security.Cryptography.SHA512Managed",
                            "2.16.840.1.101.3.4.2.3"
                        },
                        { 
                            "RIPEMD160",
                            "1.3.36.3.2.1"
                        },
                        { 
                            "System.Security.Cryptography.RIPEMD160",
                            "1.3.36.3.2.1"
                        },
                        { 
                            "System.Security.Cryptography.RIPEMD160Managed",
                            "1.3.36.3.2.1"
                        },
                        { 
                            "MD5",
                            "1.2.840.113549.2.5"
                        },
                        { 
                            "System.Security.Cryptography.MD5",
                            "1.2.840.113549.2.5"
                        },
                        { 
                            "System.Security.Cryptography.MD5CryptoServiceProvider",
                            "1.2.840.113549.2.5"
                        },
                        { 
                            "System.Security.Cryptography.MD5Managed",
                            "1.2.840.113549.2.5"
                        },
                        { 
                            "TripleDESKeyWrap",
                            "1.2.840.113549.1.9.16.3.6"
                        },
                        { 
                            "RC2",
                            "1.2.840.113549.3.2"
                        },
                        { 
                            "System.Security.Cryptography.RC2CryptoServiceProvider",
                            "1.2.840.113549.3.2"
                        },
                        { 
                            "DES",
                            "1.3.14.3.2.7"
                        },
                        { 
                            "System.Security.Cryptography.DESCryptoServiceProvider",
                            "1.3.14.3.2.7"
                        },
                        { 
                            "TripleDES",
                            "1.2.840.113549.3.7"
                        },
                        { 
                            "System.Security.Cryptography.TripleDESCryptoServiceProvider",
                            "1.2.840.113549.3.7"
                        }
                    };
                    defaultOidHT = hashtable;
                }
                return defaultOidHT;
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }
    }
}

