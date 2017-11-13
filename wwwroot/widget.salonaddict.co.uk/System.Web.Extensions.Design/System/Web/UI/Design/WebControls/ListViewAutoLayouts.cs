namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Design;
    using System.Web.UI.Design;

    internal static class ListViewAutoLayouts
    {
        public static Collection<ListViewAutoLayout> GetLayouts(IDesignerHost designerHost, IDataSourceViewSchema schema)
        {
            Collection<ListViewAutoLayout> collection = new Collection<ListViewAutoLayout>();
            IDataSourceFieldSchema[] fieldSchema = null;
            if (schema != null)
            {
                fieldSchema = schema.GetFields();
            }
            collection.Add(new ListViewGridLayout(designerHost, fieldSchema));
            collection.Add(new ListViewTiledLayout(designerHost, fieldSchema));
            collection.Add(new ListViewBulletedListLayout(designerHost, fieldSchema));
            collection.Add(new ListViewFlowLayout(designerHost, fieldSchema));
            collection.Add(new ListViewSingleRowLayout(designerHost, fieldSchema));
            return collection;
        }
    }
}

