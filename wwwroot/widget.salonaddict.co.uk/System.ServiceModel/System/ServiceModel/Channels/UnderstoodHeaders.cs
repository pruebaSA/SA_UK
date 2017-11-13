namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class UnderstoodHeaders : IEnumerable<MessageHeaderInfo>, IEnumerable
    {
        private MessageHeaders messageHeaders;
        private bool modified;

        internal UnderstoodHeaders(MessageHeaders messageHeaders)
        {
            this.messageHeaders = messageHeaders;
        }

        public void Add(MessageHeaderInfo headerInfo)
        {
            this.messageHeaders.AddUnderstood(headerInfo);
            this.modified = true;
        }

        public bool Contains(MessageHeaderInfo headerInfo) => 
            this.messageHeaders.IsUnderstood(headerInfo);

        public IEnumerator<MessageHeaderInfo> GetEnumerator() => 
            this.messageHeaders.GetUnderstoodEnumerator();

        public void Remove(MessageHeaderInfo headerInfo)
        {
            this.messageHeaders.RemoveUnderstood(headerInfo);
            this.modified = true;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        internal bool Modified
        {
            get => 
                this.modified;
            set
            {
                this.modified = value;
            }
        }
    }
}

