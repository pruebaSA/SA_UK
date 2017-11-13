namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Metadata.Edm;

    internal class ExtentPair
    {
        private EntitySetBase m_left;
        private EntitySetBase m_right;

        internal ExtentPair(EntitySetBase left, EntitySetBase right)
        {
            this.m_left = left;
            this.m_right = right;
        }

        public override bool Equals(object obj)
        {
            ExtentPair pair = obj as ExtentPair;
            return (((pair != null) && pair.Left.Equals(this.Left)) && pair.Right.Equals(this.Right));
        }

        public override int GetHashCode() => 
            ((this.Left.GetHashCode() << 4) ^ this.Right.GetHashCode());

        internal EntitySetBase Left =>
            this.m_left;

        internal EntitySetBase Right =>
            this.m_right;
    }
}

