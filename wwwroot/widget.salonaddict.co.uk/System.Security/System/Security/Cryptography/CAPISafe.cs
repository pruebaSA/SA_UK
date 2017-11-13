namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal abstract class CAPISafe : System.Security.Cryptography.CAPINative
    {
        protected CAPISafe()
        {
        }

        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern System.Security.Cryptography.SafeCertContextHandle CertCreateCertificateContext([In] uint dwCertEncodingType, [In] System.Security.Cryptography.SafeLocalAllocHandle pbCertEncoded, [In] uint cbCertEncoded);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern System.Security.Cryptography.SafeCertContextHandle CertDuplicateCertificateContext([In] IntPtr pCertContext);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern bool CertFreeCertificateContext([In] IntPtr pCertContext);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CertGetCertificateChain([In] IntPtr hChainEngine, [In] System.Security.Cryptography.SafeCertContextHandle pCertContext, [In] ref System.Runtime.InteropServices.ComTypes.FILETIME pTime, [In] System.Security.Cryptography.SafeCertStoreHandle hAdditionalStore, [In] ref System.Security.Cryptography.CAPIBase.CERT_CHAIN_PARA pChainPara, [In] uint dwFlags, [In] IntPtr pvReserved, [In, Out] ref System.Security.Cryptography.SafeCertChainHandle ppChainContext);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CertGetCertificateContextProperty([In] System.Security.Cryptography.SafeCertContextHandle pCertContext, [In] uint dwPropId, [In, Out] System.Security.Cryptography.SafeLocalAllocHandle pvData, [In, Out] ref uint pcbData);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern uint CertGetPublicKeyLength([In] uint dwCertEncodingType, [In] IntPtr pPublicKey);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern uint CertNameToStrW([In] uint dwCertEncodingType, [In] IntPtr pName, [In] uint dwStrType, [In, Out] System.Security.Cryptography.SafeLocalAllocHandle psz, [In] uint csz);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CertVerifyCertificateChainPolicy([In] IntPtr pszPolicyOID, [In] System.Security.Cryptography.SafeCertChainHandle pChainContext, [In] ref System.Security.Cryptography.CAPIBase.CERT_CHAIN_POLICY_PARA pPolicyPara, [In, Out] ref System.Security.Cryptography.CAPIBase.CERT_CHAIN_POLICY_STATUS pPolicyStatus);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptAcquireCertificatePrivateKey([In] System.Security.Cryptography.SafeCertContextHandle pCert, [In] uint dwFlags, [In] IntPtr pvReserved, [In, Out] ref System.Security.Cryptography.SafeCryptProvHandle phCryptProv, [In, Out] ref uint pdwKeySpec, [In, Out] ref bool pfCallerFreeProv);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptDecodeObject([In] uint dwCertEncodingType, [In] IntPtr lpszStructType, [In] IntPtr pbEncoded, [In] uint cbEncoded, [In] uint dwFlags, [In, Out] System.Security.Cryptography.SafeLocalAllocHandle pvStructInfo, [In, Out] IntPtr pcbStructInfo);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptDecodeObject([In] uint dwCertEncodingType, [In] IntPtr lpszStructType, [In] byte[] pbEncoded, [In] uint cbEncoded, [In] uint dwFlags, [In, Out] System.Security.Cryptography.SafeLocalAllocHandle pvStructInfo, [In, Out] IntPtr pcbStructInfo);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptEncodeObject([In] uint dwCertEncodingType, [In] IntPtr lpszStructType, [In] IntPtr pvStructInfo, [In, Out] System.Security.Cryptography.SafeLocalAllocHandle pbEncoded, [In, Out] IntPtr pcbEncoded);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptEncodeObject([In] uint dwCertEncodingType, [In, MarshalAs(UnmanagedType.LPStr)] string lpszStructType, [In] IntPtr pvStructInfo, [In, Out] System.Security.Cryptography.SafeLocalAllocHandle pbEncoded, [In, Out] IntPtr pcbEncoded);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern IntPtr CryptFindOIDInfo([In] uint dwKeyType, [In] IntPtr pvKey, [In] uint dwGroupId);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern IntPtr CryptFindOIDInfo([In] uint dwKeyType, [In] System.Security.Cryptography.SafeLocalAllocHandle pvKey, [In] uint dwGroupId);
        [DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptGetProvParam([In] System.Security.Cryptography.SafeCryptProvHandle hProv, [In] uint dwParam, [In] IntPtr pbData, [In] IntPtr pdwDataLen, [In] uint dwFlags);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptMsgGetParam([In] System.Security.Cryptography.SafeCryptMsgHandle hCryptMsg, [In] uint dwParamType, [In] uint dwIndex, [In, Out] IntPtr pvData, [In, Out] IntPtr pcbData);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptMsgGetParam([In] System.Security.Cryptography.SafeCryptMsgHandle hCryptMsg, [In] uint dwParamType, [In] uint dwIndex, [In, Out] System.Security.Cryptography.SafeLocalAllocHandle pvData, [In, Out] IntPtr pcbData);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern System.Security.Cryptography.SafeCryptMsgHandle CryptMsgOpenToDecode([In] uint dwMsgEncodingType, [In] uint dwFlags, [In] uint dwMsgType, [In] IntPtr hCryptProv, [In] IntPtr pRecipientInfo, [In] IntPtr pStreamInfo);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptMsgUpdate([In] System.Security.Cryptography.SafeCryptMsgHandle hCryptMsg, [In] byte[] pbData, [In] uint cbData, [In] bool fFinal);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptMsgUpdate([In] System.Security.Cryptography.SafeCryptMsgHandle hCryptMsg, [In] IntPtr pbData, [In] uint cbData, [In] bool fFinal);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CryptMsgVerifyCountersignatureEncoded([In] IntPtr hCryptProv, [In] uint dwEncodingType, [In] IntPtr pbSignerInfo, [In] uint cbSignerInfo, [In] IntPtr pbSignerInfoCountersignature, [In] uint cbSignerInfoCountersignature, [In] IntPtr pciCountersigner);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool FreeLibrary([In] IntPtr hModule);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern IntPtr GetProcAddress([In] IntPtr hModule, [In, MarshalAs(UnmanagedType.LPStr)] string lpProcName);
        [DllImport("kernel32.dll", EntryPoint="LoadLibraryA", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern IntPtr LoadLibrary([In, MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern System.Security.Cryptography.SafeLocalAllocHandle LocalAlloc([In] uint uFlags, [In] IntPtr sizetdwBytes);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", SetLastError=true)]
        internal static extern IntPtr LocalFree(IntPtr handle);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("advapi32.dll", SetLastError=true)]
        internal static extern int LsaNtStatusToWinError([In] int status);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", SetLastError=true)]
        internal static extern void ZeroMemory(IntPtr handle, uint length);
    }
}

