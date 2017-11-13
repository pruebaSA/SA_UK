namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel.Design;
    using System.Design;

    internal class FormViewActionList : DesignerActionList
    {
        private bool _allowDynamicData;
        private bool _allowPaging;
        private FormViewDesigner _formViewDesigner;

        public FormViewActionList(FormViewDesigner formViewDesigner) : base(formViewDesigner.Component)
        {
            this._formViewDesigner = formViewDesigner;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            if (this.AllowDynamicData)
            {
                items.Add(new DesignerActionPropertyItem("EnableDynamicData", System.Design.SR.GetString("FormView_EnableDynamicData"), "Behavior", System.Design.SR.GetString("FormView_EnableDynamicDataDesc")));
            }
            if (this.AllowPaging)
            {
                items.Add(new DesignerActionPropertyItem("EnablePaging", System.Design.SR.GetString("FormView_EnablePaging"), "Behavior", System.Design.SR.GetString("FormView_EnablePagingDesc")));
            }
            return items;
        }

        internal bool AllowDynamicData
        {
            get => 
                this._allowDynamicData;
            set
            {
                this._allowDynamicData = value;
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

        public override bool AutoShow
        {
            get => 
                true;
            set
            {
            }
        }

        public bool EnableDynamicData
        {
            get => 
                this._formViewDesigner.EnableDynamicData;
            set
            {
                this._formViewDesigner.EnableDynamicData = value;
            }
        }

        public bool EnablePaging
        {
            get => 
                this._formViewDesigner.EnablePaging;
            set
            {
                this._formViewDesigner.EnablePaging = value;
            }
        }
    }
}

