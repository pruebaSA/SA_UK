namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Globalization;

    internal sealed class StorageFunctionMemberPath
    {
        internal readonly System.Data.Metadata.Edm.AssociationSetEnd AssociationSetEnd;
        internal readonly ReadOnlyCollection<EdmMember> Members;

        internal StorageFunctionMemberPath(IEnumerable<EdmMember> members, AssociationSet associationSetNavigation)
        {
            this.Members = new ReadOnlyCollection<EdmMember>(new List<EdmMember>(EntityUtil.CheckArgumentNull<IEnumerable<EdmMember>>(members, "members")));
            if (associationSetNavigation != null)
            {
                this.AssociationSetEnd = associationSetNavigation.AssociationSetEnds[this.Members[1].Name];
            }
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[] { (this.AssociationSetEnd == null) ? string.Empty : ("[" + this.AssociationSetEnd.ParentAssociationSet.ToString() + "]"), StringUtil.BuildDelimitedList<EdmMember>(this.Members, null, ".") });
    }
}

