namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Linq.Expressions;
    using System.Text;

    internal sealed class CoordinatorFactory<TElement> : CoordinatorFactory
    {
        private readonly string Description;
        internal readonly Func<Shaper, TElement> Element;
        internal readonly Func<Shaper, TElement> ElementWithErrorHandling;

        public CoordinatorFactory(int depth, int stateSlot, Expression hasData, Expression setKeys, Expression checkKeys, CoordinatorFactory[] nestedCoordinators, Expression element, Expression elementWithErrorHandling, RecordStateFactory[] recordStateFactories) : base(depth, stateSlot, CoordinatorFactory<TElement>.CompilePredicate(hasData), CoordinatorFactory<TElement>.CompilePredicate(setKeys), CoordinatorFactory<TElement>.CompilePredicate(checkKeys), nestedCoordinators, recordStateFactories)
        {
            this.Element = Translator.Compile<TElement>(element);
            this.ElementWithErrorHandling = Translator.Compile<TElement>(elementWithErrorHandling);
            this.Description = new StringBuilder().Append("HasData: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression(hasData)).Append("SetKeys: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression(setKeys)).Append("CheckKeys: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression(checkKeys)).Append("Element: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression(element)).Append("ElementWithExceptionHandling: ").AppendLine(CoordinatorFactory<TElement>.DescribeExpression(elementWithErrorHandling)).ToString();
        }

        private static Func<Shaper, bool> CompilePredicate(Expression predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            return Translator.Compile<bool>(predicate);
        }

        internal override Coordinator CreateCoordinator(Coordinator parent, Coordinator next) => 
            new Coordinator<TElement>((CoordinatorFactory<TElement>) this, parent, next);

        private static string DescribeExpression(Expression expression) => 
            expression?.ToString();

        internal RecordState GetDefaultRecordState(Shaper<RecordState> shaper)
        {
            RecordState state = null;
            if (base.RecordStateFactories.Count > 0)
            {
                state = (RecordState) shaper.State[base.RecordStateFactories[0].StateSlotNumber];
                state.ResetToDefaultState();
            }
            return state;
        }

        public override string ToString() => 
            this.Description;
    }
}

