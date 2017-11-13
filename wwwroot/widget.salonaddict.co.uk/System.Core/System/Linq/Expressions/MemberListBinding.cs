namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Text;

    public sealed class MemberListBinding : MemberBinding
    {
        private ReadOnlyCollection<ElementInit> initializers;

        internal MemberListBinding(MemberInfo member, ReadOnlyCollection<ElementInit> initializers) : base(MemberBindingType.ListBinding, member)
        {
            this.initializers = initializers;
        }

        internal override void BuildString(StringBuilder builder)
        {
            builder.Append(base.Member.Name);
            builder.Append(" = {");
            int num = 0;
            int count = this.initializers.Count;
            while (num < count)
            {
                if (num > 0)
                {
                    builder.Append(", ");
                }
                this.initializers[num].BuildString(builder);
                num++;
            }
            builder.Append("}");
        }

        public ReadOnlyCollection<ElementInit> Initializers =>
            this.initializers;
    }
}

