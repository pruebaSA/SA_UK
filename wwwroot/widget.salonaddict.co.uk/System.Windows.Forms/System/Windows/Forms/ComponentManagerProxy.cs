namespace System.Windows.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class ComponentManagerProxy : MarshalByRefObject, UnsafeNativeMethods.IMsoComponentManager, UnsafeNativeMethods.IMsoComponent
    {
        private UnsafeNativeMethods.IMsoComponent _activeComponent;
        private int _activeComponentId;
        private ComponentManagerBroker _broker;
        private int _componentId;
        private Dictionary<int, UnsafeNativeMethods.IMsoComponent> _components;
        private int _creationThread;
        private int _nextComponentId;
        private UnsafeNativeMethods.IMsoComponentManager _original;
        private int _refCount;
        private UnsafeNativeMethods.IMsoComponent _trackingComponent;
        private int _trackingComponentId;

        internal ComponentManagerProxy(ComponentManagerBroker broker, UnsafeNativeMethods.IMsoComponentManager original)
        {
            this._broker = broker;
            this._original = original;
            this._creationThread = SafeNativeMethods.GetCurrentThreadId();
            this._refCount = 0;
        }

        private void Dispose()
        {
            if (this._original != null)
            {
                Marshal.ReleaseComObject(this._original);
                this._original = null;
                this._components = null;
                this._componentId = 0;
                this._refCount = 0;
                this._broker.ClearComponentManager();
            }
        }

        public override object InitializeLifetimeService() => 
            null;

        private bool RevokeComponent() => 
            this._original.FRevokeComponent(this._componentId);

        bool UnsafeNativeMethods.IMsoComponent.FContinueMessageLoop(int reason, int pvLoopData, NativeMethods.MSG[] msgPeeked)
        {
            bool flag = false;
            if (((this._refCount == 0) && (this._componentId != 0)) && this.RevokeComponent())
            {
                this._components.Clear();
                this._componentId = 0;
            }
            if (this._components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent component in this._components.Values)
                {
                    flag |= component.FContinueMessageLoop(reason, pvLoopData, msgPeeked);
                }
            }
            return flag;
        }

        bool UnsafeNativeMethods.IMsoComponent.FDebugMessage(IntPtr hInst, int msg, IntPtr wparam, IntPtr lparam)
        {
            UnsafeNativeMethods.IMsoComponent component = this.Component;
            return ((component != null) && component.FDebugMessage(hInst, msg, wparam, lparam));
        }

        bool UnsafeNativeMethods.IMsoComponent.FDoIdle(int grfidlef)
        {
            bool flag = false;
            if (this._components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent component in this._components.Values)
                {
                    flag |= component.FDoIdle(grfidlef);
                }
            }
            return flag;
        }

        bool UnsafeNativeMethods.IMsoComponent.FPreTranslateMessage(ref NativeMethods.MSG msg)
        {
            UnsafeNativeMethods.IMsoComponent component = this.Component;
            return ((component != null) && component.FPreTranslateMessage(ref msg));
        }

        bool UnsafeNativeMethods.IMsoComponent.FQueryTerminate(bool fPromptUser) => 
            true;

        IntPtr UnsafeNativeMethods.IMsoComponent.HwndGetWindow(int dwWhich, int dwReserved)
        {
            UnsafeNativeMethods.IMsoComponent component = this.Component;
            if (component != null)
            {
                return component.HwndGetWindow(dwWhich, dwReserved);
            }
            return IntPtr.Zero;
        }

        void UnsafeNativeMethods.IMsoComponent.OnActivationChange(UnsafeNativeMethods.IMsoComponent component, bool fSameComponent, int pcrinfo, bool fHostIsActivating, int pchostinfo, int dwReserved)
        {
            if (this._components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent component2 in this._components.Values)
                {
                    component2.OnActivationChange(component, fSameComponent, pcrinfo, fHostIsActivating, pchostinfo, dwReserved);
                }
            }
        }

        void UnsafeNativeMethods.IMsoComponent.OnAppActivate(bool fActive, int dwOtherThreadID)
        {
            if (this._components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent component in this._components.Values)
                {
                    component.OnAppActivate(fActive, dwOtherThreadID);
                }
            }
        }

        void UnsafeNativeMethods.IMsoComponent.OnEnterState(int uStateID, bool fEnter)
        {
            if (this._components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent component in this._components.Values)
                {
                    component.OnEnterState(uStateID, fEnter);
                }
            }
        }

        void UnsafeNativeMethods.IMsoComponent.OnLoseActivation()
        {
            if (this._activeComponent != null)
            {
                this._activeComponent.OnLoseActivation();
            }
        }

        void UnsafeNativeMethods.IMsoComponent.Terminate()
        {
            if ((this._components != null) && (this._components.Values.Count > 0))
            {
                UnsafeNativeMethods.IMsoComponent[] array = new UnsafeNativeMethods.IMsoComponent[this._components.Values.Count];
                this._components.Values.CopyTo(array, 0);
                foreach (UnsafeNativeMethods.IMsoComponent component in array)
                {
                    component.Terminate();
                }
            }
            if (this._original != null)
            {
                this.RevokeComponent();
            }
            this.Dispose();
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FContinueIdle() => 
            this._original?.FContinueIdle();

        bool UnsafeNativeMethods.IMsoComponentManager.FCreateSubComponentManager(object punkOuter, object punkServProv, ref Guid riid, out IntPtr ppvObj) => 
            this._original?.FCreateSubComponentManager(punkOuter, punkServProv, ref riid, out ppvObj);

        bool UnsafeNativeMethods.IMsoComponentManager.FDebugMessage(IntPtr hInst, int msg, IntPtr wparam, IntPtr lparam) => 
            this._original.FDebugMessage(hInst, msg, wparam, lparam);

        bool UnsafeNativeMethods.IMsoComponentManager.FGetActiveComponent(int dwgac, UnsafeNativeMethods.IMsoComponent[] ppic, NativeMethods.MSOCRINFOSTRUCT info, int dwReserved)
        {
            if (this._original == null)
            {
                return false;
            }
            if (!this._original.FGetActiveComponent(dwgac, ppic, info, dwReserved))
            {
                return false;
            }
            if (ppic[0] == this)
            {
                if (dwgac == 0)
                {
                    ppic[0] = this._activeComponent;
                }
                else if (dwgac == 1)
                {
                    ppic[0] = this._trackingComponent;
                }
                else if ((dwgac == 2) && (this._trackingComponent != null))
                {
                    ppic[0] = this._trackingComponent;
                }
            }
            return (ppic[0] != null);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FGetParentComponentManager(out UnsafeNativeMethods.IMsoComponentManager ppicm) => 
            this._original?.FGetParentComponentManager(out ppicm);

        bool UnsafeNativeMethods.IMsoComponentManager.FInState(int uStateID, IntPtr pvoid) => 
            this._original?.FInState(uStateID, pvoid);

        bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentActivate(int dwComponentID)
        {
            if (this._original == null)
            {
                return false;
            }
            if (((this._components == null) || (dwComponentID <= 0)) || !this._components.ContainsKey(dwComponentID))
            {
                return false;
            }
            if (!this._original.FOnComponentActivate(this._componentId))
            {
                return false;
            }
            this._activeComponent = this._components[dwComponentID];
            this._activeComponentId = dwComponentID;
            return true;
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentExitState(int dwComponentID, int uStateID, int uContext, int cpicmExclude, int rgpicmExclude)
        {
            if (this._original == null)
            {
                return false;
            }
            if (((uContext == 0) || (uContext == 1)) && (this._components != null))
            {
                foreach (UnsafeNativeMethods.IMsoComponent component in this._components.Values)
                {
                    component.OnEnterState(uStateID, false);
                }
            }
            return this._original.FOnComponentExitState(this._componentId, uStateID, uContext, cpicmExclude, rgpicmExclude);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FPushMessageLoop(int dwComponentID, int reason, int pvLoopData) => 
            this._original?.FPushMessageLoop(this._componentId, reason, pvLoopData);

        bool UnsafeNativeMethods.IMsoComponentManager.FRegisterComponent(UnsafeNativeMethods.IMsoComponent component, NativeMethods.MSOCRINFOSTRUCT pcrinfo, out int dwComponentID)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            dwComponentID = 0;
            if ((this._refCount == 0) && !this._original.FRegisterComponent(this, pcrinfo, out this._componentId))
            {
                return false;
            }
            this._refCount++;
            if (this._components == null)
            {
                this._components = new Dictionary<int, UnsafeNativeMethods.IMsoComponent>();
            }
            this._nextComponentId++;
            if (this._nextComponentId == 0x7fffffff)
            {
                this._nextComponentId = 1;
            }
            bool flag = false;
            while (this._components.ContainsKey(this._nextComponentId))
            {
                this._nextComponentId++;
                if (this._nextComponentId == 0x7fffffff)
                {
                    if (flag)
                    {
                        throw new InvalidOperationException(System.Windows.Forms.SR.GetString("ComponentManagerProxyOutOfMemory"));
                    }
                    flag = true;
                    this._nextComponentId = 1;
                }
            }
            this._components.Add(this._nextComponentId, component);
            dwComponentID = this._nextComponentId;
            return true;
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FRevokeComponent(int dwComponentID)
        {
            if (this._original == null)
            {
                return false;
            }
            if (((this._components == null) || (dwComponentID <= 0)) || !this._components.ContainsKey(dwComponentID))
            {
                return false;
            }
            if (((this._refCount == 1) && (SafeNativeMethods.GetCurrentThreadId() == this._creationThread)) && !this.RevokeComponent())
            {
                return false;
            }
            this._refCount--;
            this._components.Remove(dwComponentID);
            if (this._refCount <= 0)
            {
                this.Dispose();
            }
            if (dwComponentID == this._activeComponentId)
            {
                this._activeComponent = null;
                this._activeComponentId = 0;
            }
            if (dwComponentID == this._trackingComponentId)
            {
                this._trackingComponent = null;
                this._trackingComponentId = 0;
            }
            return true;
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FSetTrackingComponent(int dwComponentID, bool fTrack)
        {
            if (this._original == null)
            {
                return false;
            }
            if (((this._components == null) || (dwComponentID <= 0)) || !this._components.ContainsKey(dwComponentID))
            {
                return false;
            }
            if (!this._original.FSetTrackingComponent(this._componentId, fTrack))
            {
                return false;
            }
            if (fTrack)
            {
                this._trackingComponent = this._components[dwComponentID];
                this._trackingComponentId = dwComponentID;
            }
            else
            {
                this._trackingComponent = null;
                this._trackingComponentId = 0;
            }
            return true;
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FUpdateComponentRegistration(int dwComponentID, NativeMethods.MSOCRINFOSTRUCT info) => 
            this._original?.FUpdateComponentRegistration(this._componentId, info);

        void UnsafeNativeMethods.IMsoComponentManager.OnComponentEnterState(int dwComponentID, int uStateID, int uContext, int cpicmExclude, int rgpicmExclude, int dwReserved)
        {
            if (this._original != null)
            {
                if (((uContext == 0) || (uContext == 1)) && (this._components != null))
                {
                    foreach (UnsafeNativeMethods.IMsoComponent component in this._components.Values)
                    {
                        component.OnEnterState(uStateID, true);
                    }
                }
                this._original.OnComponentEnterState(this._componentId, uStateID, uContext, cpicmExclude, rgpicmExclude, dwReserved);
            }
        }

        int UnsafeNativeMethods.IMsoComponentManager.QueryService(ref Guid guidService, ref Guid iid, out object ppvObj) => 
            this._original.QueryService(ref guidService, ref iid, out ppvObj);

        private UnsafeNativeMethods.IMsoComponent Component
        {
            get
            {
                if (this._trackingComponent != null)
                {
                    return this._trackingComponent;
                }
                if (this._activeComponent != null)
                {
                    return this._activeComponent;
                }
                return null;
            }
        }
    }
}

