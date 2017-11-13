namespace Microsoft.Practices.Unity
{
    using System;

    public class ParameterOverrides : OverrideCollection<ParameterOverride, string, object>
    {
        protected override ParameterOverride MakeOverride(string key, object value) => 
            new ParameterOverride(key, value);
    }
}

