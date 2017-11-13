namespace System.Security.Policy
{
    using System;

    internal interface IDelayEvaluatedEvidence
    {
        void MarkUsed();

        bool IsVerified { get; }

        bool WasUsed { get; }
    }
}

