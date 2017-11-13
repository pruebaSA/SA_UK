namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;

    internal class ISerializableDataNode : DataNode<object>
    {
        private string factoryTypeName;
        private string factoryTypeNamespace;
        private IList<ISerializableDataMember> members;

        internal ISerializableDataNode()
        {
            base.dataType = Globals.TypeOfISerializableDataNode;
        }

        public override void Clear()
        {
            base.Clear();
            this.members = null;
            this.factoryTypeName = (string) (this.factoryTypeNamespace = null);
        }

        public override void GetData(ElementData element)
        {
            base.GetData(element);
            if (this.FactoryTypeName != null)
            {
                base.AddQualifiedNameAttribute(element, "z", "FactoryType", "http://schemas.microsoft.com/2003/10/Serialization/", this.FactoryTypeName, this.FactoryTypeNamespace);
            }
        }

        internal string FactoryTypeName
        {
            get => 
                this.factoryTypeName;
            set
            {
                this.factoryTypeName = value;
            }
        }

        internal string FactoryTypeNamespace
        {
            get => 
                this.factoryTypeNamespace;
            set
            {
                this.factoryTypeNamespace = value;
            }
        }

        internal IList<ISerializableDataMember> Members
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

