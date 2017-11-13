namespace System.Runtime.InteropServices
{
    using System;
    using System.Runtime.CompilerServices;

    [ComVisible(true)]
    public sealed class ExtensibleClassFactory
    {
        private ExtensibleClassFactory()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void RegisterObjectCreationCallback(ObjectCreationDelegate callback);
    }
}

