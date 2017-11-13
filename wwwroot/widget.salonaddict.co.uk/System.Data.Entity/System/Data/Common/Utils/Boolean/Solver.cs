namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal sealed class Solver
    {
        private readonly Dictionary<Triple<Vertex, Vertex, Vertex>, Vertex> _computedIfThenElseValues = new Dictionary<Triple<Vertex, Vertex, Vertex>, Vertex>();
        private readonly Dictionary<Vertex, Vertex> _knownVertices = new Dictionary<Vertex, Vertex>(VertexValueComparer.Instance);
        private int _variableCount;
        internal static readonly Vertex[] BooleanVariableChildren = new Vertex[] { Vertex.One, Vertex.Zero };

        internal Vertex And(IEnumerable<Vertex> children) => 
            (from child in children
                orderby child.Variable descending
                select child).Aggregate<Vertex, Vertex>(Vertex.One, (left, right) => this.IfThenElse(left, right, Vertex.Zero));

        internal Vertex And(Vertex left, Vertex right) => 
            this.IfThenElse(left, right, Vertex.Zero);

        [Conditional("DEBUG")]
        private void AssertVertexValid(Vertex vertex)
        {
            vertex.IsSink();
        }

        [Conditional("DEBUG")]
        private void AssertVerticesValid(IEnumerable<Vertex> vertices)
        {
            using (IEnumerator<Vertex> enumerator = vertices.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Vertex current = enumerator.Current;
                }
            }
        }

        internal Vertex CreateLeafVertex(int variable, Vertex[] children) => 
            this.GetUniqueVertex(variable, children);

        internal int CreateVariable() => 
            ++this._variableCount;

        private static int DetermineTopVariable(Vertex condition, Vertex then, Vertex @else, out int topVariableDomainCount)
        {
            int variable;
            if (condition.Variable < then.Variable)
            {
                variable = condition.Variable;
                topVariableDomainCount = condition.Children.Length;
            }
            else
            {
                variable = then.Variable;
                topVariableDomainCount = then.Children.Length;
            }
            if (@else.Variable < variable)
            {
                variable = @else.Variable;
                topVariableDomainCount = @else.Children.Length;
            }
            return variable;
        }

        private static Vertex EvaluateFor(Vertex vertex, int variable, int variableAssigment)
        {
            if (variable < vertex.Variable)
            {
                return vertex;
            }
            return vertex.Children[variableAssigment];
        }

        private Vertex GetUniqueVertex(int variable, Vertex[] children)
        {
            Vertex vertex2;
            Vertex key = new Vertex(variable, children);
            if (this._knownVertices.TryGetValue(key, out vertex2))
            {
                return vertex2;
            }
            this._knownVertices.Add(key, key);
            return key;
        }

        private Vertex IfThenElse(Vertex condition, Vertex then, Vertex @else)
        {
            Vertex uniqueVertex;
            if (condition.IsOne())
            {
                return then;
            }
            if (condition.IsZero())
            {
                return @else;
            }
            if (then.IsOne() && @else.IsZero())
            {
                return condition;
            }
            if (then.Equals(@else))
            {
                return then;
            }
            Triple<Vertex, Vertex, Vertex> key = new Triple<Vertex, Vertex, Vertex>(condition, then, @else);
            if (!this._computedIfThenElseValues.TryGetValue(key, out uniqueVertex))
            {
                int num;
                int variable = DetermineTopVariable(condition, then, @else, out num);
                Vertex[] children = new Vertex[num];
                bool flag = true;
                for (int i = 0; i < num; i++)
                {
                    children[i] = this.IfThenElse(EvaluateFor(condition, variable, i), EvaluateFor(then, variable, i), EvaluateFor(@else, variable, i));
                    if (((i > 0) && flag) && !children[i].Equals(children[0]))
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    return children[0];
                }
                uniqueVertex = this.GetUniqueVertex(variable, children);
                this._computedIfThenElseValues.Add(key, uniqueVertex);
            }
            return uniqueVertex;
        }

        internal Vertex Not(Vertex vertex) => 
            this.IfThenElse(vertex, Vertex.Zero, Vertex.One);

        internal Vertex Or(IEnumerable<Vertex> children) => 
            (from child in children
                orderby child.Variable descending
                select child).Aggregate<Vertex, Vertex>(Vertex.Zero, (left, right) => this.IfThenElse(left, Vertex.One, right));

        private class VertexValueComparer : IEqualityComparer<Vertex>
        {
            internal static readonly Solver.VertexValueComparer Instance = new Solver.VertexValueComparer();

            private VertexValueComparer()
            {
            }

            public bool Equals(Vertex x, Vertex y)
            {
                if (x.IsSink())
                {
                    return x.Equals(y);
                }
                if ((x.Variable != y.Variable) || (x.Children.Length != y.Children.Length))
                {
                    return false;
                }
                for (int i = 0; i < x.Children.Length; i++)
                {
                    if (!x.Children[i].Equals(y.Children[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public int GetHashCode(Vertex vertex)
            {
                if (vertex.IsSink())
                {
                    return vertex.GetHashCode();
                }
                return (((vertex.Children[0].GetHashCode() << 5) + 1) + vertex.Children[1].GetHashCode());
            }
        }
    }
}

