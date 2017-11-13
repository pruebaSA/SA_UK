namespace System.Web.Hosting
{
    using System;
    using System.Web;
    using System.Web.Management;

    internal class ISAPIWorkerRequestInProcForIIS7 : ISAPIWorkerRequestInProcForIIS6
    {
        private bool _isRewriteModuleEnabled;
        private string _rawUrl;

        internal ISAPIWorkerRequestInProcForIIS7(IntPtr ecb) : base(ecb)
        {
            base._trySkipIisCustomErrors = true;
        }

        public override string GetRawUrl()
        {
            if (this._rawUrl == null)
            {
                this._rawUrl = this.GetRequestUri();
                if (this._rawUrl != null)
                {
                    this._isRewriteModuleEnabled = true;
                    return this._rawUrl;
                }
                this._rawUrl = base.GetRawUrl();
            }
            return this._rawUrl;
        }

        private string GetRequestUri()
        {
            if (base.GetUnicodeServerVariable("UNICODE_IIS_WasUrlRewritten") == "1")
            {
                string unicodeServerVariable = base.GetUnicodeServerVariable(7);
                if (unicodeServerVariable == null)
                {
                    return null;
                }
                int num = 0;
                for (int i = 0; i < unicodeServerVariable.Length; i++)
                {
                    if ((unicodeServerVariable[i] == '/') && (++num == 3))
                    {
                        return unicodeServerVariable.Substring(i);
                    }
                }
            }
            return null;
        }

        internal override void RaiseTraceEvent(WebBaseEvent webEvent)
        {
            if ((IntPtr.Zero != base._ecb) && EtwTrace.IsTraceEnabled(webEvent.InferEtwTraceVerbosity(), 1))
            {
                int num;
                string[] strArray;
                int[] numArray;
                string[] strArray2;
                int num2;
                webEvent.DeconstructWebEvent(out num2, out num, out strArray, out numArray, out strArray2);
                UnsafeNativeMethods.EcbEmitWebEventTrace(base._ecb, num2, num, strArray, numArray, strArray2);
            }
        }

        internal override void RaiseTraceEvent(IntegratedTraceType traceType, string eventData)
        {
            if (IntPtr.Zero != base._ecb)
            {
                int flag = (traceType < IntegratedTraceType.DiagCritical) ? 4 : 2;
                if (EtwTrace.IsTraceEnabled(EtwTrace.InferVerbosity(traceType), flag))
                {
                    string str = string.IsNullOrEmpty(eventData) ? string.Empty : eventData;
                    UnsafeNativeMethods.EcbEmitSimpleTrace(base._ecb, (int) traceType, str);
                }
            }
        }

        internal override void SetRawUrl(string path)
        {
            this._rawUrl = path;
        }

        internal override bool IsRewriteModuleEnabled
        {
            get
            {
                if (this._rawUrl == null)
                {
                    this.GetRawUrl();
                }
                return this._isRewriteModuleEnabled;
            }
        }

        internal override bool TrySkipIisCustomErrors
        {
            get => 
                base._trySkipIisCustomErrors;
            set
            {
                base._trySkipIisCustomErrors = value;
            }
        }
    }
}

