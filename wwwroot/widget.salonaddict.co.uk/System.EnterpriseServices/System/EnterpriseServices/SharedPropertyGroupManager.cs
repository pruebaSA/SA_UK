namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    [ComVisible(false)]
    public sealed class SharedPropertyGroupManager : IEnumerable
    {
        private ISharedPropertyGroupManager _ex;

        public SharedPropertyGroupManager()
        {
            Platform.Assert(Platform.MTS, "SharedPropertyGroupManager");
            this._ex = (ISharedPropertyGroupManager) new xSharedPropertyGroupManager();
        }

        public SharedPropertyGroup CreatePropertyGroup(string name, ref PropertyLockMode dwIsoMode, ref PropertyReleaseMode dwRelMode, out bool fExist) => 
            new SharedPropertyGroup(this._ex.CreatePropertyGroup(name, ref dwIsoMode, ref dwRelMode, out fExist));

        public IEnumerator GetEnumerator()
        {
            IEnumerator pEnum = null;
            this._ex.GetEnumerator(out pEnum);
            return pEnum;
        }

        public SharedPropertyGroup Group(string name) => 
            new SharedPropertyGroup(this._ex.Group(name));
    }
}

