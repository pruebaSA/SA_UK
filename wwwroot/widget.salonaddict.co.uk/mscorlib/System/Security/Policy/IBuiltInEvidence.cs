namespace System.Security.Policy
{
    using System;

    internal interface IBuiltInEvidence
    {
        int GetRequiredSize(bool verbose);
        int InitFromBuffer(char[] buffer, int position);
        int OutputToBuffer(char[] buffer, int position, bool verbose);
    }
}

