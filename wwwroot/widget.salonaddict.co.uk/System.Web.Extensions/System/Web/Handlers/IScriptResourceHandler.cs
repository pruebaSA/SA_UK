namespace System.Web.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    internal interface IScriptResourceHandler
    {
        string GetEmptyPageUrl(string title);
        string GetScriptResourceUrl(List<Pair<Assembly, List<Pair<string, CultureInfo>>>> assemblyResourceLists, bool zip, bool notifyScriptLoaded);
        string GetScriptResourceUrl(Assembly assembly, string resourceName, CultureInfo culture, bool zip, bool notifyScriptLoaded);
    }
}

