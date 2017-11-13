namespace System.ServiceModel.Dispatcher
{
    using System;

    internal enum OpcodeFlags
    {
        Branch = 2,
        CompressableSelect = 0x400,
        Deleted = 0x40,
        FxMatch = 0x800,
        InConditional = 0x80,
        InitialSelect = 0x200,
        Jump = 8,
        Literal = 0x10,
        NoContextCopy = 0x100,
        None = 0,
        Result = 4,
        Select = 0x20,
        Single = 1
    }
}

