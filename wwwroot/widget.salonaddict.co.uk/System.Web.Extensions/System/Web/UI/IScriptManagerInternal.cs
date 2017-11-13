namespace System.Web.UI
{
    using System;

    internal interface IScriptManagerInternal
    {
        void RegisterAsyncPostBackControl(Control control);
        void RegisterExtenderControl<TExtenderControl>(TExtenderControl extenderControl, Control targetControl) where TExtenderControl: Control, IExtenderControl;
        void RegisterPostBackControl(Control control);
        void RegisterProxy(ScriptManagerProxy proxy);
        void RegisterScriptControl<TScriptControl>(TScriptControl scriptControl) where TScriptControl: Control, IScriptControl;
        void RegisterScriptDescriptors(IExtenderControl extenderControl);
        void RegisterScriptDescriptors(IScriptControl scriptControl);
        void RegisterUpdatePanel(UpdatePanel updatePanel);
        void UnregisterUpdatePanel(UpdatePanel updatePanel);

        string AsyncPostBackSourceElementID { get; }

        bool IsInAsyncPostBack { get; }

        bool SupportsPartialRendering { get; }
    }
}

