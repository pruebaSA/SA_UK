namespace System.Deployment.Application
{
    using Microsoft.Internal.Performance;
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    internal static class DFServiceEntryPoint
    {
        private static DfsvcForm _dfsvcForm;
        private static IntPtr DfdllHandle;
        private static RegisterDeploymentServiceComDelegate RegisterDeploymentServiceCom;
        private static CreateDeploymentServiceComDelegate s_createDeploymentServiceComDelegate;
        private static UnregisterDeploymentServiceComDelegate UnregisterDeploymentServiceCom;

        private static object GetMethodDelegate(IntPtr handle, string methodName, System.Type methodDelegateType)
        {
            IntPtr procAddress = System.Deployment.Application.NativeMethods.GetProcAddress(handle, methodName);
            if (procAddress == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return Marshal.GetDelegateForFunctionPointer(procAddress, methodDelegateType);
        }

        public static void Initialize(string[] args)
        {
            CodeMarker_Singleton.Instance.CodeMarker(CodeMarkerEvent.perfNewTaskBegin);
            if (PlatformSpecific.OnWin9x)
            {
                new Thread(new ThreadStart(DFServiceEntryPoint.MessageLoopThread)).Start();
            }
            ObtainDfdllExports();
            s_createDeploymentServiceComDelegate = () => new DeploymentServiceComWrapper();
            int errorCode = RegisterDeploymentServiceCom(s_createDeploymentServiceComDelegate);
            if (errorCode < 0)
            {
                throw Marshal.GetExceptionForHR(errorCode);
            }
            CodeMarker_Singleton.Instance.CodeMarker(CodeMarkerEvent.perfNewTaskEnd);
            bool flag = LifetimeManager.WaitForEnd();
            if (_dfsvcForm != null)
            {
                _dfsvcForm.Invoke(new DfsvcForm.CloseFormDelegate(_dfsvcForm.CloseForm), new object[] { true });
            }
            UnregisterDeploymentServiceCom();
            if (!flag && PlatformSpecific.OnWin9x)
            {
                Thread.Sleep(0x1388);
            }
            CodeMarker_Singleton.Instance.UninitializePerformanceDLL(CodeMarkerApp.CLICKONCEPERF);
            Environment.Exit(0);
        }

        private static void MessageLoopThread()
        {
            if (_dfsvcForm == null)
            {
                _dfsvcForm = new DfsvcForm();
                SystemEvents.SessionEnded += new SessionEndedEventHandler(_dfsvcForm.SessionEndedEventHandler);
                SystemEvents.SessionEnding += new SessionEndingEventHandler(_dfsvcForm.SessionEndingEventHandler);
                Application.Run(_dfsvcForm);
            }
        }

        private static void ObtainDfdllExports()
        {
            DfdllHandle = System.Deployment.Application.NativeMethods.LoadLibrary(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "dfdll.dll"));
            if (DfdllHandle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            RegisterDeploymentServiceCom = (RegisterDeploymentServiceComDelegate) GetMethodDelegate(DfdllHandle, "RegisterDeploymentServiceCom", typeof(RegisterDeploymentServiceComDelegate));
            UnregisterDeploymentServiceCom = (UnregisterDeploymentServiceComDelegate) GetMethodDelegate(DfdllHandle, "UnregisterDeploymentServiceCom", typeof(UnregisterDeploymentServiceComDelegate));
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate IManagedDeploymentServiceCom CreateDeploymentServiceComDelegate();

        private class DfsvcForm : Form
        {
            private bool _formClosed;
            private bool _lifetimeManagerTerminated;
            private Container components;

            public DfsvcForm()
            {
                this.InitializeComponent();
            }

            public void CloseForm(bool lifetimeManagerAlreadyTerminated)
            {
                if (!this._formClosed)
                {
                    lock (this)
                    {
                        if (lifetimeManagerAlreadyTerminated)
                        {
                            this._lifetimeManagerTerminated = true;
                        }
                        if (!this._formClosed)
                        {
                            this._formClosed = true;
                            base.Close();
                        }
                    }
                }
            }

            private void DfsvcForm_Closed(object sender, EventArgs e)
            {
                this.TerminateLifetimeManager(true);
            }

            private void DfsvcForm_Closing(object sender, CancelEventArgs e)
            {
                e.Cancel = false;
                this.TerminateLifetimeManager(true);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && (this.components != null))
                {
                    this.components.Dispose();
                }
                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
                base.ClientSize = new Size(0x124, 0x10a);
                base.ShowInTaskbar = false;
                base.WindowState = FormWindowState.Minimized;
                base.TopMost = true;
                base.Closing += new CancelEventHandler(this.DfsvcForm_Closing);
                base.Closed += new EventHandler(this.DfsvcForm_Closed);
            }

            public void SessionEndedEventHandler(object sender, SessionEndedEventArgs e)
            {
                this.TerminateLifetimeManager(false);
            }

            public void SessionEndingEventHandler(object sender, SessionEndingEventArgs e)
            {
                e.Cancel = false;
                this.TerminateLifetimeManager(false);
            }

            private void TerminateLifetimeManager(bool formAlreadyClosed)
            {
                if (!this._lifetimeManagerTerminated)
                {
                    lock (this)
                    {
                        if (formAlreadyClosed)
                        {
                            this._formClosed = true;
                        }
                        if (!this._lifetimeManagerTerminated)
                        {
                            this._lifetimeManagerTerminated = true;
                            LifetimeManager.EndImmediately();
                        }
                    }
                }
            }

            public delegate void CloseFormDelegate(bool lifetimeManagerAlreadyTerminated);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int RegisterDeploymentServiceComDelegate([MarshalAs(UnmanagedType.FunctionPtr)] DFServiceEntryPoint.CreateDeploymentServiceComDelegate createDeploymentServiceComDelegate);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int UnregisterDeploymentServiceComDelegate();
    }
}

