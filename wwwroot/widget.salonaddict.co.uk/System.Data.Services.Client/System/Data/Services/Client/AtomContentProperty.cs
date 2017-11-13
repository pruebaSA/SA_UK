namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("AtomContentProperty {TypeName} {Name}")]
    internal class AtomContentProperty
    {
        public void EnsureProperties()
        {
            if (this.Properties == null)
            {
                this.Properties = new List<AtomContentProperty>();
            }
        }

        public AtomEntry Entry { get; set; }

        public AtomFeed Feed { get; set; }

        public bool IsNull { get; set; }

        public object MaterializedValue { get; set; }

        public string Name { get; set; }

        public List<AtomContentProperty> Properties { get; set; }

        public string Text { get; set; }

        public string TypeName { get; set; }
    }
}

