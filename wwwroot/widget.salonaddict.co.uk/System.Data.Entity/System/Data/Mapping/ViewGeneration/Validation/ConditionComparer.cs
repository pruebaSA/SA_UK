namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Globalization;

    internal class ConditionComparer : IEqualityComparer<Dictionary<MemberPath, Set<CellConstant>>>
    {
        public bool Equals(Dictionary<MemberPath, Set<CellConstant>> one, Dictionary<MemberPath, Set<CellConstant>> two)
        {
            Set<MemberPath> set = new Set<MemberPath>(one.Keys, MemberPath.EqualityComparer);
            Set<MemberPath> other = new Set<MemberPath>(two.Keys, MemberPath.EqualityComparer);
            if (!set.SetEquals(other))
            {
                return false;
            }
            foreach (MemberPath path in set)
            {
                Set<CellConstant> set3 = one[path];
                Set<CellConstant> set4 = two[path];
                if (!set3.SetEquals(set4))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(Dictionary<MemberPath, Set<CellConstant>> obj) => 
            obj.ToString().ToLower(CultureInfo.InvariantCulture).GetHashCode();
    }
}

