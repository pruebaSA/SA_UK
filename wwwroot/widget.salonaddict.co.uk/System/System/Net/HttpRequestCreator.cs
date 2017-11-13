namespace System.Net
{
    using System;

    internal class HttpRequestCreator : IWebRequestCreate
    {
        public WebRequest Create(Uri Uri) => 
            new HttpWebRequest(Uri, null);
    }
}

