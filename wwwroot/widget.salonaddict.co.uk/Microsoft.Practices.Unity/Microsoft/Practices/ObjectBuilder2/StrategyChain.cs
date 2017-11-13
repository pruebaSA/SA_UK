namespace Microsoft.Practices.ObjectBuilder2
{
    using Microsoft.Practices.Unity.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class StrategyChain : IStrategyChain, IEnumerable<IBuilderStrategy>, IEnumerable
    {
        private readonly List<IBuilderStrategy> strategies;

        public StrategyChain()
        {
            this.strategies = new List<IBuilderStrategy>();
        }

        public StrategyChain(IEnumerable strategies)
        {
            this.strategies = new List<IBuilderStrategy>();
            this.AddRange(strategies);
        }

        public void Add(IBuilderStrategy strategy)
        {
            this.strategies.Add(strategy);
        }

        public void AddRange(IEnumerable strategyEnumerable)
        {
            Guard.ArgumentNotNull(strategyEnumerable, "strategyEnumerable");
            foreach (IBuilderStrategy strategy in strategyEnumerable)
            {
                this.Add(strategy);
            }
        }

        public object ExecuteBuildUp(IBuilderContext context)
        {
            object existing;
            int num = 0;
            try
            {
                while (num < this.strategies.Count)
                {
                    if (context.BuildComplete)
                    {
                        break;
                    }
                    this.strategies[num].PreBuildUp(context);
                    num++;
                }
                if (context.BuildComplete)
                {
                    num--;
                }
                num--;
                while (num >= 0)
                {
                    this.strategies[num].PostBuildUp(context);
                    num--;
                }
                existing = context.Existing;
            }
            catch (Exception)
            {
                context.RecoveryStack.ExecuteRecovery();
                throw;
            }
            return existing;
        }

        public void ExecuteTearDown(IBuilderContext context)
        {
            int num = 0;
            try
            {
                while (num < this.strategies.Count)
                {
                    if (context.BuildComplete)
                    {
                        num--;
                        break;
                    }
                    this.strategies[num].PreTearDown(context);
                    num++;
                }
                num--;
                while (num >= 0)
                {
                    this.strategies[num].PostTearDown(context);
                    num--;
                }
            }
            catch (Exception)
            {
                context.RecoveryStack.ExecuteRecovery();
                throw;
            }
        }

        public IEnumerator GetEnumerator() => 
            this.strategies.GetEnumerator();

        public IStrategyChain Reverse()
        {
            List<IBuilderStrategy> strategies = new List<IBuilderStrategy>(this.strategies);
            strategies.Reverse();
            return new StrategyChain(strategies);
        }

        IEnumerator<IBuilderStrategy> IEnumerable<IBuilderStrategy>.GetEnumerator() => 
            this.strategies.GetEnumerator();
    }
}

