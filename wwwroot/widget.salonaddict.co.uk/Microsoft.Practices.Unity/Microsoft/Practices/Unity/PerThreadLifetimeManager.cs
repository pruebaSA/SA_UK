namespace Microsoft.Practices.Unity
{
    using System;
    using System.Collections.Generic;

    public class PerThreadLifetimeManager : LifetimeManager
    {
        private readonly Guid key = Guid.NewGuid();
        [ThreadStatic]
        private static Dictionary<Guid, object> values;

        private static void EnsureValues()
        {
            if (values == null)
            {
                values = new Dictionary<Guid, object>();
            }
        }

        public override object GetValue()
        {
            object obj2;
            EnsureValues();
            values.TryGetValue(this.key, out obj2);
            return obj2;
        }

        public override void RemoveValue()
        {
        }

        public override void SetValue(object newValue)
        {
            EnsureValues();
            values[this.key] = newValue;
        }
    }
}

