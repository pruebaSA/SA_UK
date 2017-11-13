namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Design;
    using System;
    using System.Runtime.CompilerServices;

    public class AutoCompleteDesigner : ExtenderControlBaseDesigner<AutoCompleteExtender>
    {
        [PageMethodSignature("AutoComplete", "ServicePath", "ServiceMethod", "UseContextKey")]
        private delegate string[] GetCompletionList(string prefixText, int count, string contextKey);
    }
}

