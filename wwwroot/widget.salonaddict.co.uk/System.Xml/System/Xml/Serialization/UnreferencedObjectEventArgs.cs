namespace System.Xml.Serialization
{
    using System;

    public class UnreferencedObjectEventArgs : EventArgs
    {
        private string id;
        private object o;

        public UnreferencedObjectEventArgs(object o, string id)
        {
            this.o = o;
            this.id = id;
        }

        public string UnreferencedId =>
            this.id;

        public object UnreferencedObject =>
            this.o;
    }
}

