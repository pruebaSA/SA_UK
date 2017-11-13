namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel.Design;

    public class HtmlEditorExtenderButtonCollectionEditor : CollectionEditor
    {
        public HtmlEditorExtenderButtonCollectionEditor(Type type) : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances() => 
            false;

        protected override Type[] CreateNewItemTypes() => 
            new Type[] { 
                typeof(Undo), typeof(Redo), typeof(Bold), typeof(Italic), typeof(Underline), typeof(StrikeThrough), typeof(Subscript), typeof(Superscript), typeof(JustifyLeft), typeof(JustifyCenter), typeof(JustifyRight), typeof(JustifyFull), typeof(InsertOrderedList), typeof(InsertUnorderedList), typeof(RemoveFormat), typeof(SelectAll),
                typeof(UnSelect), typeof(Delete), typeof(Cut), typeof(Copy), typeof(Paste), typeof(BackgroundColorSelector), typeof(ForeColorSelector), typeof(FontNameSelector), typeof(FontSizeSelector), typeof(Indent), typeof(Outdent), typeof(InsertHorizontalRule), typeof(HorizontalSeparator), typeof(InsertImage)
            };
    }
}

