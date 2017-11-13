namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Reflection;

    public class SelectedMethod : SelectedMemberWithParameters<MethodInfo>
    {
        public SelectedMethod(MethodInfo method) : base(method)
        {
        }

        public MethodInfo Method =>
            base.MemberInfo;
    }
}

