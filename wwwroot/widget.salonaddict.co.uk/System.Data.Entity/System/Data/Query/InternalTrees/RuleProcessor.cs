namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;

    internal class RuleProcessor
    {
        private Dictionary<SubTreeId, SubTreeId> m_processedNodeMap = new Dictionary<SubTreeId, SubTreeId>();

        internal RuleProcessor()
        {
        }

        private static bool ApplyRulesToNode(RuleProcessingContext context, ReadOnlyCollection<ReadOnlyCollection<Rule>> rules, Node currentNode, out Node newNode)
        {
            newNode = currentNode;
            context.PreProcess(currentNode);
            foreach (Rule rule in rules[(int) currentNode.Op.OpType])
            {
                if (rule.Match(currentNode) && rule.Apply(context, currentNode, out newNode))
                {
                    context.PostProcess(newNode, rule);
                    return true;
                }
            }
            context.PostProcess(currentNode, null);
            return false;
        }

        internal Node ApplyRulesToSubtree(RuleProcessingContext context, ReadOnlyCollection<ReadOnlyCollection<Rule>> rules, Node subTreeRoot) => 
            this.ApplyRulesToSubtree(context, rules, subTreeRoot, null, 0);

        private Node ApplyRulesToSubtree(RuleProcessingContext context, ReadOnlyCollection<ReadOnlyCollection<Rule>> rules, Node subTreeRoot, Node parent, int childIndexInParent)
        {
            int num = 0;
            Dictionary<SubTreeId, SubTreeId> dictionary = new Dictionary<SubTreeId, SubTreeId>();
            while (true)
            {
                Node node;
                num++;
                context.PreProcessSubTree(subTreeRoot);
                SubTreeId key = new SubTreeId(context, subTreeRoot, parent, childIndexInParent);
                if (this.m_processedNodeMap.ContainsKey(key))
                {
                    return subTreeRoot;
                }
                if (dictionary.ContainsKey(key))
                {
                    this.m_processedNodeMap[key] = key;
                    return subTreeRoot;
                }
                dictionary[key] = key;
                for (int i = 0; i < subTreeRoot.Children.Count; i++)
                {
                    subTreeRoot.Children[i] = this.ApplyRulesToSubtree(context, rules, subTreeRoot.Children[i], subTreeRoot, i);
                }
                if (!ApplyRulesToNode(context, rules, subTreeRoot, out node))
                {
                    this.m_processedNodeMap[key] = key;
                    return subTreeRoot;
                }
                subTreeRoot = node;
            }
        }
    }
}

