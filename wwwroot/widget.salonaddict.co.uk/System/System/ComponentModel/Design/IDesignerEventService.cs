namespace System.ComponentModel.Design
{
    using System;
    using System.Runtime.CompilerServices;

    public interface IDesignerEventService
    {
        event ActiveDesignerEventHandler ActiveDesignerChanged;

        event DesignerEventHandler DesignerCreated;

        event DesignerEventHandler DesignerDisposed;

        event EventHandler SelectionChanged;

        IDesignerHost ActiveDesigner { get; }

        DesignerCollection Designers { get; }
    }
}

