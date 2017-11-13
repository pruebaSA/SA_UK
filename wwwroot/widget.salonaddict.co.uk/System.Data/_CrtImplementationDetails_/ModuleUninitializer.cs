namespace <CrtImplementationDetails>
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;

    internal class ModuleUninitializer : Stack
    {
        internal static <CrtImplementationDetails>.ModuleUninitializer _ModuleUninitializer = new <CrtImplementationDetails>.ModuleUninitializer();
        private static object @lock = new object();

        private ModuleUninitializer()
        {
            EventHandler handler = new EventHandler(this.SingletonDomainUnload);
            AppDomain.CurrentDomain.DomainUnload += handler;
            AppDomain.CurrentDomain.ProcessExit += handler;
        }

        internal void AddHandler(EventHandler handler)
        {
            RuntimeHelpers.PrepareDelegate(handler);
            this.Push(handler);
        }

        [PrePrepareMethod]
        private void SingletonDomainUnload(object source, EventArgs arguments)
        {
            using (IEnumerator enumerator = this.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ((EventHandler) enumerator.Current)(source, arguments);
                }
            }
        }
    }
}

