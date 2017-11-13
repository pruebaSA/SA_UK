namespace System.Runtime.Serialization
{
    using System;

    internal class ISerializableDataMember
    {
        private string name;
        private IDataNode value;

        internal string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
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

