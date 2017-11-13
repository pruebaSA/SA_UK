namespace System.Xml.XPath
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Xml;

    [DebuggerDisplay("Position={CurrentPosition}, Current={debuggerDisplayProxy}")]
    public abstract class XPathNodeIterator : ICloneable, IEnumerable
    {
        internal int count = -1;

        protected XPathNodeIterator()
        {
        }

        public abstract XPathNodeIterator Clone();
        public virtual IEnumerator GetEnumerator() => 
            new Enumerator(this);

        public abstract bool MoveNext();
        object ICloneable.Clone() => 
            this.Clone();

        public virtual int Count
        {
            get
            {
                if (this.count == -1)
                {
                    XPathNodeIterator iterator = this.Clone();
                    while (iterator.MoveNext())
                    {
                    }
                    this.count = iterator.CurrentPosition;
                }
                return this.count;
            }
        }

        public abstract XPathNavigator Current { get; }

        public abstract int CurrentPosition { get; }

        private object debuggerDisplayProxy
        {
            get
            {
                if (this.Current != null)
                {
                    return new XPathNavigator.DebuggerDisplayProxy(this.Current);
                }
                return null;
            }
        }

        private class Enumerator : IEnumerator
        {
            private XPathNodeIterator current;
            private bool iterationStarted;
            private XPathNodeIterator original;

            public Enumerator(XPathNodeIterator original)
            {
                this.original = original.Clone();
            }

            public virtual bool MoveNext()
            {
                if (!this.iterationStarted)
                {
                    this.current = this.original.Clone();
                    this.iterationStarted = true;
                }
                if ((this.current != null) && this.current.MoveNext())
                {
                    return true;
                }
                this.current = null;
                return false;
            }

            public virtual void Reset()
            {
                this.iterationStarted = false;
            }

            public virtual object Current
            {
                get
                {
                    if (!this.iterationStarted)
                    {
                        throw new InvalidOperationException(Res.GetString("Sch_EnumNotStarted", new object[] { string.Empty }));
                    }
                    return this.current?.Current.Clone();
                }
            }
        }
    }
}

