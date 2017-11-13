namespace System.Linq.Expressions
{
    using System;
    using System.Reflection;
    using System.Text;

    public abstract class MemberBinding
    {
        private MemberInfo member;
        private MemberBindingType type;

        protected MemberBinding(MemberBindingType type, MemberInfo member)
        {
            this.type = type;
            this.member = member;
        }

        internal abstract void BuildString(StringBuilder builder);
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.BuildString(builder);
            return builder.ToString();
        }

        public MemberBindingType BindingType =>
            this.type;

        public MemberInfo Member =>
            this.member;
    }
}

