namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Reflection;
    using System.Text;

    internal class MemberPathMap : MemberPathMapBase
    {
        private Dictionary<MemberPath, int> m_indexMap = new Dictionary<MemberPath, int>(MemberPath.EqualityComparer);
        private List<MemberPath> m_members = new List<MemberPath>();

        internal MemberPathMap()
        {
        }

        internal int CreateIndex(MemberPath member)
        {
            int count;
            if (!this.m_indexMap.TryGetValue(member, out count))
            {
                count = this.m_indexMap.Count;
                this.m_indexMap[member] = count;
                this.m_members.Add(member);
            }
            return count;
        }

        internal override int IndexOf(MemberPath member)
        {
            int num;
            if (this.m_indexMap.TryGetValue(member, out num))
            {
                return num;
            }
            return -1;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append('<');
            StringUtil.ToCommaSeparatedString(builder, this.m_members);
            builder.Append('>');
        }

        internal override int Count =>
            this.m_members.Count;

        internal override MemberPath this[int index] =>
            this.m_members[index];

        internal override IEnumerable<int> KeySlots
        {
            get
            {
                List<int> list = new List<int>();
                for (int i = 0; i < this.Count; i++)
                {
                    if (ProjectedSlot.IsKeySlot(i, this, 0))
                    {
                        list.Add(i);
                    }
                }
                return list;
            }
        }

        internal override IEnumerable<MemberPath> Members =>
            this.m_members;
    }
}

