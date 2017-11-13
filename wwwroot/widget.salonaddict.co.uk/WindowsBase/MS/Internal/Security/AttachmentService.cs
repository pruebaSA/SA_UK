namespace MS.Internal.Security
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;

    [FriendAccessAllowed]
    internal sealed class AttachmentService : IDisposable
    {
        private readonly Guid _clientId = new Guid("{D5734190-005C-4d76-B0DD-2FA89BE0B622}");
        [SecurityCritical]
        private ISecuritySuppressedIAttachmentExecute _native = ((ISecuritySuppressedIAttachmentExecute) new AttachmentServices());

        [SecurityTreatAsSafe, SecurityCritical]
        private AttachmentService()
        {
            this._native.SetClientGuid(ref this._clientId);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private void Dispose(bool disposing)
        {
            if (disposing && (this._native != null))
            {
                Marshal.ReleaseComObject(this._native);
                this._native = null;
            }
        }

        ~AttachmentService()
        {
            this.Dispose(true);
        }

        [SecurityCritical]
        internal static void SaveWithUI(IntPtr parent, Uri source, Uri target)
        {
            using (AttachmentService service = new AttachmentService())
            {
                ISecuritySuppressedIAttachmentExecute execute = service._native;
                execute.SetSource(source.OriginalString);
                execute.SetLocalPath(target.LocalPath);
                execute.SaveWithUI(parent);
            }
        }

        private enum ATTACHMENT_ACTION
        {
            ATTACHMENT_ACTION_CANCEL,
            ATTACHMENT_ACTION_SAVE,
            ATTACHMENT_ACTION_EXEC
        }

        private enum ATTACHMENT_PROMPT
        {
            ATTACHMENT_PROMPT_NONE,
            ATTACHMENT_PROMPT_SAVE,
            ATTACHMENT_PROMPT_EXEC,
            ATTACHMENT_PROMPT_EXEC_OR_SAVE
        }

        [ComImport, Guid("4125DD96-E03A-4103-8F70-E0597D803B9C")]
        private class AttachmentServices
        {
        }

        [ComImport, Guid("73DB1241-1E85-4581-8E4F-A81E1D0F8C57"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ISecuritySuppressedIAttachmentExecute
        {
            int SetClientTitle(string pszTitle);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            int SetClientGuid(ref Guid guid);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            int SetLocalPath(string pszLocalPath);
            int SetFileName(string pszFileName);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            int SetSource(string pszSource);
            int SetReferrer(string pszReferrer);
            int CheckPolicy();
            int Prompt(IntPtr hwnd, AttachmentService.ATTACHMENT_PROMPT prompt, out AttachmentService.ATTACHMENT_ACTION paction);
            int Save();
            int Execute(IntPtr hwnd, string pszVerb, out IntPtr phProcess);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            int SaveWithUI(IntPtr hwnd);
            int ClearClientState();
        }
    }
}

