namespace System.Web.UI.Design
{
    using System;
    using System.ComponentModel.Design;
    using System.Windows.Forms;

    public class CollectionEditorBase : CollectionEditor
    {
        public CollectionEditorBase(System.Type type) : base(type)
        {
        }

        protected override CollectionEditor.CollectionForm CreateCollectionForm()
        {
            CollectionEditor.CollectionForm control = base.CreateCollectionForm();
            this.UpdatePropertyGridSettings(control);
            return control;
        }

        private bool UpdatePropertyGridSettings(Control control)
        {
            PropertyGrid grid = control as PropertyGrid;
            if (grid != null)
            {
                grid.HelpVisible = true;
                return true;
            }
            foreach (Control control2 in control.Controls)
            {
                if (this.UpdatePropertyGridSettings(control2))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

