namespace System.Runtime.Serialization
{
    using System;

    internal class ExtensionDataMember
    {
        private int memberIndex;
        private string name;
        private string ns;
        private IDataNode value;

        internal int MemberIndex
        {
            get => 
                this.memberIndex;
            set
            {
                this.memberIndex = value;
            }
        }

        internal string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        internal string Namespace
        {
            get => 
                this.ns;
            set
            {
                this.ns = value;
            }
        }

        internal IDataNode Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
            }
        }
    }
}

