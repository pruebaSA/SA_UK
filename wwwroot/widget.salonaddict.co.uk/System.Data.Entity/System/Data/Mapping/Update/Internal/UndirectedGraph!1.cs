namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Text;

    internal class UndirectedGraph<TVertex> : InternalBase
    {
        private IEqualityComparer<TVertex> m_comparer;
        private Graph<TVertex> m_graph;

        internal UndirectedGraph(IEqualityComparer<TVertex> comparer)
        {
            this.m_graph = new Graph<TVertex>(comparer);
            this.m_comparer = comparer;
        }

        internal void AddEdge(TVertex first, TVertex second)
        {
            this.m_graph.AddEdge(first, second);
            this.m_graph.AddEdge(second, first);
        }

        internal void AddVertex(TVertex vertex)
        {
            this.m_graph.AddVertex(vertex);
        }

        internal KeyToListMap<int, TVertex> GenerateConnectedComponents()
        {
            int compNum = 0;
            Dictionary<TVertex, ComponentNum<TVertex>> dictionary = new Dictionary<TVertex, ComponentNum<TVertex>>(this.m_comparer);
            foreach (TVertex local in this.Vertices)
            {
                dictionary.Add(local, new ComponentNum<TVertex>(compNum));
                compNum++;
            }
            foreach (KeyValuePair<TVertex, TVertex> pair in this.Edges)
            {
                if (dictionary[pair.Key].componentNum != dictionary[pair.Value].componentNum)
                {
                    int componentNum = dictionary[pair.Value].componentNum;
                    int num3 = dictionary[pair.Key].componentNum;
                    dictionary[pair.Value].componentNum = num3;
                    foreach (TVertex local2 in dictionary.Keys)
                    {
                        if (dictionary[local2].componentNum == componentNum)
                        {
                            dictionary[local2].componentNum = num3;
                        }
                    }
                }
            }
            KeyToListMap<int, TVertex> map = new KeyToListMap<int, TVertex>(EqualityComparer<int>.Default);
            foreach (TVertex local3 in this.Vertices)
            {
                int key = dictionary[local3].componentNum;
                map.Add(key, local3);
            }
            return map;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append(this.m_graph.ToString());
        }

        internal IEnumerable<KeyValuePair<TVertex, TVertex>> Edges =>
            this.m_graph.Edges;

        internal IEnumerable<TVertex> Vertices =>
            this.m_graph.Vertices;

        private class ComponentNum
        {
            internal int componentNum;

            internal ComponentNum(int compNum)
            {
                this.componentNum = compNum;
            }

            public override string ToString() => 
                StringUtil.FormatInvariant("{0}", new object[] { this.componentNum });
        }
    }
}

