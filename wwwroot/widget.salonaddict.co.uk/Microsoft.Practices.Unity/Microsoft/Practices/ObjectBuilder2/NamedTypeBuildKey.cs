namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Globalization;

    public class NamedTypeBuildKey
    {
        private readonly string name;
        private readonly System.Type type;

        public NamedTypeBuildKey(System.Type type) : this(type, null)
        {
        }

        public NamedTypeBuildKey(System.Type type, string name)
        {
            this.type = type;
            this.name = !string.IsNullOrEmpty(name) ? name : null;
        }

        public override bool Equals(object obj)
        {
            NamedTypeBuildKey key = obj as NamedTypeBuildKey;
            if (key == null)
            {
                return false;
            }
            return (this == key);
        }

        public override int GetHashCode()
        {
            int num = (this.type == null) ? 0 : this.type.GetHashCode();
            int num2 = (this.name == null) ? 0 : this.name.GetHashCode();
            return ((num + 0x25) ^ (num2 + 0x11));
        }

        public static NamedTypeBuildKey Make<T>() => 
            new NamedTypeBuildKey(typeof(T));

        public static NamedTypeBuildKey Make<T>(string name) => 
            new NamedTypeBuildKey(typeof(T), name);

        public static bool operator ==(NamedTypeBuildKey left, NamedTypeBuildKey right)
        {
            bool flag = object.ReferenceEquals(left, null);
            bool flag2 = object.ReferenceEquals(right, null);
            if (flag && flag2)
            {
                return true;
            }
            if (flag || flag2)
            {
                return false;
            }
            return ((left.type == right.type) && (string.Compare(left.name, right.name, StringComparison.Ordinal) == 0));
        }

        public static bool operator !=(NamedTypeBuildKey left, NamedTypeBuildKey right) => 
            !(left == right);

        public override string ToString()
        {
            object[] args = new object[] { this.type, this.name ?? "null" };
            return string.Format(CultureInfo.InvariantCulture, "Build Key[{0}, {1}]", args);
        }

        public string Name =>
            this.name;

        public System.Type Type =>
            this.type;
    }
}

