namespace System.Web.Util
{
    using System;
    using System.Web;

    internal abstract class FileData
    {
        protected string _path;
        protected UnsafeNativeMethods.WIN32_FIND_DATA _wfd;

        protected FileData()
        {
        }

        internal FindFileData GetFindFileData() => 
            new FindFileData(ref this._wfd);

        internal string FullName =>
            (this._path + @"\" + this._wfd.cFileName);

        internal bool IsDirectory =>
            ((this._wfd.dwFileAttributes & 0x10) != 0);

        internal bool IsHidden =>
            ((this._wfd.dwFileAttributes & 2) != 0);

        internal string Name =>
            this._wfd.cFileName;
    }
}

