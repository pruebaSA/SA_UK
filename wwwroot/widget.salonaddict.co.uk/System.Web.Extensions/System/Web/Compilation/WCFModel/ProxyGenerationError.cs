namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.ServiceModel.Description;
    using System.Xml;
    using System.Xml.Schema;

    internal class ProxyGenerationError
    {
        private GeneratorState m_ErrorGeneratorState;
        private bool m_IsWarning;
        private int m_LineNumber;
        private int m_LinePosition;
        private string m_Message;
        private string m_MetadataFile;

        public ProxyGenerationError(MetadataConversionError errorMessage)
        {
            this.m_ErrorGeneratorState = GeneratorState.GenerateCode;
            this.m_IsWarning = errorMessage.IsWarning;
            this.m_Message = errorMessage.Message;
            this.m_MetadataFile = string.Empty;
            this.m_LineNumber = -1;
            this.m_LinePosition = -1;
        }

        public ProxyGenerationError(GeneratorState generatorState, string fileName, Exception errorException)
        {
            this.m_ErrorGeneratorState = generatorState;
            this.m_IsWarning = false;
            this.m_Message = errorException.Message;
            this.m_MetadataFile = fileName;
            this.m_LineNumber = -1;
            this.m_LinePosition = -1;
        }

        public ProxyGenerationError(GeneratorState generatorState, string fileName, XmlSchemaException errorException)
        {
            this.m_ErrorGeneratorState = generatorState;
            this.m_IsWarning = false;
            this.m_Message = errorException.Message;
            this.m_MetadataFile = fileName;
            this.m_LineNumber = errorException.LineNumber;
            this.m_LinePosition = errorException.LinePosition;
        }

        public ProxyGenerationError(GeneratorState generatorState, string fileName, XmlException errorException)
        {
            this.m_ErrorGeneratorState = generatorState;
            this.m_IsWarning = false;
            this.m_Message = errorException.Message;
            this.m_MetadataFile = fileName;
            this.m_LineNumber = errorException.LineNumber;
            this.m_LinePosition = errorException.LinePosition;
        }

        public ProxyGenerationError(GeneratorState generatorState, string fileName, Exception errorException, bool isWarning)
        {
            this.m_ErrorGeneratorState = generatorState;
            this.m_IsWarning = isWarning;
            this.m_Message = errorException.Message;
            this.m_MetadataFile = fileName;
            this.m_LineNumber = -1;
            this.m_LinePosition = -1;
        }

        public ProxyGenerationError(GeneratorState generatorState, string fileName, XmlSchemaException errorException, bool isWarning)
        {
            this.m_ErrorGeneratorState = generatorState;
            this.m_IsWarning = isWarning;
            this.m_Message = errorException.Message;
            this.m_MetadataFile = fileName;
            this.m_LineNumber = errorException.LineNumber;
            this.m_LinePosition = errorException.LinePosition;
        }

        public GeneratorState ErrorGeneratorState =>
            this.m_ErrorGeneratorState;

        public bool IsWarning =>
            this.m_IsWarning;

        public int LineNumber =>
            this.m_LineNumber;

        public int LinePosition =>
            this.m_LinePosition;

        public string Message =>
            this.m_Message;

        public string MetadataFile =>
            this.m_MetadataFile;

        public enum GeneratorState
        {
            LoadMetadata,
            MergeMetadata,
            GenerateCode
        }
    }
}

