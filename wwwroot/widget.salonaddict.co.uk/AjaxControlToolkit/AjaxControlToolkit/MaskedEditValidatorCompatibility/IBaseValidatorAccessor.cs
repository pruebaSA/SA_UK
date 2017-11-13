namespace AjaxControlToolkit.MaskedEditValidatorCompatibility
{
    using System;

    internal interface IBaseValidatorAccessor : IWebControlAccessor
    {
        void EnsureID();
        string GetControlRenderID(string name);

        bool RenderUpLevel { get; }
    }
}

