namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Reflection;

    public class SelectedConstructor : SelectedMemberWithParameters<ConstructorInfo>
    {
        public SelectedConstructor(ConstructorInfo constructor) : base(constructor)
        {
        }

        public ConstructorInfo Constructor =>
            base.MemberInfo;
    }
}

