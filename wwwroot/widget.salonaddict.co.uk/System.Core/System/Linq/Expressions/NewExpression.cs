namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Text;

    public sealed class NewExpression : Expression
    {
        private ReadOnlyCollection<Expression> arguments;
        private ConstructorInfo constructor;
        private ReadOnlyCollection<MemberInfo> members;

        internal NewExpression(Type type, ConstructorInfo constructor, ReadOnlyCollection<Expression> arguments) : base(ExpressionType.New, type)
        {
            this.constructor = constructor;
            this.arguments = arguments;
        }

        internal NewExpression(Type type, ConstructorInfo constructor, ReadOnlyCollection<Expression> arguments, ReadOnlyCollection<MemberInfo> members) : base(ExpressionType.New, type)
        {
            this.constructor = constructor;
            this.arguments = arguments;
            this.members = members;
        }

        internal override void BuildString(StringBuilder builder)
        {
            Type type;
            type = (this.constructor == null) ? (type = base.Type) : this.constructor.DeclaringType;
            builder.Append("new ");
            int count = this.arguments.Count;
            builder.Append(type.Name);
            builder.Append("(");
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }
                    if (this.members != null)
                    {
                        PropertyInfo info = null;
                        if ((this.members[i].MemberType == MemberTypes.Method) && ((info = GetPropertyNoThrow((MethodInfo) this.members[i])) != null))
                        {
                            builder.Append(info.Name);
                        }
                        else
                        {
                            builder.Append(this.members[i].Name);
                        }
                        builder.Append(" = ");
                    }
                    this.arguments[i].BuildString(builder);
                }
            }
            builder.Append(")");
        }

        private static PropertyInfo GetPropertyNoThrow(MethodInfo method)
        {
            if (method != null)
            {
                Type declaringType = method.DeclaringType;
                BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public;
                bindingAttr |= method.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
                foreach (PropertyInfo info in declaringType.GetProperties(bindingAttr))
                {
                    if (info.CanRead && (method == info.GetGetMethod(true)))
                    {
                        return info;
                    }
                    if (info.CanWrite && (method == info.GetSetMethod(true)))
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        public ReadOnlyCollection<Expression> Arguments =>
            this.arguments;

        public ConstructorInfo Constructor =>
            this.constructor;

        public ReadOnlyCollection<MemberInfo> Members =>
            this.members;
    }
}

