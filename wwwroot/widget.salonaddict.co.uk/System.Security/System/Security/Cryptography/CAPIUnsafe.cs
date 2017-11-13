namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal abstract class CAPIUnsafe : System.Security.Cryptography.CAPISafe
    {
        protected CAPIUnsafe()
        {
        }

        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern bool CertAddCertificateContextToStore([In] System.Security.Cryptography.SafeCertStoreHandle hCertStore, [In] System.Security.Cryptography.SafeCertContextHandle pCertContext, [In] uint dwAddDisposition, [In, Out] System.Security.Cryptography.SafeCertContextHandle ppStoreContext);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern bool CertAddCertificateLinkToStore([In] System.Security.Cryptography.SafeCertStoreHandle hCertStore, [In] System.Security.Cryptography.SafeCertContextHandle pCertContext, [In] uint dwAddDisposition, [In, Out] System.Security.Cryptography.SafeCertContextHandle ppStoreContext);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern System.Security.Cryptography.SafeCertContextHandle CertCreateSelfSignCertificate([In] System.Security.Cryptography.SafeCryptProvHandle hProv, [In] IntPtr pSubjectIssuerBlob, [In] uint dwFlags, [In] IntPtr pKeyProvInfo, [In] IntPtr pSignatureAlgorithm, [In] IntPtr pStartTime, [In] IntPtr pEndTime, [In] IntPtr pExtensions);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern IntPtr CertEnumCertificatesInStore([In] System.Security.Cryptography.SafeCertStoreHandle hCertStore, [In] IntPtr pPrevCertContext);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern System.Security.Cryptography.SafeCertContextHandle CertFindCertificateInStore([In] System.Security.Cryptography.SafeCertStoreHandle hCertStore, [In] uint dwCertEncodingType, [In] uint dwFindFlags, [In] uint dwFindType, [In] IntPtr pvFindPara, [In] System.Security.Cryptography.SafeCertContextHandle pPrevCertContext);
        [DllImport("crypt32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        protected internal static extern System.Security.Cryptography.SafeCertStoreHandle CertOpenStore([In] IntPtr lpszStoreProvider, [In] uint dwMsgAndCertEncodingType, [In] IntPtr hCryptProv, [In] uint dwFlags, [In] string pvPara);
        [DllImport("advapi32.dll", EntryPoint="CryptAcquireContextA", CharSet=CharSet.Auto)]
        protected internal static extern bool CryptAcquireContext([In, Out] ref System.Security.Cryptography.SafeCryptProvHandle hCryptProv, [In, MarshalAs(UnmanagedType.LPStr)] string pszContainer, [In, MarshalAs(UnmanagedType.LPStr)] string pszProvider, [In] uint dwProvType, [In] uint dwFlags);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern bool CryptMsgControl([In] System.Security.Cryptography.SafeCryptMsgHandle hCryptMsg, [In] uint dwFlags, [In] uint dwCtrlType, [In] IntPtr pvCtrlPara);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern bool CryptMsgCountersign([In] System.Security.Cryptography.SafeCryptMsgHandle hCryptMsg, [In] uint dwIndex, [In] uint cCountersigners, [In] IntPtr rgCountersigners);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern System.Security.Cryptography.SafeCryptMsgHandle CryptMsgOpenToEncode([In] uint dwMsgEncodingType, [In] uint dwFlags, [In] uint dwMsgType, [In] IntPtr pvMsgEncodeInfo, [In] IntPtr pszInnerContentObjID, [In] IntPtr pStreamInfo);
        [DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        protected internal static extern System.Security.Cryptography.SafeCryptMsgHandle CryptMsgOpenToEncode([In] uint dwMsgEncodingType, [In] uint dwFlags, [In] uint dwMsgType, [In] IntPtr pvMsgEncodeInfo, [In, MarshalAs(UnmanagedType.LPStr)] string pszInnerContentObjID, [In] IntPtr pStreamInfo);
        [DllImport("crypt32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern bool CryptProtectData([In] IntPtr pDataIn, [In] string szDataDescr, [In] IntPtr pOptionalEntropy, [In] IntPtr pvReserved, [In] IntPtr pPromptStruct, [In] uint dwFlags, [In, Out] IntPtr pDataBlob);
        [DllImport("cryptui.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        protected internal static extern System.Security.Cryptography.SafeCertContextHandle CryptUIDlgSelectCertificateW([In, Out, MarshalAs(UnmanagedType.LPStruct)] System.Security.Cryptography.CAPIBase.CRYPTUI_SELECTCERTIFICATE_STRUCTW csc);
        [DllImport("cryptui.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        protected internal static extern bool CryptUIDlgViewCertificateW([In, MarshalAs(UnmanagedType.LPStruct)] System.Security.Cryptography.CAPIBase.CRYPTUI_VIEWCERTIFICATE_STRUCTW ViewInfo, [In, Out] IntPtr pfPropertiesChanged);
        [DllImport("crypt32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern bool CryptUnprotectData([In] IntPtr pDataIn, [In] IntPtr ppszDataDescr, [In] IntPtr pOptionalEntropy, [In] IntPtr pvReserved, [In] IntPtr pPromptStruct, [In] uint dwFlags, [In, Out] IntPtr pDataBlob);
        [DllImport("advapi32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern int SystemFunction040([In, Out] byte[] pDataIn, [In] uint cbDataIn, [In] uint dwFlags);
        [DllImport("advapi32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern int SystemFunction041([In, Out] byte[] pDataIn, [In] uint cbDataIn, [In] uint dwFlags);
    }
}

