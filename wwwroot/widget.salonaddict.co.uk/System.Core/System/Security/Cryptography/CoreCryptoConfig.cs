namespace System.Security.Cryptography
{
    using Microsoft.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Security.Permissions;

    internal static class CoreCryptoConfig
    {
        private static bool? s_enforceFipsAlgorithms;
        private static Dictionary<string, Type> s_nameMap;

        [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
        public static T CreateFromName<T>(string name) where T: class
        {
            Type type;
            Contract.Requires(name != null);
            if (AlgorithmNameMap.TryGetValue(name, out type))
            {
                return (T) Activator.CreateInstance(type);
            }
            return (T) CryptoConfig.CreateFromName(name);
        }

        private static Dictionary<string, Type> AlgorithmNameMap
        {
            get
            {
                if (s_nameMap == null)
                {
                    Dictionary<string, Type> dictionary = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) {
                        { 
                            "AES",
                            typeof(AesCryptoServiceProvider)
                        },
                        { 
                            typeof(AesCryptoServiceProvider).Name,
                            typeof(AesCryptoServiceProvider)
                        },
                        { 
                            typeof(AesCryptoServiceProvider).FullName,
                            typeof(AesCryptoServiceProvider)
                        },
                        { 
                            typeof(AesManaged).Name,
                            typeof(AesManaged)
                        },
                        { 
                            typeof(AesManaged).FullName,
                            typeof(AesManaged)
                        },
                        { 
                            "ECDsa",
                            typeof(ECDsaCng)
                        },
                        { 
                            typeof(ECDsaCng).Name,
                            typeof(ECDsaCng)
                        },
                        { 
                            typeof(ECDsaCng).FullName,
                            typeof(ECDsaCng)
                        },
                        { 
                            "ECDH",
                            typeof(ECDiffieHellmanCng)
                        },
                        { 
                            "ECDiffieHellman",
                            typeof(ECDiffieHellmanCng)
                        },
                        { 
                            typeof(ECDiffieHellmanCng).Name,
                            typeof(ECDiffieHellmanCng)
                        },
                        { 
                            typeof(ECDiffieHellmanCng).FullName,
                            typeof(ECDiffieHellmanCng)
                        }
                    };
                    s_nameMap = dictionary;
                }
                return s_nameMap;
            }
        }

        internal static bool EnforceFipsAlgorithms
        {
            get
            {
                if (!s_enforceFipsAlgorithms.HasValue)
                {
                    try
                    {
                        using (new SHA1Managed())
                        {
                            s_enforceFipsAlgorithms = false;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        s_enforceFipsAlgorithms = true;
                    }
                }
                return s_enforceFipsAlgorithms.Value;
            }
        }
    }
}

