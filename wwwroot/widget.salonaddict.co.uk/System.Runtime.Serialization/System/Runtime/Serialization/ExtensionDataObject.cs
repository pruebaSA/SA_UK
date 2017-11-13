namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;

    public sealed class ExtensionDataObject
    {
        private IList<ExtensionDataMember> members;

        internal ExtensionDataObject()
        {
        }

        internal IList<ExtensionDataMember> Members
        {
            get => 
                this.members;
            set
            {
                this.members = value;
            }
        }
    }
}

