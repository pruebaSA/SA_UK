namespace System.Net
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal delegate void CompletionDelegate(byte[] responseBytes, Exception exception, AsyncOperation asyncOp);
}

