namespace System.Windows.Forms.Design
{
    using System;
    using System.ComponentModel;

    internal class DataGridViewComponentPropertyGridSite : ISite, IServiceProvider
    {
        private IComponent comp;
        private bool inGetService;
        private IServiceProvider sp;

        public DataGridViewComponentPropertyGridSite(IServiceProvider sp, IComponent comp)
        {
            this.sp = sp;
            this.comp = comp;
        }

        public object GetService(Type t)
        {
            if (!this.inGetService && (this.sp != null))
            {
                try
                {
                    this.inGetService = true;
                    return this.sp.GetService(t);
                }
                finally
                {
                    this.inGetService = false;
                }
            }
            return null;
        }

        public IComponent Component =>
            this.comp;

        public IContainer Container =>
            null;

        public bool DesignMode =>
            false;

        public string Name
        {
            get => 
                null;
            set
            {
            }
        }
    }
}

