namespace System.Xml
{
    using System;

    internal abstract class Ref
    {
        protected Ref()
        {
        }

        public static bool Equal(string strA, string strB) => 
            (strA == strB);
    }
}

