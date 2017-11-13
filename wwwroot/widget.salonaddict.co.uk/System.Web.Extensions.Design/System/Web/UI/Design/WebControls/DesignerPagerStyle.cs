namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Web.UI.WebControls;

    internal abstract class DesignerPagerStyle
    {
        private ISite _site;

        protected DesignerPagerStyle(ISite site)
        {
            this._site = site;
        }

        public abstract void ApplyStyle(DataPager pager);
        public virtual DataPager CreatePager()
        {
            DataPager pager = new DataPager();
            string str = "DataPager1";
            if (this.Site != null)
            {
                INameCreationService service = (INameCreationService) this.Site.GetService(typeof(INameCreationService));
                if (service != null)
                {
                    str = service.CreateName(this.Site.Container, typeof(DataPager));
                }
            }
            pager.ID = str;
            this.ApplyStyle(pager);
            return pager;
        }

        public virtual bool IsPagerType(DataPager pager)
        {
            DataPager prototypePager = this.CreatePager();
            return ((prototypePager != null) && IsPagerTypeInternal(pager, prototypePager));
        }

        protected static bool IsPagerTypeInternal(DataPager pager, DataPager prototypePager)
        {
            if (pager.Fields.Count != prototypePager.Fields.Count)
            {
                return false;
            }
            for (int i = 0; i < pager.Fields.Count; i++)
            {
                if (!prototypePager.Fields[i].Equals(pager.Fields[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString() => 
            this.Name;

        public abstract string Name { get; }

        public ISite Site =>
            this._site;
    }
}

