namespace System.Security.Cryptography.X509Certificates
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class X509Certificate2UI
    {
        private X509Certificate2UI()
        {
        }

        public static void DisplayCertificate(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }
            DisplayX509Certificate(System.Security.Cryptography.X509Certificates.X509Utils.GetCertContext(certificate), IntPtr.Zero);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void DisplayCertificate(X509Certificate2 certificate, IntPtr hwndParent)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }
            DisplayX509Certificate(System.Security.Cryptography.X509Certificates.X509Utils.GetCertContext(certificate), hwndParent);
        }

        private static void DisplayX509Certificate(System.Security.Cryptography.SafeCertContextHandle safeCertContext, IntPtr hwndParent)
        {
            if (safeCertContext.IsInvalid)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_InvalidHandle"), "safeCertContext");
            }
            int num = 0;
            System.Security.Cryptography.CAPIBase.CRYPTUI_VIEWCERTIFICATE_STRUCTW structure = new System.Security.Cryptography.CAPIBase.CRYPTUI_VIEWCERTIFICATE_STRUCTW();
            structure.dwSize = (uint) Marshal.SizeOf(structure);
            structure.hwndParent = hwndParent;
            structure.dwFlags = 0;
            structure.szTitle = null;
            structure.pCertContext = safeCertContext.DangerousGetHandle();
            structure.rgszPurposes = IntPtr.Zero;
            structure.cPurposes = 0;
            structure.pCryptProviderData = IntPtr.Zero;
            structure.fpCryptProviderDataTrustedUsage = false;
            structure.idxSigner = 0;
            structure.idxCert = 0;
            structure.fCounterSigner = false;
            structure.idxCounterSigner = 0;
            structure.cStores = 0;
            structure.rghStores = IntPtr.Zero;
            structure.cPropSheetPages = 0;
            structure.rgPropSheetPages = IntPtr.Zero;
            structure.nStartPage = 0;
            if (!System.Security.Cryptography.CAPI.CryptUIDlgViewCertificateW(structure, IntPtr.Zero))
            {
                num = Marshal.GetLastWin32Error();
            }
            if ((num != 0) && (num != 0x4c7))
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }
        }

        public static X509Certificate2Collection SelectFromCollection(X509Certificate2Collection certificates, string title, string message, X509SelectionFlag selectionFlag) => 
            SelectFromCollectionHelper(certificates, title, message, selectionFlag, IntPtr.Zero);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static X509Certificate2Collection SelectFromCollection(X509Certificate2Collection certificates, string title, string message, X509SelectionFlag selectionFlag, IntPtr hwndParent) => 
            SelectFromCollectionHelper(certificates, title, message, selectionFlag, hwndParent);

        private static X509Certificate2Collection SelectFromCollectionHelper(X509Certificate2Collection certificates, string title, string message, X509SelectionFlag selectionFlag, IntPtr hwndParent)
        {
            if (certificates == null)
            {
                throw new ArgumentNullException("certificates");
            }
            if ((selectionFlag < X509SelectionFlag.SingleSelection) || (selectionFlag > X509SelectionFlag.MultiSelection))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SecurityResources.GetResourceString("Arg_EnumIllegalVal"), new object[] { "selectionFlag" }));
            }
            new StorePermission(StorePermissionFlags.AllFlags).Assert();
            System.Security.Cryptography.SafeCertStoreHandle safeSourceStoreHandle = System.Security.Cryptography.X509Certificates.X509Utils.ExportToMemoryStore(certificates);
            System.Security.Cryptography.SafeCertStoreHandle invalidHandle = System.Security.Cryptography.SafeCertStoreHandle.InvalidHandle;
            invalidHandle = SelectFromStore(safeSourceStoreHandle, title, message, selectionFlag, hwndParent);
            X509Certificate2Collection certificates2 = System.Security.Cryptography.X509Certificates.X509Utils.GetCertificates(invalidHandle);
            invalidHandle.Dispose();
            safeSourceStoreHandle.Dispose();
            return certificates2;
        }

        private static unsafe System.Security.Cryptography.SafeCertStoreHandle SelectFromStore(System.Security.Cryptography.SafeCertStoreHandle safeSourceStoreHandle, string title, string message, X509SelectionFlag selectionFlags, IntPtr hwndParent)
        {
            int num = 0;
            System.Security.Cryptography.SafeCertStoreHandle hCertStore = System.Security.Cryptography.CAPI.CertOpenStore((IntPtr) 2L, 0x10001, IntPtr.Zero, 0, null);
            if ((hCertStore == null) || hCertStore.IsInvalid)
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }
            System.Security.Cryptography.CAPIBase.CRYPTUI_SELECTCERTIFICATE_STRUCTW csc = new System.Security.Cryptography.CAPIBase.CRYPTUI_SELECTCERTIFICATE_STRUCTW {
                dwSize = (uint) ((int) Marshal.OffsetOf(typeof(System.Security.Cryptography.CAPIBase.CRYPTUI_SELECTCERTIFICATE_STRUCTW), "hSelectedCertStore")),
                hwndParent = hwndParent,
                dwFlags = (uint) selectionFlags,
                szTitle = title,
                dwDontUseColumn = 0,
                szDisplayString = message,
                pFilterCallback = IntPtr.Zero,
                pDisplayCallback = IntPtr.Zero,
                pvCallbackData = IntPtr.Zero,
                cDisplayStores = 1
            };
            IntPtr handle = safeSourceStoreHandle.DangerousGetHandle();
            csc.rghDisplayStores = new IntPtr((void*) &handle);
            csc.cStores = 0;
            csc.rghStores = IntPtr.Zero;
            csc.cPropSheetPages = 0;
            csc.rgPropSheetPages = IntPtr.Zero;
            csc.hSelectedCertStore = hCertStore.DangerousGetHandle();
            System.Security.Cryptography.SafeCertContextHandle pCertContext = System.Security.Cryptography.CAPI.CryptUIDlgSelectCertificateW(csc);
            if ((pCertContext != null) && !pCertContext.IsInvalid)
            {
                System.Security.Cryptography.SafeCertContextHandle invalidHandle = System.Security.Cryptography.SafeCertContextHandle.InvalidHandle;
                if (!System.Security.Cryptography.CAPI.CertAddCertificateContextToStore(hCertStore, pCertContext, 7, invalidHandle))
                {
                    num = Marshal.GetLastWin32Error();
                }
            }
            if (num != 0)
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }
            return hCertStore;
        }
    }
}

