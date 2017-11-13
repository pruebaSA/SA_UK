namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class MachineKeyValidationConverter : ConfigurationConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            switch (((string) data))
            {
                case "SHA1":
                    return MachineKeyValidation.SHA1;

                case "MD5":
                    return MachineKeyValidation.MD5;

                case "3DES":
                    return MachineKeyValidation.TripleDES;

                case "AES":
                    return MachineKeyValidation.AES;
            }
            throw new ArgumentException(System.Web.SR.GetString("Config_Invalid_enum_value", new object[] { "SHA1, MD5, 3DES, AES" }));
        }

        public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
        {
            if ((value != null) && (value.GetType() != typeof(MachineKeyValidation)))
            {
                throw new ArgumentException(System.Web.SR.GetString("Invalid_enum_value", new object[] { "SHA1, MD5, 3DES, AES" }));
            }
            switch (((MachineKeyValidation) value))
            {
                case MachineKeyValidation.MD5:
                    return "MD5";

                case MachineKeyValidation.SHA1:
                    return "SHA1";

                case MachineKeyValidation.TripleDES:
                    return "3DES";

                case MachineKeyValidation.AES:
                    return "AES";
            }
            throw new ArgumentOutOfRangeException("value");
        }
    }
}

