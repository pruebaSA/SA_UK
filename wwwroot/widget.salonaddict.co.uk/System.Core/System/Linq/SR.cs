namespace System.Linq
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    internal sealed class SR
    {
        internal const string ArgumentArrayHasTooManyElements = "ArgumentArrayHasTooManyElements";
        internal const string ArgumentNotIEnumerableGeneric = "ArgumentNotIEnumerableGeneric";
        internal const string ArgumentNotLambda = "ArgumentNotLambda";
        internal const string ArgumentNotSequence = "ArgumentNotSequence";
        internal const string ArgumentNotValid = "ArgumentNotValid";
        internal const string EmptyEnumerable = "EmptyEnumerable";
        internal const string IncompatibleElementTypes = "IncompatibleElementTypes";
        private static System.Linq.SR loader;
        internal const string MoreThanOneElement = "MoreThanOneElement";
        internal const string MoreThanOneMatch = "MoreThanOneMatch";
        internal const string NoArgumentMatchingMethodsInQueryable = "NoArgumentMatchingMethodsInQueryable";
        internal const string NoElements = "NoElements";
        internal const string NoMatch = "NoMatch";
        internal const string NoMethodOnType = "NoMethodOnType";
        internal const string NoMethodOnTypeMatchingArguments = "NoMethodOnTypeMatchingArguments";
        internal const string NoNameMatchingMethodsInQueryable = "NoNameMatchingMethodsInQueryable";
        internal const string OwningTeam = "OwningTeam";
        private ResourceManager resources;

        internal SR()
        {
            this.resources = new ResourceManager("System.Linq", base.GetType().Assembly);
        }

        private static System.Linq.SR GetLoader()
        {
            if (loader == null)
            {
                System.Linq.SR sr = new System.Linq.SR();
                Interlocked.CompareExchange<System.Linq.SR>(ref loader, sr, null);
            }
            return loader;
        }

        public static object GetObject(string name)
        {
            System.Linq.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, Culture);
        }

        public static string GetString(string name)
        {
            System.Linq.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, params object[] args)
        {
            System.Linq.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, Culture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string str2 = args[i] as string;
                if ((str2 != null) && (str2.Length > 0x400))
                {
                    args[i] = str2.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static CultureInfo Culture =>
            null;

        public static ResourceManager Resources =>
            GetLoader().resources;
    }
}

