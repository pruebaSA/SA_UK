﻿namespace System.Web.Compilation
{
    using System;
    using System.CodeDom;
    using System.ComponentModel.Design;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.UI;
    using System.Web.UI.Design;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class ExpressionBuilder
    {
        private static System.Web.Compilation.ExpressionBuilder dataBindingExpressionBuilder;

        protected ExpressionBuilder()
        {
        }

        internal virtual void BuildExpression(BoundPropertyEntry bpe, ControlBuilder controlBuilder, CodeExpression controlReference, CodeStatementCollection methodStatements, CodeStatementCollection statements, CodeLinePragma linePragma, ref bool hasTempObject)
        {
            CodeExpression expression = this.GetCodeExpression(bpe, bpe.ParsedExpressionData, new ExpressionBuilderContext(controlBuilder.VirtualPath));
            CodeDomUtility.CreatePropertySetStatements(methodStatements, statements, controlReference, bpe.Name, bpe.Type, expression, linePragma);
        }

        public virtual object EvaluateExpression(object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context) => 
            null;

        public abstract CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context);
        internal static System.Web.Compilation.ExpressionBuilder GetExpressionBuilder(string expressionPrefix, VirtualPath virtualPath) => 
            GetExpressionBuilder(expressionPrefix, virtualPath, null);

        internal static System.Web.Compilation.ExpressionBuilder GetExpressionBuilder(string expressionPrefix, VirtualPath virtualPath, IDesignerHost host)
        {
            if (expressionPrefix.Length == 0)
            {
                if (dataBindingExpressionBuilder == null)
                {
                    dataBindingExpressionBuilder = new DataBindingExpressionBuilder();
                }
                return dataBindingExpressionBuilder;
            }
            CompilationSection compilation = null;
            if (host != null)
            {
                IWebApplication application = (IWebApplication) host.GetService(typeof(IWebApplication));
                if (application != null)
                {
                    compilation = application.OpenWebConfiguration(true).GetSection("system.web/compilation") as CompilationSection;
                }
            }
            if (compilation == null)
            {
                compilation = RuntimeConfig.GetConfig(virtualPath).Compilation;
            }
            System.Web.Configuration.ExpressionBuilder builder = compilation.ExpressionBuilders[expressionPrefix];
            if (builder == null)
            {
                throw new HttpParseException(System.Web.SR.GetString("InvalidExpressionPrefix", new object[] { expressionPrefix }));
            }
            Type c = null;
            if (host != null)
            {
                ITypeResolutionService service = (ITypeResolutionService) host.GetService(typeof(ITypeResolutionService));
                if (service != null)
                {
                    c = service.GetType(builder.Type);
                }
            }
            if (c == null)
            {
                c = builder.TypeInternal;
            }
            if (!typeof(System.Web.Compilation.ExpressionBuilder).IsAssignableFrom(c))
            {
                throw new HttpParseException(System.Web.SR.GetString("ExpressionBuilder_InvalidType", new object[] { c.FullName }));
            }
            return (System.Web.Compilation.ExpressionBuilder) HttpRuntime.FastCreatePublicInstance(c);
        }

        public virtual object ParseExpression(string expression, Type propertyType, ExpressionBuilderContext context) => 
            null;

        public virtual bool SupportsEvaluate =>
            false;
    }
}

