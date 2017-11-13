namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Design;
    using System.Globalization;
    using System.Reflection;
    using System.Security;
    using System.Text.RegularExpressions;

    internal static class CommentEmitter
    {
        private static readonly Regex LeadingBlanks = new Regex(@"^(?<LeadingBlanks>\s{1,})\S", RegexOptions.Singleline | RegexOptions.Compiled);

        public static void EmitComments(string[] commentLines, CodeCommentStatementCollection commentCollection, bool docComment)
        {
            foreach (string str in commentLines)
            {
                commentCollection.Add(new CodeCommentStatement(str, docComment));
            }
        }

        private static void EmitOtherDocumentationComments(Documentation documentation, CodeCommentStatementCollection commentCollection)
        {
            if ((documentation != null) && !string.IsNullOrEmpty(documentation.LongDescription))
            {
                EmitXmlComments("LongDescription", GetFormattedLines(documentation.LongDescription, true), commentCollection);
            }
        }

        public static void EmitParamComments(CodeParameterDeclarationExpression parameter, string comment, CodeCommentStatementCollection commentCollection)
        {
            string text = string.Format(CultureInfo.CurrentCulture, "<param name=\"{0}\">{1}</param>", new object[] { parameter.Name, comment });
            commentCollection.Add(new CodeCommentStatement(text, true));
        }

        public static void EmitSummaryComments(MetadataItem item, CodeCommentStatementCollection commentCollection)
        {
            Documentation documentation = GetDocumentation(item);
            string[] summaryComments = null;
            if ((documentation != null) && !System.Data.Common.Utils.StringUtil.IsNullOrEmptyOrWhiteSpace(documentation.Summary))
            {
                summaryComments = GetFormattedLines(documentation.Summary, true);
            }
            else
            {
                string missingDocumentationNoName;
                BuiltInTypeKind builtInTypeKind = item.BuiltInTypeKind;
                if (builtInTypeKind != BuiltInTypeKind.ComplexType)
                {
                    if (builtInTypeKind != BuiltInTypeKind.EdmProperty)
                    {
                        PropertyInfo property = item.GetType().GetProperty("FullName");
                        if (property == null)
                        {
                            property = item.GetType().GetProperty("Name");
                        }
                        object obj2 = null;
                        if (property != null)
                        {
                            obj2 = property.GetValue(item, null);
                        }
                        if (obj2 != null)
                        {
                            missingDocumentationNoName = Strings.MissingDocumentation(obj2.ToString());
                        }
                        else
                        {
                            missingDocumentationNoName = Strings.MissingDocumentationNoName;
                        }
                    }
                    else
                    {
                        missingDocumentationNoName = Strings.MissingPropertyDocumentation(((EdmProperty) item).Name);
                    }
                }
                else
                {
                    missingDocumentationNoName = Strings.MissingComplexTypeDocumentation(((ComplexType) item).FullName);
                }
                summaryComments = new string[] { missingDocumentationNoName };
            }
            EmitSummaryComments(summaryComments, commentCollection);
            EmitOtherDocumentationComments(documentation, commentCollection);
        }

        public static void EmitSummaryComments(string summaryComments, CodeCommentStatementCollection commentCollection)
        {
            if (!string.IsNullOrEmpty(summaryComments) && !string.IsNullOrEmpty(summaryComments = summaryComments.TrimEnd(new char[0])))
            {
                EmitSummaryComments(SplitIntoLines(summaryComments), commentCollection);
            }
        }

        private static void EmitSummaryComments(string[] summaryComments, CodeCommentStatementCollection commentCollection)
        {
            EmitXmlComments("summary", summaryComments, commentCollection);
        }

        private static void EmitXmlComments(string tag, string[] summaryComments, CodeCommentStatementCollection commentCollection)
        {
            commentCollection.Add(new CodeCommentStatement(string.Format(CultureInfo.InvariantCulture, "<{0}>", new object[] { tag }), true));
            EmitComments(summaryComments, commentCollection, true);
            commentCollection.Add(new CodeCommentStatement(string.Format(CultureInfo.InvariantCulture, "</{0}>", new object[] { tag }), true));
        }

        private static Documentation GetDocumentation(MetadataItem item)
        {
            if (item is Documentation)
            {
                return (Documentation) item;
            }
            return item.Documentation;
        }

        public static string[] GetFormattedLines(string text, bool escapeForXml)
        {
            if (System.Data.Common.Utils.StringUtil.IsNullOrEmptyOrWhiteSpace(text))
            {
                return new string[] { "" };
            }
            text = text.Replace("\r", "");
            bool flag = false;
            int index = text.IndexOf('\n');
            if ((index >= 0) && System.Data.Common.Utils.StringUtil.IsNullOrEmptyOrWhiteSpace(text, 0, index + 1))
            {
                index++;
                flag = true;
            }
            else
            {
                index = 0;
            }
            int offset = text.LastIndexOf('\n');
            if ((offset > (index - 1)) && System.Data.Common.Utils.StringUtil.IsNullOrEmptyOrWhiteSpace(text, offset))
            {
                offset--;
                flag = true;
            }
            else
            {
                offset = text.Length - 1;
            }
            if (flag)
            {
                text = text.Substring(index, (offset - index) + 1);
            }
            if (escapeForXml)
            {
                text = SecurityElement.Escape(text);
            }
            string[] strArray = SplitIntoLines(text);
            if (strArray.Length == 1)
            {
                strArray[0] = strArray[0].Trim();
                return strArray;
            }
            string str = null;
            foreach (string str2 in strArray)
            {
                if (System.Data.Common.Utils.StringUtil.IsNullOrEmptyOrWhiteSpace(str2))
                {
                    continue;
                }
                Match match = LeadingBlanks.Match(str2);
                if (!match.Success)
                {
                    str = "";
                    break;
                }
                if (str == null)
                {
                    str = match.Groups["LeadingBlanks"].Value;
                    continue;
                }
                string str3 = match.Groups["LeadingBlanks"].Value;
                if ((str3 != str) && !str3.StartsWith(str, StringComparison.Ordinal))
                {
                    if (str.StartsWith(str3, StringComparison.OrdinalIgnoreCase))
                    {
                        str = str3;
                        continue;
                    }
                    int num3 = Math.Min(str.Length, str3.Length);
                    for (int j = 0; j < num3; j++)
                    {
                        if (str[j] != str3[j])
                        {
                            if (j == 0)
                            {
                                str = "";
                            }
                            else
                            {
                                str = str.Substring(0, j);
                            }
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(str))
                    {
                        break;
                    }
                }
            }
            int length = str.Length;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].Length >= length)
                {
                    strArray[i] = strArray[i].Substring(length);
                }
                strArray[i] = strArray[i].TrimEnd(new char[0]);
            }
            return strArray;
        }

        private static string[] SplitIntoLines(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new string[] { "" };
            }
            return text.Replace("\r", "").Split(new char[] { '\n' });
        }
    }
}

