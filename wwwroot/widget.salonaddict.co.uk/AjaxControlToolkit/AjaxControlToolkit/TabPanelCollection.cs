namespace AjaxControlToolkit
{
    using System;
    using System.Reflection;
    using System.Web.UI;

    public class TabPanelCollection : ControlCollection
    {
        public TabPanelCollection(Control owner) : base(owner)
        {
        }

        public override void Add(Control child)
        {
            if (!(child is TabPanel))
            {
                throw new ArgumentException("TabPanelCollection can only contain TabPanel controls.");
            }
            base.Add(child);
        }

        public override void AddAt(int index, Control child)
        {
            if (!(child is TabPanel))
            {
                throw new ArgumentException("TabPanelCollection can only contain TabPanel controls.");
            }
            base.AddAt(index, child);
        }

        public TabPanel this[int index] =>
            ((TabPanel) base[index]);
    }
}

