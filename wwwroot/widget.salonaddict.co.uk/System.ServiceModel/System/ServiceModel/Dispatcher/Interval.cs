namespace System.ServiceModel.Dispatcher
{
    using System;

    internal class Interval
    {
        private QueryBranch branch;
        private double lowerBound = double.MinValue;
        private IntervalOp lowerOp = IntervalOp.LessThanEquals;
        private double upperBound = double.MaxValue;
        private IntervalOp upperOp = IntervalOp.LessThanEquals;

        internal Interval(double literal, RelationOperator op)
        {
            switch (op)
            {
                case RelationOperator.Gt:
                    this.lowerBound = literal;
                    this.lowerOp = IntervalOp.LessThan;
                    return;

                case RelationOperator.Ge:
                    this.lowerBound = literal;
                    return;

                case RelationOperator.Lt:
                    this.upperBound = literal;
                    this.upperOp = IntervalOp.LessThan;
                    return;

                case RelationOperator.Le:
                    this.upperBound = literal;
                    return;
            }
        }

        internal bool Equals(double lowerBound, IntervalOp lowerOp, double upperBound, IntervalOp upperOp) => 
            ((((this.lowerBound == lowerBound) && (this.lowerOp == lowerOp)) && (this.upperBound == upperBound)) && (this.upperOp == upperOp));

        internal bool HasMatchingEndPoint(double endpoint)
        {
            if (this.lowerBound != endpoint)
            {
                return (this.upperBound == endpoint);
            }
            return true;
        }

        internal QueryBranch Branch
        {
            get => 
                this.branch;
            set
            {
                this.branch = value;
            }
        }

        internal double LowerBound =>
            this.lowerBound;

        internal IntervalOp LowerOp =>
            this.lowerOp;

        internal double UpperBound =>
            this.upperBound;

        internal IntervalOp UpperOp =>
            this.upperOp;
    }
}

