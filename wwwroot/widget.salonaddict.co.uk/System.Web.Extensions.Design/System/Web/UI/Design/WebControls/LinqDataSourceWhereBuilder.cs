namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.UI.WebControls;

    internal class LinqDataSourceWhereBuilder
    {
        internal static bool IsOperatorChar(char c)
        {
            if (((c != '=') && (c != '!')) && (c != '<'))
            {
                return (c == '>');
            }
            return true;
        }

        public LinqDataSourceWhereExpression MakeWhereExpression(string expression, ParameterCollection whereParams)
        {
            WhereExpressionParseState fieldNameFirstChar = WhereExpressionParseState.FieldNameFirstChar;
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            StringBuilder builder3 = new StringBuilder();
            expression = expression.Trim();
            int num = 0;
            while (num < expression.Length)
            {
                char c = expression[num];
                switch (fieldNameFirstChar)
                {
                    case WhereExpressionParseState.FieldNameFirstChar:
                        if (char.IsLetter(c) || (c == '_'))
                        {
                            break;
                        }
                        return null;

                    case WhereExpressionParseState.FieldName:
                    {
                        if (char.IsLetterOrDigit(c) || (c == '_'))
                        {
                            goto Label_008E;
                        }
                        fieldNameFirstChar = WhereExpressionParseState.Operator;
                        continue;
                    }
                    case WhereExpressionParseState.Operator:
                    {
                        if (!char.IsWhiteSpace(c))
                        {
                            goto Label_00B3;
                        }
                        num++;
                        continue;
                    }
                    case WhereExpressionParseState.ParameterFirstChar:
                        if ((char.IsLetter(c) || (c == '_')) || (c == '@'))
                        {
                            goto Label_00FF;
                        }
                        return null;

                    case WhereExpressionParseState.Parameter:
                        if (char.IsLetterOrDigit(c) || (c == '_'))
                        {
                            goto Label_0123;
                        }
                        return null;

                    default:
                    {
                        continue;
                    }
                }
                builder.Append(c);
                fieldNameFirstChar = WhereExpressionParseState.FieldName;
                num++;
                continue;
            Label_008E:
                builder.Append(c);
                num++;
                continue;
            Label_00B3:
                if (IsOperatorChar(c))
                {
                    builder2.Append(c);
                    num++;
                    continue;
                }
                if ((c == '@') && (builder2.Length > 0))
                {
                    fieldNameFirstChar = WhereExpressionParseState.ParameterFirstChar;
                    num++;
                    continue;
                }
                return null;
            Label_00FF:
                builder3.Append(c);
                fieldNameFirstChar = WhereExpressionParseState.Parameter;
                num++;
                continue;
            Label_0123:
                builder3.Append(c);
                num++;
            }
            foreach (Parameter parameter in whereParams)
            {
                if (string.Equals(parameter.Name, builder3.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return new LinqDataSourceWhereExpression(builder.ToString(), builder2.ToString(), builder3.ToString());
                }
            }
            return null;
        }

        public LinqDataSourceWhereStatement MakeWhereStatement(string where, ParameterCollection clonedWhereParameters)
        {
            if (string.IsNullOrEmpty(where))
            {
                return null;
            }
            LinqDataSourceWhereStatement statement = new LinqDataSourceWhereStatement();
            string[] strArray = where.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>();
            foreach (string str in strArray)
            {
                LinqDataSourceWhereExpression item = this.MakeWhereExpression(str, clonedWhereParameters);
                if (((item == null) || (item.Operator == LinqDataSourceOperators.None)) || list.Contains(item.ParameterName))
                {
                    return null;
                }
                list.Add(item.ParameterName);
                statement.Add(item);
            }
            if (list.Count != clonedWhereParameters.Count)
            {
                return null;
            }
            foreach (Parameter parameter in clonedWhereParameters)
            {
                statement.Parameters.Add(parameter);
            }
            return statement;
        }

        private enum WhereExpressionParseState
        {
            FieldNameFirstChar,
            FieldName,
            Operator,
            ParameterFirstChar,
            Parameter
        }
    }
}

