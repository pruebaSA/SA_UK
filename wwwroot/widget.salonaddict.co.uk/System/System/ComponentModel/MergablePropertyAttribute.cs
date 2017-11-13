namespace System.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class MergablePropertyAttribute : Attribute
    {
        private bool allowMerge;
        public static readonly MergablePropertyAttribute Default = Yes;
        public static readonly MergablePropertyAttribute No = new MergablePropertyAttribute(false);
        public static readonly MergablePropertyAttribute Yes = new MergablePropertyAttribute(true);

        public MergablePropertyAttribute(bool allowMerge)
        {
            this.allowMerge = allowMerge;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            MergablePropertyAttribute attribute = obj as MergablePropertyAttribute;
            return ((attribute != null) && (attribute.AllowMerge == this.allowMerge));
        }

        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public bool AllowMerge =>
            this.allowMerge;
    }
}

