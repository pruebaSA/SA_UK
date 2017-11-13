namespace System.Xml.Xsl
{
    using System;
    using System.CodeDom.Compiler;

    public sealed class XsltSettings
    {
        private bool checkOnly;
        private bool enableDocumentFunction;
        private bool enableScript;
        private bool includeDebugInformation;
        private TempFileCollection tempFiles;
        private bool treatWarningsAsErrors;
        private int warningLevel;

        public XsltSettings()
        {
            this.warningLevel = -1;
        }

        public XsltSettings(bool enableDocumentFunction, bool enableScript)
        {
            this.warningLevel = -1;
            this.enableDocumentFunction = enableDocumentFunction;
            this.enableScript = enableScript;
        }

        internal bool CheckOnly
        {
            get => 
                this.checkOnly;
            set
            {
                this.checkOnly = value;
            }
        }

        public static XsltSettings Default =>
            new XsltSettings(false, false);

        public bool EnableDocumentFunction
        {
            get => 
                this.enableDocumentFunction;
            set
            {
                this.enableDocumentFunction = value;
            }
        }

        public bool EnableScript
        {
            get => 
                this.enableScript;
            set
            {
                this.enableScript = value;
            }
        }

        internal bool IncludeDebugInformation
        {
            get => 
                this.includeDebugInformation;
            set
            {
                this.includeDebugInformation = value;
            }
        }

        internal TempFileCollection TempFiles
        {
            get => 
                this.tempFiles;
            set
            {
                this.tempFiles = value;
            }
        }

        internal bool TreatWarningsAsErrors
        {
            get => 
                this.treatWarningsAsErrors;
            set
            {
                this.treatWarningsAsErrors = value;
            }
        }

        public static XsltSettings TrustedXslt =>
            new XsltSettings(true, true);

        internal int WarningLevel
        {
            get => 
                this.warningLevel;
            set
            {
                this.warningLevel = value;
            }
        }
    }
}

