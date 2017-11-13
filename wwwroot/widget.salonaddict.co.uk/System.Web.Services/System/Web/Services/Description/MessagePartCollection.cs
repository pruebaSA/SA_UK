namespace System.Web.Services.Description
{
    using System;
    using System.Reflection;

    public sealed class MessagePartCollection : ServiceDescriptionBaseCollection
    {
        internal MessagePartCollection(Message message) : base(message)
        {
        }

        public int Add(MessagePart messagePart) => 
            base.List.Add(messagePart);

        public bool Contains(MessagePart messagePart) => 
            base.List.Contains(messagePart);

        public void CopyTo(MessagePart[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        protected override string GetKey(object value) => 
            ((MessagePart) value).Name;

        public int IndexOf(MessagePart messagePart) => 
            base.List.IndexOf(messagePart);

        public void Insert(int index, MessagePart messagePart)
        {
            base.List.Insert(index, messagePart);
        }

        public void Remove(MessagePart messagePart)
        {
            base.List.Remove(messagePart);
        }

        protected override void SetParent(object value, object parent)
        {
            ((MessagePart) value).SetParent((Message) parent);
        }

        public MessagePart this[int index]
        {
            get => 
                ((MessagePart) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }

        public MessagePart this[string name] =>
            ((MessagePart) this.Table[name]);
    }
}

