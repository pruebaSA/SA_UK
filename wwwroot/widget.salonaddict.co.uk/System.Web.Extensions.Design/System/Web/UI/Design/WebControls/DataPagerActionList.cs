namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.Resources.Design;

    internal class DataPagerActionList : DesignerActionList
    {
        private DataPagerDesigner _dataPagerDesigner;

        public DataPagerActionList(DataPagerDesigner dataPagerDesigner) : base(dataPagerDesigner.Component)
        {
            this._dataPagerDesigner = dataPagerDesigner;
        }

        public void EditFields()
        {
            this._dataPagerDesigner.EditFields();
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionPropertyItem("PagerStyle", AtlasWebDesign.DataPager_ChoosePagerStyleVerb, string.Empty, AtlasWebDesign.DataPager_ChoosePagerStyleDesc));
            items.Add(new DesignerActionMethodItem(this, "EditFields", AtlasWebDesign.DataPager_EditFieldsVerb, "Action", AtlasWebDesign.DataPager_EditFieldsDesc));
            return items;
        }

        public override bool AutoShow
        {
            get => 
                true;
            set
            {
            }
        }

        public bool IsCustomStyle =>
            (this._dataPagerDesigner.GetDataPagerStyle() is CustomPagerStyle);

        public bool IsNullStyle =>
            (this._dataPagerDesigner.GetDataPagerStyle() is NullPagerStyle);

        [TypeConverter(typeof(DataPagerStylesTypeConverter))]
        public DesignerPagerStyle PagerStyle
        {
            get => 
                this._dataPagerDesigner.GetDataPagerStyle();
            set
            {
                if (value != null)
                {
                    this._dataPagerDesigner.SetDataPagerStyle(value);
                }
            }
        }
    }
}

