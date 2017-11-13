namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal class PlanCompiler
    {
        private static BooleanSwitch _applyTransformationsRegardlessOfSize = new BooleanSwitch("System.Data.EntityClient.IgnoreOptimizationLimit", "The Entity Framework should try to optimize the query regardless of its size");
        private static int _objectTypeCount;
        private System.Data.Query.InternalTrees.Command m_command;
        private DbCommandTree m_ctree;
        private int m_neededPhases;
        private PlanCompilerPhase m_phase;
        private const int MaxNodeCountForTransformations = 0x186a0;
        internal readonly int ObjectID = Interlocked.Increment(ref _objectTypeCount);
        internal const Bid.ApiGroup PlanCompilerTracePoints = ((Bid.ApiGroup) 0x8000);

        private PlanCompiler(DbCommandTree ctree)
        {
            this.m_ctree = ctree;
        }

        internal static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.AssertionFailed, 0, message);
            }
        }

        private void Compile(out List<ProviderCommandInfo> providerCommands, out ColumnMap resultColumnMap, out int columnCount, out Set<EntitySet> entitySets)
        {
            EntityBid.Trace("<pc.PlanCompiler.Compile|ADV> %d# Compiling Plan for DbCommandTree=%d#\n", this.ObjectID, this.m_ctree.ObjectId);
            this.Initialize();
            this.m_neededPhases = 0x55;
            StructuredTypeInfo typeInfo = null;
            this.SwitchToPhase(PlanCompilerPhase.PreProcessor);
            PreProcessor.Process(this, out typeInfo);
            entitySets = typeInfo.GetEntitySets();
            if (this.IsPhaseNeeded(PlanCompilerPhase.NTE))
            {
                this.SwitchToPhase(PlanCompilerPhase.NTE);
                NominalTypeEliminator.Process(this, typeInfo);
            }
            if (this.IsPhaseNeeded(PlanCompilerPhase.ProjectionPruning))
            {
                this.SwitchToPhase(PlanCompilerPhase.ProjectionPruning);
                ProjectionPruner.Process(this);
            }
            if (this.IsPhaseNeeded(PlanCompilerPhase.NestPullup))
            {
                this.SwitchToPhase(PlanCompilerPhase.NestPullup);
                NestPullup.Process(this);
                this.SwitchToPhase(PlanCompilerPhase.ProjectionPruning);
                ProjectionPruner.Process(this);
            }
            if (this.IsPhaseNeeded(PlanCompilerPhase.Transformations) && this.MayApplyTransformationRules())
            {
                this.MarkPhaseAsNotNeeded(PlanCompilerPhase.ProjectionPruning);
                this.SwitchToPhase(PlanCompilerPhase.Transformations);
                TransformationRules.ApplyAllRules(this);
                if (this.IsPhaseNeeded(PlanCompilerPhase.ProjectionPruning))
                {
                    this.SwitchToPhase(PlanCompilerPhase.ProjectionPruning);
                    ProjectionPruner.Process(this);
                    TransformationRules.ApplyProjectRules(this);
                }
            }
            if (this.IsPhaseNeeded(PlanCompilerPhase.JoinElimination))
            {
                this.MarkPhaseAsNotNeeded(PlanCompilerPhase.Transformations);
                this.SwitchToPhase(PlanCompilerPhase.JoinElimination);
                JoinElimination.Process(this);
                if (this.IsPhaseNeeded(PlanCompilerPhase.Transformations) && this.MayApplyTransformationRules())
                {
                    TransformationRules.ApplyKeyInfoDependentRules(this);
                }
            }
            this.SwitchToPhase(PlanCompilerPhase.CodeGen);
            CodeGen.Process(this, out providerCommands, out resultColumnMap, out columnCount);
            if (EntityBid.TraceOn)
            {
                int num = 0;
                foreach (ProviderCommandInfo info2 in providerCommands)
                {
                    EntityBid.Trace("<pc.PlanCompiler.Compile|ADV> %d# resulting command %d with DbCommandTree=%d#\n", this.ObjectID, num++, info2.CommandTree.ObjectId);
                }
            }
        }

        internal static void Compile(DbCommandTree ctree, out List<ProviderCommandInfo> providerCommands, out ColumnMap resultColumnMap, out int columnCount, out Set<EntitySet> entitySets)
        {
            Assert(ctree != null, "Expected a valid, non-null Command Tree input");
            new System.Data.Query.PlanCompiler.PlanCompiler(ctree).Compile(out providerCommands, out resultColumnMap, out columnCount, out entitySets);
        }

        private void Initialize()
        {
            DbQueryCommandTree ctree = this.m_ctree as DbQueryCommandTree;
            Assert(ctree != null, "Unexpected command tree kind. Only query command tree is supported.");
            this.m_command = ITreeGenerator.Generate(ctree);
            Assert(this.m_command != null, "Unable to generate internal tree from Command Tree");
        }

        internal bool IsPhaseNeeded(PlanCompilerPhase phase) => 
            ((this.m_neededPhases & (((int) 1) << phase)) != 0);

        internal void MarkPhaseAsNeeded(PlanCompilerPhase phase)
        {
            this.m_neededPhases |= ((int) 1) << phase;
        }

        internal void MarkPhaseAsNotNeeded(PlanCompilerPhase phase)
        {
            this.m_neededPhases &= -1 ^ (((int) 1) << phase);
        }

        private bool MayApplyTransformationRules()
        {
            if (!_applyTransformationsRegardlessOfSize.Enabled)
            {
                return (this.m_command.NextNodeId < 0x186a0);
            }
            return true;
        }

        private string SwitchToPhase(PlanCompilerPhase newPhase)
        {
            string str = string.Empty;
            this.m_phase = newPhase;
            if (EntityBid.PlanCompilerOn)
            {
                str = Dump.ToXml(this.m_command);
                EntityBid.PlanCompilerTrace("<pc.PlanCompiler.SwitchToPhase|ADV> %d# phase=%d\n", this.ObjectID, (int) newPhase);
                EntityBid.PlanCompilerPutStr(str);
            }
            return str;
        }

        internal System.Data.Query.InternalTrees.Command Command =>
            this.m_command;

        internal System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this.m_ctree.MetadataWorkspace;
    }
}

