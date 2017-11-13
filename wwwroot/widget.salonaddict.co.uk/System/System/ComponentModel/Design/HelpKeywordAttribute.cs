namespace System.ComponentModel.Design
{
    using System;

    [Serializable, AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=false)]
    public sealed class HelpKeywordAttribute : Attribute
    {
        private string contextKeyword;
        public static readonly HelpKeywordAttribute Default = new HelpKeywordAttribute();

        public HelpKeywordAttribute()
        {
        }

        public HelpKeywordAttribute(string keyword)
        {
            if (keyword == null)
            {
                throw new ArgumentNullException("keyword");
            }
            this.contextKeyword = keyword;
        }

        public HelpKeywordAttribute(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            this.contextKeyword = t.FullName;
        }

        public override bool Equals(object obj) => 
            ((obj == this) || (((obj != null) && (obj is HelpKeywordAttribute)) && (((HelpKeywordAttribute) obj).HelpKeyword == this.HelpKeyword)));

        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public string HelpKeyword =>
            this.contextKeyword;
    }
}

