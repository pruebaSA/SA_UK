namespace System.Runtime.Serialization
{
    using System;
    using System.Reflection.Emit;

    internal class SwitchState
    {
        private bool defaultDefined;
        private Label defaultLabel;
        private Label endOfSwitchLabel;

        internal SwitchState(Label defaultLabel, Label endOfSwitchLabel)
        {
            this.defaultLabel = defaultLabel;
            this.endOfSwitchLabel = endOfSwitchLabel;
            this.defaultDefined = false;
        }

        internal bool DefaultDefined
        {
            get => 
                this.defaultDefined;
            set
            {
                this.defaultDefined = value;
            }
        }

        internal Label DefaultLabel =>
            this.defaultLabel;

        internal Label EndOfSwitchLabel =>
            this.endOfSwitchLabel;
    }
}

