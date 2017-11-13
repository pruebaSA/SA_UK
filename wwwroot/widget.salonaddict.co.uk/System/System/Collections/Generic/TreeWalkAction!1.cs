namespace System.Collections.Generic
{
    using System;
    using System.Runtime.CompilerServices;

    internal delegate bool TreeWalkAction<T>(TreeSet<T>.Node node);
}

