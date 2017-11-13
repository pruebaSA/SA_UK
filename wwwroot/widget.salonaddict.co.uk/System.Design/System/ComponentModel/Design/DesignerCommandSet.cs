namespace System.ComponentModel.Design
{
    using System;
    using System.Collections;

    public class DesignerCommandSet
    {
        public virtual ICollection GetCommands(string name) => 
            null;

        public DesignerActionListCollection ActionLists =>
            ((DesignerActionListCollection) this.GetCommands("ActionLists"));

        public DesignerVerbCollection Verbs =>
            ((DesignerVerbCollection) this.GetCommands("Verbs"));
    }
}

