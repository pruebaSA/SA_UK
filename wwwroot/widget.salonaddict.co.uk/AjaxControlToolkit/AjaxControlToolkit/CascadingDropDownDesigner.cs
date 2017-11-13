namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Design;
    using System;
    using System.Runtime.CompilerServices;

    public class CascadingDropDownDesigner : ExtenderControlBaseDesigner<CascadingDropDown>
    {
        [PageMethodSignature("CascadingDropDown", "ServicePath", "ServiceMethod", "UseContextKey")]
        private delegate CascadingDropDownNameValue[] GetDropDownContents(string knownCategoryValues, string category);
    }
}

