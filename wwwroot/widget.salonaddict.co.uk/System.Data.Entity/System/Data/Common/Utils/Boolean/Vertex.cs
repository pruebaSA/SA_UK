namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Diagnostics;
    using System.Globalization;

    internal sealed class Vertex : IEquatable<Vertex>
    {
        internal readonly Vertex[] Children;
        internal static readonly Vertex One = new Vertex();
        internal readonly int Variable;
        internal static readonly Vertex Zero = new Vertex();

        private Vertex()
        {
            this.Variable = 0x7fffffff;
            this.Children = new Vertex[0];
        }

        internal Vertex(int variable, Vertex[] children)
        {
            EntityUtil.BoolExprAssert(variable < 0x7fffffff, "exceeded number of supported variables");
            this.Variable = variable;
            this.Children = children;
        }

        [Conditional("DEBUG")]
        private static void AssertConstructorArgumentsValid(int variable, Vertex[] children)
        {
            Vertex[] vertexArray = children;
            for (int i = 0; i < vertexArray.Length; i++)
            {
                Vertex vertex1 = vertexArray[i];
            }
        }

        public bool Equals(Vertex other) => 
            object.ReferenceEquals(this, other);

        public override bool Equals(object obj) => 
            base.Equals(obj);

        public override int GetHashCode() => 
            base.GetHashCode();

        internal bool IsOne() => 
            object.ReferenceEquals(One, this);

        internal bool IsSink() => 
            (this.Variable == 0x7fffffff);

        internal bool IsZero() => 
            object.ReferenceEquals(Zero, this);

        public override string ToString()
        {
            if (this.IsOne())
            {
                return "_1_";
            }
            if (this.IsZero())
            {
                return "_0_";
            }
            return string.Format(CultureInfo.InvariantCulture, "<{0}, {1}>", new object[] { this.Variable, StringUtil.ToCommaSeparatedString(this.Children) });
        }
    }
}

