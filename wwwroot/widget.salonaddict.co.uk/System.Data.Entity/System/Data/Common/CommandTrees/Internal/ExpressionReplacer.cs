namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Runtime.InteropServices;

    internal class ExpressionReplacer : DbExpressionVisitor<DbExpression>
    {
        private ExpressionReplacement _replacement = new ExpressionReplacement();
        private ReplacerCallback _replacer;

        internal ExpressionReplacer(ReplacerCallback replacer)
        {
            this._replacer = replacer;
        }

        private bool DoReplacement(DbExpression original, out DbExpression result)
        {
            result = original;
            this._replacement.Current = original;
            this._replacement.Replacement = original;
            this._replacement.VisitReplacement = true;
            this._replacer(this._replacement);
            if (object.ReferenceEquals(this._replacement.Current, this._replacement.Replacement))
            {
                return false;
            }
            if (this._replacement.VisitReplacement)
            {
                result = this.Replace(this._replacement.Replacement);
            }
            else
            {
                result = this._replacement.Replacement;
            }
            return true;
        }

        internal DbExpression Replace(DbExpression toReplace) => 
            toReplace.Accept<DbExpression>(this);

        private DbExpression ReplaceBinary(DbBinaryExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            DbExpression objA = this.Replace(e.Left);
            if (!object.ReferenceEquals(objA, e.Left))
            {
                e.Left = objA;
            }
            objA = this.Replace(e.Right);
            if (!object.ReferenceEquals(objA, e.Right))
            {
                e.Right = objA;
            }
            return e;
        }

        internal void ReplaceBinding(DbExpressionBinding binding)
        {
            DbExpression objA = this.Replace(binding.Expression);
            if (!object.ReferenceEquals(objA, binding.Expression))
            {
                binding.Expression = objA;
            }
        }

        private void ReplaceBinding(DbGroupExpressionBinding binding)
        {
            DbExpression objA = this.Replace(binding.Expression);
            if (!object.ReferenceEquals(objA, binding.Expression))
            {
                binding.Expression = objA;
            }
        }

        private void ReplaceList(IList<DbExpression> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                DbExpression objA = this.Replace(list[i]);
                if (!object.ReferenceEquals(objA, list[i]))
                {
                    ExpressionList list2 = list as ExpressionList;
                    if (list2 != null)
                    {
                        list2.SetAt(i, objA);
                    }
                    else
                    {
                        list[i] = objA;
                    }
                }
            }
        }

        private void ReplaceSortKeys(IList<DbSortClause> keys)
        {
            foreach (DbSortClause clause in keys)
            {
                DbExpression objA = this.Replace(clause.Expression);
                if (!object.ReferenceEquals(objA, clause.Expression))
                {
                    clause.Expression = objA;
                }
            }
        }

        private DbExpression ReplaceTerminal(DbExpression e)
        {
            DbExpression result = null;
            this.DoReplacement(e, out result);
            return result;
        }

        private DbExpression ReplaceUnary(DbUnaryExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            DbExpression objA = this.Replace(e.Argument);
            if (!object.ReferenceEquals(objA, e.Argument))
            {
                e.Argument = objA;
            }
            return e;
        }

        public override DbExpression Visit(DbAndExpression e) => 
            this.ReplaceBinary(e);

        public override DbExpression Visit(DbApplyExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceBinding(e.Input);
            this.ReplaceBinding(e.Apply);
            return e;
        }

        public override DbExpression Visit(DbArithmeticExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceList(e.Arguments);
            return e;
        }

        public override DbExpression Visit(DbCaseExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceList(e.When);
            this.ReplaceList(e.Then);
            result = this.Replace(e.Else);
            if (!object.ReferenceEquals(result, e.Else))
            {
                e.Else = result;
            }
            return e;
        }

        public override DbExpression Visit(DbCastExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbComparisonExpression e) => 
            this.ReplaceBinary(e);

        public override DbExpression Visit(DbConstantExpression e) => 
            this.ReplaceTerminal(e);

        public override DbExpression Visit(DbCrossJoinExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            foreach (DbExpressionBinding binding in e.Inputs)
            {
                this.ReplaceBinding(binding);
            }
            return e;
        }

        public override DbExpression Visit(DbDerefExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbDistinctExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbElementExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbEntityRefExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbExceptExpression e) => 
            this.ReplaceBinary(e);

        public override DbExpression Visit(DbExpression e)
        {
            throw EntityUtil.NotSupported(Strings.Cqt_General_UnsupportedExpression(e.GetType().FullName));
        }

        public override DbExpression Visit(DbFilterExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceBinding(e.Input);
            DbExpression objA = this.Replace(e.Predicate);
            if (!object.ReferenceEquals(objA, e.Predicate))
            {
                e.Predicate = objA;
            }
            return e;
        }

        public override DbExpression Visit(DbFunctionExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceList(e.Arguments);
            if (e.IsLambda)
            {
                DbExpression objA = this.Replace(e.LambdaBody);
                if (!object.ReferenceEquals(objA, e.LambdaBody))
                {
                    e.LambdaBody = objA;
                }
            }
            return e;
        }

        public override DbExpression Visit(DbGroupByExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceBinding(e.Input);
            this.ReplaceList(e.Keys);
            foreach (DbAggregate aggregate in e.Aggregates)
            {
                this.ReplaceList(aggregate.Arguments);
            }
            return e;
        }

        public override DbExpression Visit(DbIntersectExpression e) => 
            this.ReplaceBinary(e);

        public override DbExpression Visit(DbIsEmptyExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbIsNullExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbIsOfExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbJoinExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceBinding(e.Left);
            this.ReplaceBinding(e.Right);
            DbExpression objA = this.Replace(e.JoinCondition);
            if (!object.ReferenceEquals(objA, e.JoinCondition))
            {
                e.JoinCondition = objA;
            }
            return e;
        }

        public override DbExpression Visit(DbLikeExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            DbExpression objA = this.Replace(e.Argument);
            if (!object.ReferenceEquals(objA, e.Argument))
            {
                e.Argument = objA;
            }
            objA = this.Replace(e.Pattern);
            if (!object.ReferenceEquals(objA, e.Pattern))
            {
                e.Pattern = objA;
            }
            objA = this.Replace(e.Escape);
            if (!object.ReferenceEquals(objA, e.Escape))
            {
                e.Escape = objA;
            }
            return e;
        }

        public override DbExpression Visit(DbLimitExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            DbExpression objA = this.Replace(e.Argument);
            if (!object.ReferenceEquals(objA, e.Argument))
            {
                e.Argument = objA;
            }
            objA = this.Replace(e.Limit);
            if (!object.ReferenceEquals(objA, e.Limit))
            {
                e.Limit = objA;
            }
            return e;
        }

        public override DbExpression Visit(DbNewInstanceExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceList(e.Arguments);
            if (e.HasRelatedEntityReferences)
            {
                foreach (DbRelatedEntityRef ref2 in e.RelatedEntityReferences)
                {
                    DbExpression objA = this.Replace(ref2.TargetEntityReference);
                    if (!object.ReferenceEquals(objA, ref2.TargetEntityReference))
                    {
                        ref2.TargetEntityReference = objA;
                    }
                }
            }
            return e;
        }

        public override DbExpression Visit(DbNotExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbNullExpression e) => 
            this.ReplaceTerminal(e);

        public override DbExpression Visit(DbOfTypeExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbOrExpression e) => 
            this.ReplaceBinary(e);

        public override DbExpression Visit(DbParameterReferenceExpression e) => 
            this.ReplaceTerminal(e);

        public override DbExpression Visit(DbProjectExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceBinding(e.Input);
            DbExpression objA = this.Replace(e.Projection);
            if (!object.ReferenceEquals(objA, e.Projection))
            {
                e.Projection = objA;
            }
            return e;
        }

        public override DbExpression Visit(DbPropertyExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            if (e.Instance != null)
            {
                DbExpression objA = this.Replace(e.Instance);
                if (!object.ReferenceEquals(objA, e.Instance))
                {
                    e.Instance = objA;
                }
            }
            return e;
        }

        public override DbExpression Visit(DbQuantifierExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceBinding(e.Input);
            DbExpression objA = this.Replace(e.Predicate);
            if (!object.ReferenceEquals(objA, e.Predicate))
            {
                e.Predicate = objA;
            }
            return e;
        }

        public override DbExpression Visit(DbRefExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbRefKeyExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbRelationshipNavigationExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            DbExpression objA = this.Replace(e.NavigationSource);
            if (!object.ReferenceEquals(objA, e.NavigationSource))
            {
                e.NavigationSource = objA;
            }
            return e;
        }

        public override DbExpression Visit(DbScanExpression e) => 
            this.ReplaceTerminal(e);

        public override DbExpression Visit(DbSkipExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceBinding(e.Input);
            this.ReplaceSortKeys(e.SortOrder);
            DbExpression objA = this.Replace(e.Count);
            if (!object.ReferenceEquals(objA, e.Count))
            {
                e.Count = objA;
            }
            return e;
        }

        public override DbExpression Visit(DbSortExpression e)
        {
            DbExpression result = null;
            if (this.DoReplacement(e, out result))
            {
                return result;
            }
            this.ReplaceBinding(e.Input);
            this.ReplaceSortKeys(e.SortOrder);
            return e;
        }

        public override DbExpression Visit(DbTreatExpression e) => 
            this.ReplaceUnary(e);

        public override DbExpression Visit(DbUnionAllExpression e) => 
            this.ReplaceBinary(e);

        public override DbExpression Visit(DbVariableReferenceExpression e) => 
            this.ReplaceTerminal(e);
    }
}

