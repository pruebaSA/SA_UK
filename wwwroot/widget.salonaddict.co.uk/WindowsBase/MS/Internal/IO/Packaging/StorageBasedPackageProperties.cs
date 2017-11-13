namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.Interop;
    using MS.Internal.IO.Packaging.CompoundFile;
    using MS.Internal.WindowsBase;
    using System;
    using System.IO;
    using System.IO.Packaging;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Windows;

    internal class StorageBasedPackageProperties : PackageProperties, IDisposable
    {
        private bool _disposed;
        private int _grfMode;
        private IPropertyStorage _psDocSummInfo;
        private IPropertySetStorage _pss;
        private IPropertyStorage _psSummInfo;

        internal StorageBasedPackageProperties(StorageRoot root)
        {
            this._pss = (IPropertySetStorage) root.GetRootIStorage();
            this._grfMode = 0x10;
            SafeNativeCompoundFileMethods.UpdateModeFlagFromFileAccess(root.OpenAccess, ref this._grfMode);
            this.OpenPropertyStorage(ref FormatId.SummaryInformation, out this._psSummInfo);
            this.OpenPropertyStorage(ref FormatId.DocumentSummaryInformation, out this._psDocSummInfo);
        }

        private void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StorageBasedPackagePropertiesDiposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!this._disposed && disposing)
                {
                    if (this._psSummInfo != null)
                    {
                        try
                        {
                            ((IDisposable) this._psSummInfo).Dispose();
                        }
                        finally
                        {
                            this._psSummInfo = null;
                        }
                    }
                    if (this._psDocSummInfo != null)
                    {
                        try
                        {
                            ((IDisposable) this._psDocSummInfo).Dispose();
                        }
                        finally
                        {
                            this._psDocSummInfo = null;
                        }
                    }
                }
            }
            finally
            {
                this._disposed = true;
                base.Dispose(disposing);
            }
        }

        ~StorageBasedPackageProperties()
        {
            this.Dispose(false);
        }

        private DateTime? GetDateTimeProperty(Guid fmtid, uint propId)
        {
            object oleProperty = this.GetOleProperty(fmtid, propId);
            if (oleProperty == null)
            {
                return null;
            }
            return (DateTime?) oleProperty;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private object GetOleProperty(Guid fmtid, uint propId)
        {
            this.CheckDisposed();
            IPropertyStorage storage = (fmtid == FormatId.SummaryInformation) ? this._psSummInfo : this._psDocSummInfo;
            if (storage == null)
            {
                return null;
            }
            PROPSPEC[] rgpspec = new PROPSPEC[1];
            PROPVARIANT[] rgpropvar = new PROPVARIANT[1];
            rgpspec[0].propType = 1;
            rgpspec[0].union.propId = propId;
            VARTYPE vtFromPropId = this.GetVtFromPropId(fmtid, propId);
            int hr = storage.ReadMultiple(1, rgpspec, rgpropvar);
            if (hr == 0)
            {
                try
                {
                    if (rgpropvar[0].vt != vtFromPropId)
                    {
                        throw new FileFormatException(System.Windows.SR.Get("WrongDocumentPropertyVariantType", new object[] { propId, fmtid.ToString(), rgpropvar[0].vt, vtFromPropId }));
                    }
                    switch (rgpropvar[0].vt)
                    {
                        case VARTYPE.VT_LPSTR:
                        {
                            IntPtr pszVal = rgpropvar[0].union.pszVal;
                            int length = Marshal.PtrToStringAnsi(pszVal).Length;
                            byte[] destination = new byte[length];
                            Marshal.Copy(pszVal, destination, 0, length);
                            return Encoding.UTF8.GetString(destination);
                        }
                        case VARTYPE.VT_FILETIME:
                            return new DateTime?(DateTime.FromFileTime(rgpropvar[0].union.hVal));
                    }
                    throw new FileFormatException(System.Windows.SR.Get("InvalidDocumentPropertyVariantType", new object[] { rgpropvar[0].vt }));
                }
                finally
                {
                    SafeNativeCompoundFileMethods.SafePropVariantClear(ref rgpropvar[0]);
                }
            }
            if (hr != 1)
            {
                SecurityHelper.ThrowExceptionForHR(hr);
            }
            return null;
        }

        private VARTYPE GetVtFromPropId(Guid fmtid, uint propId)
        {
            if (!(fmtid == FormatId.SummaryInformation))
            {
                if (fmtid == FormatId.DocumentSummaryInformation)
                {
                    switch (propId)
                    {
                        case 0x1a:
                        case 0x1b:
                        case 0x1c:
                        case 0x1d:
                        case 2:
                        case 0x12:
                            return VARTYPE.VT_LPSTR;
                    }
                    throw new ArgumentException(System.Windows.SR.Get("UnknownDocumentProperty", new object[] { fmtid.ToString(), propId }), "propId");
                }
                throw new ArgumentException(System.Windows.SR.Get("UnknownDocumentProperty", new object[] { fmtid.ToString(), propId }), "fmtid");
            }
            switch (propId)
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 8:
                case 9:
                    return VARTYPE.VT_LPSTR;

                case 11:
                case 12:
                case 13:
                    return VARTYPE.VT_FILETIME;
            }
            throw new ArgumentException(System.Windows.SR.Get("UnknownDocumentProperty", new object[] { fmtid.ToString(), propId }), "propId");
        }

        private void OpenPropertyStorage(ref Guid fmtid, out IPropertyStorage ips)
        {
            int hr = this._pss.Open(ref fmtid, (uint) this._grfMode, out ips);
            if (hr == -2147287038)
            {
                ips = null;
            }
            else
            {
                SecurityHelper.ThrowExceptionForHR(hr);
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private void SetOleProperty(Guid fmtid, uint propId, object propVal)
        {
            this.CheckDisposed();
            IPropertyStorage ppprstg = (fmtid == FormatId.SummaryInformation) ? this._psSummInfo : this._psDocSummInfo;
            if (ppprstg == null)
            {
                if (propVal == null)
                {
                    return;
                }
                this._pss.Create(ref fmtid, ref fmtid, 2, (uint) this._grfMode, out ppprstg);
                if (fmtid == FormatId.SummaryInformation)
                {
                    this._psSummInfo = ppprstg;
                }
                else
                {
                    this._psDocSummInfo = ppprstg;
                }
            }
            PROPSPEC[] rgpspec = new PROPSPEC[1];
            PROPVARIANT[] rgpropvar = new PROPVARIANT[1];
            rgpspec[0].propType = 1;
            rgpspec[0].union.propId = propId;
            if (propVal == null)
            {
                ppprstg.DeleteMultiple(1, rgpspec);
            }
            else
            {
                IntPtr zero = IntPtr.Zero;
                try
                {
                    if (propVal is string)
                    {
                        string s = propVal as string;
                        zero = Marshal.StringToCoTaskMemAnsi(s);
                        string strB = Marshal.PtrToStringAnsi(zero);
                        if (string.CompareOrdinal(s, strB) != 0)
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(s);
                            int length = bytes.Length;
                            if (zero != IntPtr.Zero)
                            {
                                Marshal.FreeCoTaskMem(zero);
                                zero = IntPtr.Zero;
                            }
                            zero = Marshal.AllocCoTaskMem(length + 1);
                            Marshal.Copy(bytes, 0, zero, length);
                            Marshal.WriteByte(zero, length, 0);
                        }
                        rgpropvar[0].vt = VARTYPE.VT_LPSTR;
                        rgpropvar[0].union.pszVal = zero;
                    }
                    else
                    {
                        if (!(propVal is DateTime))
                        {
                            throw new ArgumentException(System.Windows.SR.Get("InvalidDocumentPropertyType", new object[] { propVal.GetType().ToString() }), "propVal");
                        }
                        rgpropvar[0].vt = VARTYPE.VT_FILETIME;
                        rgpropvar[0].union.hVal = ((DateTime) propVal).ToFileTime();
                    }
                    ppprstg.WriteMultiple(1, rgpspec, rgpropvar, 0);
                }
                finally
                {
                    if (zero != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(zero);
                    }
                }
            }
        }

        public override string Category
        {
            get => 
                (this.GetOleProperty(FormatId.DocumentSummaryInformation, 2) as string);
            set
            {
                this.SetOleProperty(FormatId.DocumentSummaryInformation, 2, value);
            }
        }

        public override string ContentStatus
        {
            get => 
                (this.GetOleProperty(FormatId.DocumentSummaryInformation, 0x1d) as string);
            set
            {
                this.SetOleProperty(FormatId.DocumentSummaryInformation, 0x1d, value);
            }
        }

        public override string ContentType
        {
            get
            {
                string oleProperty = this.GetOleProperty(FormatId.DocumentSummaryInformation, 0x1a) as string;
                if (oleProperty == null)
                {
                    return oleProperty;
                }
                return new MS.Internal.ContentType(oleProperty).ToString();
            }
            set
            {
                if (value == null)
                {
                    this.SetOleProperty(FormatId.DocumentSummaryInformation, 0x1a, value);
                }
                else
                {
                    this.SetOleProperty(FormatId.DocumentSummaryInformation, 0x1a, new MS.Internal.ContentType(value).ToString());
                }
            }
        }

        public override DateTime? Created
        {
            get => 
                this.GetDateTimeProperty(FormatId.SummaryInformation, 12);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 12, value);
            }
        }

        public override string Creator
        {
            get => 
                (this.GetOleProperty(FormatId.SummaryInformation, 4) as string);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 4, value);
            }
        }

        public override string Description
        {
            get => 
                (this.GetOleProperty(FormatId.SummaryInformation, 6) as string);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 6, value);
            }
        }

        public override string Identifier
        {
            get => 
                (this.GetOleProperty(FormatId.DocumentSummaryInformation, 0x12) as string);
            set
            {
                this.SetOleProperty(FormatId.DocumentSummaryInformation, 0x12, value);
            }
        }

        public override string Keywords
        {
            get => 
                (this.GetOleProperty(FormatId.SummaryInformation, 5) as string);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 5, value);
            }
        }

        public override string Language
        {
            get => 
                (this.GetOleProperty(FormatId.DocumentSummaryInformation, 0x1b) as string);
            set
            {
                this.SetOleProperty(FormatId.DocumentSummaryInformation, 0x1b, value);
            }
        }

        public override string LastModifiedBy
        {
            get => 
                (this.GetOleProperty(FormatId.SummaryInformation, 8) as string);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 8, value);
            }
        }

        public override DateTime? LastPrinted
        {
            get => 
                this.GetDateTimeProperty(FormatId.SummaryInformation, 11);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 11, value);
            }
        }

        public override DateTime? Modified
        {
            get => 
                this.GetDateTimeProperty(FormatId.SummaryInformation, 13);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 13, value);
            }
        }

        public override string Revision
        {
            get => 
                (this.GetOleProperty(FormatId.SummaryInformation, 9) as string);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 9, value);
            }
        }

        public override string Subject
        {
            get => 
                (this.GetOleProperty(FormatId.SummaryInformation, 3) as string);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 3, value);
            }
        }

        public override string Title
        {
            get => 
                (this.GetOleProperty(FormatId.SummaryInformation, 2) as string);
            set
            {
                this.SetOleProperty(FormatId.SummaryInformation, 2, value);
            }
        }

        public override string Version
        {
            get => 
                (this.GetOleProperty(FormatId.DocumentSummaryInformation, 0x1c) as string);
            set
            {
                this.SetOleProperty(FormatId.DocumentSummaryInformation, 0x1c, value);
            }
        }
    }
}

