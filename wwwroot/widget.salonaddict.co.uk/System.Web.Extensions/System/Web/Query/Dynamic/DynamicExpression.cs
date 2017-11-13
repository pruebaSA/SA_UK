namespace System.Web.Query.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal static class DynamicExpression
    {
        private static readonly Type[] funcTypes = new Type[] { typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), typeof(Func<,,,,>) };

        public static Type CreateClass(IEnumerable<DynamicProperty> properties) => 
            ClassFactory.Instance.GetDynamicClass(properties);

        public static Type CreateClass(params DynamicProperty[] properties) => 
            ClassFactory.Instance.GetDynamicClass(properties);

        public static Type GetFuncType(params Type[] typeArgs)
        {
            if (((typeArgs == null) || (typeArgs.Length < 1)) || (typeArgs.Length > 5))
            {
                throw new ArgumentException();
            }
            return funcTypes[typeArgs.Length - 1].MakeGenericType(typeArgs);
        }

        public static LambdaExpression Lambda(Expression body, params ParameterExpression[] parameters)
        {
            int index = (parameters == null) ? 0 : parameters.Length;
            Type[] typeArgs = new Type[index + 1];
            for (int i = 0; i < index; i++)
            {
                typeArgs[i] = parameters[i].Type;
            }
            typeArgs[index] = body.Type;
            return Expression.Lambda(GetFuncType(typeArgs), body, parameters);
        }

        public static Expression Parse(Type resultType, string expression, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(null, expression, values);
            return parser.Parse(resultType);
        }

        public static Expression<Func<T, S>> ParseLambda<T, S>(string expression, params object[] values) => 
            ((Expression<Func<T, S>>) ParseLambda(typeof(T), typeof(S), expression, values));

        public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values) => 
            ParseLambda(new ParameterExpression[] { Expression.Parameter(itType, "") }, resultType, expression, values);

        public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(parameters, expression, values);
            return Lambda(parser.Parse(resultType), parameters);
        }
    }
}

