namespace System.ServiceModel
{
    using System;

    internal static class DeadLetterQueueHelper
    {
        public static bool IsDefined(DeadLetterQueue mode) => 
            ((mode >= DeadLetterQueue.None) && (mode <= DeadLetterQueue.Custom));
    }
}

