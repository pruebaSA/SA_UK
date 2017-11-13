namespace System.Data.Services.Common
{
    using System;
    using System.Data.Services.Client;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Xml;

    internal sealed class EpmCustomContentWriterNodeData : IDisposable
    {
        private bool disposed;

        internal EpmCustomContentWriterNodeData(EpmTargetPathSegment segment, object element)
        {
            this.XmlContentStream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            this.XmlContentWriter = XmlWriter.Create(this.XmlContentStream, settings);
            this.PopulateData(segment, element);
        }

        internal EpmCustomContentWriterNodeData(EpmCustomContentWriterNodeData parentData, EpmTargetPathSegment segment, object element)
        {
            this.XmlContentStream = parentData.XmlContentStream;
            this.XmlContentWriter = parentData.XmlContentWriter;
            this.PopulateData(segment, element);
        }

        internal void AddContentToTarget(XmlWriter target)
        {
            this.XmlContentWriter.Close();
            this.XmlContentWriter = null;
            this.XmlContentStream.Seek(0L, SeekOrigin.Begin);
            XmlReaderSettings settings = new XmlReaderSettings {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            XmlReader reader = XmlReader.Create(this.XmlContentStream, settings);
            this.XmlContentStream = null;
            target.WriteNode(reader, false);
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                if (this.XmlContentWriter != null)
                {
                    this.XmlContentWriter.Close();
                    this.XmlContentWriter = null;
                }
                if (this.XmlContentStream != null)
                {
                    this.XmlContentStream.Dispose();
                    this.XmlContentStream = null;
                }
                this.disposed = true;
            }
        }

        private void PopulateData(EpmTargetPathSegment segment, object element)
        {
            if (segment.EpmInfo != null)
            {
                object obj2;
                try
                {
                    obj2 = ClientType.ReadPropertyValue(element, segment.EpmInfo.ActualType, segment.EpmInfo.Attribute.SourcePath);
                }
                catch (TargetInvocationException)
                {
                    throw;
                }
                this.Data = (obj2 == null) ? string.Empty : ClientConvert.ToString(obj2, false);
            }
        }

        internal string Data { get; private set; }

        internal MemoryStream XmlContentStream { get; private set; }

        internal XmlWriter XmlContentWriter { get; private set; }
    }
}

