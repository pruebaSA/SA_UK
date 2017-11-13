namespace System.Runtime.Versioning
{
    using Microsoft.Win32;
    using System;

    public static class VersioningHelper
    {
        private static ResourceScope ResTypeMask = (ResourceScope.Library | ResourceScope.AppDomain | ResourceScope.Process | ResourceScope.Machine);
        private static ResourceScope VisibilityMask = (ResourceScope.Assembly | ResourceScope.Private);

        private static SxSRequirements GetRequirements(ResourceScope consumeAsScope, ResourceScope calleeScope)
        {
            ResourceScope scope3;
            SxSRequirements none = SxSRequirements.None;
            switch ((calleeScope & ResTypeMask))
            {
                case ResourceScope.Machine:
                    switch ((consumeAsScope & ResTypeMask))
                    {
                        case ResourceScope.Machine:
                            goto Label_00AC;

                        case ResourceScope.Process:
                            none |= SxSRequirements.ProcessID;
                            goto Label_00AC;

                        case ResourceScope.AppDomain:
                            none |= SxSRequirements.ProcessID | SxSRequirements.AppDomainID;
                            goto Label_00AC;
                    }
                    break;

                case ResourceScope.Process:
                    if ((consumeAsScope & ResourceScope.AppDomain) != ResourceScope.None)
                    {
                        none |= SxSRequirements.AppDomainID;
                    }
                    goto Label_00AC;

                case ResourceScope.AppDomain:
                    goto Label_00AC;

                default:
                    throw new ArgumentException(Environment.GetResourceString("Argument_BadResourceScopeTypeBits", new object[] { calleeScope }), "calleeScope");
            }
            throw new ArgumentException(Environment.GetResourceString("Argument_BadResourceScopeTypeBits", new object[] { consumeAsScope }), "consumeAsScope");
        Label_00AC:
            scope3 = calleeScope & VisibilityMask;
            if (scope3 != ResourceScope.None)
            {
                if (scope3 != ResourceScope.Private)
                {
                    if (scope3 != ResourceScope.Assembly)
                    {
                        throw new ArgumentException(Environment.GetResourceString("Argument_BadResourceScopeVisibilityBits", new object[] { calleeScope }), "calleeScope");
                    }
                    if ((consumeAsScope & ResourceScope.Private) != ResourceScope.None)
                    {
                        none |= SxSRequirements.TypeName;
                    }
                }
                return none;
            }
            switch ((consumeAsScope & VisibilityMask))
            {
                case ResourceScope.None:
                    return none;

                case ResourceScope.Private:
                    return (none | (SxSRequirements.TypeName | SxSRequirements.AssemblyName));

                case ResourceScope.Assembly:
                    return (none | SxSRequirements.AssemblyName);
            }
            throw new ArgumentException(Environment.GetResourceString("Argument_BadResourceScopeVisibilityBits", new object[] { consumeAsScope }), "consumeAsScope");
        }

        public static string MakeVersionSafeName(string name, ResourceScope from, ResourceScope to) => 
            MakeVersionSafeName(name, from, to, null);

        public static string MakeVersionSafeName(string name, ResourceScope from, ResourceScope to, Type type)
        {
            ResourceScope scope = from & ResTypeMask;
            ResourceScope scope2 = to & ResTypeMask;
            if (scope > scope2)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_ResourceScopeWrongDirection", new object[] { scope, scope2 }), "from");
            }
            SxSRequirements requirements = GetRequirements(to, from);
            if (((requirements & (SxSRequirements.TypeName | SxSRequirements.AssemblyName)) != SxSRequirements.None) && (type == null))
            {
                throw new ArgumentNullException("type", Environment.GetResourceString("ArgumentNull_TypeRequiredByResourceScope"));
            }
            string str = "";
            if ((requirements & SxSRequirements.ProcessID) != SxSRequirements.None)
            {
                str = str + "_" + Win32Native.GetCurrentProcessId();
            }
            if ((requirements & SxSRequirements.AppDomainID) != SxSRequirements.None)
            {
                str = str + "_" + AppDomain.CurrentDomain.GetAppDomainId();
            }
            if ((requirements & SxSRequirements.TypeName) != SxSRequirements.None)
            {
                str = str + "_" + type.Name;
            }
            if ((requirements & SxSRequirements.AssemblyName) != SxSRequirements.None)
            {
                str = str + "_" + type.Assembly.FullName;
            }
            return (name + str);
        }
    }
}

