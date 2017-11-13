namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Web.UI;

    [ToolboxItem(false), DefaultProperty("Animations"), ParseChildren(true), PersistChildren(false)]
    public class AnimationExtenderControlBase : ExtenderControlBase
    {
        private string _animations;

        protected Animation GetAnimation(ref Animation animation, string name)
        {
            if (animation == null)
            {
                animation = Animation.Deserialize(base.GetPropertyValue<string>(name, ""));
            }
            return animation;
        }

        protected void ResolveControlIDs(Animation animation)
        {
            if (animation != null)
            {
                string str;
                if (animation.Properties.TryGetValue("AnimationTarget", out str) && !string.IsNullOrEmpty(str))
                {
                    Control control = null;
                    for (Control control2 = this.NamingContainer; (control2 != null) && ((control = control2.FindControl(str)) == null); control2 = control2.Parent)
                    {
                    }
                    if (control != null)
                    {
                        animation.Properties["AnimationTarget"] = control.ClientID;
                    }
                }
                foreach (Animation animation2 in animation.Children)
                {
                    this.ResolveControlIDs(animation2);
                }
            }
        }

        protected void SetAnimation(ref Animation animation, string name, Animation value)
        {
            animation = value;
            base.SetPropertyValue<string>(name, (animation != null) ? animation.ToString() : string.Empty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeAnimations() => 
            (base.DesignMode && !string.IsNullOrEmpty(this._animations));

        private static string TrimForDesigner(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            int startIndex = 0;
            while (startIndex < value.Length)
            {
                if (!char.IsWhiteSpace(value[startIndex]))
                {
                    break;
                }
                startIndex++;
            }
            startIndex = value.LastIndexOf('\n', startIndex);
            if (startIndex >= 0)
            {
                value = value.Substring(startIndex + 1);
            }
            return value.TrimEnd(new char[0]);
        }

        [TypeConverter(typeof(MultilineStringConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor)), PersistenceMode(PersistenceMode.InnerProperty), ExtenderControlProperty]
        public string Animations
        {
            get => 
                (this._animations ?? string.Empty);
            set
            {
                if (value != null)
                {
                    value = TrimForDesigner(value);
                }
                if (this._animations != value)
                {
                    this._animations = value;
                    Animation.Parse(this._animations, this);
                }
            }
        }
    }
}

