namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.XPath;

    internal class SafeSeekableNavigator : SeekableXPathNavigator, INodeCounter
    {
        private SafeSeekableNavigator counter;
        private SeekableXPathNavigator navigator;
        private int nodeCount;
        private int nodeCountMax;

        internal SafeSeekableNavigator(SafeSeekableNavigator nav)
        {
            this.navigator = (SeekableXPathNavigator) nav.navigator.Clone();
            this.counter = nav.counter;
        }

        internal SafeSeekableNavigator(SeekableXPathNavigator navigator, int nodeCountMax)
        {
            this.navigator = navigator;
            this.counter = this;
            this.nodeCount = nodeCountMax;
            this.nodeCountMax = nodeCountMax;
        }

        public override XPathNavigator Clone() => 
            new SafeSeekableNavigator(this);

        public override XmlNodeOrder ComparePosition(XPathNavigator navigator)
        {
            if (navigator != null)
            {
                SafeSeekableNavigator navigator2 = navigator as SafeSeekableNavigator;
                if (navigator2 != null)
                {
                    return this.navigator.ComparePosition(navigator2.navigator);
                }
            }
            return XmlNodeOrder.Unknown;
        }

        public override XmlNodeOrder ComparePosition(long x, long y) => 
            this.navigator.ComparePosition(x, y);

        public int ElapsedCount(int marker) => 
            (marker - this.counter.nodeCount);

        public override string GetAttribute(string localName, string namespaceURI)
        {
            this.IncrementNodeCount();
            return this.navigator.GetAttribute(localName, namespaceURI);
        }

        public override string GetLocalName(long nodePosition) => 
            this.navigator.GetLocalName(nodePosition);

        public override string GetName(long nodePosition) => 
            this.navigator.GetName(nodePosition);

        public override string GetNamespace(long nodePosition) => 
            this.navigator.GetNamespace(nodePosition);

        public override string GetNamespace(string name)
        {
            this.IncrementNodeCount();
            return this.navigator.GetNamespace(name);
        }

        public override XPathNodeType GetNodeType(long nodePosition) => 
            this.navigator.GetNodeType(nodePosition);

        public override string GetValue(long nodePosition) => 
            this.navigator.GetValue(nodePosition);

        public void Increase()
        {
            this.IncrementNodeCount();
        }

        public void IncreaseBy(int count)
        {
            this.counter.nodeCount -= count - 1;
            this.Increase();
        }

        internal void IncrementNodeCount()
        {
            if (this.counter.nodeCount <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XPathNavigatorException(System.ServiceModel.SR.GetString("FilterNodeQuotaExceeded", new object[] { this.counter.nodeCountMax })));
            }
            this.counter.nodeCount--;
        }

        public override bool IsDescendant(XPathNavigator navigator)
        {
            if (navigator == null)
            {
                return false;
            }
            SafeSeekableNavigator navigator2 = navigator as SafeSeekableNavigator;
            return ((navigator2 != null) && this.navigator.IsDescendant(navigator2.navigator));
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            if (other == null)
            {
                return false;
            }
            SafeSeekableNavigator navigator = other as SafeSeekableNavigator;
            return ((navigator != null) && this.navigator.IsSamePosition(navigator.navigator));
        }

        public override bool MoveTo(XPathNavigator other)
        {
            if (other == null)
            {
                return false;
            }
            this.IncrementNodeCount();
            SafeSeekableNavigator navigator = other as SafeSeekableNavigator;
            return ((navigator != null) && this.navigator.MoveTo(navigator.navigator));
        }

        public override bool MoveToAttribute(string localName, string namespaceURI)
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToAttribute(localName, namespaceURI);
        }

        public override bool MoveToFirst()
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToFirst();
        }

        public override bool MoveToFirstAttribute()
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToFirstAttribute();
        }

        public override bool MoveToFirstChild()
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToFirstChild();
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToFirstNamespace(namespaceScope);
        }

        public override bool MoveToId(string id)
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToId(id);
        }

        public override bool MoveToNamespace(string name)
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToNamespace(name);
        }

        public override bool MoveToNext()
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToNext();
        }

        public override bool MoveToNextAttribute()
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToNextAttribute();
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToNextNamespace(namespaceScope);
        }

        public override bool MoveToParent()
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToParent();
        }

        public override bool MoveToPrevious()
        {
            this.IncrementNodeCount();
            return this.navigator.MoveToPrevious();
        }

        public override void MoveToRoot()
        {
            this.IncrementNodeCount();
            this.navigator.MoveToRoot();
        }

        public override string BaseURI =>
            this.navigator.BaseURI;

        public int CounterMarker
        {
            get => 
                this.counter.nodeCount;
            set
            {
                this.counter.nodeCount = value;
            }
        }

        public override long CurrentPosition
        {
            get => 
                this.navigator.CurrentPosition;
            set
            {
                this.navigator.CurrentPosition = value;
            }
        }

        public override bool HasAttributes =>
            this.navigator.HasAttributes;

        public override bool HasChildren =>
            this.navigator.HasChildren;

        public override bool IsEmptyElement =>
            this.navigator.IsEmptyElement;

        public override string LocalName =>
            this.navigator.LocalName;

        public int MaxCounter
        {
            set
            {
                this.counter.nodeCountMax = value;
            }
        }

        public override string Name =>
            this.navigator.Name;

        public override string NamespaceURI =>
            this.navigator.NamespaceURI;

        public override XmlNameTable NameTable =>
            this.navigator.NameTable;

        public override XPathNodeType NodeType =>
            this.navigator.NodeType;

        public override string Prefix =>
            this.navigator.Prefix;

        public override string Value =>
            this.navigator.Value;

        public override string XmlLang =>
            this.navigator.XmlLang;
    }
}

