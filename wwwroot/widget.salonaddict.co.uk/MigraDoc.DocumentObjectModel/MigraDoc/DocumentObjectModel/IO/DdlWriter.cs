namespace MigraDoc.DocumentObjectModel.IO
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.IO;
    using System.Text;

    public class DdlWriter
    {
        private Serializer serializer;
        private StreamWriter writer;

        public DdlWriter(Stream stream)
        {
            this.writer = new StreamWriter(stream);
            this.serializer = new Serializer(this.writer);
        }

        public DdlWriter(TextWriter writer)
        {
            this.serializer = new Serializer(writer);
        }

        public DdlWriter(string filename)
        {
            this.writer = new StreamWriter(filename, false, Encoding.Default);
            this.serializer = new Serializer(this.writer);
        }

        public void Close()
        {
            if (this.serializer != null)
            {
                this.serializer = null;
            }
            if (this.writer != null)
            {
                this.writer.Close();
                this.writer = null;
            }
        }

        public void Flush()
        {
            this.serializer.Flush();
        }

        public void WriteDocument(DocumentObject documentObject)
        {
            documentObject.Serialize(this.serializer);
            this.serializer.Flush();
        }

        public void WriteDocument(DocumentObjectCollection documentObjectContainer)
        {
            documentObjectContainer.Serialize(this.serializer);
            this.serializer.Flush();
        }

        public static void WriteToFile(DocumentObject docObject, string filename)
        {
            WriteToFile(docObject, filename, 2, 0);
        }

        public static void WriteToFile(DocumentObjectCollection docObjectContainer, string filename)
        {
            WriteToFile(docObjectContainer, filename, 2, 0);
        }

        public static void WriteToFile(DocumentObject docObject, string filename, int indent)
        {
            WriteToFile(docObject, filename, indent, 0);
        }

        public static void WriteToFile(DocumentObjectCollection docObjectContainer, string filename, int indent)
        {
            WriteToFile(docObjectContainer, filename, indent, 0);
        }

        public static void WriteToFile(DocumentObject docObject, string filename, int indent, int initialIndent)
        {
            DdlWriter writer = null;
            try
            {
                writer = new DdlWriter(filename) {
                    Indent = indent,
                    InitialIndent = initialIndent
                };
                writer.WriteDocument(docObject);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public static void WriteToFile(DocumentObjectCollection docObjectContainer, string filename, int indent, int initialIndent)
        {
            DdlWriter writer = null;
            try
            {
                writer = new DdlWriter(filename) {
                    Indent = indent,
                    InitialIndent = initialIndent
                };
                writer.WriteDocument(docObjectContainer);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public static string WriteToString(DocumentObject docObject) => 
            WriteToString(docObject, 2, 0);

        public static string WriteToString(DocumentObjectCollection docObjectContainer) => 
            WriteToString(docObjectContainer, 2, 0);

        public static string WriteToString(DocumentObject docObject, int indent) => 
            WriteToString(docObject, indent, 0);

        public static string WriteToString(DocumentObjectCollection docObjectContainer, int indent) => 
            WriteToString(docObjectContainer, indent, 0);

        public static string WriteToString(DocumentObject docObject, int indent, int initialIndent)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = null;
            DdlWriter writer2 = null;
            try
            {
                writer = new StringWriter(sb);
                writer2 = new DdlWriter(writer) {
                    Indent = indent,
                    InitialIndent = initialIndent
                };
                writer2.WriteDocument(docObject);
                writer2.Close();
            }
            finally
            {
                if (writer2 != null)
                {
                    writer2.Close();
                }
                if (writer != null)
                {
                    writer.Close();
                }
            }
            return sb.ToString();
        }

        public static string WriteToString(DocumentObjectCollection docObjectContainer, int indent, int initialIndent)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = null;
            DdlWriter writer2 = null;
            try
            {
                writer = new StringWriter(sb);
                writer2 = new DdlWriter(writer) {
                    Indent = indent,
                    InitialIndent = initialIndent
                };
                writer2.WriteDocument(docObjectContainer);
                writer2.Close();
            }
            finally
            {
                if (writer2 != null)
                {
                    writer2.Close();
                }
                if (writer != null)
                {
                    writer.Close();
                }
            }
            return sb.ToString();
        }

        public int Indent
        {
            get => 
                this.serializer.Indent;
            set
            {
                this.serializer.Indent = value;
            }
        }

        public int InitialIndent
        {
            get => 
                this.serializer.InitialIndent;
            set
            {
                this.serializer.InitialIndent = value;
            }
        }
    }
}

