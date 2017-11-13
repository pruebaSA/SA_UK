namespace Microsoft.Practices.ObjectBuilder2
{
    using System;

    public class SelectedMemberWithParameters<TMemberInfoType> : SelectedMemberWithParameters
    {
        private TMemberInfoType memberInfo;

        protected SelectedMemberWithParameters(TMemberInfoType memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        protected TMemberInfoType MemberInfo =>
            this.memberInfo;
    }
}

