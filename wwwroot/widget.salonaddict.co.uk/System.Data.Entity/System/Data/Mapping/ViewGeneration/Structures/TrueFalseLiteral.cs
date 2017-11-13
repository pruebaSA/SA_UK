namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Linq;

    internal abstract class TrueFalseLiteral : BoolLiteral
    {
        protected TrueFalseLiteral()
        {
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> FixRange(System.Data.Common.Utils.Set<CellConstant> range, MemberDomainMap memberDomainMap)
        {
            ScalarConstant constant = (ScalarConstant) range.First<CellConstant>();
            BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> domainBoolExpression = this.GetDomainBoolExpression(memberDomainMap);
            if (!((bool) constant.Value))
            {
                domainBoolExpression = new NotExpr<DomainConstraint<BoolLiteral, CellConstant>>(domainBoolExpression);
            }
            return domainBoolExpression;
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> GetDomainBoolExpression(MemberDomainMap domainMap)
        {
            IEnumerable<CellConstant> elements = new CellConstant[] { new ScalarConstant(true) };
            IEnumerable<CellConstant> enumerable2 = new CellConstant[] { new ScalarConstant(true), new ScalarConstant(false) };
            System.Data.Common.Utils.Set<CellConstant> domain = new System.Data.Common.Utils.Set<CellConstant>(enumerable2, CellConstant.EqualityComparer).MakeReadOnly();
            System.Data.Common.Utils.Set<CellConstant> range = new System.Data.Common.Utils.Set<CellConstant>(elements, CellConstant.EqualityComparer).MakeReadOnly();
            return BoolLiteral.MakeTermExpression(this, domain, range);
        }
    }
}

