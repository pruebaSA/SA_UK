namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Reflection.Emit;

    internal class IfState
    {
        private Label elseBegin;
        private Label endIf;

        internal Label ElseBegin
        {
            get => 
                this.elseBegin;
            set
            {
                this.elseBegin = value;
            }
        }

        internal Label EndIf
        {
            get => 
                this.endIf;
            set
            {
                this.endIf = value;
            }
        }
    }
}

