namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal static class ViewSimplifier
    {
        private static void CollapseNestedProjection(ExpressionReplacement replacement, DbProjectExpression outerProject, DbExpression outerProjection, DbProjectExpression innerProject, DbNewInstanceExpression innerNew)
        {
            replacement.VisitReplacement = true;
            Dictionary<string, DbExpression> varRefMemberBindings = new Dictionary<string, DbExpression>(innerNew.Arguments.Count);
            RowType edmType = (RowType) innerNew.ResultType.EdmType;
            for (int i = 0; i < edmType.Members.Count; i++)
            {
                varRefMemberBindings[edmType.Members[i].Name] = innerNew.Arguments[i];
            }
            ProjectionCollapser collapser = new ProjectionCollapser(varRefMemberBindings, outerProject.Input);
            DbExpression projection = new ExpressionReplacer(new ReplacerCallback(collapser.CollapseProjection)).Replace(outerProjection);
            DbProjectExpression expression2 = outerProject.CommandTree.CreateProjectExpression(innerProject.Input, projection);
            if (!collapser.IsDoomed)
            {
                replacement.Replacement = expression2;
            }
        }

        private static void CollapseProject(ExpressionReplacement replacement)
        {
            DbProjectExpression expression2;
            DbExpression expression3;
            DbProjectExpression expression4;
            DbNewInstanceExpression expression5;
            if (TryMatchNestedProjectPattern(replacement.Current, out expression2, out expression3, out expression4, out expression5))
            {
                CollapseNestedProjection(replacement, expression2, expression3, expression4, expression5);
            }
        }

        private static void SimplifyCaseStatements(ExpressionReplacement replacement)
        {
            if (replacement.Current.ExpressionKind == DbExpressionKind.Case)
            {
                DbCaseExpression current = (DbCaseExpression) replacement.Current;
                bool flag = false;
                List<DbExpression> whenExpressions = new List<DbExpression>(current.When.Count);
                foreach (DbExpression expression3 in current.When)
                {
                    DbExpression expression4;
                    if (TrySimplifyPredicate(expression3, out expression4))
                    {
                        whenExpressions.Add(expression4);
                        flag = true;
                    }
                    else
                    {
                        whenExpressions.Add(expression3);
                    }
                }
                if (flag)
                {
                    replacement.Replacement = current.CommandTree.CreateCaseExpression(whenExpressions, current.Then, current.Else);
                    replacement.VisitReplacement = true;
                }
            }
        }

        internal static DbQueryCommandTree SimplifyView(DbQueryCommandTree view)
        {
            view = view.Clone();
            view.Replace(new ReplacerCallback(ViewSimplifier.CollapseProject));
            view.Replace(new ReplacerCallback(ViewSimplifier.SimplifyCaseStatements));
            return view;
        }

        private static bool TryMatchNestedProjectPattern(DbExpression outerCandidate, out DbProjectExpression outerProject, out DbExpression outerProjection, out DbProjectExpression innerProject, out DbNewInstanceExpression innerNew)
        {
            outerProject = null;
            outerProjection = null;
            innerProject = null;
            innerNew = null;
            if (outerCandidate.ExpressionKind != DbExpressionKind.Project)
            {
                return false;
            }
            outerProject = (DbProjectExpression) outerCandidate;
            outerProjection = outerProject.Projection;
            if (outerProject.Input.Expression.ExpressionKind != DbExpressionKind.Project)
            {
                return false;
            }
            innerProject = (DbProjectExpression) outerProject.Input.Expression;
            if (innerProject.Projection.ExpressionKind != DbExpressionKind.NewInstance)
            {
                return false;
            }
            innerNew = (DbNewInstanceExpression) innerProject.Projection;
            return true;
        }

        private static bool TrySimplifyPredicate(DbExpression predicate, out DbExpression simplified)
        {
            simplified = null;
            if (predicate.ExpressionKind != DbExpressionKind.Case)
            {
                return false;
            }
            DbCaseExpression expression = (DbCaseExpression) predicate;
            if ((expression.Then.Count != 1) && (expression.Then[0].ExpressionKind == DbExpressionKind.Constant))
            {
                return false;
            }
            DbConstantExpression expression2 = (DbConstantExpression) expression.Then[0];
            bool flag = true;
            if (!flag.Equals(expression2.Value))
            {
                return false;
            }
            if (expression.Else != null)
            {
                if (expression.Else.ExpressionKind != DbExpressionKind.Constant)
                {
                    return false;
                }
                DbConstantExpression @else = (DbConstantExpression) expression.Else;
                bool flag2 = false;
                if (!flag2.Equals(@else.Value))
                {
                    return false;
                }
            }
            simplified = expression.When[0];
            return true;
        }

        private class ProjectionCollapser
        {
            private bool m_doomed;
            private DbExpressionBinding m_outerBinding;
            private Dictionary<string, DbExpression> m_varRefMemberBindings;

            internal ProjectionCollapser(Dictionary<string, DbExpression> varRefMemberBindings, DbExpressionBinding outerBinding)
            {
                this.m_varRefMemberBindings = varRefMemberBindings;
                this.m_outerBinding = outerBinding;
            }

            internal void CollapseProjection(ExpressionReplacement replacement)
            {
                if (replacement.Current.ExpressionKind == DbExpressionKind.Property)
                {
                    DbPropertyExpression current = (DbPropertyExpression) replacement.Current;
                    if ((current.Instance.ExpressionKind == DbExpressionKind.VariableReference) && this.IsOuterBindingVarRef((DbVariableReferenceExpression) current.Instance))
                    {
                        replacement.VisitReplacement = false;
                        replacement.Replacement = this.m_varRefMemberBindings[current.Property.Name];
                    }
                }
                else if (replacement.Current.ExpressionKind == DbExpressionKind.VariableReference)
                {
                    DbVariableReferenceExpression varRef = (DbVariableReferenceExpression) replacement.Current;
                    if (this.IsOuterBindingVarRef(varRef))
                    {
                        this.m_doomed = true;
                    }
                }
            }

            private bool IsOuterBindingVarRef(DbVariableReferenceExpression varRef) => 
                (varRef.VariableName == this.m_outerBinding.VariableName);

            internal bool IsDoomed =>
                this.m_doomed;
        }
    }
}

