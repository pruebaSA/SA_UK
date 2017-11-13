namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Reflection;

    internal static class XmlILConstructors
    {
        public static readonly ConstructorInfo Debuggable = GetConstructor(typeof(DebuggableAttribute), new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
        public static readonly ConstructorInfo DecFromInt32 = GetConstructor(typeof(decimal), new Type[] { typeof(int) });
        public static readonly ConstructorInfo DecFromInt64 = GetConstructor(typeof(decimal), new Type[] { typeof(long) });
        public static readonly ConstructorInfo DecFromParts = GetConstructor(typeof(decimal), new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) });
        public static readonly ConstructorInfo NonUserCode = GetConstructor(typeof(DebuggerNonUserCodeAttribute));
        public static readonly ConstructorInfo QName = GetConstructor(typeof(XmlQualifiedName), new Type[] { typeof(string), typeof(string) });
        public static readonly ConstructorInfo StepThrough = GetConstructor(typeof(DebuggerStepThroughAttribute));
        public static readonly ConstructorInfo Transparent = GetConstructor(typeof(SecurityTransparentAttribute));

        private static ConstructorInfo GetConstructor(Type className) => 
            className.GetConstructor(new Type[0]);

        private static ConstructorInfo GetConstructor(Type className, params Type[] args) => 
            className.GetConstructor(args);
    }
}

