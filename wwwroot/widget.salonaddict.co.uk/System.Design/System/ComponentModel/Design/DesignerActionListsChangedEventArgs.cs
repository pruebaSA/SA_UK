namespace System.ComponentModel.Design
{
    using System;

    public class DesignerActionListsChangedEventArgs : EventArgs
    {
        private DesignerActionListCollection actionLists;
        private DesignerActionListsChangedType changeType;
        private object relatedObject;

        public DesignerActionListsChangedEventArgs(object relatedObject, DesignerActionListsChangedType changeType, DesignerActionListCollection actionLists)
        {
            this.relatedObject = relatedObject;
            this.changeType = changeType;
            this.actionLists = actionLists;
        }

        public DesignerActionListCollection ActionLists =>
            this.actionLists;

        public DesignerActionListsChangedType ChangeType =>
            this.changeType;

        public object RelatedObject =>
            this.relatedObject;
    }
}

