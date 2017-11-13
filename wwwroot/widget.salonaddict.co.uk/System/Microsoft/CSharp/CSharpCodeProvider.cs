namespace Microsoft.CSharp
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    public class CSharpCodeProvider : CodeDomProvider
    {
        private CSharpCodeGenerator generator;

        public CSharpCodeProvider()
        {
            this.generator = new CSharpCodeGenerator();
        }

        public CSharpCodeProvider(IDictionary<string, string> providerOptions)
        {
            if (providerOptions == null)
            {
                throw new ArgumentNullException("providerOptions");
            }
            this.generator = new CSharpCodeGenerator(providerOptions);
        }

        [Obsolete("Callers should not use the ICodeCompiler interface and should instead use the methods directly on the CodeDomProvider class.")]
        public override ICodeCompiler CreateCompiler() => 
            this.generator;

        [Obsolete("Callers should not use the ICodeGenerator interface and should instead use the methods directly on the CodeDomProvider class.")]
        public override ICodeGenerator CreateGenerator() => 
            this.generator;

        public override void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
        {
            this.generator.GenerateCodeFromMember(member, writer, options);
        }

        public override TypeConverter GetConverter(Type type)
        {
            if (type == typeof(MemberAttributes))
            {
                return CSharpMemberAttributeConverter.Default;
            }
            if (type == typeof(TypeAttributes))
            {
                return CSharpTypeAttributeConverter.Default;
            }
            return base.GetConverter(type);
        }

        public override string FileExtension =>
            "cs";
    }
}

