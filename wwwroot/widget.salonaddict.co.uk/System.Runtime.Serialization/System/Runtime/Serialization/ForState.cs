namespace System.Runtime.Serialization
{
    using System;
    using System.Reflection.Emit;

    internal class ForState
    {
        private Label beginLabel;
        private object end;
        private Label endLabel;
        private LocalBuilder indexVar;
        private bool requiresEndLabel;
        private Label testLabel;

        internal ForState(LocalBuilder indexVar, Label beginLabel, Label testLabel, object end)
        {
            this.indexVar = indexVar;
            this.beginLabel = beginLabel;
            this.testLabel = testLabel;
            this.end = end;
        }

        internal Label BeginLabel =>
            this.beginLabel;

        internal object End =>
            this.end;

        internal Label EndLabel
        {
            get => 
                this.endLabel;
            set
            {
                this.endLabel = value;
            }
        }

        internal LocalBuilder Index =>
            this.indexVar;

        internal bool RequiresEndLabel
        {
            get => 
                this.requiresEndLabel;
            set
            {
                this.requiresEndLabel = value;
            }
        }

        internal Label TestLabel =>
            this.testLabel;
    }
}

