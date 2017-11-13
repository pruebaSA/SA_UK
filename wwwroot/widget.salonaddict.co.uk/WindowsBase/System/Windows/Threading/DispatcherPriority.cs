namespace System.Windows.Threading
{
    using System;

    public enum DispatcherPriority
    {
        ApplicationIdle = 2,
        Background = 4,
        ContextIdle = 3,
        DataBind = 8,
        Inactive = 0,
        Input = 5,
        Invalid = -1,
        Loaded = 6,
        Normal = 9,
        Render = 7,
        Send = 10,
        SystemIdle = 1
    }
}

