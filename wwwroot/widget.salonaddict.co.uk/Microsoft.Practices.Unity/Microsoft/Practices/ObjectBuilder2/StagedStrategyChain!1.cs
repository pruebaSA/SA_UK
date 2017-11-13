namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class StagedStrategyChain<TStageEnum> : IStagedStrategyChain
    {
        private readonly StagedStrategyChain<TStageEnum> innerChain;
        private readonly object lockObject;
        private readonly List<IBuilderStrategy>[] stages;

        public StagedStrategyChain()
        {
            this.lockObject = new object();
            this.stages = new List<IBuilderStrategy>[StagedStrategyChain<TStageEnum>.NumberOfEnumValues()];
            for (int i = 0; i < this.stages.Length; i++)
            {
                this.stages[i] = new List<IBuilderStrategy>();
            }
        }

        public StagedStrategyChain(StagedStrategyChain<TStageEnum> innerChain) : this()
        {
            this.innerChain = innerChain;
        }

        public void Add(IBuilderStrategy strategy, TStageEnum stage)
        {
            lock (this.lockObject)
            {
                this.stages[Convert.ToInt32(stage)].Add(strategy);
            }
        }

        public void AddNew<TStrategy>(TStageEnum stage) where TStrategy: IBuilderStrategy, new()
        {
            this.Add(new TStrategy(), stage);
        }

        public void Clear()
        {
            lock (this.lockObject)
            {
                foreach (List<IBuilderStrategy> list in this.stages)
                {
                    list.Clear();
                }
            }
        }

        private void FillStrategyChain(StrategyChain chain, int index)
        {
            lock (this.lockObject)
            {
                if (this.innerChain != null)
                {
                    this.innerChain.FillStrategyChain(chain, index);
                }
                chain.AddRange(this.stages[index]);
            }
        }

        public IStrategyChain MakeStrategyChain()
        {
            lock (this.lockObject)
            {
                StrategyChain chain = new StrategyChain();
                for (int i = 0; i < this.stages.Length; i++)
                {
                    this.FillStrategyChain(chain, i);
                }
                return chain;
            }
        }

        private static int NumberOfEnumValues() => 
            typeof(TStageEnum).GetFields(BindingFlags.Public | BindingFlags.Static).Length;
    }
}

