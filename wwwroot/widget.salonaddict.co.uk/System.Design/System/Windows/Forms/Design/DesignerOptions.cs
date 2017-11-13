namespace System.Windows.Forms.Design
{
    using System;
    using System.ComponentModel;
    using System.Design;
    using System.Drawing;

    public class DesignerOptions
    {
        private bool enableComponentCache;
        private bool enableInSituEditing = true;
        private Size gridSize = new Size(8, 8);
        private const int maxGridSize = 200;
        private const int minGridSize = 2;
        private bool objectBoundSmartTagAutoShow = true;
        private bool showGrid = true;
        private bool snapToGrid = true;
        private bool useSmartTags;
        private bool useSnapLines;

        [SRDisplayName("DesignerOptions_EnableInSituEditingDisplay"), System.Design.SRDescription("DesignerOptions_EnableInSituEditingDesc"), Browsable(false), System.Design.SRCategory("DesignerOptions_EnableInSituEditingCat")]
        public virtual bool EnableInSituEditing
        {
            get => 
                this.enableInSituEditing;
            set
            {
                this.enableInSituEditing = value;
            }
        }

        [System.Design.SRDescription("DesignerOptions_GridSizeDesc"), System.Design.SRCategory("DesignerOptions_LayoutSettings")]
        public virtual Size GridSize
        {
            get => 
                this.gridSize;
            set
            {
                if (value.Width < 2)
                {
                    value.Width = 2;
                }
                if (value.Height < 2)
                {
                    value.Height = 2;
                }
                if (value.Width > 200)
                {
                    value.Width = 200;
                }
                if (value.Height > 200)
                {
                    value.Height = 200;
                }
                this.gridSize = value;
            }
        }

        [System.Design.SRDescription("DesignerOptions_ObjectBoundSmartTagAutoShow"), System.Design.SRCategory("DesignerOptions_ObjectBoundSmartTagSettings"), SRDisplayName("DesignerOptions_ObjectBoundSmartTagAutoShowDisplayName")]
        public virtual bool ObjectBoundSmartTagAutoShow
        {
            get => 
                this.objectBoundSmartTagAutoShow;
            set
            {
                this.objectBoundSmartTagAutoShow = value;
            }
        }

        [System.Design.SRCategory("DesignerOptions_LayoutSettings"), System.Design.SRDescription("DesignerOptions_ShowGridDesc")]
        public virtual bool ShowGrid
        {
            get => 
                this.showGrid;
            set
            {
                this.showGrid = value;
            }
        }

        [System.Design.SRCategory("DesignerOptions_LayoutSettings"), System.Design.SRDescription("DesignerOptions_SnapToGridDesc")]
        public virtual bool SnapToGrid
        {
            get => 
                this.snapToGrid;
            set
            {
                this.snapToGrid = value;
            }
        }

        [SRDisplayName("DesignerOptions_CodeGenDisplay"), System.Design.SRCategory("DesignerOptions_CodeGenSettings"), System.Design.SRDescription("DesignerOptions_OptimizedCodeGen")]
        public virtual bool UseOptimizedCodeGeneration
        {
            get => 
                this.enableComponentCache;
            set
            {
                this.enableComponentCache = value;
            }
        }

        [System.Design.SRDescription("DesignerOptions_UseSmartTags"), System.Design.SRCategory("DesignerOptions_LayoutSettings")]
        public virtual bool UseSmartTags
        {
            get => 
                this.useSmartTags;
            set
            {
                this.useSmartTags = value;
            }
        }

        [System.Design.SRDescription("DesignerOptions_UseSnapLines"), System.Design.SRCategory("DesignerOptions_LayoutSettings")]
        public virtual bool UseSnapLines
        {
            get => 
                this.useSnapLines;
            set
            {
                this.useSnapLines = value;
            }
        }
    }
}

