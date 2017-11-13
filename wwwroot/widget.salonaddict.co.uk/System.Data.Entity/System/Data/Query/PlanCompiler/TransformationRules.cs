namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Query.InternalTrees;

    internal static class TransformationRules
    {
        private static List<Rule> allRules;
        internal static readonly ReadOnlyCollection<ReadOnlyCollection<Rule>> AllRulesTable = BuildLookupTableForRules(AllRules);
        private static List<Rule> keyInfoDependentRules;
        internal static readonly ReadOnlyCollection<ReadOnlyCollection<Rule>> KeyInfoDependentRulesTable = BuildLookupTableForRules(KeyInfoDependentRules);
        internal static readonly ReadOnlyCollection<ReadOnlyCollection<Rule>> ProjectRulesTable = BuildLookupTableForRules(ProjectOpRules.Rules);
        internal static readonly HashSet<Rule> RulesRequiringProjectionPruning = InitializeRulesRequiringProjectionPruning();

        internal static void ApplyAllRules(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            ApplyRules(compilerState, AllRulesTable);
        }

        internal static void ApplyKeyInfoDependentRules(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            ApplyRules(compilerState, KeyInfoDependentRulesTable);
        }

        internal static void ApplyProjectRules(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            ApplyRules(compilerState, ProjectRulesTable);
        }

        private static void ApplyRules(System.Data.Query.PlanCompiler.PlanCompiler compilerState, ReadOnlyCollection<ReadOnlyCollection<Rule>> rulesTable)
        {
            RuleProcessor processor = new RuleProcessor();
            TransformationRulesContext context = new TransformationRulesContext(compilerState);
            compilerState.Command.Root = processor.ApplyRulesToSubtree(context, rulesTable, compilerState.Command.Root);
        }

        private static ReadOnlyCollection<ReadOnlyCollection<Rule>> BuildLookupTableForRules(IEnumerable<Rule> rules)
        {
            ReadOnlyCollection<Rule> onlys = new ReadOnlyCollection<Rule>(new Rule[0]);
            List<Rule>[] listArray = new List<Rule>[70];
            foreach (Rule rule in rules)
            {
                List<Rule> list = listArray[(int) rule.RuleOpType];
                if (list == null)
                {
                    list = new List<Rule>();
                    listArray[(int) rule.RuleOpType] = list;
                }
                list.Add(rule);
            }
            ReadOnlyCollection<Rule>[] onlysArray = new ReadOnlyCollection<Rule>[listArray.Length];
            for (int i = 0; i < listArray.Length; i++)
            {
                if (listArray[i] != null)
                {
                    onlysArray[i] = new ReadOnlyCollection<Rule>(listArray[i].ToArray());
                }
                else
                {
                    onlysArray[i] = onlys;
                }
            }
            return new ReadOnlyCollection<ReadOnlyCollection<Rule>>(onlysArray);
        }

        private static HashSet<Rule> InitializeRulesRequiringProjectionPruning() => 
            new HashSet<Rule> { 
                ApplyOpRules.Rule_OuterApplyOverProject,
                JoinOpRules.Rule_CrossJoinOverProject1,
                JoinOpRules.Rule_CrossJoinOverProject2,
                JoinOpRules.Rule_InnerJoinOverProject1,
                JoinOpRules.Rule_InnerJoinOverProject2,
                JoinOpRules.Rule_OuterJoinOverProject2,
                ProjectOpRules.Rule_ProjectWithNoLocalDefs,
                FilterOpRules.Rule_FilterOverProject,
                FilterOpRules.Rule_FilterWithConstantPredicate
            };

        private static List<Rule> AllRules
        {
            get
            {
                if (allRules == null)
                {
                    allRules = new List<Rule>();
                    allRules.AddRange(ScalarOpRules.Rules);
                    allRules.AddRange(FilterOpRules.Rules);
                    allRules.AddRange(ProjectOpRules.Rules);
                    allRules.AddRange(ApplyOpRules.Rules);
                    allRules.AddRange(JoinOpRules.Rules);
                    allRules.AddRange(SingleRowOpRules.Rules);
                    allRules.AddRange(SetOpRules.Rules);
                    allRules.AddRange(GroupByOpRules.Rules);
                    allRules.AddRange(SortOpRules.Rules);
                    allRules.AddRange(ConstrainedSortOpRules.Rules);
                    allRules.AddRange(DistinctOpRules.Rules);
                }
                return allRules;
            }
        }

        private static List<Rule> KeyInfoDependentRules
        {
            get
            {
                if (keyInfoDependentRules == null)
                {
                    keyInfoDependentRules = new List<Rule>();
                    keyInfoDependentRules.AddRange(ProjectOpRules.Rules);
                    keyInfoDependentRules.AddRange(DistinctOpRules.Rules);
                }
                return keyInfoDependentRules;
            }
        }
    }
}

