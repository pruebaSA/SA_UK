namespace AjaxControlToolkit.MaskedEditValidatorCompatibility
{
    using System;

    internal interface IBaseCompareValidatorAccessor : IBaseValidatorAccessor, IWebControlAccessor
    {
        string GetDateElementOrder();

        int CutoffYear { get; }
    }
}

