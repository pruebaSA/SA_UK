namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.ObjectModel;

    internal static class SqlServerCompatibilityCheck
    {
        internal static void ThrowIfUnsupported(SqlNode node, SqlNodeAnnotations annotations, SqlProvider.ProviderMode provider)
        {
            if (annotations.HasAnnotationType(typeof(SqlServerCompatibilityAnnotation)))
            {
                Visitor visitor = new Visitor(provider) {
                    annotations = annotations
                };
                visitor.Visit(node);
                if (visitor.reasons.Count > 0)
                {
                    throw Error.ExpressionNotSupportedForSqlServerVersion(visitor.reasons);
                }
            }
        }

        private class Visitor : SqlVisitor
        {
            internal SqlNodeAnnotations annotations;
            private SqlProvider.ProviderMode provider;
            internal Collection<string> reasons = new Collection<string>();

            internal Visitor(SqlProvider.ProviderMode provider)
            {
                this.provider = provider;
            }

            internal override SqlNode Visit(SqlNode node)
            {
                if (this.annotations.NodeIsAnnotated(node))
                {
                    foreach (SqlNodeAnnotation annotation in this.annotations.Get(node))
                    {
                        SqlServerCompatibilityAnnotation annotation2 = annotation as SqlServerCompatibilityAnnotation;
                        if ((annotation2 != null) && annotation2.AppliesTo(this.provider))
                        {
                            this.reasons.Add(annotation.Message);
                        }
                    }
                }
                return base.Visit(node);
            }
        }
    }
}

