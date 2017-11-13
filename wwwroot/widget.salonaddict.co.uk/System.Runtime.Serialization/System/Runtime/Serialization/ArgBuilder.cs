namespace System.Runtime.Serialization
{
    using System;

    internal class ArgBuilder
    {
        internal Type ArgType;
        internal int Index;

        internal ArgBuilder(int index, Type argType)
        {
            this.Index = index;
            this.ArgType = argType;
        }
    }
}

