namespace System.EnterpriseServices.Internal
{
    using System;
    using System.Collections;
    using System.EnterpriseServices;
    using System.EnterpriseServices.Thunk;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.MetadataServices;
    using System.Threading;

    internal class GenAssemblyFromWsdl
    {
        private bool ExceptionThrown;
        private string filename = "";
        private string pathname = "";
        private Exception SavedException;
        private Thread thisthread;
        private IntPtr threadtoken = IntPtr.Zero;
        private const uint TOKEN_IMPERSONATE = 4;
        private string wsdlurl = "";

        public GenAssemblyFromWsdl()
        {
            this.thisthread = new Thread(new ThreadStart(this.Generate));
        }

        public void Generate()
        {
            try
            {
                if ((this.threadtoken != IntPtr.Zero) && !NativeMethods.SetThreadToken(IntPtr.Zero, this.threadtoken))
                {
                    throw new COMException(Resource.FormatString("Err_SetThreadToken"), Marshal.GetHRForLastWin32Error());
                }
                if (this.wsdlurl.Length > 0)
                {
                    Stream outputStream = new MemoryStream();
                    ArrayList outCodeStreamList = new ArrayList();
                    MetaData.RetrieveSchemaFromUrlToStream(this.wsdlurl, outputStream);
                    outputStream.Position = 0L;
                    MetaData.ConvertSchemaStreamToCodeSourceStream(true, this.pathname, outputStream, outCodeStreamList);
                    MetaData.ConvertCodeSourceStreamToAssemblyFile(outCodeStreamList, this.filename, null);
                }
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
                this.SavedException = exception;
                this.ExceptionThrown = true;
            }
            catch
            {
                string s = Resource.FormatString("Err_NonClsException", "GenAssemblyFromWsdl.Generate");
                ComSoapPublishError.Report(s);
                this.SavedException = new RegistrationException(s);
                this.ExceptionThrown = true;
                throw;
            }
        }

        public void Run(string WsdlUrl, string FileName, string PathName)
        {
            try
            {
                if ((WsdlUrl.Length > 0) && (FileName.Length > 0))
                {
                    this.wsdlurl = WsdlUrl;
                    this.filename = PathName + FileName;
                    this.pathname = PathName;
                    if (!NativeMethods.OpenThreadToken(NativeMethods.GetCurrentThread(), 4, true, ref this.threadtoken) && (Marshal.GetLastWin32Error() != System.EnterpriseServices.Util.ERROR_NO_TOKEN))
                    {
                        throw new COMException(Resource.FormatString("Err_OpenThreadToken"), Marshal.GetHRForLastWin32Error());
                    }
                    SafeUserTokenHandle handle = null;
                    try
                    {
                        handle = new SafeUserTokenHandle(Security.SuspendImpersonation(), true);
                        this.thisthread.Start();
                    }
                    finally
                    {
                        if (handle != null)
                        {
                            Security.ResumeImpersonation(handle.DangerousGetHandle());
                            handle.Dispose();
                        }
                    }
                    this.thisthread.Join();
                    if (this.ExceptionThrown)
                    {
                        throw this.SavedException;
                    }
                }
            }
            catch (Exception exception)
            {
                ComSoapPublishError.Report(exception.ToString());
                throw;
            }
            catch
            {
                ComSoapPublishError.Report(Resource.FormatString("Err_NonClsException", "GenAssemblyFromWsdl.Run"));
                throw;
            }
        }
    }
}

