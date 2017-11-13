namespace System.Windows
{
    using MS.Utility;
    using System;

    internal class DependentList : FrugalObjectList<Dependent>
    {
        public void Add(DependencyObject d, DependencyProperty dp, Expression expr)
        {
            if (base.Count == base.Capacity)
            {
                this.CleanUpDeadWeakReferences();
            }
            Dependent dependent = new Dependent(d, dp, expr);
            base.Add(dependent);
        }

        private void CleanUpDeadWeakReferences()
        {
            int newCount = 0;
            for (int i = base.Count - 1; i >= 0; i--)
            {
                Dependent dependent = base[i];
                if (dependent.IsValid())
                {
                    newCount++;
                }
            }
            if (newCount != base.Count)
            {
                FrugalObjectList<Dependent>.Compacter compacter = new FrugalObjectList<Dependent>.Compacter(this, newCount);
                int start = 0;
                bool flag = false;
                int end = 0;
                int count = base.Count;
                while (end < count)
                {
                    Dependent dependent2 = base[end];
                    if (flag != dependent2.IsValid())
                    {
                        if (flag)
                        {
                            compacter.Include(start, end);
                        }
                        start = end;
                        flag = !flag;
                    }
                    end++;
                }
                if (flag)
                {
                    compacter.Include(start, base.Count);
                }
                compacter.Finish();
            }
        }

        public void InvalidateDependents(DependencyObject source, DependencyPropertyChangedEventArgs sourceArgs)
        {
            Dependent[] dependentArray = base.ToArray();
            for (int i = 0; i < dependentArray.Length; i++)
            {
                Expression expr = dependentArray[i].Expr;
                if (expr != null)
                {
                    expr.OnPropertyInvalidation(source, sourceArgs);
                    if (!expr.ForwardsInvalidations)
                    {
                        DependencyObject dO = dependentArray[i].DO;
                        DependencyProperty dP = dependentArray[i].DP;
                        if ((dO != null) && (dP != null))
                        {
                            dO.InvalidateProperty(dP);
                        }
                    }
                }
            }
        }

        public void Remove(DependencyObject d, DependencyProperty dp, Expression expr)
        {
            Dependent dependent = new Dependent(d, dp, expr);
            base.Remove(dependent);
        }

        public bool IsEmpty
        {
            get
            {
                for (int i = base.Count - 1; i >= 0; i--)
                {
                    Dependent dependent = base[i];
                    if (dependent.IsValid())
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}

