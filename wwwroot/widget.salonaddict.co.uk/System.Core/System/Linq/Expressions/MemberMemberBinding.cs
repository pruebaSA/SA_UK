namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Text;

    public sealed class MemberMemberBinding : MemberBinding
    {
        private ReadOnlyCollection<MemberBinding> bindings;

        internal MemberMemberBinding(MemberInfo member, ReadOnlyCollection<MemberBinding> bindings) : base(MemberBindingType.MemberBinding, member)
        {
            this.bindings = bindings;
        }

        internal override void BuildString(StringBuilder builder)
        {
            builder.Append(base.Member.Name);
            builder.Append(" = {");
            int num = 0;
            int count = this.bindings.Count;
            while (num < count)
            {
                if (num > 0)
                {
                    builder.Append(", ");
                }
                this.bindings[num].BuildString(builder);
                num++;
            }
            builder.Append("}");
        }

        public ReadOnlyCollection<MemberBinding> Bindings =>
            this.bindings;
    }
}

