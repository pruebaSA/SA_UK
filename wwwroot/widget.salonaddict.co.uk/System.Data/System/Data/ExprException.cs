namespace System.Data
{
    using System;
    using System.Globalization;

    internal sealed class ExprException
    {
        private ExprException()
        {
        }

        private static EvaluateException _Eval(string error)
        {
            EvaluateException e = new EvaluateException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static EvaluateException _Eval(string error, Exception innerException)
        {
            EvaluateException e = new EvaluateException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static InvalidExpressionException _Expr(string error)
        {
            InvalidExpressionException e = new InvalidExpressionException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static OverflowException _Overflow(string error)
        {
            OverflowException e = new OverflowException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static SyntaxErrorException _Syntax(string error)
        {
            SyntaxErrorException e = new SyntaxErrorException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        public static Exception AggregateArgument() => 
            _Syntax(Res.GetString("Expr_AggregateArgument"));

        public static Exception AggregateUnbound(string expr) => 
            _Eval(Res.GetString("Expr_AggregateUnbound", new object[] { expr }));

        public static Exception AmbiguousBinop(int op, Type type1, Type type2) => 
            _Eval(Res.GetString("Expr_AmbiguousBinop", new object[] { Operators.ToString(op), type1.ToString(), type2.ToString() }));

        public static Exception ArgumentType(string function, int arg, Type type) => 
            _Eval(Res.GetString("Expr_ArgumentType", new object[] { function, arg.ToString(CultureInfo.InvariantCulture), type.ToString() }));

        public static Exception ArgumentTypeInteger(string function, int arg) => 
            _Eval(Res.GetString("Expr_ArgumentTypeInteger", new object[] { function, arg.ToString(CultureInfo.InvariantCulture) }));

        internal static EvaluateException BindFailure(string relationName) => 
            _Eval(Res.GetString("Expr_BindFailure", new object[] { relationName }));

        public static Exception ComputeNotAggregate(string expr) => 
            _Eval(Res.GetString("Expr_ComputeNotAggregate", new object[] { expr }));

        public static Exception DatatypeConvertion(Type type1, Type type2) => 
            _Eval(Res.GetString("Expr_DatatypeConvertion", new object[] { type1.ToString(), type2.ToString() }));

        public static Exception DatavalueConvertion(object value, Type type, Exception innerException) => 
            _Eval(Res.GetString("Expr_DatavalueConvertion", new object[] { value.ToString(), type.ToString() }), innerException);

        public static Exception EvalNoContext() => 
            _Eval(Res.GetString("Expr_EvalNoContext"));

        public static Exception ExpressionTooComplex() => 
            _Eval(Res.GetString("Expr_ExpressionTooComplex"));

        public static Exception ExpressionUnbound(string expr) => 
            _Eval(Res.GetString("Expr_ExpressionUnbound", new object[] { expr }));

        public static Exception FilterConvertion(string expr) => 
            _Eval(Res.GetString("Expr_FilterConvertion", new object[] { expr }));

        public static Exception FunctionArgumentCount(string name) => 
            _Eval(Res.GetString("Expr_FunctionArgumentCount", new object[] { name }));

        public static Exception FunctionArgumentOutOfRange(string arg, string func) => 
            ExceptionBuilder._ArgumentOutOfRange(arg, Res.GetString("Expr_ArgumentOutofRange", new object[] { func }));

        public static Exception InvalidDate(string date) => 
            _Syntax(Res.GetString("Expr_InvalidDate", new object[] { date }));

        public static Exception InvalidHoursArgument() => 
            _Eval(Res.GetString("Expr_InvalidHoursArgument"));

        public static Exception InvalidIsSyntax() => 
            _Syntax(Res.GetString("Expr_IsSyntax"));

        public static Exception InvalidMinutesArgument() => 
            _Eval(Res.GetString("Expr_InvalidMinutesArgument"));

        public static Exception InvalidName(string name) => 
            _Syntax(Res.GetString("Expr_InvalidName", new object[] { name }));

        public static Exception InvalidNameBracketing(string name) => 
            _Syntax(Res.GetString("Expr_InvalidNameBracketing", new object[] { name }));

        public static Exception InvalidPattern(string pat) => 
            _Eval(Res.GetString("Expr_InvalidPattern", new object[] { pat }));

        public static Exception InvalidString(string str) => 
            _Syntax(Res.GetString("Expr_InvalidString", new object[] { str }));

        public static Exception InvalidTimeZoneRange() => 
            _Eval(Res.GetString("Expr_InvalidTimeZoneRange"));

        public static Exception InvalidType(string typeName) => 
            _Eval(Res.GetString("Expr_InvalidType", new object[] { typeName }));

        public static Exception InvokeArgument() => 
            ExceptionBuilder._Argument(Res.GetString("Expr_InvokeArgument"));

        public static Exception InWithoutList() => 
            _Syntax(Res.GetString("Expr_InWithoutList"));

        public static Exception InWithoutParentheses() => 
            _Syntax(Res.GetString("Expr_InWithoutParentheses"));

        public static Exception LookupArgument() => 
            _Syntax(Res.GetString("Expr_LookupArgument"));

        public static Exception MismatchKindandTimeSpan() => 
            _Eval(Res.GetString("Expr_MismatchKindandTimeSpan"));

        public static Exception MissingOperand(OperatorInfo before) => 
            _Syntax(Res.GetString("Expr_MissingOperand", new object[] { Operators.ToString(before.op) }));

        public static Exception MissingOperandBefore(string op) => 
            _Syntax(Res.GetString("Expr_MissingOperandBefore", new object[] { op }));

        public static Exception MissingOperator(string token) => 
            _Syntax(Res.GetString("Expr_MissingOperand", new object[] { token }));

        public static Exception MissingRightParen() => 
            _Syntax(Res.GetString("Expr_MissingRightParen"));

        public static Exception NonConstantArgument() => 
            _Eval(Res.GetString("Expr_NonConstantArgument"));

        public static Exception NYI(string moreinfo) => 
            _Expr(Res.GetString("Expr_NYI", new object[] { moreinfo }));

        public static Exception Overflow(Type type) => 
            _Overflow(Res.GetString("Expr_Overflow", new object[] { type.Name }));

        public static Exception SyntaxError() => 
            _Syntax(Res.GetString("Expr_Syntax"));

        public static Exception TooManyRightParentheses() => 
            _Syntax(Res.GetString("Expr_TooManyRightParentheses"));

        public static Exception TypeMismatch(string expr) => 
            _Eval(Res.GetString("Expr_TypeMismatch", new object[] { expr }));

        public static Exception TypeMismatchInBinop(int op, Type type1, Type type2) => 
            _Eval(Res.GetString("Expr_TypeMismatchInBinop", new object[] { Operators.ToString(op), type1.ToString(), type2.ToString() }));

        public static Exception UnboundName(string name) => 
            _Eval(Res.GetString("Expr_UnboundName", new object[] { name }));

        public static Exception UndefinedFunction(string name) => 
            _Eval(Res.GetString("Expr_UndefinedFunction", new object[] { name }));

        public static Exception UnknownToken(string token, int position) => 
            _Syntax(Res.GetString("Expr_UnknownToken", new object[] { token, position.ToString(CultureInfo.InvariantCulture) }));

        public static Exception UnknownToken(Tokens tokExpected, Tokens tokCurr, int position) => 
            _Syntax(Res.GetString("Expr_UnknownToken1", new object[] { tokExpected.ToString(), tokCurr.ToString(), position.ToString(CultureInfo.InvariantCulture) }));

        public static Exception UnresolvedRelation(string name, string expr) => 
            _Eval(Res.GetString("Expr_UnresolvedRelation", new object[] { name, expr }));

        public static Exception UnsupportedOperator(int op) => 
            _Eval(Res.GetString("Expr_UnsupportedOperator", new object[] { Operators.ToString(op) }));
    }
}

