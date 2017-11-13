namespace System.Data.Common.Utils
{
    using System;
    using System.Text;

    internal abstract class InternalBase
    {
        protected InternalBase()
        {
        }

        internal virtual bool CheckRepInvariant() => 
            true;

        internal abstract void ToCompactString(StringBuilder builder);
        internal virtual string ToFullString()
        {
            StringBuilder builder = new StringBuilder();
            this.ToFullString(builder);
            return builder.ToString();
        }

        internal virtual void ToFullString(StringBuilder builder)
        {
            this.ToCompactString(builder);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.ToCompactString(builder);
            return builder.ToString();
        }
    }
}

