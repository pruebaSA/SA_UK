namespace System.Windows.Interop
{
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Input;

    public interface IKeyboardInputSite
    {
        bool OnNoMoreTabStops(TraversalRequest request);
        [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        void Unregister();

        IKeyboardInputSink Sink { get; }
    }
}

