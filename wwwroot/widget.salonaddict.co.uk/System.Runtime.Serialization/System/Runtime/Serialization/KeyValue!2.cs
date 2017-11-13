namespace System.Runtime.Serialization
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), DataContract(Namespace="http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
    internal struct KeyValue<K, V>
    {
        private K key;
        private V value;
        internal KeyValue(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        [DataMember(IsRequired=true)]
        public K Key
        {
            get => 
                this.key;
            set
            {
                this.key = value;
            }
        }
        [DataMember(IsRequired=true)]
        public V Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
            }
        }
    }
}

