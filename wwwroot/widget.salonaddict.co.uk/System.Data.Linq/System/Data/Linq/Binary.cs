namespace System.Data.Linq
{
    using System;
    using System.Runtime.Serialization;
    using System.Text;

    [Serializable, DataContract]
    public sealed class Binary : IEquatable<Binary>
    {
        [DataMember(Name="Bytes")]
        private byte[] bytes;
        private int? hashCode;

        public Binary(byte[] value)
        {
            if (value == null)
            {
                throw Error.ArgumentNull("value");
            }
            this.bytes = new byte[value.Length];
            Array.Copy(value, this.bytes, value.Length);
            this.ComputeHash();
        }

        private void ComputeHash()
        {
            int num = 0x13a;
            int num2 = 0x9f;
            this.hashCode = 0;
            for (int i = 0; i < this.bytes.Length; i++)
            {
                int? hashCode = this.hashCode;
                int num4 = num;
                int? nullable3 = hashCode.HasValue ? new int?(hashCode.GetValueOrDefault() * num4) : null;
                int num5 = this.bytes[i];
                this.hashCode = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault() + num5) : null;
                num *= num2;
            }
        }

        public bool Equals(Binary other) => 
            this.EqualsTo(other);

        public override bool Equals(object obj) => 
            this.EqualsTo(obj as Binary);

        private bool EqualsTo(Binary binary)
        {
            if (this != binary)
            {
                if (binary == null)
                {
                    return false;
                }
                if (this.bytes.Length != binary.bytes.Length)
                {
                    return false;
                }
                if (this.hashCode != binary.hashCode)
                {
                    return false;
                }
                int index = 0;
                int length = this.bytes.Length;
                while (index < length)
                {
                    if (this.bytes[index] != binary.bytes[index])
                    {
                        return false;
                    }
                    index++;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (!this.hashCode.HasValue)
            {
                this.ComputeHash();
            }
            return this.hashCode.Value;
        }

        public static bool operator ==(Binary binary1, Binary binary2) => 
            ((binary1 == binary2) || (((binary1 == null) && (binary2 == null)) || (((binary1 != null) && (binary2 != null)) && binary1.EqualsTo(binary2))));

        public static implicit operator Binary(byte[] value) => 
            new Binary(value);

        public static bool operator !=(Binary binary1, Binary binary2)
        {
            if (binary1 == binary2)
            {
                return false;
            }
            if ((binary1 == null) && (binary2 == null))
            {
                return false;
            }
            if ((binary1 != null) && (binary2 != null))
            {
                return !binary1.EqualsTo(binary2);
            }
            return true;
        }

        public byte[] ToArray()
        {
            byte[] destinationArray = new byte[this.bytes.Length];
            Array.Copy(this.bytes, destinationArray, destinationArray.Length);
            return destinationArray;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\"");
            builder.Append(Convert.ToBase64String(this.bytes, 0, this.bytes.Length));
            builder.Append("\"");
            return builder.ToString();
        }

        public int Length =>
            this.bytes.Length;
    }
}

