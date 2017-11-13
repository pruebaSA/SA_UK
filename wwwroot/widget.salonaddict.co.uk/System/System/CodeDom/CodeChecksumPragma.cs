namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeChecksumPragma : CodeDirective
    {
        private Guid checksumAlgorithmId;
        private byte[] checksumData;
        private string fileName;

        public CodeChecksumPragma()
        {
        }

        public CodeChecksumPragma(string fileName, Guid checksumAlgorithmId, byte[] checksumData)
        {
            this.fileName = fileName;
            this.checksumAlgorithmId = checksumAlgorithmId;
            this.checksumData = checksumData;
        }

        public Guid ChecksumAlgorithmId
        {
            get => 
                this.checksumAlgorithmId;
            set
            {
                this.checksumAlgorithmId = value;
            }
        }

        public byte[] ChecksumData
        {
            get => 
                this.checksumData;
            set
            {
                this.checksumData = value;
            }
        }

        public string FileName
        {
            get
            {
                if (this.fileName != null)
                {
                    return this.fileName;
                }
                return string.Empty;
            }
            set
            {
                this.fileName = value;
            }
        }
    }
}

