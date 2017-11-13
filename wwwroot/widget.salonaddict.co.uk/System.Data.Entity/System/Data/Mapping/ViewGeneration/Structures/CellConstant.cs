namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class CellConstant : InternalBase
    {
        internal static readonly CellConstant AllOtherConstants = new CellConstant();
        internal static readonly IEqualityComparer<CellConstant> EqualityComparer = new CellConstantComparer();
        internal static readonly CellConstant NotNull = NegatedCellConstant.CreateNotNull();
        internal static readonly CellConstant Null = new CellConstant();
        internal static readonly CellConstant Undefined = new CellConstant();

        protected CellConstant()
        {
        }

        internal virtual StringBuilder AsCql(StringBuilder builder, MemberPath outputMember, string blockAlias)
        {
            EdmType edmType = Helper.GetModelTypeUsage(outputMember.LastMember).EdmType;
            builder.Append("CAST(NULL AS ");
            CqlWriter.AppendEscapedTypeName(builder, edmType);
            builder.Append(')');
            return builder;
        }

        internal static void ConstantsToUserString(StringBuilder builder, Set<CellConstant> constants)
        {
            bool flag = true;
            foreach (CellConstant constant in constants)
            {
                if (!flag)
                {
                    builder.Append(Strings.ViewGen_CommaBlank);
                }
                flag = false;
                string str = constant.ToUserString();
                builder.Append(str);
            }
        }

        public override bool Equals(object obj)
        {
            CellConstant right = obj as CellConstant;
            if (right == null)
            {
                return false;
            }
            return this.IsEqualTo(right);
        }

        protected virtual int GetHash() => 
            0;

        public override int GetHashCode() => 
            base.GetHashCode();

        internal virtual bool HasNotNull() => 
            false;

        private void InternalToString(StringBuilder builder, bool isInvariant)
        {
            string str;
            if (this == Null)
            {
                str = isInvariant ? "NULL" : Strings.ViewGen_Null;
            }
            else if (this == Undefined)
            {
                str = "?";
            }
            else if (this == NotNull)
            {
                str = isInvariant ? "NOT_NULL" : Strings.ViewGen_NotNull;
            }
            else if (this == AllOtherConstants)
            {
                str = "AllOtherConstants";
            }
            else
            {
                str = isInvariant ? "FAILURE" : Strings.ViewGen_Failure;
            }
            builder.Append(str);
        }

        protected virtual bool IsEqualTo(CellConstant right) => 
            (this == right);

        internal virtual bool IsNotNull() => 
            false;

        internal bool IsNull() => 
            EqualityComparer.Equals(this, Null);

        internal bool IsUndefined() => 
            EqualityComparer.Equals(this, Undefined);

        internal override void ToCompactString(StringBuilder builder)
        {
            this.InternalToString(builder, true);
        }

        internal virtual string ToUserString()
        {
            StringBuilder builder = new StringBuilder();
            this.InternalToString(builder, false);
            return builder.ToString();
        }

        private class CellConstantComparer : IEqualityComparer<CellConstant>
        {
            public bool Equals(CellConstant left, CellConstant right) => 
                (object.ReferenceEquals(left, right) || (((left != null) && (right != null)) && left.IsEqualTo(right)));

            public int GetHashCode(CellConstant key)
            {
                EntityUtil.CheckArgumentNull<CellConstant>(key, "key");
                return key.GetHash();
            }
        }
    }
}

