namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Reflection;
    using System.Xml.Xsl;

    internal class QilList : QilNode
    {
        private int count;
        private QilNode[] members;

        public QilList(QilNodeType nodeType) : base(nodeType)
        {
            this.members = new QilNode[4];
            base.xmlType = null;
        }

        public override void Insert(int index, QilNode node)
        {
            if ((index < 0) || (index > this.count))
            {
                throw new IndexOutOfRangeException();
            }
            if (this.count == this.members.Length)
            {
                QilNode[] destinationArray = new QilNode[this.count * 2];
                Array.Copy(this.members, destinationArray, this.count);
                this.members = destinationArray;
            }
            if (index < this.count)
            {
                Array.Copy(this.members, index, this.members, index + 1, this.count - index);
            }
            this.count++;
            this.members[index] = node;
            base.xmlType = null;
        }

        public override void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this.count))
            {
                throw new IndexOutOfRangeException();
            }
            this.count--;
            if (index < this.count)
            {
                Array.Copy(this.members, index + 1, this.members, index, this.count - index);
            }
            this.members[this.count] = null;
            base.xmlType = null;
        }

        public override QilNode ShallowClone(QilFactory f)
        {
            QilList list = (QilList) base.MemberwiseClone();
            list.members = (QilNode[]) this.members.Clone();
            return list;
        }

        public override int Count =>
            this.count;

        public override QilNode this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.count))
                {
                    throw new IndexOutOfRangeException();
                }
                return this.members[index];
            }
            set
            {
                if ((index < 0) || (index >= this.count))
                {
                    throw new IndexOutOfRangeException();
                }
                this.members[index] = value;
                base.xmlType = null;
            }
        }

        public override XmlQueryType XmlType
        {
            get
            {
                if (base.xmlType == null)
                {
                    XmlQueryType empty = XmlQueryTypeFactory.Empty;
                    if (this.count > 0)
                    {
                        if (base.nodeType == QilNodeType.Sequence)
                        {
                            for (int i = 0; i < this.count; i++)
                            {
                                empty = XmlQueryTypeFactory.Sequence(empty, this.members[i].XmlType);
                            }
                            if (empty.IsDod)
                            {
                                empty = XmlQueryTypeFactory.PrimeProduct(XmlQueryTypeFactory.NodeNotRtfS, empty.Cardinality);
                            }
                        }
                        else if (base.nodeType == QilNodeType.BranchList)
                        {
                            empty = this.members[0].XmlType;
                            for (int j = 1; j < this.count; j++)
                            {
                                empty = XmlQueryTypeFactory.Choice(empty, this.members[j].XmlType);
                            }
                        }
                    }
                    base.xmlType = empty;
                }
                return base.xmlType;
            }
        }
    }
}

