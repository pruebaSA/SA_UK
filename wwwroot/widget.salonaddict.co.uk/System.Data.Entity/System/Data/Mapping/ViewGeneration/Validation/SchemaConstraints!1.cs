namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Text;

    internal class SchemaConstraints<TKeyConstraint> : InternalBase where TKeyConstraint: InternalBase
    {
        private List<TKeyConstraint> m_keyConstraints;

        internal SchemaConstraints()
        {
            this.m_keyConstraints = new List<TKeyConstraint>();
        }

        internal void Add(TKeyConstraint constraint)
        {
            EntityUtil.CheckArgumentNull<TKeyConstraint>(constraint, "constraint");
            this.m_keyConstraints.Add(constraint);
        }

        private static void ConstraintsToBuilder<Constraint>(IEnumerable<Constraint> constraints, StringBuilder builder) where Constraint: InternalBase
        {
            foreach (Constraint local in constraints)
            {
                local.ToCompactString(builder);
                builder.Append(Environment.NewLine);
            }
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            SchemaConstraints<TKeyConstraint>.ConstraintsToBuilder<TKeyConstraint>(this.m_keyConstraints, builder);
        }

        internal IEnumerable<TKeyConstraint> KeyConstraints =>
            this.m_keyConstraints;
    }
}

