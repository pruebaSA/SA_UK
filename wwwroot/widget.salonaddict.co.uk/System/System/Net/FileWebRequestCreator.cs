namespace System.Net
{
    using System;

    internal class FileWebRequestCreator : IWebRequestCreate
    {
        internal FileWebRequestCreator()
        {
        }

        public WebRequest Create(Uri uri) => 
            new FileWebRequest(uri);
    }
}

