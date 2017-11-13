namespace System.Xml.Serialization
{
    using System;

    internal class MembersMapping : TypeMapping
    {
        private bool hasWrapperElement = true;
        private MemberMapping[] members;
        private bool validateRpcWrapperElement;
        private bool writeAccessors = true;
        private MemberMapping xmlnsMember;

        internal bool HasWrapperElement
        {
            get => 
                this.hasWrapperElement;
            set
            {
                this.hasWrapperElement = value;
            }
        }

        internal MemberMapping[] Members
        {
            get => 
                this.members;
            set
            {
                this.members = value;
            }
        }

        internal bool ValidateRpcWrapperElement
        {
            get => 
                this.validateRpcWrapperElement;
            set
            {
                this.validateRpcWrapperElement = value;
            }
        }

        internal bool WriteAccessors
        {
            get => 
                this.writeAccessors;
            set
            {
                this.writeAccessors = value;
            }
        }

        internal MemberMapping XmlnsMember
        {
            get => 
                this.xmlnsMember;
            set
            {
                this.xmlnsMember = value;
            }
        }
    }
}

