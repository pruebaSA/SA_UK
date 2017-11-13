namespace System.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.Xml.Linq;

    public sealed class ReadingWritingEntityEventArgs : EventArgs
    {
        private XElement data;
        private object entity;

        internal ReadingWritingEntityEventArgs(object entity, XElement data)
        {
            this.entity = entity;
            this.data = data;
        }

        public XElement Data =>
            this.data;

        public object Entity =>
            this.entity;
    }
}

