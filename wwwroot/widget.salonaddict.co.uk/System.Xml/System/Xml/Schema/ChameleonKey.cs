namespace System.Xml.Schema
{
    using System;

    internal class ChameleonKey
    {
        internal Uri chameleonLocation;
        private int hashCode;
        internal string targetNS;

        public ChameleonKey(string ns, Uri location)
        {
            this.targetNS = ns;
            this.chameleonLocation = location;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            ChameleonKey key = obj as ChameleonKey;
            if (key == null)
            {
                return false;
            }
            return (this.targetNS.Equals(key.targetNS) && this.chameleonLocation.Equals(key.chameleonLocation));
        }

        public override int GetHashCode()
        {
            if (this.hashCode == 0)
            {
                this.hashCode = this.targetNS.GetHashCode() + this.chameleonLocation.GetHashCode();
            }
            return this.hashCode;
        }
    }
}

