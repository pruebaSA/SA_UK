namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Web.Resources;
    using System.Xml.Serialization;

    internal class ExternalFile
    {
        private Exception m_ErrorInLoading;
        private string m_FileName;
        private bool m_IsExistingFile;

        public ExternalFile()
        {
            this.m_FileName = string.Empty;
        }

        public ExternalFile(string fileName)
        {
            this.FileName = fileName;
        }

        public static bool IsLocalFileName(string fileName) => 
            ((fileName?.IndexOfAny(Path.GetInvalidFileNameChars()) < 0) && (fileName.IndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar }) < 0));

        [XmlIgnore]
        public Exception ErrorInLoading
        {
            get => 
                this.m_ErrorInLoading;
            set
            {
                this.m_ErrorInLoading = value;
            }
        }

        [XmlAttribute]
        public string FileName
        {
            get => 
                this.m_FileName;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!IsLocalFileName(value))
                {
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_InvalidFileName, new object[] { value }));
                }
                this.m_FileName = value;
            }
        }

        [XmlIgnore]
        public bool IsExistingFile
        {
            get => 
                this.m_IsExistingFile;
            set
            {
                this.m_IsExistingFile = value;
            }
        }
    }
}

