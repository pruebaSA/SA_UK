namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal class SqlNodeAnnotations
    {
        private Dictionary<SqlNode, List<SqlNodeAnnotation>> annotationMap = new Dictionary<SqlNode, List<SqlNodeAnnotation>>();
        private Dictionary<Type, string> uniqueTypes = new Dictionary<Type, string>();

        internal void Add(SqlNode node, SqlNodeAnnotation annotation)
        {
            List<SqlNodeAnnotation> list = null;
            if (!this.annotationMap.TryGetValue(node, out list))
            {
                list = new List<SqlNodeAnnotation>();
                this.annotationMap[node] = list;
            }
            this.uniqueTypes[annotation.GetType()] = string.Empty;
            list.Add(annotation);
        }

        internal List<SqlNodeAnnotation> Get(SqlNode node)
        {
            List<SqlNodeAnnotation> list = null;
            this.annotationMap.TryGetValue(node, out list);
            return list;
        }

        internal bool HasAnnotationType(Type type) => 
            this.uniqueTypes.ContainsKey(type);

        internal bool NodeIsAnnotated(SqlNode node)
        {
            if (node == null)
            {
                return false;
            }
            return this.annotationMap.ContainsKey(node);
        }
    }
}

