namespace System.Windows.Interop
{
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Input;

    public interface IKeyboardInputSink
    {
        bool HasFocusWithin();
        [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        bool OnMnemonic(ref MSG msg, ModifierKeys modifiers);
        [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        IKeyboardInputSite RegisterKeyboardInputSink(IKeyboardInputSink sink);
        bool TabInto(TraversalRequest request);
        [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        bool TranslateAccelerator(ref MSG msg, ModifierKeys modifiers);
        [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)]
        bool TranslateChar(ref MSG msg, ModifierKeys modifiers);

        IKeyboardInputSite KeyboardInputSite { get; [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] set; }
    }
}

