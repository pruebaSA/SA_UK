namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Text;

    internal abstract class OneOfConst : BoolLiteral
    {
        private bool m_isFullyDone;
        private JoinTreeSlot m_slot;
        private CellConstantDomain m_values;

        protected OneOfConst(JoinTreeSlot slot, IEnumerable<CellConstant> values)
        {
            this.m_slot = slot;
            this.m_values = new CellConstantDomain(values, values);
        }

        protected OneOfConst(JoinTreeSlot slot, CellConstant value) : this(slot, new CellConstant[] { value })
        {
        }

        protected OneOfConst(JoinTreeSlot slot, CellConstantDomain domain)
        {
            this.m_slot = slot;
            this.m_values = domain;
            this.m_isFullyDone = true;
        }

        protected OneOfConst(JoinTreeSlot slot, IEnumerable<CellConstant> values, IEnumerable<CellConstant> possibleValues) : this(slot, new CellConstantDomain(values, possibleValues))
        {
        }

        protected OneOfConst(JoinTreeSlot slot, CellConstant value, IEnumerable<CellConstant> possibleValues) : this(slot, new CellConstantDomain(value, possibleValues))
        {
        }

        internal override StringBuilder AsNegatedUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            builder.Append("NOT(");
            builder = this.AsUserString(builder, blockAlias, canSkipIsNotNull);
            builder.Append(")");
            return builder;
        }

        internal override StringBuilder AsUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull) => 
            this.AsCql(builder, blockAlias, canSkipIsNotNull);

        internal static OneOfConst CreateFullOneOfConst(OneOfConst oneOfConst, IEnumerable<CellConstant> possibleValues)
        {
            if (oneOfConst is OneOfTypeConst)
            {
                return new OneOfTypeConst(oneOfConst.Slot, new CellConstantDomain(oneOfConst.Values.Values, possibleValues));
            }
            return new OneOfScalarConst(oneOfConst.Slot, new CellConstantDomain(oneOfConst.Values.Values, possibleValues));
        }

        private static OneOfConst Factory(JoinTreeSlot slot, CellConstantDomain domain, bool isTypeConstant)
        {
            if (isTypeConstant)
            {
                return new OneOfTypeConst(slot, domain);
            }
            return new OneOfScalarConst(slot, domain);
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> GetDomainBoolExpression(MemberDomainMap domainMap)
        {
            if (domainMap != null)
            {
                IEnumerable<CellConstant> domain = domainMap.GetDomain(this.m_slot.MemberPath);
                return BoolLiteral.MakeTermExpression(this, domain, this.m_values.Values);
            }
            return BoolLiteral.MakeTermExpression(this, this.m_values.AllPossibleValues, this.m_values.Values);
        }

        protected override int GetHash() => 
            (ProjectedSlot.EqualityComparer.GetHashCode(this.m_slot) ^ this.m_values.GetHash());

        protected override int GetIdentifierHash() => 
            ProjectedSlot.EqualityComparer.GetHashCode(this.m_slot);

        internal override void GetRequiredSlots(MemberPathMapBase projectedSlotMap, bool[] requiredSlots)
        {
            MemberPath memberPath = this.Slot.MemberPath;
            int index = projectedSlotMap.IndexOf(memberPath);
            requiredSlots[index] = true;
        }

        private void InvertOutputStringForTypeConstant(StringBuilder builder, System.Data.Common.Utils.Set<CellConstant> constants, MetadataWorkspace workspace)
        {
            StringBuilder builder2 = new StringBuilder();
            System.Data.Common.Utils.Set<EdmType> set = new System.Data.Common.Utils.Set<EdmType>();
            foreach (EdmType type2 in MetadataHelper.GetTypeAndSubtypesOf(this.Slot.MemberPath.EdmType, workspace, false))
            {
                set.Add(type2);
            }
            System.Data.Common.Utils.Set<EdmType> other = new System.Data.Common.Utils.Set<EdmType>();
            foreach (CellConstant constant in constants)
            {
                TypeConstant constant2 = (TypeConstant) constant;
                other.Add(constant2.CdmType);
            }
            set.Subtract(other);
            bool flag = true;
            foreach (EdmType type3 in set)
            {
                if (!flag)
                {
                    builder2.Append(System.Data.Entity.Strings.ViewGen_CommaBlank);
                }
                flag = false;
                builder2.Append(type3.Name);
            }
            builder.Append(System.Data.Entity.Strings.ViewGen_OneOfConst_IsOneOfTypes_0(builder2.ToString()));
        }

        protected override bool IsEqualTo(BoolLiteral right)
        {
            OneOfConst objB = right as OneOfConst;
            if (objB == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, objB))
            {
                return true;
            }
            if (!ProjectedSlot.EqualityComparer.Equals(this.m_slot, objB.m_slot))
            {
                return false;
            }
            return this.m_values.IsEqualTo(objB.m_values);
        }

        protected override bool IsIdentifierEqualTo(BoolLiteral right)
        {
            OneOfConst objB = right as OneOfConst;
            if (objB == null)
            {
                return false;
            }
            return (object.ReferenceEquals(this, objB) || ProjectedSlot.EqualityComparer.Equals(this.m_slot, objB.m_slot));
        }

        internal override BoolLiteral RemapBool(Dictionary<JoinTreeNode, JoinTreeNode> remap)
        {
            JoinTreeSlot slot = (JoinTreeSlot) this.Slot.RemapSlot(remap);
            return Factory(slot, this.Values, base.GetType() == typeof(OneOfTypeConst));
        }

        internal void ToUserString(bool invertOutput, StringBuilder builder, MetadataWorkspace workspace)
        {
            NegatedCellConstant constant = null;
            System.Data.Common.Utils.Set<CellConstant> set;
            foreach (CellConstant constant2 in this.Values.Values)
            {
                constant = constant2 as NegatedCellConstant;
                if (constant != null)
                {
                    break;
                }
            }
            if (constant != null)
            {
                invertOutput = !invertOutput;
                set = new System.Data.Common.Utils.Set<CellConstant>(constant.Elements, CellConstant.EqualityComparer);
                foreach (CellConstant constant3 in this.Values.Values)
                {
                    if (!(constant3 is NegatedCellConstant))
                    {
                        set.Remove(constant3);
                    }
                }
            }
            else
            {
                set = new System.Data.Common.Utils.Set<CellConstant>(this.Values.Values, CellConstant.EqualityComparer);
            }
            bool flag = (set.Count == 1) && set.Single<CellConstant>().IsNull();
            bool flag2 = this is OneOfTypeConst;
            Func<object, string> func = null;
            Func<object, object, string> func2 = null;
            if (invertOutput)
            {
                if (flag)
                {
                    func = flag2 ? new Func<object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_IsNonNullable_0) : new Func<object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_MustBeNonNullable_0);
                }
                else if (set.Count == 1)
                {
                    func2 = flag2 ? new Func<object, object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_IsNotEqualTo_1) : new Func<object, object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_MustNotBeEqualTo_1);
                }
                else
                {
                    func2 = flag2 ? new Func<object, object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_IsNotOneOf_1) : new Func<object, object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_MustNotBeOneOf_1);
                }
            }
            else if (flag)
            {
                func = flag2 ? new Func<object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_MustBeNull_0) : new Func<object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_MustBeNull_0);
            }
            else if (set.Count == 1)
            {
                func2 = flag2 ? new Func<object, object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_IsEqualTo_1) : new Func<object, object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_MustBeEqualTo_1);
            }
            else
            {
                func2 = flag2 ? new Func<object, object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_IsOneOf_1) : new Func<object, object, string>(System.Data.Entity.Strings.ViewGen_OneOfConst_MustBeOneOf_1);
            }
            StringBuilder builder2 = new StringBuilder();
            CellConstant.ConstantsToUserString(builder2, set);
            string arg = this.m_slot.MemberPath.PathToString(false);
            if (flag2)
            {
                arg = "TypeOf(" + arg + ")";
            }
            if (func != null)
            {
                builder.Append(func(arg));
            }
            else
            {
                builder.Append(func2(arg, builder2.ToString()));
            }
            if (invertOutput && flag2)
            {
                this.InvertOutputStringForTypeConstant(builder, set, workspace);
            }
        }

        internal bool IsFullyDone =>
            this.m_isFullyDone;

        internal JoinTreeSlot Slot =>
            this.m_slot;

        internal CellConstantDomain Values =>
            this.m_values;
    }
}

