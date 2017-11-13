namespace PdfSharp.Pdf
{
    using PdfSharp;
    using PdfSharp.Pdf.AcroForms;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.IO;
    using PdfSharp.Pdf.Security;
    using System;
    using System.Diagnostics;
    using System.IO;

    [DebuggerDisplay("(Name={Name})")]
    public sealed class PdfDocument : PdfObject, IDisposable
    {
        private PdfCatalog catalog;
        internal DateTime creation;
        private PdfCustomValues customValues;
        private PdfExtGStateTable extGStateTable;
        internal long fileSize;
        private PdfFontTable fontTable;
        private PdfFormXObjectTable formTable;
        internal string fullPath;
        private System.Guid guid;
        private DocumentHandle handle;
        private PdfImageTable imageTable;
        private PdfDocumentInformation info;
        private PdfInternals internals;
        internal PdfReferenceTable irefTable;
        internal Lexer lexer;
        private string name;
        private static int nameCount;
        internal PdfDocumentOpenMode openMode;
        private PdfDocumentOptions options;
        internal Stream outStream;
        private PdfPages pages;
        internal PdfSecuritySettings securitySettings;
        private PdfDocumentSettings settings;
        internal DocumentState state;
        private object tag;
        [ThreadStatic]
        private static ThreadLocalStorage tls;
        internal PdfTrailer trailer;
        internal int version;

        public PdfDocument()
        {
            this.name = NewName();
            this.fullPath = string.Empty;
            this.guid = System.Guid.NewGuid();
            this.creation = DateTime.Now;
            this.state = DocumentState.Created;
            this.version = 14;
            this.Initialize();
            this.Info.CreationDate = this.creation;
        }

        internal PdfDocument(Lexer lexer)
        {
            this.name = NewName();
            this.fullPath = string.Empty;
            this.guid = System.Guid.NewGuid();
            this.creation = DateTime.Now;
            this.state = DocumentState.Imported;
            this.irefTable = new PdfReferenceTable(this);
            this.lexer = lexer;
        }

        public PdfDocument(Stream outputStream)
        {
            this.name = NewName();
            this.fullPath = string.Empty;
            this.guid = System.Guid.NewGuid();
            this.creation = DateTime.Now;
            this.state = DocumentState.Created;
            this.Initialize();
            this.Info.CreationDate = this.creation;
            this.outStream = outputStream;
        }

        public PdfDocument(string filename)
        {
            this.name = NewName();
            this.fullPath = string.Empty;
            this.guid = System.Guid.NewGuid();
            this.creation = DateTime.Now;
            this.state = DocumentState.Created;
            this.version = 14;
            this.Initialize();
            this.Info.CreationDate = this.creation;
            this.outStream = new FileStream(filename, FileMode.Create);
        }

        public PdfPage AddPage()
        {
            if (!this.CanModify)
            {
                throw new InvalidOperationException(PSSR.CannotModify);
            }
            return this.Catalog.Pages.Add();
        }

        public PdfPage AddPage(PdfPage page)
        {
            if (!this.CanModify)
            {
                throw new InvalidOperationException(PSSR.CannotModify);
            }
            return this.Catalog.Pages.Add(page);
        }

        public bool CanSave(ref string message)
        {
            if (!this.SecuritySettings.CanSave(ref message))
            {
                return false;
            }
            return true;
        }

        public void Close()
        {
            if (!this.CanModify)
            {
                throw new InvalidOperationException(PSSR.CannotModify);
            }
            if (this.outStream != null)
            {
                PdfStandardSecurityHandler securityHandler = null;
                if (this.SecuritySettings.DocumentSecurityLevel != PdfDocumentSecurityLevel.None)
                {
                    securityHandler = this.SecuritySettings.SecurityHandler;
                }
                PdfWriter writer = new PdfWriter(this.outStream, securityHandler);
                try
                {
                    this.DoSave(writer);
                }
                finally
                {
                    writer.Close();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (this.state != DocumentState.Disposed)
            {
            }
            this.state = DocumentState.Disposed;
        }

        internal Exception DocumentNotImported() => 
            new InvalidOperationException("Document not imported.");

        private void DoSave(PdfWriter writer)
        {
            if ((this.pages == null) || (this.pages.Count == 0))
            {
                throw new InvalidOperationException("Cannot save a PDF document with no pages.");
            }
            try
            {
                bool flag = this.securitySettings.DocumentSecurityLevel != PdfDocumentSecurityLevel.None;
                if (flag)
                {
                    PdfStandardSecurityHandler securityHandler = this.securitySettings.SecurityHandler;
                    if (securityHandler.Reference == null)
                    {
                        this.irefTable.Add(securityHandler);
                    }
                    this.trailer.Elements["/Encrypt"] = this.securitySettings.SecurityHandler.Reference;
                }
                else
                {
                    this.trailer.Elements.Remove("/Encrypt");
                }
                this.PrepareForSave();
                if (flag)
                {
                    this.securitySettings.SecurityHandler.PrepareEncryption();
                }
                writer.WriteFileHeader(this);
                PdfReference[] allReferences = this.irefTable.AllReferences;
                int length = allReferences.Length;
                for (int i = 0; i < length; i++)
                {
                    PdfReference reference = allReferences[i];
                    reference.Position = writer.Position;
                    reference.Value.WriteObject(writer);
                }
                int position = writer.Position;
                this.irefTable.WriteObject(writer);
                writer.WriteRaw("trailer\n");
                this.trailer.Elements.SetInteger("/Size", length + 1);
                this.trailer.WriteObject(writer);
                writer.WriteEof(this, position);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Stream.Flush();
                }
            }
        }

        internal bool HasVersion(string version) => 
            (string.Compare(this.Catalog.Version, version) >= 0);

        private void Initialize()
        {
            this.fontTable = new PdfFontTable(this);
            this.imageTable = new PdfImageTable(this);
            this.trailer = new PdfTrailer(this);
            this.irefTable = new PdfReferenceTable(this);
            this.trailer.CreateNewDocumentIDs();
        }

        public PdfPage InsertPage(int index)
        {
            if (!this.CanModify)
            {
                throw new InvalidOperationException(PSSR.CannotModify);
            }
            return this.Catalog.Pages.Insert(index);
        }

        public PdfPage InsertPage(int index, PdfPage page)
        {
            if (!this.CanModify)
            {
                throw new InvalidOperationException(PSSR.CannotModify);
            }
            return this.Catalog.Pages.Insert(index, page);
        }

        private static string NewName() => 
            ("Document " + nameCount++);

        internal void OnExternalDocumentFinalized(DocumentHandle handle)
        {
            if (tls != null)
            {
                tls.DetachDocument(handle);
            }
            if (this.formTable != null)
            {
                this.formTable.DetachDocument(handle);
            }
        }

        internal override void PrepareForSave()
        {
            PdfDocumentInformation info = this.Info;
            if (info.Elements["/Creator"] == null)
            {
                info.Creator = "PDFsharp 1.32.2608-g (www.pdfsharp.net)";
            }
            string producer = info.Producer;
            if (producer.Length == 0)
            {
                producer = "PDFsharp 1.32.2608-g (www.pdfsharp.net)";
            }
            else if (!producer.StartsWith("PDFsharp"))
            {
                producer = "PDFsharp 1.32.2608-g (www.pdfsharp.net) (Original: " + producer + ")";
            }
            info.Elements.SetString("/Producer", producer);
            if (this.fontTable != null)
            {
                this.fontTable.PrepareForSave();
            }
            this.Catalog.PrepareForSave();
            this.irefTable.Compact();
            this.irefTable.Renumber();
        }

        public void Save(Stream stream)
        {
            this.Save(stream, true);
        }

        public void Save(string path)
        {
            if (!this.CanModify)
            {
                throw new InvalidOperationException(PSSR.CannotModify);
            }
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            this.Save(stream);
        }

        public void Save(Stream stream, bool closeStream)
        {
            if (!this.CanModify)
            {
                throw new InvalidOperationException(PSSR.CannotModify);
            }
            string message = "";
            if (!this.CanSave(ref message))
            {
                throw new PdfSharpException(message);
            }
            PdfStandardSecurityHandler securityHandler = null;
            if (this.SecuritySettings.DocumentSecurityLevel != PdfDocumentSecurityLevel.None)
            {
                securityHandler = this.SecuritySettings.SecurityHandler;
            }
            PdfWriter writer = new PdfWriter(stream, securityHandler);
            try
            {
                this.DoSave(writer);
            }
            finally
            {
                if (stream != null)
                {
                    if (closeStream)
                    {
                        stream.Close();
                    }
                    else
                    {
                        stream.Position = 0L;
                    }
                }
                if (writer != null)
                {
                    writer.Close(closeStream);
                }
            }
        }

        public PdfAcroForm AcroForm =>
            this.Catalog.AcroForm;

        internal bool CanModify =>
            (this.openMode == PdfDocumentOpenMode.Modify);

        internal PdfCatalog Catalog
        {
            get
            {
                if (this.catalog == null)
                {
                    this.catalog = this.trailer.Root;
                }
                return this.catalog;
            }
        }

        public PdfCustomValues CustomValues
        {
            get
            {
                if (this.customValues == null)
                {
                    this.customValues = PdfCustomValues.Get(this.Catalog.Elements);
                }
                return this.customValues;
            }
            set
            {
                if (value != null)
                {
                    throw new ArgumentException("Only null is allowed to clear all custom values.");
                }
                PdfCustomValues.Remove(this.Catalog.Elements);
                this.customValues = null;
            }
        }

        internal bool EarlyWrite =>
            false;

        internal PdfExtGStateTable ExtGStateTable
        {
            get
            {
                if (this.extGStateTable == null)
                {
                    this.extGStateTable = new PdfExtGStateTable(this);
                }
                return this.extGStateTable;
            }
        }

        public long FileSize =>
            this.fileSize;

        internal PdfFontTable FontTable
        {
            get
            {
                if (this.fontTable == null)
                {
                    this.fontTable = new PdfFontTable(this);
                }
                return this.fontTable;
            }
        }

        internal PdfFormXObjectTable FormTable
        {
            get
            {
                if (this.formTable == null)
                {
                    this.formTable = new PdfFormXObjectTable(this);
                }
                return this.formTable;
            }
        }

        public string FullPath =>
            this.fullPath;

        public System.Guid Guid =>
            this.guid;

        internal DocumentHandle Handle
        {
            get
            {
                if (this.handle == null)
                {
                    this.handle = new DocumentHandle(this);
                }
                return this.handle;
            }
        }

        internal PdfImageTable ImageTable
        {
            get
            {
                if (this.imageTable == null)
                {
                    this.imageTable = new PdfImageTable(this);
                }
                return this.imageTable;
            }
        }

        public PdfDocumentInformation Info
        {
            get
            {
                if (this.info == null)
                {
                    this.info = this.trailer.Info;
                }
                return this.info;
            }
        }

        public PdfInternals Internals
        {
            get
            {
                if (this.internals == null)
                {
                    this.internals = new PdfInternals(this);
                }
                return this.internals;
            }
        }

        public bool IsImported =>
            ((this.state & DocumentState.Imported) != 0);

        public bool IsReadOnly =>
            (this.openMode != PdfDocumentOpenMode.Modify);

        public string Language
        {
            get => 
                this.Catalog.Elements.GetString("/Lang");
            set
            {
                this.Catalog.Elements.SetString("/Lang", value);
            }
        }

        private string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        public PdfDocumentOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    this.options = new PdfDocumentOptions(this);
                }
                return this.options;
            }
        }

        public PdfOutline.PdfOutlineCollection Outlines =>
            this.Catalog.Outlines;

        public int PageCount
        {
            get
            {
                if (this.CanModify)
                {
                    return this.Pages.Count;
                }
                PdfDictionary dictionary = (PdfDictionary) this.Catalog.Elements.GetObject("/Pages");
                return dictionary.Elements.GetInteger("/Count");
            }
        }

        public PdfPageLayout PageLayout
        {
            get => 
                this.Catalog.PageLayout;
            set
            {
                if (!this.CanModify)
                {
                    throw new InvalidOperationException(PSSR.CannotModify);
                }
                this.Catalog.PageLayout = value;
            }
        }

        public PdfPageMode PageMode
        {
            get => 
                this.Catalog.PageMode;
            set
            {
                if (!this.CanModify)
                {
                    throw new InvalidOperationException(PSSR.CannotModify);
                }
                this.Catalog.PageMode = value;
            }
        }

        public PdfPages Pages
        {
            get
            {
                if (this.pages == null)
                {
                    this.pages = this.Catalog.Pages;
                }
                return this.pages;
            }
        }

        public PdfStandardSecurityHandler SecurityHandler =>
            this.trailer.SecurityHandler;

        public PdfSecuritySettings SecuritySettings
        {
            get
            {
                if (this.securitySettings == null)
                {
                    this.securitySettings = new PdfSecuritySettings(this);
                }
                return this.securitySettings;
            }
        }

        public PdfDocumentSettings Settings
        {
            get
            {
                if (this.settings == null)
                {
                    this.settings = new PdfDocumentSettings(this);
                }
                return this.settings;
            }
        }

        public object Tag
        {
            get => 
                this.tag;
            set
            {
                this.tag = value;
            }
        }

        internal static ThreadLocalStorage Tls
        {
            get
            {
                if (tls == null)
                {
                    tls = new ThreadLocalStorage();
                }
                return tls;
            }
        }

        public int Version
        {
            get => 
                this.version;
            set
            {
                if (!this.CanModify)
                {
                    throw new InvalidOperationException(PSSR.CannotModify);
                }
                if ((value < 12) || (value > 0x11))
                {
                    throw new ArgumentException(PSSR.InvalidVersionNumber, "value");
                }
                this.version = value;
            }
        }

        public PdfViewerPreferences ViewerPreferences =>
            this.Catalog.ViewerPreferences;

        [DebuggerDisplay("(ID={ID}, alive={IsAlive})")]
        internal class DocumentHandle
        {
            public string ID;
            private WeakReference weakRef;

            public DocumentHandle(PdfDocument document)
            {
                this.weakRef = new WeakReference(document);
                this.ID = document.guid.ToString("B").ToUpper();
            }

            public override bool Equals(object obj)
            {
                PdfDocument.DocumentHandle objA = obj as PdfDocument.DocumentHandle;
                return (!object.ReferenceEquals(objA, null) && (this.ID == objA.ID));
            }

            public override int GetHashCode() => 
                this.ID.GetHashCode();

            public static bool operator ==(PdfDocument.DocumentHandle left, PdfDocument.DocumentHandle right)
            {
                if (object.ReferenceEquals(left, null))
                {
                    return object.ReferenceEquals(right, null);
                }
                return left.Equals(right);
            }

            public static bool operator !=(PdfDocument.DocumentHandle left, PdfDocument.DocumentHandle right) => 
                !(left == right);

            public bool IsAlive =>
                this.weakRef.IsAlive;

            public PdfDocument Target =>
                (this.weakRef.Target as PdfDocument);
        }
    }
}

