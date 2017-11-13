namespace System.ComponentModel.Design
{
    using System;

    internal class DesignerActionVerbItem : DesignerActionMethodItem
    {
        private DesignerVerb _targetVerb;

        public DesignerActionVerbItem(DesignerVerb verb)
        {
            if (verb == null)
            {
                throw new ArgumentNullException();
            }
            this._targetVerb = verb;
        }

        public override void Invoke()
        {
            this._targetVerb.Invoke();
        }

        public override string Category =>
            "Verbs";

        public override string Description =>
            this._targetVerb.Description;

        public override string DisplayName =>
            this._targetVerb.Text;

        public override bool IncludeAsDesignerVerb =>
            false;

        public override string MemberName =>
            null;
    }
}

