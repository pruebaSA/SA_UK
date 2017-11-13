namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.Resources.Design;

    internal class ListViewActionList : DesignerActionList
    {
        private ListViewDesigner _listViewDesigner;

        public ListViewActionList(ListViewDesigner listViewDesigner) : base(listViewDesigner.Component)
        {
            this._listViewDesigner = listViewDesigner;
        }

        public void Configure()
        {
            this._listViewDesigner.Configure();
        }

        public string[] GetAllViewNames()
        {
            List<int> viewIndices = this._listViewDesigner.GetViewIndices();
            string[] templateNames = this._listViewDesigner.GetTemplateNames();
            string[] strArray2 = new string[viewIndices.Count];
            int index = 0;
            foreach (int num2 in viewIndices)
            {
                strArray2[index] = templateNames[num2];
                index++;
            }
            return strArray2;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            if (this.AllowConfiguration)
            {
                items.Add(new DesignerActionMethodItem(this, "Configure", AtlasWebDesign.ListView_ConfigureVerb, "Action", AtlasWebDesign.ListView_ConfigureDesc));
                if (this._listViewDesigner.EssentialTemplatesExist)
                {
                    items.Add(new DesignerActionPropertyItem("CurrentView", AtlasWebDesign.ListView_ChooseCurrentViewVerb, string.Empty, AtlasWebDesign.ListView_ChooseCurrentViewDesc));
                }
            }
            return items;
        }

        private int GetView(string viewName)
        {
            string[] templateNames = this._listViewDesigner.GetTemplateNames();
            for (int i = 0; i < templateNames.Length; i++)
            {
                if (string.Equals(templateNames[i], viewName))
                {
                    return i;
                }
            }
            throw new ArgumentOutOfRangeException("viewName");
        }

        public bool AllowConfiguration =>
            this._listViewDesigner.AllowConfiguration;

        public override bool AutoShow
        {
            get => 
                true;
            set
            {
            }
        }

        [TypeConverter(typeof(ListViewViewsTypeConverter))]
        public string CurrentView
        {
            get => 
                this._listViewDesigner.GetTemplateNames()[this._listViewDesigner.CurrentView];
            set
            {
                this._listViewDesigner.CurrentView = this.GetView(value);
            }
        }
    }
}

