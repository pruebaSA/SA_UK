namespace System.Security
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Threading;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SecurityContextSwitcher : IDisposable
    {
        internal SecurityContext prevSC;
        internal SecurityContext currSC;
        internal ExecutionContext currEC;
        internal CompressedStackSwitcher cssw;
        internal WindowsImpersonationContext wic;
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is SecurityContextSwitcher))
            {
                return false;
            }
            SecurityContextSwitcher switcher = (SecurityContextSwitcher) obj;
            return ((((this.prevSC == switcher.prevSC) && (this.currSC == switcher.currSC)) && ((this.currEC == switcher.currEC) && (this.cssw == switcher.cssw))) && (this.wic == switcher.wic));
        }

        public override int GetHashCode() => 
            this.ToString().GetHashCode();

        public static bool operator ==(SecurityContextSwitcher c1, SecurityContextSwitcher c2) => 
            c1.Equals(c2);

        public static bool operator !=(SecurityContextSwitcher c1, SecurityContextSwitcher c2) => 
            !c1.Equals(c2);

        void IDisposable.Dispose()
        {
            this.Undo();
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal bool UndoNoThrow()
        {
            try
            {
                this.Undo();
            }
            catch
            {
                return false;
            }
            return true;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public void Undo()
        {
            if (this.currEC != null)
            {
                if (this.currEC != Thread.CurrentThread.GetExecutionContextNoCreate())
                {
                    Environment.FailFast(Environment.GetResourceString("InvalidOperation_SwitcherCtxMismatch"));
                }
                if (this.currSC != this.currEC.SecurityContext)
                {
                    Environment.FailFast(Environment.GetResourceString("InvalidOperation_SwitcherCtxMismatch"));
                }
                this.currEC.SecurityContext = this.prevSC;
                this.currEC = null;
                bool flag = true;
                try
                {
                    if (this.wic != null)
                    {
                        flag &= this.wic.UndoNoThrow();
                    }
                }
                catch
                {
                    flag &= this.cssw.UndoNoThrow();
                    Environment.FailFast(Environment.GetResourceString("ExecutionContext_UndoFailed"));
                }
                if (!(flag & this.cssw.UndoNoThrow()))
                {
                    Environment.FailFast(Environment.GetResourceString("ExecutionContext_UndoFailed"));
                }
            }
        }
    }
}

