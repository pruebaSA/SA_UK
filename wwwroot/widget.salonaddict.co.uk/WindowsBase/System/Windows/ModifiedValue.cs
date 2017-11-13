namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal class ModifiedValue
    {
        private object _animatedValue;
        private object _baseValue;
        private object _coercedValue;
        private object _expressionValue;

        internal object AnimatedValue
        {
            get => 
                this._animatedValue;
            set
            {
                this._animatedValue = value;
            }
        }

        internal object BaseValue
        {
            get => 
                this._baseValue;
            set
            {
                this._baseValue = value;
            }
        }

        internal object CoercedValue
        {
            get => 
                this._coercedValue;
            set
            {
                this._coercedValue = value;
            }
        }

        internal object ExpressionValue
        {
            get => 
                this._expressionValue;
            set
            {
                this._expressionValue = value;
            }
        }
    }
}

