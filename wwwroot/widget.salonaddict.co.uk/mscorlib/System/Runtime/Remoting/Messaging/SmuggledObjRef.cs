namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Runtime.Remoting;

    internal class SmuggledObjRef
    {
        private System.Runtime.Remoting.ObjRef _objRef;

        public SmuggledObjRef(System.Runtime.Remoting.ObjRef objRef)
        {
            this._objRef = objRef;
        }

        public System.Runtime.Remoting.ObjRef ObjRef =>
            this._objRef;
    }
}

