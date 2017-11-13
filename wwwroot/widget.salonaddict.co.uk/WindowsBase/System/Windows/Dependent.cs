namespace System.Windows
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Dependent
    {
        private DependencyProperty _DP;
        private WeakReference _wrDO;
        private WeakReference _wrEX;
        public bool IsValid()
        {
            if (!this._wrEX.IsAlive)
            {
                return false;
            }
            if ((this._wrDO != null) && !this._wrDO.IsAlive)
            {
                return false;
            }
            return true;
        }

        public Dependent(DependencyObject o, DependencyProperty p, Expression e)
        {
            this._wrEX = (e == null) ? null : new WeakReference(e);
            this._DP = p;
            this._wrDO = (o == null) ? null : new WeakReference(o);
        }

        public DependencyObject DO
        {
            get
            {
                if (this._wrDO == null)
                {
                    return null;
                }
                return (DependencyObject) this._wrDO.Target;
            }
        }
        public DependencyProperty DP =>
            this._DP;
        public Expression Expr
        {
            get
            {
                if (this._wrEX == null)
                {
                    return null;
                }
                return (Expression) this._wrEX.Target;
            }
        }
        public override bool Equals(object o)
        {
            if (!(o is Dependent))
            {
                return false;
            }
            Dependent dependent = (Dependent) o;
            if (!this.IsValid() || !dependent.IsValid())
            {
                return false;
            }
            if (this._wrEX.Target != dependent._wrEX.Target)
            {
                return false;
            }
            if (this._DP != dependent._DP)
            {
                return false;
            }
            if ((this._wrDO != null) && (dependent._wrDO != null))
            {
                if (this._wrDO.Target != dependent._wrDO.Target)
                {
                    return false;
                }
            }
            else if ((this._wrDO != null) || (dependent._wrDO != null))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(Dependent first, Dependent second) => 
            first.Equals(second);

        public static bool operator !=(Dependent first, Dependent second) => 
            !first.Equals(second);

        public override int GetHashCode()
        {
            Expression target = (Expression) this._wrEX.Target;
            int num = (target == null) ? 0 : target.GetHashCode();
            if (this._wrDO != null)
            {
                DependencyObject obj2 = (DependencyObject) this._wrDO.Target;
                num += (obj2 == null) ? 0 : obj2.GetHashCode();
            }
            return (num + ((this._DP == null) ? 0 : this._DP.GetHashCode()));
        }
    }
}

