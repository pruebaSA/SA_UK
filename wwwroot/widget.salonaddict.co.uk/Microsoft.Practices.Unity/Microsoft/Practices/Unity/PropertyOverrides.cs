namespace Microsoft.Practices.Unity
{
    using System;

    public class PropertyOverrides : OverrideCollection<PropertyOverride, string, object>
    {
        protected override PropertyOverride MakeOverride(string key, object value) => 
            new PropertyOverride(key, value);
    }
}

