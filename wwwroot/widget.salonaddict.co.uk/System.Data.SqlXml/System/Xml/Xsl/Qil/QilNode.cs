namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Xsl;

    internal class QilNode : IList<QilNode>, ICollection<QilNode>, IEnumerable<QilNode>, IEnumerable
    {
        protected object annotation;
        protected QilNodeType nodeType;
        protected ISourceLineInfo sourceLine;
        protected XmlQueryType xmlType;

        public QilNode(QilNodeType nodeType)
        {
            this.nodeType = nodeType;
        }

        public QilNode(QilNodeType nodeType, XmlQueryType xmlType)
        {
            this.nodeType = nodeType;
            this.xmlType = xmlType;
        }

        public virtual void Add(IList<QilNode> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                this.Insert(this.Count, list[i]);
            }
        }

        public virtual void Add(QilNode node)
        {
            this.Insert(this.Count, node);
        }

        public virtual void Clear()
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                this.RemoveAt(i);
            }
        }

        public virtual bool Contains(QilNode node) => 
            (this.IndexOf(node) != -1);

        public virtual void CopyTo(QilNode[] array, int index)
        {
            for (int i = 0; i < this.Count; i++)
            {
                array[index + i] = this[i];
            }
        }

        public virtual QilNode DeepClone(QilFactory f) => 
            new QilCloneVisitor(f).Clone(this);

        public IEnumerator<QilNode> GetEnumerator() => 
            new IListEnumerator<QilNode>(this);

        public virtual int IndexOf(QilNode node)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (node.Equals(this[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual void Insert(int index, QilNode node)
        {
            throw new NotSupportedException();
        }

        public virtual bool Remove(QilNode node)
        {
            int index = this.IndexOf(node);
            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }
            return false;
        }

        public virtual void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public virtual QilNode ShallowClone(QilFactory f) => 
            ((QilNode) base.MemberwiseClone());

        IEnumerator IEnumerable.GetEnumerator() => 
            new IListEnumerator<QilNode>(this);

        public object Annotation
        {
            get => 
                this.annotation;
            set
            {
                this.annotation = value;
            }
        }

        public virtual int Count =>
            0;

        public virtual bool IsReadOnly =>
            false;

        public virtual QilNode this[int index]
        {
            get
            {
                throw new IndexOutOfRangeException();
            }
            set
            {
                throw new IndexOutOfRangeException();
            }
        }

        public QilNodeType NodeType
        {
            get => 
                this.nodeType;
            set
            {
                this.nodeType = value;
            }
        }

        public ISourceLineInfo SourceLine
        {
            get => 
                this.sourceLine;
            set
            {
                this.sourceLine = value;
            }
        }

        public virtual XmlQueryType XmlType
        {
            get => 
                this.xmlType;
            set
            {
                this.xmlType = value;
            }
        }
    }
}

