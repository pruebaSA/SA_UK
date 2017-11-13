namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel.Design;
    using System.Design;

    internal class DetailsViewActionList : DesignerActionList
    {
        private bool _allowDeleting;
        private bool _allowEditing;
        private bool _allowInserting;
        private bool _allowMoveDown;
        private bool _allowMoveUp;
        private bool _allowPaging;
        private bool _allowRemoveField;
        private DetailsViewDesigner _detailsViewDesigner;

        public DetailsViewActionList(DetailsViewDesigner detailsViewDesigner) : base(detailsViewDesigner.Component)
        {
            this._detailsViewDesigner = detailsViewDesigner;
        }

        public void AddNewField()
        {
            this._detailsViewDesigner.AddNewField();
        }

        public void EditFields()
        {
            this._detailsViewDesigner.EditFields();
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionMethodItem(this, "EditFields", System.Design.SR.GetString("DetailsView_EditFieldsVerb"), "Action", System.Design.SR.GetString("DetailsView_EditFieldsDesc")));
            items.Add(new DesignerActionMethodItem(this, "AddNewField", System.Design.SR.GetString("DetailsView_AddNewFieldVerb"), "Action", System.Design.SR.GetString("DetailsView_AddNewFieldDesc")));
            if (this.AllowMoveUp)
            {
                items.Add(new DesignerActionMethodItem(this, "MoveFieldUp", System.Design.SR.GetString("DetailsView_MoveFieldUpVerb"), "Action", System.Design.SR.GetString("DetailsView_MoveFieldUpDesc")));
            }
            if (this.AllowMoveDown)
            {
                items.Add(new DesignerActionMethodItem(this, "MoveFieldDown", System.Design.SR.GetString("DetailsView_MoveFieldDownVerb"), "Action", System.Design.SR.GetString("DetailsView_MoveFieldDownDesc")));
            }
            if (this.AllowRemoveField)
            {
                items.Add(new DesignerActionMethodItem(this, "RemoveField", System.Design.SR.GetString("DetailsView_RemoveFieldVerb"), "Action", System.Design.SR.GetString("DetailsView_RemoveFieldDesc")));
            }
            if (this.AllowPaging)
            {
                items.Add(new DesignerActionPropertyItem("EnablePaging", System.Design.SR.GetString("DetailsView_EnablePaging"), "Behavior", System.Design.SR.GetString("DetailsView_EnablePagingDesc")));
            }
            if (this.AllowInserting)
            {
                items.Add(new DesignerActionPropertyItem("EnableInserting", System.Design.SR.GetString("DetailsView_EnableInserting"), "Behavior", System.Design.SR.GetString("DetailsView_EnableInsertingDesc")));
            }
            if (this.AllowEditing)
            {
                items.Add(new DesignerActionPropertyItem("EnableEditing", System.Design.SR.GetString("DetailsView_EnableEditing"), "Behavior", System.Design.SR.GetString("DetailsView_EnableEditingDesc")));
            }
            if (this.AllowDeleting)
            {
                items.Add(new DesignerActionPropertyItem("EnableDeleting", System.Design.SR.GetString("DetailsView_EnableDeleting"), "Behavior", System.Design.SR.GetString("DetailsView_EnableDeletingDesc")));
            }
            return items;
        }

        public void MoveFieldDown()
        {
            this._detailsViewDesigner.MoveDown();
        }

        public void MoveFieldUp()
        {
            this._detailsViewDesigner.MoveUp();
        }

        public void RemoveField()
        {
            this._detailsViewDesigner.RemoveField();
        }

        internal bool AllowDeleting
        {
            get => 
                this._allowDeleting;
            set
            {
                this._allowDeleting = value;
            }
        }

        internal bool AllowEditing
        {
            get => 
                this._allowEditing;
            set
            {
                this._allowEditing = value;
            }
        }

        internal bool AllowInserting
        {
            get => 
                this._allowInserting;
            set
            {
                this._allowInserting = value;
            }
        }

        internal bool AllowMoveDown
        {
            get => 
                this._allowMoveDown;
            set
            {
                this._allowMoveDown = value;
            }
        }

        internal bool AllowMoveUp
        {
            get => 
                this._allowMoveUp;
            set
            {
                this._allowMoveUp = value;
            }
        }

        internal bool AllowPaging
        {
            get => 
                this._allowPaging;
            set
            {
                this._allowPaging = value;
            }
        }

        internal bool AllowRemoveField
        {
            get => 
                this._allowRemoveField;
            set
            {
                this._allowRemoveField = value;
            }
        }

        public override bool AutoShow
        {
            get => 
                true;
            set
            {
            }
        }

        public bool EnableDeleting
        {
            get => 
                this._detailsViewDesigner.EnableDeleting;
            set
            {
                this._detailsViewDesigner.EnableDeleting = value;
            }
        }

        public bool EnableEditing
        {
            get => 
                this._detailsViewDesigner.EnableEditing;
            set
            {
                this._detailsViewDesigner.EnableEditing = value;
            }
        }

        public bool EnableInserting
        {
            get => 
                this._detailsViewDesigner.EnableInserting;
            set
            {
                this._detailsViewDesigner.EnableInserting = value;
            }
        }

        public bool EnablePaging
        {
            get => 
                this._detailsViewDesigner.EnablePaging;
            set
            {
                this._detailsViewDesigner.EnablePaging = value;
            }
        }
    }
}

