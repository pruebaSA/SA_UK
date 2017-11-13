namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Mapping.ViewGeneration.Validation;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    internal class QueryRewriter
    {
        private CellTreeNode _basicView;
        private Dictionary<MemberPath, CaseStatement> _caseStatements = new Dictionary<MemberPath, CaseStatement>();
        private ConfigViewGenerator _config;
        private MemberDomainMap _domainMap;
        private FragmentQuery _domainQuery;
        private ErrorLog _errorLog = new ErrorLog();
        private MemberPath _extentPath;
        private List<FragmentQuery> _fragmentQueries = new List<FragmentQuery>();
        private EdmType _generatedType;
        private CqlIdentifiers _identifiers;
        private List<MemberPath> _keyAttributes;
        private CellNormalizer _normalizer;
        private RewritingProcessor<Tile<FragmentQuery>> _qp;
        private BoolExpression _topLevelWhereClause;
        private ViewGenerationMode _typesGenerationMode;
        private List<LeftCellWrapper> _usedCells = new List<LeftCellWrapper>();
        private HashSet<FragmentQuery> _usedViews = new HashSet<FragmentQuery>();
        private List<Tile<FragmentQuery>> _views = new List<Tile<FragmentQuery>>();
        private ViewTarget _viewTarget;
        private MetadataWorkspace _workspace;
        private static Tile<FragmentQuery> TrueViewSurrogate = CreateTile(FragmentQuery.Create(BoolExpression.True));

        internal QueryRewriter(EdmType generatedType, CellNormalizer normalizer, ViewGenerationMode typesGenerationMode)
        {
            this._typesGenerationMode = typesGenerationMode;
            this._normalizer = normalizer;
            this._generatedType = generatedType;
            this._workspace = normalizer.Workspace;
            this._viewTarget = normalizer.SchemaContext.ViewTarget;
            this._domainMap = normalizer.MemberMaps.LeftDomainMap;
            this._config = normalizer.Config;
            this._identifiers = normalizer.CqlIdentifiers;
            this._qp = new RewritingProcessor<Tile<FragmentQuery>>(new DefaultTileProcessor<FragmentQuery>(normalizer.LeftFragmentQP));
            this._extentPath = new MemberPath(normalizer.Extent, this._workspace);
            this._keyAttributes = new List<MemberPath>(MemberPath.GetKeyMembers(normalizer.Extent, this._domainMap, this._workspace));
            foreach (LeftCellWrapper wrapper in this._normalizer.AllWrappersForExtent)
            {
                FragmentQuery fragmentQuery = wrapper.FragmentQuery;
                Tile<FragmentQuery> item = CreateTile(fragmentQuery);
                this._fragmentQueries.Add(fragmentQuery);
                this._views.Add(item);
            }
            this.AdjustMemberDomainsForUpdateViews();
            this._domainQuery = this.GetDomainQuery(this.FragmentQueries, generatedType);
            this._usedViews = new HashSet<FragmentQuery>();
        }

        private void AddElseDefaultToCaseStatement(MemberPath currentPath, CaseStatement caseStatement, List<CellConstant> domain, CellTreeNode rightDomainQuery, Tile<FragmentQuery> unionCaseRewriting)
        {
            CellConstant constant;
            bool flag = CellConstantDomain.TryGetDefaultValueForMemberPath(currentPath, out constant);
            if (!flag || !domain.Contains(constant))
            {
                CellTreeNode node = TileToCellTree(unionCaseRewriting, this._normalizer);
                FragmentQuery query = this._normalizer.RightFragmentQP.Difference(rightDomainQuery.RightFragmentQuery, node.RightFragmentQuery);
                if (this._normalizer.RightFragmentQP.IsSatisfiable(query))
                {
                    if (flag)
                    {
                        caseStatement.AddWhenThen(BoolExpression.True, new ConstantSlot(constant));
                    }
                    else
                    {
                        query.Condition.ExpensiveSimplify();
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine(System.Data.Entity.Strings.ViewGen_No_Default_Value_For_Configuration_0(currentPath.PathToString(false)));
                        RewritingValidator.EntityConfigurationToUserString(query.Condition, builder);
                        this._errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.NoDefaultValue, builder.ToString(), this._normalizer.AllWrappersForExtent, string.Empty));
                    }
                }
            }
        }

        private bool AddRewritingToCaseStatement(Tile<FragmentQuery> rewriting, CaseStatement caseStatement, MemberPath currentPath, CellConstant domainValue)
        {
            ProjectedSlot slot;
            BoolExpression @true = BoolExpression.True;
            bool flag = this._qp.IsContainedIn(CreateTile(this._domainQuery), rewriting);
            if (this._qp.IsDisjointFrom(CreateTile(this._domainQuery), rewriting))
            {
                return false;
            }
            if (domainValue.HasNotNull())
            {
                slot = this.CreateSlot(currentPath);
            }
            else
            {
                slot = new ConstantSlot(domainValue);
            }
            if (!flag)
            {
                @true = TileToBoolExpr(rewriting);
            }
            else
            {
                @true = BoolExpression.True;
            }
            caseStatement.AddWhenThen(@true, slot);
            return flag;
        }

        internal void AddTrivialCaseStatementsForConditionMembers()
        {
            for (int i = 0; i < this._normalizer.MemberMaps.ProjectedSlotMap.Count; i++)
            {
                MemberPath key = this._normalizer.MemberMaps.ProjectedSlotMap[i];
                if (!key.IsScalarType() && !this._caseStatements.ContainsKey(key))
                {
                    CellConstant constant = new TypeConstant(key.EdmType);
                    CaseStatement statement = new CaseStatement(key);
                    statement.AddWhenThen(BoolExpression.True, new ConstantSlot(constant));
                    this._caseStatements[key] = statement;
                }
            }
        }

        private void AddUnrecoverableAttributesError(IEnumerable<MemberPath> attributes, BoolExpression domainAddedWhereClause, ErrorLog errorLog)
        {
            StringBuilder builder = new StringBuilder();
            string str = StringUtil.FormatInvariant("{0}", new object[] { this._extentPath });
            string str2 = System.Data.Entity.Strings.ViewGen_Extent;
            string str3 = StringUtil.ToCommaSeparatedString(GetTypeBasedMemberPathList(attributes));
            builder.AppendLine(System.Data.Entity.Strings.ViewGen_Cannot_Recover_Attributes_2(str3, str2, str));
            RewritingValidator.EntityConfigurationToUserString(domainAddedWhereClause, builder);
            ErrorLog.Record record = new ErrorLog.Record(true, ViewGenErrorCode.AttributesUnrecoverable, builder.ToString(), this._normalizer.AllWrappersForExtent, string.Empty);
            errorLog.AddEntry(record);
        }

        private void AdjustMemberDomainsForUpdateViews()
        {
            if (this._viewTarget == ViewTarget.UpdateView)
            {
                List<MemberPath> list = new List<MemberPath>(this._domainMap.ConditionMembers(this._extentPath.Extent));
                using (List<MemberPath>.Enumerator enumerator = list.GetEnumerator())
                {
                    Func<CellConstant, bool> predicate = null;
                    MemberPath currentPath;
                    while (enumerator.MoveNext())
                    {
                        currentPath = enumerator.Current;
                        if (predicate == null)
                        {
                            predicate = domainValue => IsDefaultValue(domainValue, currentPath);
                        }
                        CellConstant constant = this._domainMap.GetDomain(currentPath).FirstOrDefault<CellConstant>(predicate);
                        if (constant != null)
                        {
                            this.RemoveUnusedValueFromStoreDomain(constant, currentPath);
                        }
                        CellConstant constant2 = this._domainMap.GetDomain(currentPath).FirstOrDefault<CellConstant>(domainValue => domainValue is NegatedCellConstant);
                        if (constant2 != null)
                        {
                            this.RemoveUnusedValueFromStoreDomain(constant2, currentPath);
                        }
                    }
                }
            }
        }

        private bool CoverAttribute(MemberPath projectedAttribute, FragmentQuery view, Dictionary<MemberPath, FragmentQuery> attributeConditions, FragmentQuery toFillQuery)
        {
            FragmentQuery query;
            if (!attributeConditions.TryGetValue(projectedAttribute, out query))
            {
                return false;
            }
            query = FragmentQuery.Create(BoolExpression.CreateAndNot(query.Condition, view.Condition));
            if (this._qp.IsEmpty(CreateTile(query)))
            {
                attributeConditions.Remove(projectedAttribute);
            }
            else
            {
                attributeConditions[projectedAttribute] = query;
            }
            return true;
        }

        private bool CoverAttributes(ref Tile<FragmentQuery> rewriting, FragmentQuery toFillQuery, Dictionary<MemberPath, FragmentQuery> attributeConditions)
        {
            HashSet<FragmentQuery> set = new HashSet<FragmentQuery>(rewriting.GetNamedQueries());
            foreach (FragmentQuery query in set)
            {
                foreach (MemberPath path in this.NonKeys(query.Attributes))
                {
                    this.CoverAttribute(path, query, attributeConditions, toFillQuery);
                }
                if (attributeConditions.Count == 0)
                {
                    return true;
                }
            }
            Tile<FragmentQuery> a = null;
            foreach (FragmentQuery query2 in this._fragmentQueries)
            {
                foreach (MemberPath path2 in this.NonKeys(query2.Attributes))
                {
                    if (this.CoverAttribute(path2, query2, attributeConditions, toFillQuery))
                    {
                        a = (a == null) ? CreateTile(query2) : this._qp.Union(a, CreateTile(query2));
                    }
                }
                if (attributeConditions.Count == 0)
                {
                    break;
                }
            }
            if (attributeConditions.Count == 0)
            {
                rewriting = this._qp.Join(rewriting, a);
                return true;
            }
            return false;
        }

        private BoolExpression CreateMemberCondition(MemberPath path, CellConstant domainValue) => 
            FragmentQuery.CreateMemberCondition(path, domainValue, this._domainMap, this._workspace);

        private FragmentQuery CreateMemberConditionQuery(MemberPath currentPath, CellConstant domainValue) => 
            CreateMemberConditionQuery(currentPath, domainValue, this._keyAttributes, this._domainMap, this._workspace);

        internal static FragmentQuery CreateMemberConditionQuery(MemberPath currentPath, CellConstant domainValue, IEnumerable<MemberPath> keyAttributes, MemberDomainMap domainMap, MetadataWorkspace workspace)
        {
            BoolExpression whereClause = FragmentQuery.CreateMemberCondition(currentPath, domainValue, domainMap, workspace);
            IEnumerable<MemberPath> attrs = keyAttributes;
            if (domainValue is NegatedCellConstant)
            {
                attrs = keyAttributes.Concat<MemberPath>(new MemberPath[] { currentPath });
            }
            return FragmentQuery.Create(attrs, whereClause);
        }

        private JoinTreeSlot CreateSlot(MemberPath path) => 
            FragmentQuery.CreateSlot(path, this._workspace);

        private static TileNamed<FragmentQuery> CreateTile(FragmentQuery query) => 
            new TileNamed<FragmentQuery>(query);

        private void EnsureConfigurationIsFullyMapped(MemberPath currentPath, BoolExpression currentWhereClause, HashSet<FragmentQuery> outputUsedViews, ErrorLog errorLog)
        {
            foreach (CellConstant constant in this.GetDomain(currentPath))
            {
                if (constant != CellConstant.Undefined)
                {
                    Tile<FragmentQuery> tile;
                    BoolExpression domainAddedWhereClause = this.CreateMemberCondition(currentPath, constant);
                    BoolExpression whereClause = BoolExpression.CreateAnd(new BoolExpression[] { currentWhereClause, domainAddedWhereClause });
                    if (!this.FindRewritingAndUsedViews(this._keyAttributes, whereClause, outputUsedViews, out tile))
                    {
                        if (!ErrorPatternMatcher.FindMappingErrors(this._normalizer, this._domainMap, this._errorLog))
                        {
                            StringBuilder builder = new StringBuilder();
                            string str = StringUtil.FormatInvariant("{0}", new object[] { this._extentPath });
                            BoolExpression condition = tile.Query.Condition;
                            condition.ExpensiveSimplify();
                            if (condition.RepresentsAllTypeConditions)
                            {
                                string str2 = System.Data.Entity.Strings.ViewGen_Extent;
                                builder.AppendLine(System.Data.Entity.Strings.ViewGen_Cannot_Recover_Types_1(str2, str));
                            }
                            else
                            {
                                string str3 = System.Data.Entity.Strings.ViewGen_Entities;
                                builder.AppendLine(System.Data.Entity.Strings.ViewGen_Cannot_Disambiguate_MultiConstant_2(str3, str));
                            }
                            RewritingValidator.EntityConfigurationToUserString(condition, builder);
                            ErrorLog.Record record = new ErrorLog.Record(true, ViewGenErrorCode.AmbiguousMultiConstants, builder.ToString(), this._normalizer.AllWrappersForExtent, string.Empty);
                            errorLog.AddEntry(record);
                        }
                    }
                    else
                    {
                        TypeConstant constant2 = constant as TypeConstant;
                        if (constant2 != null)
                        {
                            IEnumerable<MemberPath> enumerable;
                            EdmType cdmType = constant2.CdmType;
                            List<MemberPath> attributes = GetNonConditionalScalarMembers(cdmType, currentPath, this._domainMap).Union<MemberPath>(GetNonConditionalComplexMembers(cdmType, currentPath, this._domainMap)).ToList<MemberPath>();
                            if ((attributes.Count > 0) && !this.FindRewritingAndUsedViews(attributes, whereClause, outputUsedViews, out tile, out enumerable))
                            {
                                attributes = new List<MemberPath>(from a in attributes
                                    where !a.IsPartOfKey
                                    select a);
                                this.AddUnrecoverableAttributesError(enumerable, domainAddedWhereClause, errorLog);
                            }
                            else
                            {
                                foreach (MemberPath path in GetConditionalComplexMembers(cdmType, currentPath, this._domainMap))
                                {
                                    this.EnsureConfigurationIsFullyMapped(path, whereClause, outputUsedViews, errorLog);
                                }
                                foreach (MemberPath path2 in GetConditionalScalarMembers(cdmType, currentPath, this._domainMap))
                                {
                                    this.EnsureConfigurationIsFullyMapped(path2, whereClause, outputUsedViews, errorLog);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void EnsureExtentIsFullyMapped(HashSet<FragmentQuery> outputUsedViews)
        {
            switch (this._viewTarget)
            {
                case ViewTarget.QueryView:
                    this.EnsureConfigurationIsFullyMapped(this._extentPath, BoolExpression.True, outputUsedViews, this._errorLog);
                    if (this._errorLog.Count <= 0)
                    {
                        break;
                    }
                    ExceptionHelpers.ThrowMappingException(this._errorLog, this._config);
                    return;

                case ViewTarget.UpdateView:
                    foreach (MemberPath path in this._normalizer.MemberMaps.ProjectedSlotMap.Members)
                    {
                        CellConstant constant;
                        if ((path.IsScalarType() && !path.IsPartOfKey) && (!this._domainMap.IsConditionMember(path) && !CellConstantDomain.TryGetDefaultValueForMemberPath(path, out constant)))
                        {
                            HashSet<MemberPath> attrs = new HashSet<MemberPath>(this._keyAttributes) {
                                path
                            };
                            foreach (LeftCellWrapper wrapper in this._normalizer.AllWrappersForExtent)
                            {
                                Tile<FragmentQuery> tile2;
                                IEnumerable<MemberPath> enumerable;
                                FragmentQuery fragmentQuery = wrapper.FragmentQuery;
                                FragmentQuery query = new FragmentQuery(fragmentQuery.Description, fragmentQuery.FromVariable, attrs, fragmentQuery.Condition);
                                Tile<FragmentQuery> toAvoid = CreateTile(FragmentQuery.Create(this._keyAttributes, BoolExpression.CreateNot(fragmentQuery.Condition)));
                                if (!this.RewriteQuery(CreateTile(query), toAvoid, out tile2, out enumerable, false))
                                {
                                    CellConstantDomain.GetDefaultValueForMemberPath(path, new LeftCellWrapper[] { wrapper }, this._config);
                                }
                            }
                        }
                    }
                    using (List<Tile<FragmentQuery>>.Enumerator enumerator3 = this._views.GetEnumerator())
                    {
                        Func<LeftCellWrapper, bool> predicate = null;
                        Tile<FragmentQuery> toFill;
                        while (enumerator3.MoveNext())
                        {
                            Tile<FragmentQuery> tile3;
                            IEnumerable<MemberPath> enumerable2;
                            toFill = enumerator3.Current;
                            Tile<FragmentQuery> tile4 = CreateTile(FragmentQuery.Create(this._keyAttributes, BoolExpression.CreateNot(toFill.Query.Condition)));
                            if (!this.RewriteQuery(toFill, tile4, out tile3, out enumerable2, true))
                            {
                                if (predicate == null)
                                {
                                    predicate = lcr => lcr.FragmentQuery.Equals(toFill.Query);
                                }
                                LeftCellWrapper wrapper2 = this._normalizer.AllWrappersForExtent.First<LeftCellWrapper>(predicate);
                                ErrorLog.Record record = new ErrorLog.Record(true, ViewGenErrorCode.ImpopssibleCondition, System.Data.Entity.Strings.Viewgen_QV_RewritingNotFound(wrapper2.RightExtent.ToString()), wrapper2.Cells, string.Empty);
                                this._errorLog.AddEntry(record);
                            }
                            else
                            {
                                outputUsedViews.UnionWith(tile3.GetNamedQueries());
                            }
                        }
                    }
                    break;

                default:
                    return;
            }
        }

        private bool FindRewriting(IEnumerable<MemberPath> attributes, BoolExpression whereClause, out Tile<FragmentQuery> rewriting, out IEnumerable<MemberPath> notCoveredAttributes)
        {
            Tile<FragmentQuery> toFill = CreateTile(FragmentQuery.Create(attributes, whereClause));
            Tile<FragmentQuery> toAvoid = CreateTile(FragmentQuery.Create(this._keyAttributes, BoolExpression.CreateNot(whereClause)));
            bool isRelaxed = this._viewTarget == ViewTarget.UpdateView;
            return this.RewriteQuery(toFill, toAvoid, out rewriting, out notCoveredAttributes, isRelaxed);
        }

        private bool FindRewritingAndUsedViews(IEnumerable<MemberPath> attributes, BoolExpression whereClause, HashSet<FragmentQuery> outputUsedViews, out Tile<FragmentQuery> rewriting)
        {
            IEnumerable<MemberPath> enumerable;
            return this.FindRewritingAndUsedViews(attributes, whereClause, outputUsedViews, out rewriting, out enumerable);
        }

        private bool FindRewritingAndUsedViews(IEnumerable<MemberPath> attributes, BoolExpression whereClause, HashSet<FragmentQuery> outputUsedViews, out Tile<FragmentQuery> rewriting, out IEnumerable<MemberPath> notCoveredAttributes)
        {
            if (this.FindRewriting(attributes, whereClause, out rewriting, out notCoveredAttributes))
            {
                outputUsedViews.UnionWith(rewriting.GetNamedQueries());
                return true;
            }
            return false;
        }

        private void GenerateCaseStatements(IEnumerable<MemberPath> members, HashSet<FragmentQuery> outputUsedViews)
        {
            IEnumerable<LeftCellWrapper> enumerable = from w in this._normalizer.AllWrappersForExtent
                where this._usedViews.Contains(w.FragmentQuery)
                select w;
            CellTreeNode rightDomainQuery = new OpCellTreeNode(this._normalizer, CellTreeOpType.Union, (from wrapper in enumerable select new LeafCellTreeNode(this._normalizer, wrapper)).ToArray<LeafCellTreeNode>());
            foreach (MemberPath path in members)
            {
                List<CellConstant> source = this.GetDomain(path).ToList<CellConstant>();
                CaseStatement caseStatement = new CaseStatement(path);
                Tile<FragmentQuery> a = null;
                bool flag = ((source.Count != 2) || !source.Contains<CellConstant>(CellConstant.Null, CellConstant.EqualityComparer)) || !source.Contains<CellConstant>(CellConstant.NotNull, CellConstant.EqualityComparer);
                foreach (CellConstant constant in source)
                {
                    if ((constant == CellConstant.Undefined) && (this._viewTarget == ViewTarget.QueryView))
                    {
                        caseStatement.AddWhenThen(BoolExpression.False, new ConstantSlot(CellConstant.Undefined));
                    }
                    else
                    {
                        Tile<FragmentQuery> tile2;
                        FragmentQuery query = this.CreateMemberConditionQuery(path, constant);
                        if (this.FindRewritingAndUsedViews(query.Attributes, query.Condition, outputUsedViews, out tile2))
                        {
                            if (this._viewTarget == ViewTarget.UpdateView)
                            {
                                a = (a != null) ? this._qp.Union(a, tile2) : tile2;
                            }
                            if (flag && this.AddRewritingToCaseStatement(tile2, caseStatement, path, constant))
                            {
                                break;
                            }
                        }
                        else if (!IsDefaultValue(constant, path) && !ErrorPatternMatcher.FindMappingErrors(this._normalizer, this._domainMap, this._errorLog))
                        {
                            StringBuilder builder = new StringBuilder();
                            string str = StringUtil.FormatInvariant("{0}", new object[] { this._extentPath });
                            string str2 = (this._viewTarget == ViewTarget.QueryView) ? System.Data.Entity.Strings.ViewGen_Entities : System.Data.Entity.Strings.ViewGen_Tuples;
                            builder.AppendLine(EntityRes.GetString("ViewGen_Cannot_Disambiguate_MultiConstant_2", new object[] { str2, str }));
                            RewritingValidator.EntityConfigurationToUserString(query.Condition, builder);
                            ErrorLog.Record record = new ErrorLog.Record(true, ViewGenErrorCode.AmbiguousMultiConstants, builder.ToString(), this._normalizer.AllWrappersForExtent, string.Empty);
                            this._errorLog.AddEntry(record);
                        }
                    }
                }
                if (this._errorLog.Count == 0)
                {
                    if ((this._viewTarget == ViewTarget.UpdateView) && flag)
                    {
                        this.AddElseDefaultToCaseStatement(path, caseStatement, source, rightDomainQuery, a);
                    }
                    if (caseStatement.Clauses.Count > 0)
                    {
                        this._caseStatements[path] = caseStatement;
                    }
                }
            }
        }

        internal void GenerateViewComponents()
        {
            this.EnsureExtentIsFullyMapped(this._usedViews);
            this.GenerateCaseStatements(this._domainMap.ConditionMembers(this._extentPath.Extent), this._usedViews);
            this.AddTrivialCaseStatementsForConditionMembers();
            if ((this._usedViews.Count == 0) || (this._errorLog.Count > 0))
            {
                ExceptionHelpers.ThrowMappingException(this._errorLog, this._config);
            }
            this._topLevelWhereClause = this.GetTopLevelWhereClause(this._usedViews);
            ViewTarget target1 = this._viewTarget;
            this._usedCells = this.RemapFromVariables();
            this._basicView = new BasicViewGenerator(this._normalizer.MemberMaps.ProjectedSlotMap, this._usedCells, this._domainQuery, this._normalizer, this._domainMap, this._errorLog, this._config).CreateViewExpression();
            if (this._normalizer.LeftFragmentQP.IsContainedIn(this._basicView.LeftFragmentQuery, this._domainQuery))
            {
                this._topLevelWhereClause = BoolExpression.True;
            }
            if (this._errorLog.Count > 0)
            {
                ExceptionHelpers.ThrowMappingException(this._errorLog, this._config);
            }
        }

        private static IEnumerable<MemberPath> GetConditionalComplexMembers(EdmType edmType, MemberPath currentPath, MemberDomainMap domainMap) => 
            currentPath.GetMembers(edmType, false, true, null, domainMap);

        private static IEnumerable<MemberPath> GetConditionalScalarMembers(EdmType edmType, MemberPath currentPath, MemberDomainMap domainMap) => 
            currentPath.GetMembers(edmType, true, true, null, domainMap);

        private IEnumerable<CellConstant> GetDomain(MemberPath currentPath)
        {
            IEnumerable<EdmType> enumerable;
            if ((this._viewTarget != ViewTarget.QueryView) || !MemberPath.EqualityComparer.Equals(currentPath, this._extentPath))
            {
                return this._domainMap.GetDomain(currentPath);
            }
            if (this._typesGenerationMode == ViewGenerationMode.OfTypeOnlyViews)
            {
                HashSet<EdmType> set = new HashSet<EdmType> {
                    this._generatedType
                };
                enumerable = set;
            }
            else
            {
                enumerable = MetadataHelper.GetTypeAndSubtypesOf(this._generatedType, this._workspace, false);
            }
            return GetTypeConstants(enumerable);
        }

        internal FragmentQuery GetDomainQuery(IEnumerable<FragmentQuery> fragmentQueries, EdmType generatedType)
        {
            BoolExpression whereClause = null;
            if (this._viewTarget == ViewTarget.QueryView)
            {
                if (generatedType == null)
                {
                    whereClause = BoolExpression.True;
                }
                else
                {
                    IEnumerable<EdmType> enumerable;
                    if (this._typesGenerationMode == ViewGenerationMode.OfTypeOnlyViews)
                    {
                        HashSet<EdmType> set = new HashSet<EdmType> {
                            this._generatedType
                        };
                        enumerable = set;
                    }
                    else
                    {
                        enumerable = MetadataHelper.GetTypeAndSubtypesOf(generatedType, this._workspace, false);
                    }
                    CellConstantDomain domain = new CellConstantDomain(GetTypeConstants(enumerable), this._domainMap.GetDomain(this._extentPath));
                    whereClause = BoolExpression.CreateLiteral(new OneOfTypeConst(this.CreateSlot(this._extentPath), domain), this._domainMap);
                }
                return FragmentQuery.Create(this._keyAttributes, whereClause);
            }
            BoolExpression expression2 = BoolExpression.CreateOr((from fragmentQuery in fragmentQueries select fragmentQuery.Condition).ToArray<BoolExpression>());
            return FragmentQuery.Create(this._keyAttributes, expression2);
        }

        private static IEnumerable<MemberPath> GetNonConditionalComplexMembers(EdmType edmType, MemberPath currentPath, MemberDomainMap domainMap) => 
            currentPath.GetMembers(edmType, false, false, null, domainMap);

        private static IEnumerable<MemberPath> GetNonConditionalScalarMembers(EdmType edmType, MemberPath currentPath, MemberDomainMap domainMap) => 
            currentPath.GetMembers(edmType, true, false, null, domainMap);

        private IEnumerable<Tile<FragmentQuery>> GetRelevantViews(FragmentQuery query, bool isRelaxed)
        {
            System.Data.Common.Utils.Set<MemberPath> variables = this.GetVariables(query);
            Tile<FragmentQuery> a = null;
            List<Tile<FragmentQuery>> list = new List<Tile<FragmentQuery>>();
            Tile<FragmentQuery> item = null;
            foreach (Tile<FragmentQuery> tile3 in this._views)
            {
                if (this.GetVariables(tile3.Query).Overlaps(variables))
                {
                    a = (a == null) ? tile3 : this._qp.Union(a, tile3);
                    list.Add(tile3);
                }
                else if (this.IsTrue(tile3.Query) && (item == null))
                {
                    item = tile3;
                }
            }
            if ((a != null) && this.IsTrue(a.Query))
            {
                return list;
            }
            if (item == null)
            {
                Tile<FragmentQuery> tile4 = null;
                foreach (FragmentQuery query2 in this._fragmentQueries)
                {
                    tile4 = (tile4 == null) ? CreateTile(query2) : this._qp.Union(tile4, CreateTile(query2));
                    if (this.IsTrue(tile4.Query))
                    {
                        item = TrueViewSurrogate;
                        break;
                    }
                }
            }
            if (item != null)
            {
                list.Add(item);
                return list;
            }
            return this._views;
        }

        private BoolExpression GetTopLevelWhereClause(HashSet<FragmentQuery> outputUsedViews)
        {
            Tile<FragmentQuery> tile;
            BoolExpression @true = BoolExpression.True;
            if (((this._viewTarget == ViewTarget.QueryView) && !this._domainQuery.Condition.IsTrue) && this.FindRewritingAndUsedViews(this._keyAttributes, this._domainQuery.Condition, outputUsedViews, out tile))
            {
                @true = TileToBoolExpr(tile);
                @true.ExpensiveSimplify();
            }
            return @true;
        }

        private static List<string> GetTypeBasedMemberPathList(IEnumerable<MemberPath> nonConditionalScalarAttributes)
        {
            List<string> list = new List<string>();
            foreach (MemberPath path in nonConditionalScalarAttributes)
            {
                EdmMember lastMember = path.LastMember;
                list.Add(lastMember.DeclaringType.Name + "." + lastMember);
            }
            return list;
        }

        private static IEnumerable<CellConstant> GetTypeConstants(IEnumerable<EdmType> types)
        {
            foreach (EdmType iteratorVariable0 in types)
            {
                yield return new TypeConstant(iteratorVariable0);
            }
        }

        private HashSet<FragmentQuery> GetUsedViewsAndRemoveTrueSurrogate(ref Tile<FragmentQuery> rewriting)
        {
            HashSet<FragmentQuery> first = new HashSet<FragmentQuery>(rewriting.GetNamedQueries());
            if (first.Contains(TrueViewSurrogate.Query))
            {
                first.Remove(TrueViewSurrogate.Query);
                Tile<FragmentQuery> a = null;
                foreach (FragmentQuery query in first.Concat<FragmentQuery>(this._fragmentQueries))
                {
                    a = (a == null) ? CreateTile(query) : this._qp.Union(a, CreateTile(query));
                    first.Add(query);
                    if (this.IsTrue(a.Query))
                    {
                        rewriting = rewriting.Replace(TrueViewSurrogate, a);
                        return first;
                    }
                }
            }
            return first;
        }

        private System.Data.Common.Utils.Set<MemberPath> GetVariables(FragmentQuery query) => 
            new System.Data.Common.Utils.Set<MemberPath>(from domainConstraint in query.Condition.VariableConstraints
                where (domainConstraint.Variable.Identifier is OneOfConst) && !domainConstraint.Variable.Domain.All<CellConstant>(constant => domainConstraint.Range.Contains(constant))
                select ((OneOfConst) domainConstraint.Variable.Identifier).Slot.MemberPath, MemberPath.EqualityComparer);

        private static bool IsDefaultValue(CellConstant domainValue, MemberPath path)
        {
            if (domainValue.IsNull() && path.IsNullable)
            {
                return true;
            }
            if (path.DefaultValue != null)
            {
                ScalarConstant constant = domainValue as ScalarConstant;
                return (constant.Value == path.DefaultValue);
            }
            return false;
        }

        private bool IsTrue(FragmentQuery query) => 
            !this._normalizer.LeftFragmentQP.IsSatisfiable(FragmentQuery.Create(BoolExpression.CreateNot(query.Condition)));

        private IEnumerable<MemberPath> NonKeys(IEnumerable<MemberPath> attributes) => 
            (from attr in attributes
                where !attr.IsPartOfKey
                select attr);

        [Conditional("DEBUG")]
        private void PrintStatistics(RewritingProcessor<Tile<FragmentQuery>> qp)
        {
            int num;
            int num2;
            int num3;
            int num4;
            int num5;
            qp.GetStatistics(out num, out num2, out num4, out num3, out num5);
        }

        private List<LeftCellWrapper> RemapFromVariables()
        {
            List<LeftCellWrapper> list = new List<LeftCellWrapper>();
            int index = 0;
            Dictionary<BoolLiteral, BoolLiteral> remap = new Dictionary<BoolLiteral, BoolLiteral>(BoolLiteral.EqualityIdentifierComparer);
            foreach (LeftCellWrapper wrapper in this._normalizer.AllWrappersForExtent)
            {
                if (this._usedViews.Contains(wrapper.FragmentQuery))
                {
                    list.Add(wrapper);
                    int cellNumber = wrapper.OnlyInputCell.CellNumber;
                    if (index != cellNumber)
                    {
                        remap[new CellIdBoolean(this._identifiers, cellNumber)] = new CellIdBoolean(this._identifiers, index);
                    }
                    index++;
                }
            }
            if (remap.Count > 0)
            {
                this._topLevelWhereClause = this._topLevelWhereClause.RemapLiterals(remap);
                Dictionary<MemberPath, CaseStatement> dictionary2 = new Dictionary<MemberPath, CaseStatement>();
                foreach (KeyValuePair<MemberPath, CaseStatement> pair in this._caseStatements)
                {
                    CaseStatement statement = new CaseStatement(pair.Key);
                    foreach (CaseStatement.WhenThen then in pair.Value.Clauses)
                    {
                        statement.AddWhenThen(then.Condition.RemapLiterals(remap), then.Value);
                    }
                    dictionary2[pair.Key] = statement;
                }
                this._caseStatements = dictionary2;
            }
            return list;
        }

        private void RemoveUnusedValueFromStoreDomain(CellConstant domainValue, MemberPath currentPath)
        {
            Tile<FragmentQuery> tile;
            BoolExpression whereClause = this.CreateMemberCondition(currentPath, domainValue);
            HashSet<FragmentQuery> outputUsedViews = new HashSet<FragmentQuery>();
            bool flag = false;
            if (this.FindRewritingAndUsedViews(this._keyAttributes, whereClause, outputUsedViews, out tile))
            {
                CellTreeNode n = TileToCellTree(tile, this._normalizer);
                flag = !this._normalizer.IsEmpty(n);
            }
            if (!flag)
            {
                System.Data.Common.Utils.Set<CellConstant> domainValues = new System.Data.Common.Utils.Set<CellConstant>(this._domainMap.GetDomain(currentPath), CellConstant.EqualityComparer);
                domainValues.Remove(domainValue);
                this._domainMap.UpdateConditionMemberDomain(currentPath, domainValues);
                foreach (FragmentQuery query in this._fragmentQueries)
                {
                    query.Condition.FixDomainMap(this._domainMap);
                }
            }
        }

        private bool RewriteQuery(Tile<FragmentQuery> toFill, Tile<FragmentQuery> toAvoid, out Tile<FragmentQuery> rewriting, out IEnumerable<MemberPath> notCoveredAttributes, bool isRelaxed)
        {
            notCoveredAttributes = new List<MemberPath>();
            FragmentQuery query = toFill.Query;
            if (this._normalizer.TryGetCachedRewriting(query, out rewriting))
            {
                return true;
            }
            IEnumerable<Tile<FragmentQuery>> relevantViews = this.GetRelevantViews(query, isRelaxed);
            FragmentQuery query2 = query;
            if (!this.RewriteQueryCached(CreateTile(FragmentQuery.Create(query.Condition)), toAvoid, relevantViews, out rewriting))
            {
                if (!isRelaxed)
                {
                    return false;
                }
                query = FragmentQuery.Create(query.Attributes, BoolExpression.CreateAndNot(query.Condition, rewriting.Query.Condition));
                if (this._qp.IsEmpty(CreateTile(query)) || !this.RewriteQueryCached(CreateTile(FragmentQuery.Create(query.Condition)), toAvoid, relevantViews, out rewriting))
                {
                    return false;
                }
            }
            if (query.Attributes.Count == 0)
            {
                return true;
            }
            Dictionary<MemberPath, FragmentQuery> attributeConditions = new Dictionary<MemberPath, FragmentQuery>();
            foreach (MemberPath path in this.NonKeys(query.Attributes))
            {
                attributeConditions[path] = query;
            }
            if ((attributeConditions.Count == 0) || this.CoverAttributes(ref rewriting, query, attributeConditions))
            {
                this.GetUsedViewsAndRemoveTrueSurrogate(ref rewriting);
                this._normalizer.SetCachedRewriting(query2, rewriting);
                return true;
            }
            if (isRelaxed)
            {
                foreach (MemberPath path2 in this.NonKeys(query.Attributes))
                {
                    FragmentQuery query3;
                    if (attributeConditions.TryGetValue(path2, out query3))
                    {
                        attributeConditions[path2] = FragmentQuery.Create(BoolExpression.CreateAndNot(query.Condition, query3.Condition));
                    }
                    else
                    {
                        attributeConditions[path2] = query;
                    }
                }
                if (this.CoverAttributes(ref rewriting, query, attributeConditions))
                {
                    this.GetUsedViewsAndRemoveTrueSurrogate(ref rewriting);
                    this._normalizer.SetCachedRewriting(query2, rewriting);
                    return true;
                }
            }
            notCoveredAttributes = attributeConditions.Keys;
            return false;
        }

        private bool RewriteQueryCached(Tile<FragmentQuery> toFill, Tile<FragmentQuery> toAvoid, IEnumerable<Tile<FragmentQuery>> views, out Tile<FragmentQuery> rewriting)
        {
            if (this._normalizer.TryGetCachedRewriting(toFill.Query, out rewriting))
            {
                return true;
            }
            bool flag = this._qp.RewriteQuery(toFill, toAvoid, views, out rewriting);
            if (flag)
            {
                this._normalizer.SetCachedRewriting(toFill.Query, rewriting);
            }
            return flag;
        }

        private static BoolExpression TileToBoolExpr(Tile<FragmentQuery> tile)
        {
            switch (tile.OpKind)
            {
                case TileOpKind.Union:
                    return BoolExpression.CreateOr(new BoolExpression[] { TileToBoolExpr(tile.Arg1), TileToBoolExpr(tile.Arg2) });

                case TileOpKind.Join:
                    return BoolExpression.CreateAnd(new BoolExpression[] { TileToBoolExpr(tile.Arg1), TileToBoolExpr(tile.Arg2) });

                case TileOpKind.AntiSemiJoin:
                    return BoolExpression.CreateAnd(new BoolExpression[] { TileToBoolExpr(tile.Arg1), BoolExpression.CreateNot(TileToBoolExpr(tile.Arg2)) });

                case TileOpKind.Named:
                {
                    FragmentQuery namedQuery = ((TileNamed<FragmentQuery>) tile).NamedQuery;
                    if (!namedQuery.Condition.IsAlwaysTrue())
                    {
                        return namedQuery.FromVariable;
                    }
                    return BoolExpression.True;
                }
            }
            return null;
        }

        internal static CellTreeNode TileToCellTree(Tile<FragmentQuery> tile, CellNormalizer normalizer)
        {
            CellTreeOpType union;
            switch (tile.OpKind)
            {
                case TileOpKind.Union:
                    union = CellTreeOpType.Union;
                    break;

                case TileOpKind.Join:
                    union = CellTreeOpType.IJ;
                    break;

                case TileOpKind.AntiSemiJoin:
                    union = CellTreeOpType.LASJ;
                    break;

                case TileOpKind.Named:
                {
                    FragmentQuery view = ((TileNamed<FragmentQuery>) tile).NamedQuery;
                    return new LeafCellTreeNode(normalizer, normalizer.AllWrappersForExtent.First<LeftCellWrapper>(w => w.FragmentQuery == view));
                }
                default:
                    return null;
            }
            return new OpCellTreeNode(normalizer, union, new CellTreeNode[] { TileToCellTree(tile.Arg1, normalizer), TileToCellTree(tile.Arg2, normalizer) });
        }

        [Conditional("DEBUG")]
        internal void TraceVerbose(string msg, params object[] parameters)
        {
            if (this._config.IsVerboseTracing)
            {
                Helpers.FormatTraceLine(msg, parameters);
            }
        }

        internal CellTreeNode BasicView =>
            this._basicView.MakeCopy();

        internal Dictionary<MemberPath, CaseStatement> CaseStatements =>
            this._caseStatements;

        private IEnumerable<FragmentQuery> FragmentQueries =>
            this._fragmentQueries;

        internal CellNormalizer Normalizer =>
            this._normalizer;

        internal BoolExpression TopLevelWhereClause =>
            this._topLevelWhereClause;

        internal List<LeftCellWrapper> UsedCells =>
            this._usedCells;

    }
}

