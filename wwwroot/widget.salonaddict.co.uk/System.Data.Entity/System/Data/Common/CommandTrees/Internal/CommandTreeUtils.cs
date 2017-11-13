namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class CommandTreeUtils
    {
        private static readonly HashSet<DbExpressionKind> _associativeExpressionKinds = new HashSet<DbExpressionKind>(new DbExpressionKind[] { DbExpressionKind.Or, DbExpressionKind.And, DbExpressionKind.Plus, DbExpressionKind.Multiply });

        internal static void CheckNamed<T>(string strVarName, KeyValuePair<string, T> element)
        {
            if (string.IsNullOrEmpty(element.Key))
            {
                throw EntityUtil.ArgumentNull(string.Format(CultureInfo.InvariantCulture, "{0}.Key", new object[] { strVarName }));
            }
            if (element.Value == null)
            {
                throw EntityUtil.ArgumentNull(string.Format(CultureInfo.InvariantCulture, "{0}.Value", new object[] { strVarName }));
            }
        }

        internal static void CheckNamedList<T>(string listVarName, IList<KeyValuePair<string, T>> list, bool bAllowEmpty, CheckNamedListCallback<T> callBack)
        {
            if (list == null)
            {
                throw EntityUtil.ArgumentNull(listVarName);
            }
            if ((list.Count == 0) && !bAllowEmpty)
            {
                throw EntityUtil.Argument(Strings.Cqt_Util_CheckListEmptyInvalid, listVarName);
            }
            List<string> list2 = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                KeyValuePair<string, T> element = list[i];
                string strVarName = FormatIndex(listVarName, i);
                CheckNamed<T>(strVarName, element);
                int index = list2.IndexOf(element.Key);
                if (index > -1)
                {
                    throw EntityUtil.Argument(Strings.Cqt_Util_CheckListDuplicateName(index, i, element.Key), strVarName);
                }
                if (callBack != null)
                {
                    callBack(element, i);
                }
            }
        }

        internal static List<T> CreateList<T>(T element) => 
            new List<T> { element };

        internal static List<T> CreateList<T>(T element1, T element2) => 
            new List<T> { 
                element1,
                element2
            };

        private static void ExtractAssociativeArguments(DbExpressionKind expressionKind, List<DbExpression> argumentList, DbExpression expression)
        {
            if (expression.ExpressionKind != expressionKind)
            {
                argumentList.Add(expression);
            }
            else
            {
                DbBinaryExpression expression2 = expression as DbBinaryExpression;
                if (expression2 != null)
                {
                    ExtractAssociativeArguments(expressionKind, argumentList, expression2.Left);
                    ExtractAssociativeArguments(expressionKind, argumentList, expression2.Right);
                }
                else
                {
                    DbArithmeticExpression expression3 = (DbArithmeticExpression) expression;
                    ExtractAssociativeArguments(expressionKind, argumentList, expression3.Arguments[0]);
                    ExtractAssociativeArguments(expressionKind, argumentList, expression3.Arguments[1]);
                }
            }
        }

        internal static IEnumerable<DbExpression> FlattenAssociativeExpression(DbExpression expression) => 
            FlattenAssociativeExpression(expression.ExpressionKind, new DbExpression[] { expression });

        internal static IEnumerable<DbExpression> FlattenAssociativeExpression(DbExpressionKind expressionKind, params DbExpression[] arguments)
        {
            if (!_associativeExpressionKinds.Contains(expressionKind))
            {
                return arguments;
            }
            List<DbExpression> argumentList = new List<DbExpression>();
            foreach (DbExpression expression in arguments)
            {
                ExtractAssociativeArguments(expressionKind, argumentList, expression);
            }
            return argumentList;
        }

        internal static string FormatIndex(string arrayVarName, int idx)
        {
            StringBuilder builder = new StringBuilder((arrayVarName.Length + 10) + 2);
            return builder.Append(arrayVarName).Append('[').Append(idx).Append(']').ToString();
        }

        internal static bool IsValidDataSpace(DataSpace dataSpace)
        {
            if ((dataSpace != DataSpace.OSpace) && (DataSpace.CSpace != dataSpace))
            {
                return (DataSpace.SSpace == dataSpace);
            }
            return true;
        }

        internal static ReadOnlyCollection<TElementType> NewReadOnlyList<TElementType>(IList<TElementType> sourceList)
        {
            TElementType[] array = new TElementType[sourceList.Count];
            sourceList.CopyTo(array, 0);
            return new ReadOnlyCollection<TElementType>(array);
        }

        internal static bool TryGetPrimtiveTypeKind(Type clrType, out PrimitiveTypeKind primitiveTypeKind)
        {
            primitiveTypeKind = PrimitiveTypeKind.Binary;
            PrimitiveType primitiveType = null;
            if (ClrProviderManifest.Instance.TryGetPrimitiveType(clrType, out primitiveType))
            {
                primitiveTypeKind = primitiveType.PrimitiveTypeKind;
                return true;
            }
            return false;
        }

        internal delegate void CheckNamedListCallback<T>(KeyValuePair<string, T> element, int index);
    }
}

