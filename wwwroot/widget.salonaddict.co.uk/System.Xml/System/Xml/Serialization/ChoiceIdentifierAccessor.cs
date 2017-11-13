namespace System.Xml.Serialization
{
    using System;

    internal class ChoiceIdentifierAccessor : Accessor
    {
        private string[] memberIds;
        private string memberName;

        internal string[] MemberIds
        {
            get => 
                this.memberIds;
            set
            {
                this.memberIds = value;
            }
        }

        internal string MemberName
        {
            get => 
                this.memberName;
            set
            {
                this.memberName = value;
            }
        }
    }
}

