namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.EnterpriseServices;
    using System.Runtime.InteropServices;

    [Guid("7B2801E6-0BC6-4c92-B742-6BE9B01AE874"), ComVisible(true)]
    public class DllHostInitializer : IProcessInitializer
    {
        private DllHostInitializeWorker worker = new DllHostInitializeWorker();

        public void Shutdown()
        {
            this.worker.Shutdown();
        }

        public void Startup(object punkProcessControl)
        {
            IProcessInitControl control = punkProcessControl as IProcessInitControl;
            this.worker.Startup(control);
        }
    }
}

