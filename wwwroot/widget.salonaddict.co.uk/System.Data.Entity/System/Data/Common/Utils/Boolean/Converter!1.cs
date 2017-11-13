namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Data.Common.Utils;

    internal sealed class Converter<T_Identifier>
    {
        private CnfSentence<T_Identifier> _cnf;
        private readonly ConversionContext<T_Identifier> _context;
        private DnfSentence<T_Identifier> _dnf;
        private readonly System.Data.Common.Utils.Boolean.Vertex _vertex;

        internal Converter(BoolExpr<T_Identifier> expr, ConversionContext<T_Identifier> context)
        {
            this._context = context ?? IdentifierService<T_Identifier>.Instance.CreateConversionContext();
            this._vertex = ToDecisionDiagramConverter<T_Identifier>.TranslateToRobdd(expr, this._context);
        }

        private void FindAllPaths(System.Data.Common.Utils.Boolean.Vertex vertex, Set<CnfClause<T_Identifier>> cnfClauses, Set<DnfClause<T_Identifier>> dnfClauses, Set<Literal<T_Identifier>> path)
        {
            if (vertex.IsOne())
            {
                DnfClause<T_Identifier> element = new DnfClause<T_Identifier>(path);
                dnfClauses.Add(element);
            }
            else if (vertex.IsZero())
            {
                CnfClause<T_Identifier> clause2 = new CnfClause<T_Identifier>(new Set<Literal<T_Identifier>>(from l in path select l.MakeNegated()));
                cnfClauses.Add(clause2);
            }
            else
            {
                foreach (LiteralVertexPair<T_Identifier> pair in this._context.GetSuccessors(vertex))
                {
                    path.Add(pair.Literal);
                    this.FindAllPaths(pair.Vertex, cnfClauses, dnfClauses, path);
                    path.Remove(pair.Literal);
                }
            }
        }

        private void InitializeNormalForms()
        {
            if (this._cnf == null)
            {
                if (this._vertex.IsOne())
                {
                    this._cnf = new CnfSentence<T_Identifier>(Set<CnfClause<T_Identifier>>.Empty);
                    DnfClause<T_Identifier> clause = new DnfClause<T_Identifier>(Set<Literal<T_Identifier>>.Empty);
                    Set<DnfClause<T_Identifier>> set = new Set<DnfClause<T_Identifier>> {
                        clause
                    };
                    this._dnf = new DnfSentence<T_Identifier>(set.MakeReadOnly());
                }
                else if (this._vertex.IsZero())
                {
                    CnfClause<T_Identifier> clause2 = new CnfClause<T_Identifier>(Set<Literal<T_Identifier>>.Empty);
                    Set<CnfClause<T_Identifier>> set2 = new Set<CnfClause<T_Identifier>> {
                        clause2
                    };
                    this._cnf = new CnfSentence<T_Identifier>(set2.MakeReadOnly());
                    this._dnf = new DnfSentence<T_Identifier>(Set<DnfClause<T_Identifier>>.Empty);
                }
                else
                {
                    Set<DnfClause<T_Identifier>> dnfClauses = new Set<DnfClause<T_Identifier>>();
                    Set<CnfClause<T_Identifier>> cnfClauses = new Set<CnfClause<T_Identifier>>();
                    Set<Literal<T_Identifier>> path = new Set<Literal<T_Identifier>>();
                    this.FindAllPaths(this._vertex, cnfClauses, dnfClauses, path);
                    this._cnf = new CnfSentence<T_Identifier>(cnfClauses.MakeReadOnly());
                    this._dnf = new DnfSentence<T_Identifier>(dnfClauses.MakeReadOnly());
                }
            }
        }

        internal CnfSentence<T_Identifier> Cnf
        {
            get
            {
                this.InitializeNormalForms();
                return this._cnf;
            }
        }

        internal DnfSentence<T_Identifier> Dnf
        {
            get
            {
                this.InitializeNormalForms();
                return this._dnf;
            }
        }

        internal System.Data.Common.Utils.Boolean.Vertex Vertex =>
            this._vertex;
    }
}

