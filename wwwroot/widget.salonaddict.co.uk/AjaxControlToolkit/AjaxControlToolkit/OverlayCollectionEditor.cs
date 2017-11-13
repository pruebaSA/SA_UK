namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel.Design;

    public class OverlayCollectionEditor : CollectionEditor
    {
        public OverlayCollectionEditor(Type type) : base(type)
        {
        }

        protected override bool CanSelectMultipleInstances() => 
            false;

        protected override Type[] CreateNewItemTypes() => 
            new Type[] { typeof(SeadragonFixedOverlay), typeof(SeadragonScalableOverlay) };
    }
}

