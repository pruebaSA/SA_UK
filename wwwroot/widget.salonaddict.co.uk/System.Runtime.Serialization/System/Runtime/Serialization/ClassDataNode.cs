namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;

    internal class ClassDataNode : DataNode<object>
    {
        private IList<ExtensionDataMember> members;

        internal ClassDataNode()
        {
            base.dataType = Globals.TypeOfClassDataNode;
        }

        public override void Clear()
        {
            base.Clear();
            this.members = null;
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

