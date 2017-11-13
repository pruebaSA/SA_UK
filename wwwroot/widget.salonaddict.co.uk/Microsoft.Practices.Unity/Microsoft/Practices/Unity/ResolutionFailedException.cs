namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity.Properties;
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;

    [Serializable]
    public class ResolutionFailedException : Exception
    {
        private readonly string nameRequested;
        private readonly string typeRequested;

        protected ResolutionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.typeRequested = info.GetString("typeRequested");
            this.nameRequested = info.GetString("nameRequested");
        }

        public ResolutionFailedException(Type typeRequested, string nameRequested, Exception innerException, IBuilderContext context) : base(CreateMessage(typeRequested, nameRequested, innerException, context), innerException)
        {
            Guard.ArgumentNotNull(typeRequested, "typeRequested");
            if (typeRequested != null)
            {
                this.typeRequested = typeRequested.Name;
            }
            this.nameRequested = nameRequested;
        }

        private static void AddContextDetails(StringBuilder builder, IBuilderContext context, int depth)
        {
            if (context != null)
            {
                string str = new string(' ', depth * 2);
                NamedTypeBuildKey buildKey = context.BuildKey;
                NamedTypeBuildKey originalBuildKey = context.OriginalBuildKey;
                builder.Append(str);
                if (buildKey == originalBuildKey)
                {
                    builder.AppendFormat(CultureInfo.CurrentCulture, Resources.ResolutionTraceDetail, new object[] { buildKey.Type, FormatName(buildKey.Name) });
                }
                else
                {
                    builder.AppendFormat(CultureInfo.CurrentCulture, Resources.ResolutionWithMappingTraceDetail, new object[] { buildKey.Type, FormatName(buildKey.Name), originalBuildKey.Type, FormatName(originalBuildKey.Name) });
                }
                builder.AppendLine();
                if (context.CurrentOperation != null)
                {
                    builder.Append(str);
                    builder.AppendFormat(CultureInfo.CurrentCulture, context.CurrentOperation.ToString(), new object[0]);
                    builder.AppendLine();
                }
                AddContextDetails(builder, context.ChildContext, depth + 1);
            }
        }

        private static string CreateMessage(Type typeRequested, string nameRequested, Exception innerException, IBuilderContext context)
        {
            Guard.ArgumentNotNull(typeRequested, "typeRequested");
            Guard.ArgumentNotNull(context, "context");
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.CurrentCulture, Resources.ResolutionFailed, new object[] { typeRequested, FormatName(nameRequested), ExceptionReason(context), (innerException != null) ? innerException.GetType().Name : "ResolutionFailedException", innerException?.Message });
            builder.AppendLine();
            AddContextDetails(builder, context, 1);
            return builder.ToString();
        }

        private static string ExceptionReason(IBuilderContext context)
        {
            IBuilderContext childContext = context;
            while (childContext.ChildContext != null)
            {
                childContext = childContext.ChildContext;
            }
            if (childContext.CurrentOperation != null)
            {
                return childContext.CurrentOperation.ToString();
            }
            return Resources.NoOperationExceptionReason;
        }

        private static string FormatName(string name) => 
            (string.IsNullOrEmpty(name) ? "(none)" : name);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Guard.ArgumentNotNull(info, "info");
            base.GetObjectData(info, context);
            info.AddValue("typeRequested", this.typeRequested, typeof(string));
            info.AddValue("nameRequested", this.nameRequested, typeof(string));
        }

        public string NameRequested =>
            this.nameRequested;

        public string TypeRequested =>
            this.typeRequested;
    }
}

