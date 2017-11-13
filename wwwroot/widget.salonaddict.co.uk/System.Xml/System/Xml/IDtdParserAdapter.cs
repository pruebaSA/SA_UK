namespace System.Xml
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml.Schema;

    internal interface IDtdParserAdapter
    {
        void OnNewLine(int pos);
        void OnPublicId(string publicId, LineInfo keywordLineInfo, LineInfo publicLiteralLineInfo);
        void OnSystemId(string systemId, LineInfo keywordLineInfo, LineInfo systemLiteralLineInfo);
        void ParseComment(BufferBuilder sb);
        int ParseNamedCharRef(bool expand, BufferBuilder internalSubsetBuilder);
        int ParseNumericCharRef(BufferBuilder internalSubsetBuilder);
        void ParsePI(BufferBuilder sb);
        bool PopEntity(out SchemaEntity oldEntity, out int newEntityId);
        bool PushEntity(SchemaEntity entity, int entityId);
        bool PushExternalSubset(string systemId, string publicId);
        void PushInternalDtd(string baseUri, string internalDtd);
        int ReadData();
        void SendValidationEvent(XmlSeverityType severity, XmlSchemaException exception);
        void Throw(Exception e);

        Uri BaseUri { get; }

        int CurrentPosition { get; set; }

        bool DtdValidation { get; }

        int EntityStackLength { get; }

        ValidationEventHandler EventHandler { get; set; }

        bool IsEntityEolNormalized { get; }

        bool IsEof { get; }

        int LineNo { get; }

        int LineStartPosition { get; }

        XmlNamespaceManager NamespaceManager { get; }

        bool Namespaces { get; }

        XmlNameTable NameTable { get; }

        bool Normalization { get; }

        char[] ParsingBuffer { get; }

        int ParsingBufferLength { get; }

        bool V1CompatibilityMode { get; }
    }
}

