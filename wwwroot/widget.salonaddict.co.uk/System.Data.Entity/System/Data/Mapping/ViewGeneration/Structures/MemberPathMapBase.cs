namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Reflection;

    internal abstract class MemberPathMapBase : InternalBase
    {
        protected MemberPathMapBase()
        {
        }

        internal abstract int IndexOf(MemberPath member);

        internal abstract int Count { get; }

        internal abstract MemberPath this[int index] { get; }

        internal abstract IEnumerable<int> KeySlots { get; }

        internal abstract IEnumerable<MemberPath> Members { get; }
    }
}

