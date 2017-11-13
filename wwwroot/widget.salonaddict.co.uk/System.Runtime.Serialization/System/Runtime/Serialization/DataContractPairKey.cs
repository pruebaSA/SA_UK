namespace System.Runtime.Serialization
{
    using System;

    internal class DataContractPairKey
    {
        private object object1;
        private object object2;

        public DataContractPairKey(object object1, object object2)
        {
            this.object1 = object1;
            this.object2 = object2;
        }

        public override bool Equals(object other)
        {
            DataContractPairKey key = other as DataContractPairKey;
            if (key == null)
            {
                return false;
            }
            return (((key.object1 == this.object1) && (key.object2 == this.object2)) || ((key.object1 == this.object2) && (key.object2 == this.object1)));
        }

        public override int GetHashCode() => 
            (this.object1.GetHashCode() ^ this.object2.GetHashCode());
    }
}

