namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Text;

    internal class ErrorPatternMatcher
    {
        private MemberDomainMap m_domainMap;
        private ErrorLog m_errorLog;
        private IEnumerable<MemberPath> m_keyAttributes;
        private CellNormalizer m_normalizer;
        private int m_originalErrorCount;
        private const int NUM_PARTITION_ERR_TO_FIND = 5;

        private ErrorPatternMatcher(CellNormalizer normalizer, MemberDomainMap domainMap, ErrorLog errorLog)
        {
            this.m_normalizer = normalizer;
            this.m_domainMap = domainMap;
            this.m_keyAttributes = MemberPath.GetKeyMembers(normalizer.Extent, domainMap, normalizer.Workspace);
            this.m_errorLog = errorLog;
            this.m_originalErrorCount = this.m_errorLog.Count;
        }

        private string BuildCommaSeparatedErrorString<T>(IEnumerable<T> members)
        {
            StringBuilder builder = new StringBuilder();
            T local = members.First<T>();
            foreach (T local2 in members)
            {
                if (!local2.Equals(local))
                {
                    builder.Append(", ");
                }
                builder.Append("'" + local2.ToString() + "'");
            }
            return builder.ToString();
        }

        private void CheckConditionMemberIsNotMapped(MemberPath conditionMember, List<LeftCellWrapper> mappingFragments, System.Data.Common.Utils.Set<MemberPath> mappedConditionMembers)
        {
            foreach (LeftCellWrapper wrapper in mappingFragments)
            {
                foreach (Cell cell in wrapper.Cells)
                {
                    if (cell.GetLeftQuery(this.m_normalizer.SchemaContext.ViewTarget).GetProjectedMembers().Contains<MemberPath>(conditionMember))
                    {
                        mappedConditionMembers.Add(conditionMember);
                        this.m_errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.ErrorPatternConditionError, System.Data.Entity.Strings.Viewgen_ErrorPattern_ConditionMemberIsMapped(conditionMember.ToString()), cell, ""));
                    }
                }
            }
        }

        private bool Compare(bool lookingForC, ComparisonOP op, CellNormalizer normalizer, LeftCellWrapper leftWrapper1, LeftCellWrapper leftWrapper2, FragmentQuery rightQuery1, FragmentQuery rightQuery2)
        {
            LCWComparer comparer;
            if ((lookingForC && this.IsQueryView()) || (!lookingForC && !this.IsQueryView()))
            {
                if (op == ComparisonOP.IsContainedIn)
                {
                    comparer = new LCWComparer(normalizer.LeftFragmentQP.IsContainedIn);
                }
                else if (op == ComparisonOP.IsDisjointFrom)
                {
                    comparer = new LCWComparer(normalizer.LeftFragmentQP.IsDisjointFrom);
                }
                else
                {
                    return false;
                }
                return comparer(leftWrapper1.FragmentQuery, leftWrapper2.FragmentQuery);
            }
            if (op == ComparisonOP.IsContainedIn)
            {
                comparer = new LCWComparer(normalizer.RightFragmentQP.IsContainedIn);
            }
            else if (op == ComparisonOP.IsDisjointFrom)
            {
                comparer = new LCWComparer(normalizer.RightFragmentQP.IsDisjointFrom);
            }
            else
            {
                return false;
            }
            return comparer(rightQuery1, rightQuery2);
        }

        private bool CompareC(ComparisonOP op, CellNormalizer normalizer, LeftCellWrapper leftWrapper1, LeftCellWrapper leftWrapper2, FragmentQuery rightQuery1, FragmentQuery rightQuery2) => 
            this.Compare(true, op, normalizer, leftWrapper1, leftWrapper2, rightQuery1, rightQuery2);

        private bool CompareS(ComparisonOP op, CellNormalizer normalizer, LeftCellWrapper leftWrapper1, LeftCellWrapper leftWrapper2, FragmentQuery rightQuery1, FragmentQuery rightQuery2) => 
            this.Compare(false, op, normalizer, leftWrapper1, leftWrapper2, rightQuery1, rightQuery2);

        private FragmentQuery CreateRightFragmentQuery(LeftCellWrapper wrapper) => 
            FragmentQuery.Create(wrapper.OnlyInputCell.CellLabel.ToString(), wrapper.CreateRoleBoolean(), wrapper.OnlyInputCell.GetRightQuery(this.m_normalizer.SchemaContext.ViewTarget));

        private bool CSideHasDifferentEntitySets(LeftCellWrapper a, LeftCellWrapper b)
        {
            if (this.IsQueryView())
            {
                return (a.LeftExtent == b.LeftExtent);
            }
            return (a.RightCellQuery == b.RightCellQuery);
        }

        public static bool FindMappingErrors(CellNormalizer normalizer, MemberDomainMap domainMap, ErrorLog errorLog)
        {
            ErrorPatternMatcher matcher = new ErrorPatternMatcher(normalizer, domainMap, errorLog);
            matcher.MatchMissingMappingErrors();
            matcher.MatchConditionErrors();
            matcher.MatchSplitErrors();
            if (matcher.m_errorLog.Count == matcher.m_originalErrorCount)
            {
                matcher.MatchPartitionErrors();
            }
            if (matcher.m_errorLog.Count > matcher.m_originalErrorCount)
            {
                ExceptionHelpers.ThrowMappingException(matcher.m_errorLog, matcher.m_normalizer.Config);
            }
            return false;
        }

        private bool FoundTooManyErrors() => 
            (this.m_errorLog.Count > (this.m_originalErrorCount + 5));

        private bool IsQueryView() => 
            (this.m_normalizer.SchemaContext.ViewTarget == ViewTarget.QueryView);

        private void MatchConditionErrors()
        {
            List<LeftCellWrapper> allWrappersForExtent = this.m_normalizer.AllWrappersForExtent;
            System.Data.Common.Utils.Set<MemberPath> mappedConditionMembers = new System.Data.Common.Utils.Set<MemberPath>();
            System.Data.Common.Utils.Set<Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>>> set2 = new System.Data.Common.Utils.Set<Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>>>(new ConditionComparer());
            Dictionary<Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>>, LeftCellWrapper> dictionary = new Dictionary<Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>>, LeftCellWrapper>(new ConditionComparer());
            foreach (LeftCellWrapper wrapper in allWrappersForExtent)
            {
                Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>> element = new Dictionary<MemberPath, System.Data.Common.Utils.Set<CellConstant>>();
                foreach (OneOfConst @const in wrapper.OnlyInputCell.GetLeftQuery(this.m_normalizer.SchemaContext.ViewTarget).GetConjunctsFromWhereClause())
                {
                    MemberPath memberPath = @const.Slot.MemberPath;
                    if (this.m_domainMap.IsConditionMember(memberPath))
                    {
                        OneOfScalarConst const2 = @const as OneOfScalarConst;
                        if ((((const2 != null) && !mappedConditionMembers.Contains(memberPath)) && (!const2.Values.Contains(CellConstant.NotNull) && !const2.Values.Contains(CellConstant.Null))) && !wrapper.OnlyInputCell.CQuery.WhereClause.Equals(wrapper.OnlyInputCell.SQuery.WhereClause))
                        {
                            this.CheckConditionMemberIsNotMapped(memberPath, allWrappersForExtent, mappedConditionMembers);
                        }
                        foreach (CellConstant constant in @const.Values.Values)
                        {
                            System.Data.Common.Utils.Set<CellConstant> set3;
                            if (!element.TryGetValue(memberPath, out set3))
                            {
                                set3 = new System.Data.Common.Utils.Set<CellConstant>(CellConstant.EqualityComparer);
                                element.Add(memberPath, set3);
                            }
                            set3.Add(constant);
                        }
                    }
                }
                if (element.Count > 0)
                {
                    if (set2.Contains(element))
                    {
                        if (!this.RightSideEqual(dictionary[element], wrapper))
                        {
                            this.m_errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.ErrorPatternConditionError, System.Data.Entity.Strings.Viewgen_ErrorPattern_DuplicateConditionValue(this.BuildCommaSeparatedErrorString<MemberPath>(element.Keys)), this.ToIEnum(dictionary[element].OnlyInputCell, wrapper.OnlyInputCell), ""));
                        }
                    }
                    else
                    {
                        set2.Add(element);
                        dictionary.Add(element, wrapper);
                    }
                }
            }
        }

        private void MatchMissingMappingErrors()
        {
            if (this.m_normalizer.SchemaContext.ViewTarget == ViewTarget.QueryView)
            {
                System.Data.Common.Utils.Set<EdmType> members = new System.Data.Common.Utils.Set<EdmType>(MetadataHelper.GetTypeAndSubtypesOf(this.m_normalizer.Extent.ElementType, this.m_normalizer.Workspace, false));
                foreach (LeftCellWrapper wrapper in this.m_normalizer.AllWrappersForExtent)
                {
                    foreach (Cell cell in wrapper.Cells)
                    {
                        foreach (OneOfConst @const in cell.CQuery.GetConjunctsFromOriginalWhereClause())
                        {
                            foreach (CellConstant constant in @const.Values.Values)
                            {
                                TypeConstant constant2 = constant as TypeConstant;
                                if (constant2 != null)
                                {
                                    members.Remove(constant2.CdmType);
                                }
                            }
                        }
                    }
                }
                if (members.Count > 0)
                {
                    this.m_errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.ErrorPatternMissingMappingError, System.Data.Entity.Strings.ViewGen_Missing_Type_Mapping_0(this.BuildCommaSeparatedErrorString<EdmType>(members)), this.m_normalizer.AllWrappersForExtent, ""));
                }
            }
        }

        private void MatchPartitionErrors()
        {
            List<LeftCellWrapper> allWrappersForExtent = this.m_normalizer.AllWrappersForExtent;
            int num = 0;
            foreach (LeftCellWrapper wrapper in allWrappersForExtent)
            {
                foreach (LeftCellWrapper wrapper2 in allWrappersForExtent.Skip<LeftCellWrapper>(++num))
                {
                    bool flag3;
                    bool flag4;
                    bool flag8;
                    FragmentQuery query = this.CreateRightFragmentQuery(wrapper);
                    FragmentQuery query2 = this.CreateRightFragmentQuery(wrapper2);
                    bool flag = this.CompareS(ComparisonOP.IsDisjointFrom, this.m_normalizer, wrapper, wrapper2, query, query2);
                    bool flag2 = this.CompareC(ComparisonOP.IsDisjointFrom, this.m_normalizer, wrapper, wrapper2, query, query2);
                    if (flag)
                    {
                        if (flag2)
                        {
                            continue;
                        }
                        flag3 = this.CompareC(ComparisonOP.IsContainedIn, this.m_normalizer, wrapper, wrapper2, query, query2);
                        flag4 = this.CompareC(ComparisonOP.IsContainedIn, this.m_normalizer, wrapper2, wrapper, query2, query);
                        flag8 = flag3 && flag4;
                        StringBuilder builder = new StringBuilder();
                        if (flag8)
                        {
                            builder.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Disj_Eq);
                        }
                        else if (flag3 || flag4)
                        {
                            if (this.CSideHasDifferentEntitySets(wrapper, wrapper2))
                            {
                                builder.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Disj_Subs_Ref);
                            }
                            else
                            {
                                builder.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Disj_Subs);
                            }
                        }
                        else
                        {
                            builder.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Disj_Unk);
                        }
                        this.m_errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.ErrorPatternInvalidPartitionError, builder.ToString(), this.ToIEnum(wrapper.OnlyInputCell, wrapper2.OnlyInputCell), ""));
                        if (this.FoundTooManyErrors())
                        {
                            break;
                        }
                    }
                    else
                    {
                        flag3 = this.CompareC(ComparisonOP.IsContainedIn, this.m_normalizer, wrapper, wrapper2, query, query2);
                        flag4 = this.CompareC(ComparisonOP.IsContainedIn, this.m_normalizer, wrapper2, wrapper, query2, query);
                    }
                    bool flag5 = this.CompareS(ComparisonOP.IsContainedIn, this.m_normalizer, wrapper, wrapper2, query, query2);
                    bool flag6 = this.CompareS(ComparisonOP.IsContainedIn, this.m_normalizer, wrapper2, wrapper, query2, query);
                    flag8 = flag3 && flag4;
                    if (flag5 && flag6)
                    {
                        if (flag8)
                        {
                            continue;
                        }
                        StringBuilder builder2 = new StringBuilder();
                        if (flag2)
                        {
                            builder2.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Eq_Disj);
                        }
                        else if (flag3 || flag4)
                        {
                            if (this.CSideHasDifferentEntitySets(wrapper, wrapper2))
                            {
                                builder2.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Eq_Subs_Ref);
                            }
                            else
                            {
                                builder2.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Eq_Subs);
                            }
                        }
                        else if (!this.IsQueryView() && ((wrapper.OnlyInputCell.CQuery.Extent is AssociationSet) || (wrapper2.OnlyInputCell.CQuery.Extent is AssociationSet)))
                        {
                            builder2.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Eq_Unk_Ass);
                        }
                        else
                        {
                            builder2.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Eq_Unk);
                        }
                        this.m_errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.ErrorPatternInvalidPartitionError, builder2.ToString(), this.ToIEnum(wrapper.OnlyInputCell, wrapper2.OnlyInputCell), ""));
                        if (!this.FoundTooManyErrors())
                        {
                            continue;
                        }
                        break;
                    }
                    if (((flag5 || flag6) && ((!flag5 || !flag3) || flag4)) && ((!flag6 || !flag4) || flag3))
                    {
                        StringBuilder builder3 = new StringBuilder();
                        if (flag2)
                        {
                            builder3.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Sub_Disj);
                        }
                        else if (flag8)
                        {
                            if (this.CSideHasDifferentEntitySets(wrapper, wrapper2))
                            {
                                builder3.Append(" " + System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Sub_Eq_Ref);
                            }
                            else
                            {
                                builder3.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Sub_Eq);
                            }
                        }
                        else
                        {
                            builder3.Append(System.Data.Entity.Strings.Viewgen_ErrorPattern_Partition_Sub_Unk);
                        }
                        this.m_errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.ErrorPatternInvalidPartitionError, builder3.ToString(), this.ToIEnum(wrapper.OnlyInputCell, wrapper2.OnlyInputCell), ""));
                        if (this.FoundTooManyErrors())
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void MatchSplitErrors()
        {
            IEnumerable<LeftCellWrapper> source = from r in this.m_normalizer.AllWrappersForExtent
                where !(r.LeftExtent is AssociationSet) && !(r.RightCellQuery.Extent is AssociationSet)
                select r;
            if ((this.m_normalizer.SchemaContext.ViewTarget == ViewTarget.UpdateView) && source.Any<LeftCellWrapper>())
            {
                LeftCellWrapper wrapper = source.First<LeftCellWrapper>();
                EntitySetBase extent = wrapper.RightCellQuery.Extent;
                foreach (LeftCellWrapper wrapper2 in source)
                {
                    if (!wrapper2.RightCellQuery.Extent.EdmEquals(extent) && !this.RightSideEqual(wrapper2, wrapper))
                    {
                        this.m_errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.ErrorPatternSplittingError, System.Data.Entity.Strings.Viewgen_ErrorPattern_TableMappedToMultipleES(wrapper2.LeftExtent.ToString(), wrapper2.RightCellQuery.Extent.ToString(), extent.ToString()), wrapper2.Cells.First<Cell>(), ""));
                    }
                }
            }
        }

        private bool RightSideEqual(LeftCellWrapper wrapper1, LeftCellWrapper wrapper2)
        {
            FragmentQuery query = this.CreateRightFragmentQuery(wrapper1);
            FragmentQuery query2 = this.CreateRightFragmentQuery(wrapper2);
            return this.m_normalizer.RightFragmentQP.IsEquivalentTo(query, query2);
        }

        private IEnumerable<Cell> ToIEnum(Cell one, Cell two) => 
            new List<Cell> { 
                one,
                two
            };

        private enum ComparisonOP
        {
            IsContainedIn,
            IsDisjointFrom
        }
    }
}

