namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class CoordinatorScratchpad
    {
        private Expression _checkKeys;
        private int _depth;
        private Expression _element;
        private Type _elementType;
        private readonly Dictionary<Expression, Expression> _expressionWithErrorHandlingMap;
        private Expression _hasData;
        private readonly HashSet<LambdaExpression> _inlineDelegates;
        private readonly List<CoordinatorScratchpad> _nestedCoordinatorScratchpads;
        private CoordinatorScratchpad _parent;
        private List<RecordStateScratchpad> _recordStateScratchpads;
        private Expression _setKeys;
        private int _stateSlotNumber;

        internal CoordinatorScratchpad(Type elementType)
        {
            this._elementType = elementType;
            this._nestedCoordinatorScratchpads = new List<CoordinatorScratchpad>();
            this._expressionWithErrorHandlingMap = new Dictionary<Expression, Expression>();
            this._inlineDelegates = new HashSet<LambdaExpression>();
        }

        internal void AddExpressionWithErrorHandling(Expression expression, Expression expressionWithErrorHandling)
        {
            this._expressionWithErrorHandlingMap[expression] = expressionWithErrorHandling;
        }

        internal void AddInlineDelegate(LambdaExpression expression)
        {
            this._inlineDelegates.Add(expression);
        }

        internal void AddNestedCoordinator(CoordinatorScratchpad nested)
        {
            nested._parent = this;
            this._nestedCoordinatorScratchpads.Add(nested);
        }

        internal CoordinatorFactory Compile()
        {
            RecordStateFactory[] factoryArray;
            if (this._recordStateScratchpads != null)
            {
                factoryArray = new RecordStateFactory[this._recordStateScratchpads.Count];
                for (int j = 0; j < factoryArray.Length; j++)
                {
                    factoryArray[j] = this._recordStateScratchpads[j].Compile();
                }
            }
            else
            {
                factoryArray = new RecordStateFactory[0];
            }
            CoordinatorFactory[] factoryArray2 = new CoordinatorFactory[this._nestedCoordinatorScratchpads.Count];
            for (int i = 0; i < factoryArray2.Length; i++)
            {
                factoryArray2[i] = this._nestedCoordinatorScratchpads[i].Compile();
            }
            Expression expression = new ReplacementExpressionVisitor(null, this._inlineDelegates).Visit(this.Element);
            Expression expression2 = new ReplacementExpressionVisitor(this._expressionWithErrorHandlingMap, this._inlineDelegates).Visit(this.Element);
            return (CoordinatorFactory) Activator.CreateInstance(typeof(CoordinatorFactory<>).MakeGenericType(new Type[] { this._elementType }), new object[] { this.Depth, this.StateSlotNumber, this.HasData, this.SetKeys, this.CheckKeys, factoryArray2, expression, expression2, factoryArray });
        }

        internal RecordStateScratchpad CreateRecordStateScratchpad()
        {
            RecordStateScratchpad item = new RecordStateScratchpad();
            if (this._recordStateScratchpads == null)
            {
                this._recordStateScratchpads = new List<RecordStateScratchpad>();
            }
            this._recordStateScratchpads.Add(item);
            return item;
        }

        internal Expression CheckKeys
        {
            get => 
                this._checkKeys;
            set
            {
                this._checkKeys = value;
            }
        }

        internal int Depth
        {
            get => 
                this._depth;
            set
            {
                this._depth = value;
            }
        }

        internal Expression Element
        {
            get => 
                this._element;
            set
            {
                this._element = value;
            }
        }

        internal Expression HasData
        {
            get => 
                this._hasData;
            set
            {
                this._hasData = value;
            }
        }

        internal CoordinatorScratchpad Parent =>
            this._parent;

        internal Expression SetKeys
        {
            get => 
                this._setKeys;
            set
            {
                this._setKeys = value;
            }
        }

        internal int StateSlotNumber
        {
            get => 
                this._stateSlotNumber;
            set
            {
                this._stateSlotNumber = value;
            }
        }

        private class ReplacementExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
        {
            private readonly HashSet<LambdaExpression> _inlineDelegates;
            private readonly Dictionary<Expression, Expression> _replacementDictionary;

            internal ReplacementExpressionVisitor(Dictionary<Expression, Expression> replacementDictionary, HashSet<LambdaExpression> inlineDelegates)
            {
                this._replacementDictionary = replacementDictionary;
                this._inlineDelegates = inlineDelegates;
            }

            internal override Expression Visit(Expression expression)
            {
                Expression expression3;
                if (expression == null)
                {
                    return expression;
                }
                if ((this._replacementDictionary != null) && this._replacementDictionary.TryGetValue(expression, out expression3))
                {
                    return expression3;
                }
                bool flag = false;
                LambdaExpression item = null;
                if ((expression.NodeType == ExpressionType.Lambda) && (this._inlineDelegates != null))
                {
                    item = (LambdaExpression) expression;
                    flag = this._inlineDelegates.Contains(item);
                }
                if (flag)
                {
                    Expression body = this.Visit(item.Body);
                    return Expression.Constant(Translator.Compile(body.Type, body));
                }
                return base.Visit(expression);
            }
        }
    }
}

