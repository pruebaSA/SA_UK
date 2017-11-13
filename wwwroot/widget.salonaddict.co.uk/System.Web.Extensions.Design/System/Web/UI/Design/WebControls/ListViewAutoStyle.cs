namespace System.Web.UI.Design.WebControls
{
    using System;

    internal abstract class ListViewAutoStyle
    {
        protected ListViewAutoStyle()
        {
        }

        public int GetIntValue(ListViewAutoLayout layout, string key)
        {
            object obj2 = this.GetValue(layout, key);
            if (obj2 != null)
            {
                return (int) obj2;
            }
            return 0;
        }

        public string GetStringValue(ListViewAutoLayout layout, string key)
        {
            object obj2 = this.GetValue(layout, key);
            if (obj2 != null)
            {
                return obj2.ToString();
            }
            return string.Empty;
        }

        private object GetValue(ListViewAutoLayout layout, string key) => 
            ListViewAutoLayoutDefinitions.GetValue(layout.ID + "_" + this.ID, key);

        public override string ToString() => 
            this.Name;

        public abstract string ID { get; }

        public abstract string Name { get; }
    }
}

