namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal class RewritingSimplifier<T_Tile> where T_Tile: class
    {
        private readonly T_Tile m_originalRewriting;
        private readonly RewritingProcessor<T_Tile> m_qp;
        private readonly T_Tile m_toAvoid;
        private readonly Dictionary<T_Tile, TileOpKind> m_usedViews;

        private RewritingSimplifier(T_Tile originalRewriting, T_Tile toAvoid, Dictionary<T_Tile, TileOpKind> usedViews, RewritingProcessor<T_Tile> qp)
        {
            this.m_usedViews = new Dictionary<T_Tile, TileOpKind>();
            this.m_originalRewriting = originalRewriting;
            this.m_toAvoid = toAvoid;
            this.m_qp = qp;
            this.m_usedViews = usedViews;
        }

        private RewritingSimplifier(T_Tile rewriting, T_Tile toFill, T_Tile toAvoid, RewritingProcessor<T_Tile> qp)
        {
            this.m_usedViews = new Dictionary<T_Tile, TileOpKind>();
            this.m_originalRewriting = toFill;
            this.m_toAvoid = toAvoid;
            this.m_qp = qp;
            this.m_usedViews = new Dictionary<T_Tile, TileOpKind>();
            this.GatherUnionedSubqueriesInUsedViews(rewriting);
        }

        private void GatherUnionedSubqueriesInUsedViews(T_Tile query)
        {
            if (query != null)
            {
                if (this.m_qp.GetOpKind(query) != TileOpKind.Union)
                {
                    this.m_usedViews[query] = TileOpKind.Union;
                }
                else
                {
                    this.GatherUnionedSubqueriesInUsedViews(this.m_qp.GetArg1(query));
                    this.GatherUnionedSubqueriesInUsedViews(this.m_qp.GetArg2(query));
                }
            }
        }

        private T_Tile GetRewritingHalf(T_Tile halfRewriting, T_Tile remainingView, TileOpKind viewKind)
        {
            switch (viewKind)
            {
                case TileOpKind.Union:
                    halfRewriting = this.m_qp.Union(halfRewriting, remainingView);
                    return halfRewriting;

                case TileOpKind.Join:
                    halfRewriting = this.m_qp.Join(halfRewriting, remainingView);
                    return halfRewriting;

                case TileOpKind.AntiSemiJoin:
                    halfRewriting = this.m_qp.AntiSemiJoin(halfRewriting, remainingView);
                    return halfRewriting;
            }
            return halfRewriting;
        }

        private bool SimplifyRewriting(out T_Tile simplifiedRewriting)
        {
            T_Tile local;
            bool flag = false;
            simplifiedRewriting = default(T_Tile);
            while (this.SimplifyRewritingOnce(out local))
            {
                flag = true;
                simplifiedRewriting = local;
            }
            return flag;
        }

        private bool SimplifyRewritingOnce(out T_Tile simplifiedRewriting)
        {
            HashSet<T_Tile> remainingViews = new HashSet<T_Tile>(this.m_usedViews.Keys);
            foreach (T_Tile local in this.m_usedViews.Keys)
            {
                switch (this.m_usedViews[local])
                {
                    case TileOpKind.Union:
                    case TileOpKind.Join:
                        remainingViews.Remove(local);
                        if (!this.SimplifyRewritingOnce(local, remainingViews, out simplifiedRewriting))
                        {
                            break;
                        }
                        return true;

                    default:
                    {
                        continue;
                    }
                }
                remainingViews.Add(local);
            }
            simplifiedRewriting = default(T_Tile);
            return false;
        }

        private bool SimplifyRewritingOnce(T_Tile newRewriting, HashSet<T_Tile> remainingViews, out T_Tile simplifiedRewriting)
        {
            simplifiedRewriting = default(T_Tile);
            if (remainingViews.Count == 0)
            {
                return false;
            }
            if (remainingViews.Count == 1)
            {
                T_Tile key = remainingViews.First<T_Tile>();
                bool flag = false;
                TileOpKind kind2 = this.m_usedViews[key];
                if (kind2 == TileOpKind.Union)
                {
                    flag = this.m_qp.IsContainedIn(this.m_originalRewriting, newRewriting);
                }
                else
                {
                    flag = this.m_qp.IsContainedIn(this.m_originalRewriting, newRewriting) && this.m_qp.IsDisjointFrom(this.m_toAvoid, newRewriting);
                }
                if (flag)
                {
                    simplifiedRewriting = newRewriting;
                    this.m_usedViews.Remove(key);
                    return true;
                }
                return false;
            }
            int num = remainingViews.Count / 2;
            int num2 = 0;
            T_Tile halfRewriting = newRewriting;
            T_Tile local3 = newRewriting;
            HashSet<T_Tile> set = new HashSet<T_Tile>();
            HashSet<T_Tile> set2 = new HashSet<T_Tile>();
            foreach (T_Tile local4 in remainingViews)
            {
                TileOpKind viewKind = this.m_usedViews[local4];
                if (num2++ < num)
                {
                    set.Add(local4);
                    halfRewriting = this.GetRewritingHalf(halfRewriting, local4, viewKind);
                }
                else
                {
                    set2.Add(local4);
                    local3 = this.GetRewritingHalf(local3, local4, viewKind);
                }
            }
            if (!this.SimplifyRewritingOnce(halfRewriting, set2, out simplifiedRewriting))
            {
                return this.SimplifyRewritingOnce(local3, set, out simplifiedRewriting);
            }
            return true;
        }

        internal static bool TrySimplifyJoinRewriting(ref T_Tile rewriting, T_Tile toAvoid, Dictionary<T_Tile, TileOpKind> usedViews, RewritingProcessor<T_Tile> qp)
        {
            T_Tile local;
            RewritingSimplifier<T_Tile> simplifier = new RewritingSimplifier<T_Tile>(rewriting, toAvoid, usedViews, qp);
            if (simplifier.SimplifyRewriting(out local))
            {
                rewriting = local;
                return true;
            }
            return false;
        }

        internal static bool TrySimplifyUnionRewriting(ref T_Tile rewriting, T_Tile toFill, T_Tile toAvoid, RewritingProcessor<T_Tile> qp)
        {
            T_Tile local;
            RewritingSimplifier<T_Tile> simplifier = new RewritingSimplifier<T_Tile>(rewriting, toFill, toAvoid, qp);
            if (simplifier.SimplifyRewriting(out local))
            {
                rewriting = local;
                return true;
            }
            return false;
        }
    }
}

