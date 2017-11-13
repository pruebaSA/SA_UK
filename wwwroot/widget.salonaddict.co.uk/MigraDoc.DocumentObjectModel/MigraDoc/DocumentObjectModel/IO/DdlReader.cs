namespace MigraDoc.DocumentObjectModel.IO
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.IO;
    using System.Text;

    public class DdlReader
    {
        private bool doClose;
        private DdlReaderErrors errorManager;
        private string fileName;
        private TextReader reader;

        public DdlReader(Stream stream) : this(stream, null)
        {
        }

        public DdlReader(TextReader reader) : this(reader, null)
        {
        }

        public DdlReader(string filename) : this(filename, null)
        {
        }

        public DdlReader(Stream stream, DdlReaderErrors errors)
        {
            this.doClose = true;
            this.errorManager = errors;
            this.reader = new StreamReader(stream);
        }

        public DdlReader(TextReader reader, DdlReaderErrors errors)
        {
            this.doClose = true;
            this.doClose = false;
            this.errorManager = errors;
            this.reader = reader;
        }

        public DdlReader(string filename, DdlReaderErrors errors)
        {
            this.doClose = true;
            this.fileName = filename;
            this.errorManager = errors;
            this.reader = new StreamReader(filename, Encoding.Default);
        }

        public void Close()
        {
            if (this.doClose && (this.reader != null))
            {
                this.reader.Close();
                this.reader = null;
            }
        }

        public static Document DocumentFromFile(string documentFileName)
        {
            Document document = null;
            DdlReader reader = null;
            try
            {
                reader = new DdlReader(documentFileName);
                document = reader.ReadDocument();
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return document;
        }

        public static Document DocumentFromString(string ddl)
        {
            StringReader reader = null;
            Document document = null;
            DdlReader reader2 = null;
            try
            {
                reader = new StringReader(ddl);
                reader2 = new DdlReader(reader);
                document = reader2.ReadDocument();
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (reader2 != null)
                {
                    reader2.Close();
                }
            }
            return document;
        }

        public static DocumentObject ObjectFromFile(string documentFileName) => 
            ObjectFromFile(documentFileName, null);

        public static DocumentObject ObjectFromFile(string documentFileName, DdlReaderErrors errors)
        {
            DdlReader reader = null;
            DocumentObject obj2 = null;
            try
            {
                reader = new DdlReader(documentFileName, errors);
                obj2 = reader.ReadObject();
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return obj2;
        }

        public static DocumentObject ObjectFromString(string ddl) => 
            ObjectFromString(ddl, null);

        public static DocumentObject ObjectFromString(string ddl, DdlReaderErrors errors)
        {
            StringReader reader = null;
            DocumentObject obj2 = null;
            DdlReader reader2 = null;
            try
            {
                reader = new StringReader(ddl);
                reader2 = new DdlReader(reader);
                obj2 = reader2.ReadObject();
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (reader2 != null)
                {
                    reader2.Close();
                }
            }
            return obj2;
        }

        public Document ReadDocument()
        {
            string ddl = this.reader.ReadToEnd();
            Document document = null;
            if ((this.fileName != null) && (this.fileName != ""))
            {
                document = new DdlParser(this.fileName, ddl, this.errorManager).ParseDocument(null);
                document.ddlFile = this.fileName;
                return document;
            }
            DdlParser parser2 = new DdlParser(ddl, this.errorManager);
            return parser2.ParseDocument(null);
        }

        public DocumentObject ReadObject()
        {
            string ddl = this.reader.ReadToEnd();
            DdlParser parser = null;
            if ((this.fileName != null) && (this.fileName != ""))
            {
                parser = new DdlParser(this.fileName, ddl, this.errorManager);
            }
            else
            {
                parser = new DdlParser(ddl, this.errorManager);
            }
            return parser.ParseDocumentObject();
        }
    }
}

