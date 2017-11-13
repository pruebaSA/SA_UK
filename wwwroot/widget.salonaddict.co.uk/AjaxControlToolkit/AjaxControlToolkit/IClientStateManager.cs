namespace AjaxControlToolkit
{
    using System;

    public interface IClientStateManager
    {
        void LoadClientState(string clientState);
        string SaveClientState();

        bool SupportsClientState { get; }
    }
}

