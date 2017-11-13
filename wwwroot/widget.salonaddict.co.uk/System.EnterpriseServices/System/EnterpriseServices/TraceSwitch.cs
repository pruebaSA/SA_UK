﻿namespace System.EnterpriseServices
{
    using System;

    internal class TraceSwitch : BaseSwitch
    {
        internal TraceSwitch(string name) : base(name)
        {
        }

        internal int Level =>
            base.Value;
    }
}

