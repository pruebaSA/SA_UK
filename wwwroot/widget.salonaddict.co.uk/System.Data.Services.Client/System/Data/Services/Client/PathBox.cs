namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal class PathBox
    {
        private readonly Dictionary<ParameterExpression, string> basePaths = new Dictionary<ParameterExpression, string>(ReferenceEqualityComparer<ParameterExpression>.Instance);
        private const char EntireEntityMarker = '*';
        private readonly List<StringBuilder> expandPaths = new List<StringBuilder>();
        private readonly Stack<ParameterExpression> parameterExpressions = new Stack<ParameterExpression>();
        private readonly List<StringBuilder> projectionPaths = new List<StringBuilder>();

        internal PathBox()
        {
            this.projectionPaths.Add(new StringBuilder());
        }

        private static void AddEntireEntityMarker(StringBuilder sb)
        {
            if (sb.Length > 0)
            {
                sb.Append('/');
            }
            sb.Append('*');
        }

        internal void AppendToPath(PropertyInfo pi)
        {
            StringBuilder builder;
            Type elementType = TypeSystem.GetElementType(pi.PropertyType);
            if (ClientType.CheckElementTypeIsEntity(elementType))
            {
                builder = this.expandPaths.Last<StringBuilder>();
                if (builder.Length > 0)
                {
                    builder.Append('/');
                }
                builder.Append(pi.Name);
            }
            builder = this.projectionPaths.Last<StringBuilder>();
            RemoveEntireEntityMarkerIfPresent(builder);
            if (builder.Length > 0)
            {
                builder.Append('/');
            }
            builder.Append(pi.Name);
            if (ClientType.CheckElementTypeIsEntity(elementType))
            {
                AddEntireEntityMarker(builder);
            }
        }

        internal void PopParamExpression()
        {
            this.parameterExpressions.Pop();
        }

        internal void PushParamExpression(ParameterExpression pe)
        {
            StringBuilder item = this.projectionPaths.Last<StringBuilder>();
            this.basePaths.Add(pe, item.ToString());
            this.projectionPaths.Remove(item);
            this.parameterExpressions.Push(pe);
        }

        private static void RemoveEntireEntityMarkerIfPresent(StringBuilder sb)
        {
            if ((sb.Length > 0) && (sb[sb.Length - 1] == '*'))
            {
                sb.Remove(sb.Length - 1, 1);
            }
            if ((sb.Length > 0) && (sb[sb.Length - 1] == '/'))
            {
                sb.Remove(sb.Length - 1, 1);
            }
        }

        internal void StartNewPath()
        {
            StringBuilder sb = new StringBuilder(this.basePaths[this.ParamExpressionInScope]);
            RemoveEntireEntityMarkerIfPresent(sb);
            this.expandPaths.Add(new StringBuilder(sb.ToString()));
            AddEntireEntityMarker(sb);
            this.projectionPaths.Add(sb);
        }

        internal IEnumerable<string> ExpandPaths =>
            (from s in this.expandPaths
                where s.Length > 0
                select s.ToString()).Distinct<string>();

        internal ParameterExpression ParamExpressionInScope =>
            this.parameterExpressions.Peek();

        internal IEnumerable<string> ProjectionPaths =>
            (from s in this.projectionPaths
                where s.Length > 0
                select s.ToString()).Distinct<string>();
    }
}

