namespace PdfSharp.Pdf.IO
{
    using PdfSharp;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.Security;
    using System;
    using System.IO;
    using System.Text;

    public static class PdfReader
    {
        internal static int GetPdfFileVersion(byte[] bytes)
        {
            try
            {
                string str = Encoding.ASCII.GetString(bytes);
                if ((str[0] == '%') || (str.IndexOf("%PDF") >= 0))
                {
                    int index = str.IndexOf("PDF-");
                    if ((index > 0) && (str[index + 5] == '.'))
                    {
                        char ch = str[index + 4];
                        char ch2 = str[index + 6];
                        if (((ch >= '1') && (ch < '2')) && ((ch2 >= '0') && (ch2 <= '9')))
                        {
                            return (((ch - '0') * 10) + (ch2 - '0'));
                        }
                    }
                }
            }
            catch
            {
            }
            return 0;
        }

        public static PdfDocument Open(Stream stream) => 
            Open(stream, PdfDocumentOpenMode.Modify);

        public static PdfDocument Open(string path) => 
            Open(path, null, PdfDocumentOpenMode.Modify, null);

        public static PdfDocument Open(Stream stream, PdfDocumentOpenMode openmode) => 
            Open(stream, (string) null, openmode);

        public static PdfDocument Open(string path, PdfDocumentOpenMode openmode) => 
            Open(path, null, openmode, null);

        public static PdfDocument Open(string path, string password) => 
            Open(path, password, PdfDocumentOpenMode.Modify, null);

        public static PdfDocument Open(Stream stream, PdfDocumentOpenMode openmode, PdfPasswordProvider passwordProvider) => 
            Open(stream, (string) null, openmode);

        public static PdfDocument Open(Stream stream, string password, PdfDocumentOpenMode openmode) => 
            Open(stream, password, openmode, null);

        public static PdfDocument Open(string path, PdfDocumentOpenMode openmode, PdfPasswordProvider provider) => 
            Open(path, null, openmode, provider);

        public static PdfDocument Open(string path, string password, PdfDocumentOpenMode openmode) => 
            Open(path, password, openmode, null);

        public static PdfDocument Open(Stream stream, string password, PdfDocumentOpenMode openmode, PdfPasswordProvider passwordProvider)
        {
            PdfDocument document = null;
            PasswordValidity validity;
            PdfReference[] referenceArray;
            Lexer lexer = new Lexer(stream);
            document = new PdfDocument(lexer);
            document.state |= DocumentState.Imported;
            document.openMode = openmode;
            document.fileSize = stream.Length;
            byte[] buffer = new byte[0x400];
            stream.Position = 0L;
            stream.Read(buffer, 0, 0x400);
            document.version = GetPdfFileVersion(buffer);
            if (document.version == 0)
            {
                throw new InvalidOperationException(PSSR.InvalidPdf);
            }
            document.irefTable.IsUnderConstruction = true;
            Parser parser = new Parser(document);
            document.trailer = parser.ReadTrailer();
            document.irefTable.IsUnderConstruction = false;
            PdfReference reference = document.trailer.Elements["/Encrypt"] as PdfReference;
            if (reference == null)
            {
                goto Label_0185;
            }
            PdfObject obj2 = parser.ReadObject(null, reference.ObjectID, false);
            obj2.Reference = reference;
            reference.Value = obj2;
            PdfStandardSecurityHandler securityHandler = document.SecurityHandler;
        Label_00EA:
            validity = securityHandler.ValidatePassword(password);
            if (validity == PasswordValidity.Invalid)
            {
                if (passwordProvider == null)
                {
                    if (password == null)
                    {
                        throw new PdfReaderException(PSSR.PasswordRequired);
                    }
                    throw new PdfReaderException(PSSR.InvalidPassword);
                }
                PdfPasswordProviderArgs args = new PdfPasswordProviderArgs();
                passwordProvider(args);
                if (args.Abort)
                {
                    return null;
                }
                password = args.Password;
                goto Label_00EA;
            }
            if ((validity == PasswordValidity.UserPassword) && (openmode == PdfDocumentOpenMode.Modify))
            {
                if (passwordProvider == null)
                {
                    throw new PdfReaderException(PSSR.OwnerPasswordRequired);
                }
                PdfPasswordProviderArgs args2 = new PdfPasswordProviderArgs();
                passwordProvider(args2);
                if (args2.Abort)
                {
                    return null;
                }
                password = args2.Password;
                goto Label_00EA;
            }
        Label_0185:
            referenceArray = document.irefTable.AllReferences;
            int length = referenceArray.Length;
            for (int i = 0; i < length; i++)
            {
                PdfReference reference2 = referenceArray[i];
                if (reference2.Value == null)
                {
                    try
                    {
                        parser.ReadObject(null, reference2.ObjectID, false).Reference = reference2;
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    reference2.GetType();
                }
                document.irefTable.maxObjectNumber = Math.Max(document.irefTable.maxObjectNumber, reference2.ObjectNumber);
            }
            if (reference != null)
            {
                document.SecurityHandler.EncryptDocument();
            }
            document.trailer.Finish();
            if (openmode == PdfDocumentOpenMode.Modify)
            {
                if (document.Internals.SecondDocumentID == "")
                {
                    document.trailer.CreateNewDocumentIDs();
                }
                else
                {
                    byte[] bytes = Guid.NewGuid().ToByteArray();
                    document.Internals.SecondDocumentID = PdfEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);
                }
                document.Info.ModificationDate = DateTime.Now;
                document.irefTable.Compact();
                PdfPages pages = document.Pages;
                document.irefTable.Renumber();
            }
            return document;
        }

        public static PdfDocument Open(string path, string password, PdfDocumentOpenMode openmode, PdfPasswordProvider provider)
        {
            PdfDocument document;
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            try
            {
                document = Open(stream, password, openmode, provider);
                if (document != null)
                {
                    document.fullPath = Path.GetFullPath(path);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return document;
        }

        public static int TestPdfFile(Stream stream)
        {
            long position = -1L;
            try
            {
                position = stream.Position;
                byte[] buffer = new byte[0x400];
                stream.Read(buffer, 0, 0x400);
                return GetPdfFileVersion(buffer);
            }
            catch
            {
            }
            finally
            {
                try
                {
                    if (position != -1L)
                    {
                        stream.Position = position;
                    }
                }
                catch
                {
                }
            }
            return 0;
        }

        public static int TestPdfFile(string path)
        {
            FileStream stream = null;
            try
            {
                int num;
                string str = XPdfForm.ExtractPageNumber(path, out num);
                if (File.Exists(str))
                {
                    stream = new FileStream(str, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    byte[] buffer = new byte[0x400];
                    stream.Read(buffer, 0, 0x400);
                    return GetPdfFileVersion(buffer);
                }
            }
            catch
            {
            }
            finally
            {
                try
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
                catch
                {
                }
            }
            return 0;
        }

        public static int TestPdfFile(byte[] data) => 
            GetPdfFileVersion(data);
    }
}

