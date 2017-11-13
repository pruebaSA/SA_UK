namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal interface IResumeMessageRpc
    {
        void Resume(out bool alreadyResumedNoLock);
        void Resume(IAsyncResult result);
        void SetCompletedSynchronously();

        ManualResetEvent AsyncOperationWaitEvent { get; }
    }
}

