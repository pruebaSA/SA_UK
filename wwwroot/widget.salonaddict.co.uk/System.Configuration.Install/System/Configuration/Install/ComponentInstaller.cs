namespace System.Configuration.Install
{
    using System;
    using System.ComponentModel;

    public abstract class ComponentInstaller : Installer
    {
        protected ComponentInstaller()
        {
        }

        public abstract void CopyFromComponent(IComponent component);
        public virtual bool IsEquivalentInstaller(ComponentInstaller otherInstaller) => 
            false;
    }
}

