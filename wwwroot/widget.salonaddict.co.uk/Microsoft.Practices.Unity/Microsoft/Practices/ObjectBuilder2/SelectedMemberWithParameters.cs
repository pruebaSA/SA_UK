namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections.Generic;

    public class SelectedMemberWithParameters
    {
        private List<string> parameterKeys = new List<string>();

        public void AddParameterKey(string newKey)
        {
            this.parameterKeys.Add(newKey);
        }

        public string[] GetParameterKeys() => 
            this.parameterKeys.ToArray();
    }
}

