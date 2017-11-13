namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal class CollectionDataNode : DataNode<Array>
    {
        private string itemName;
        private string itemNamespace;
        private IList<IDataNode> items;
        private int size = -1;

        internal CollectionDataNode()
        {
            base.dataType = Globals.TypeOfCollectionDataNode;
        }

        public override void Clear()
        {
            base.Clear();
            this.items = null;
            this.size = -1;
        }

        public override void GetData(ElementData element)
        {
            base.GetData(element);
            element.AddAttribute("z", "http://schemas.microsoft.com/2003/10/Serialization/", "Size", this.Size.ToString(NumberFormatInfo.InvariantInfo));
        }

        internal string ItemName
        {
            get => 
                this.itemName;
            set
            {
                this.itemName = value;
            }
        }

        internal string ItemNamespace
        {
            get => 
                this.itemNamespace;
            set
            {
                this.itemNamespace = value;
            }
        }

        internal IList<IDataNode> Items
        {
            get => 
                this.items;
            set
            {
                this.items = value;
            }
        }

        internal int Size
        {
            get => 
                this.size;
            set
            {
                this.size = value;
            }
        }
    }
}

