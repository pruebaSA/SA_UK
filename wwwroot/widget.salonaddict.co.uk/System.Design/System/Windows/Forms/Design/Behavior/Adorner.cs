namespace System.Windows.Forms.Design.Behavior
{
    using System;
    using System.Drawing;

    public sealed class Adorner
    {
        private System.Windows.Forms.Design.Behavior.BehaviorService behaviorService;
        private bool enabled = true;
        private GlyphCollection glyphs = new GlyphCollection();

        public void Invalidate()
        {
            if (this.behaviorService != null)
            {
                this.behaviorService.Invalidate();
            }
        }

        public void Invalidate(Rectangle rectangle)
        {
            if (this.behaviorService != null)
            {
                this.behaviorService.Invalidate(rectangle);
            }
        }

        public void Invalidate(Region region)
        {
            if (this.behaviorService != null)
            {
                this.behaviorService.Invalidate(region);
            }
        }

        public System.Windows.Forms.Design.Behavior.BehaviorService BehaviorService
        {
            get => 
                this.behaviorService;
            set
            {
                this.behaviorService = value;
            }
        }

        public bool Enabled
        {
            get => 
                this.enabled;
            set
            {
                if (value != this.enabled)
                {
                    this.enabled = value;
                    this.Invalidate();
                }
            }
        }

        public GlyphCollection Glyphs =>
            this.glyphs;
    }
}

