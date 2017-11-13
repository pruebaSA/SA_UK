namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class RewritingPass<T_Tile> where T_Tile: class
    {
        private readonly RewritingProcessor<T_Tile> m_qp;
        private readonly T_Tile m_toAvoid;
        private readonly T_Tile m_toFill;
        private readonly Dictionary<T_Tile, TileOpKind> m_usedViews;
        private readonly List<T_Tile> m_views;

        public RewritingPass(T_Tile toFill, T_Tile toAvoid, List<T_Tile> views, RewritingProcessor<T_Tile> qp)
        {
            this.m_usedViews = new Dictionary<T_Tile, TileOpKind>();
            this.m_toFill = toFill;
            this.m_toAvoid = toAvoid;
            this.m_views = views;
            this.m_qp = qp;
        }

        private bool FindContributingView(out T_Tile rewriting)
        {
            foreach (T_Tile local in this.AvailableViews)
            {
                if (!this.m_qp.IsDisjointFrom(local, this.m_toFill))
                {
                    rewriting = local;
                    this.m_usedViews[local] = TileOpKind.Join;
                    return true;
                }
            }
            rewriting = default(T_Tile);
            return false;
        }

        private bool FindRewritingByIncludedAndDisjoint(out T_Tile rewritingSoFar)
        {
            rewritingSoFar = default(T_Tile);
            foreach (T_Tile local in this.AvailableViews)
            {
                if (this.m_qp.IsContainedIn(this.m_toFill, local))
                {
                    if (((T_Tile) rewritingSoFar) == null)
                    {
                        rewritingSoFar = local;
                        this.m_usedViews[local] = TileOpKind.Join;
                    }
                    else
                    {
                        T_Tile b = this.m_qp.Join(rewritingSoFar, local);
                        if (this.m_qp.IsContainedIn(rewritingSoFar, b))
                        {
                            continue;
                        }
                        rewritingSoFar = b;
                        this.m_usedViews[local] = TileOpKind.Join;
                    }
                    if (this.m_qp.IsContainedIn(rewritingSoFar, this.m_toFill))
                    {
                        return true;
                    }
                }
            }
            if (((T_Tile) rewritingSoFar) != null)
            {
                foreach (T_Tile local3 in this.AvailableViews)
                {
                    if (this.m_qp.IsDisjointFrom(this.m_toFill, local3) && !this.m_qp.IsDisjointFrom(rewritingSoFar, local3))
                    {
                        rewritingSoFar = this.m_qp.AntiSemiJoin(rewritingSoFar, local3);
                        this.m_usedViews[local3] = TileOpKind.AntiSemiJoin;
                        if (this.m_qp.IsContainedIn(rewritingSoFar, this.m_toFill))
                        {
                            return true;
                        }
                    }
                }
            }
            return (((T_Tile) rewritingSoFar) != null);
        }

        private bool RewriteQuery(out T_Tile rewriting)
        {
            T_Tile local;
            rewriting = this.m_toFill;
            if (!this.FindRewritingByIncludedAndDisjoint(out local) && !this.FindContributingView(out local))
            {
                return false;
            }
            bool flag = !this.m_qp.IsDisjointFrom(local, this.m_toAvoid);
            if (flag)
            {
                foreach (T_Tile local2 in this.AvailableViews)
                {
                    if (this.TryJoin(local2, ref local))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag)
            {
                foreach (T_Tile local3 in this.AvailableViews)
                {
                    if (this.TryAntiSemiJoin(local3, ref local))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag)
            {
                return false;
            }
            RewritingSimplifier<T_Tile>.TrySimplifyJoinRewriting(ref local, this.m_toAvoid, this.m_usedViews, this.m_qp);
            T_Tile tile = this.m_qp.AntiSemiJoin(this.m_toFill, local);
            if (!this.m_qp.IsEmpty(tile))
            {
                T_Tile local5;
                if (!RewritingPass<T_Tile>.RewriteQueryInternal(tile, this.m_toAvoid, out local5, this.m_views, new HashSet<T_Tile>(this.m_usedViews.Keys), this.m_qp))
                {
                    rewriting = local5;
                    return false;
                }
                if (this.m_qp.IsContainedIn(local, local5))
                {
                    local = local5;
                }
                else
                {
                    local = this.m_qp.Union(local, local5);
                }
            }
            rewriting = local;
            return true;
        }

        public static bool RewriteQuery(T_Tile toFill, T_Tile toAvoid, out T_Tile rewriting, List<T_Tile> views, RewritingProcessor<T_Tile> qp)
        {
            RewritingPass<T_Tile> pass = new RewritingPass<T_Tile>(toFill, toAvoid, views, qp);
            if (pass.RewriteQuery(out rewriting))
            {
                RewritingSimplifier<T_Tile>.TrySimplifyUnionRewriting(ref rewriting, toFill, toAvoid, qp);
                return true;
            }
            return false;
        }

        private static bool RewriteQueryInternal(T_Tile toFill, T_Tile toAvoid, out T_Tile rewriting, List<T_Tile> views, HashSet<T_Tile> recentlyUsedViews, RewritingProcessor<T_Tile> qp)
        {
            if (qp.REORDER_VIEWS && (recentlyUsedViews.Count > 0))
            {
                List<T_Tile> list = new List<T_Tile>();
                foreach (T_Tile local in views)
                {
                    if (!recentlyUsedViews.Contains(local))
                    {
                        list.Add(local);
                    }
                }
                list.AddRange(recentlyUsedViews);
                views = list;
            }
            RewritingPass<T_Tile> pass = new RewritingPass<T_Tile>(toFill, toAvoid, views, qp);
            return pass.RewriteQuery(out rewriting);
        }

        private bool TryAntiSemiJoin(T_Tile view, ref T_Tile rewriting)
        {
            T_Tile tile = this.m_qp.AntiSemiJoin(rewriting, view);
            if (!this.m_qp.IsEmpty(tile))
            {
                this.m_usedViews[view] = TileOpKind.AntiSemiJoin;
                rewriting = tile;
                return this.m_qp.IsDisjointFrom(rewriting, this.m_toAvoid);
            }
            return false;
        }

        private bool TryJoin(T_Tile view, ref T_Tile rewriting)
        {
            T_Tile tile = this.m_qp.Join(rewriting, view);
            if (!this.m_qp.IsEmpty(tile))
            {
                this.m_usedViews[view] = TileOpKind.Join;
                rewriting = tile;
                return this.m_qp.IsDisjointFrom(rewriting, this.m_toAvoid);
            }
            return false;
        }

        private IEnumerable<T_Tile> AvailableViews =>
            (from view in this.m_views
                where !base.m_usedViews.ContainsKey(view)
                select view);
    }
}

