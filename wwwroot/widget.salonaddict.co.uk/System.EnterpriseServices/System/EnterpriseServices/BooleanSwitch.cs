namespace System.EnterpriseServices
{
    using System;

    internal class BooleanSwitch : BaseSwitch
    {
        internal BooleanSwitch(string name) : base(name)
        {
        }

        internal bool Enabled =>
            (base.Value != 0);
    }
}

