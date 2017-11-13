namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Design;
    using System.IO;
    using System.Runtime.InteropServices;

    internal sealed class FixUpCollection : List<FixUp>
    {
        private Dictionary<string, List<FixUp>> _classFixUps;
        private const string _CSClassKeyWord = " class ";
        private static readonly char[] _CSEndOfClassDelimiters = new char[] { ' ', ':' };
        private static readonly char[] _CSFieldMarkers = new char[] { '=', ';' };
        private LanguageOption _language;
        private static readonly char[] _VBEndOfClassDelimiters = new char[] { ' ', '(' };
        private static readonly char[] _VBNonDeclMarkers = new char[] { '=', '"', '\'' };

        private static void CopyFile(TextReader reader, TextWriter writer)
        {
            string str;
            while ((str = reader.ReadLine()) != null)
            {
                writer.WriteLine(str);
            }
        }

        public void Do(TextReader reader, TextWriter writer, LanguageOption language, bool hasNamespace)
        {
            this.Language = language;
            foreach (FixUp up in this)
            {
                List<FixUp> list = null;
                if (this.ClassFixUps.ContainsKey(up.Class))
                {
                    list = this.ClassFixUps[up.Class];
                }
                else
                {
                    list = new List<FixUp>();
                    this.ClassFixUps.Add(up.Class, list);
                }
                list.Add(up);
            }
            switch (this.Language)
            {
                case LanguageOption.GenerateCSharpCode:
                    this.DoFixUpsForCS(reader, writer, hasNamespace);
                    return;

                case LanguageOption.GenerateVBCode:
                    this.DoFixUpsForVB(reader, writer);
                    return;
            }
            CopyFile(reader, writer);
        }

        private void DoFixUpsForCS(TextReader reader, TextWriter writer, bool hasNamespace)
        {
            string str;
            int num = 0;
            string className = null;
            bool flag = false;
            FixUp up = null;
            FixUp up2 = null;
            int num2 = hasNamespace ? 1 : 0;
            while ((str = reader.ReadLine()) != null)
            {
                string str2 = str.Trim();
                if (str2 == "{")
                {
                    num++;
                }
                else if (str2 == "}")
                {
                    num--;
                    if (num < (num2 + 2))
                    {
                        up2 = null;
                        if (num < (num2 + 1))
                        {
                            className = null;
                            flag = false;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(str2) && !str2.StartsWith("//", StringComparison.Ordinal))
                {
                    string str4;
                    if (IsCSClassDefinition(str, out str4))
                    {
                        if (num == num2)
                        {
                            className = str4;
                            str4 = null;
                            flag = this.IsClassWanted(className);
                            if (flag)
                            {
                                str = this.FixUpClassDecl(className, str);
                            }
                        }
                    }
                    else if (flag)
                    {
                        if (num == (num2 + 1))
                        {
                            string str5;
                            switch (GetCSDeclType(str2, out str5))
                            {
                                case CSDeclType.Method:
                                    str = this.FixUpMethodDecl(className, str5, str);
                                    break;

                                case CSDeclType.Property:
                                    up2 = this.FixUpSetter(className, str5);
                                    up = this.FixUpGetter(className, str5);
                                    break;
                            }
                        }
                        else if (num == (num2 + 2))
                        {
                            if ((str2 == "set") && (up2 != null))
                            {
                                str = up2.Fix(LanguageOption.GenerateCSharpCode, str);
                                up2 = null;
                            }
                            else if ((str2 == "get") && (up != null))
                            {
                                str = up.Fix(LanguageOption.GenerateCSharpCode, str);
                                up = null;
                            }
                        }
                    }
                }
                writer.WriteLine(str);
            }
        }

        public void DoFixUpsForVB(TextReader reader, TextWriter writer)
        {
            string str;
            this.Language = LanguageOption.GenerateVBCode;
            Stack<VBStatementType> context = new Stack<VBStatementType>();
            int num = 0;
            string className = null;
            bool flag = false;
            FixUp up = null;
            FixUp up2 = null;
            while ((str = reader.ReadLine()) != null)
            {
                if (((str != null) && (str.Length != 0)) && (str[0] != '\''))
                {
                    string str3;
                    switch (GetVBStatementType(context, str, out str3))
                    {
                        case VBStatementType.BeginClass:
                            num++;
                            up2 = null;
                            if (num == 1)
                            {
                                className = str3;
                                flag = this.IsClassWanted(str3);
                                if (flag)
                                {
                                    str = this.FixUpClassDecl(className, str);
                                }
                            }
                            break;

                        case VBStatementType.EndClass:
                            num--;
                            if (num == 0)
                            {
                                className = null;
                            }
                            break;

                        case VBStatementType.BeginProperty:
                            if (!flag)
                            {
                                goto Label_00CD;
                            }
                            up = this.FixUpGetter(className, str3);
                            up2 = this.FixUpSetter(className, str3);
                            break;

                        case VBStatementType.EndProperty:
                            up = null;
                            up2 = null;
                            break;

                        case VBStatementType.BeginMethod:
                            if (flag)
                            {
                                str = this.FixUpMethodDecl(className, str3, str);
                            }
                            break;

                        case VBStatementType.BeginPropertyGetter:
                            goto Label_0106;

                        case VBStatementType.BeginPropertySetter:
                            goto Label_00EE;
                    }
                }
                goto Label_011C;
            Label_00CD:
                up = null;
                up2 = null;
                goto Label_011C;
            Label_00EE:
                if (up2 != null)
                {
                    str = up2.Fix(this.Language, str);
                }
                up2 = null;
                goto Label_011C;
            Label_0106:
                if (up != null)
                {
                    str = up.Fix(this.Language, str);
                }
                up = null;
            Label_011C:
                writer.WriteLine(str);
            }
        }

        private string FixUpClassDecl(string className, string line)
        {
            IList<FixUp> list = this.ClassFixUps[className];
            foreach (FixUp up in list)
            {
                if (up.Type == FixUpType.MarkClassAsStatic)
                {
                    return up.Fix(this.Language, line);
                }
            }
            return line;
        }

        private FixUp FixUpGetter(string className, string propertyName)
        {
            IList<FixUp> list = this.ClassFixUps[className];
            foreach (FixUp up in list)
            {
                if ((up.Property == propertyName) && (((up.Type == FixUpType.MarkPropertyGetAsPrivate) || (up.Type == FixUpType.MarkPropertyGetAsInternal)) || ((up.Type == FixUpType.MarkPropertyGetAsPublic) || (up.Type == FixUpType.MarkPropertyGetAsProtected))))
                {
                    return up;
                }
            }
            return null;
        }

        private string FixUpMethodDecl(string className, string methodName, string line)
        {
            IList<FixUp> list = this.ClassFixUps[className];
            foreach (FixUp up in list)
            {
                if ((up.Method == methodName) && ((up.Type == FixUpType.MarkOverrideMethodAsSealed) || (up.Type == FixUpType.MarkAbstractMethodAsPartial)))
                {
                    return up.Fix(this.Language, line);
                }
            }
            return line;
        }

        private FixUp FixUpSetter(string className, string propertyName)
        {
            IList<FixUp> list = this.ClassFixUps[className];
            foreach (FixUp up in list)
            {
                if ((up.Property == propertyName) && (((up.Type == FixUpType.MarkPropertySetAsPrivate) || (up.Type == FixUpType.MarkPropertySetAsInternal)) || ((up.Type == FixUpType.MarkPropertySetAsPublic) || (up.Type == FixUpType.MarkPropertySetAsProtected))))
                {
                    return up;
                }
            }
            return null;
        }

        private static CSDeclType GetCSDeclType(string line, out string name)
        {
            name = null;
            if (line[0] == '[')
            {
                return CSDeclType.Other;
            }
            int index = line.IndexOf('(');
            int num2 = line.IndexOf(')');
            if (((line.IndexOf('=') == -1) && (index >= 0)) && (num2 > index))
            {
                line = line.Substring(0, index).TrimEnd(null);
                name = line.Substring(line.LastIndexOf(' ') + 1);
                return CSDeclType.Method;
            }
            if (line.IndexOfAny(_CSFieldMarkers, 0) >= 0)
            {
                return CSDeclType.Other;
            }
            CSDeclType property = CSDeclType.Property;
            name = line.Substring(line.LastIndexOf(' ') + 1);
            return property;
        }

        private static VBStatementType GetVBStatementType(Stack<VBStatementType> context, string line, out string name)
        {
            name = null;
            VBStatementType other = VBStatementType.Other;
            if (line.IndexOfAny(_VBNonDeclMarkers) < 0)
            {
                string str = NormalizeForVB(line);
                if (context.Count <= 0)
                {
                    if (LineIsVBBeginClassMethodProperty(str, "Class", ref name))
                    {
                        other = VBStatementType.BeginClass;
                        context.Push(other);
                    }
                    return other;
                }
                switch (context.Peek())
                {
                    case VBStatementType.BeginClass:
                        if (str == "End Class")
                        {
                            other = VBStatementType.EndClass;
                            context.Pop();
                            return other;
                        }
                        if (LineIsVBBeginClassMethodProperty(str, "Class", ref name))
                        {
                            other = VBStatementType.BeginClass;
                            context.Push(other);
                            return other;
                        }
                        if (LineIsVBBeginClassMethodProperty(str, "MustOverride Sub", ref name))
                        {
                            return VBStatementType.BeginMethod;
                        }
                        if (LineIsVBBeginClassMethodProperty(str, "Function", ref name) || LineIsVBBeginClassMethodProperty(str, "Sub", ref name))
                        {
                            other = VBStatementType.BeginMethod;
                            context.Push(other);
                            return other;
                        }
                        if (LineIsVBBeginClassMethodProperty(str, "Property", ref name))
                        {
                            other = VBStatementType.BeginProperty;
                            context.Push(other);
                        }
                        return other;

                    case VBStatementType.EndClass:
                    case VBStatementType.EndProperty:
                    case VBStatementType.EndMethod:
                    case VBStatementType.EndPropertyGetter:
                        return other;

                    case VBStatementType.BeginProperty:
                        if (str == "End Property")
                        {
                            other = VBStatementType.EndProperty;
                            context.Pop();
                            return other;
                        }
                        if (LineIsVBBeginSetterGetter(str, "Get"))
                        {
                            other = VBStatementType.BeginPropertyGetter;
                            context.Push(other);
                            return other;
                        }
                        if (LineIsVBBeginSetterGetter(str, "Set"))
                        {
                            other = VBStatementType.BeginPropertySetter;
                            context.Push(other);
                        }
                        return other;

                    case VBStatementType.BeginMethod:
                        switch (str)
                        {
                            case "End Sub":
                            case "End Function":
                                other = VBStatementType.EndMethod;
                                context.Pop();
                                break;
                        }
                        return other;

                    case VBStatementType.BeginPropertyGetter:
                        if (str == "End Get")
                        {
                            other = VBStatementType.EndPropertyGetter;
                            context.Pop();
                        }
                        return other;

                    case VBStatementType.BeginPropertySetter:
                        if (str == "End Set")
                        {
                            other = VBStatementType.EndPropertySetter;
                            context.Pop();
                        }
                        return other;
                }
            }
            return other;
        }

        private static int IndexOfKeyword(string line, string keyword)
        {
            char ch;
            int index = line.IndexOf(keyword, StringComparison.Ordinal);
            if (index < 0)
            {
                return index;
            }
            int num2 = index + keyword.Length;
            if (((index == 0) || char.IsWhiteSpace(line, index - 1)) && (((num2 == line.Length) || ((ch = line[num2]) == '(')) || char.IsWhiteSpace(ch)))
            {
                return index;
            }
            return -1;
        }

        private bool IsClassWanted(string className) => 
            this.ClassFixUps.ContainsKey(className);

        private static bool IsCSClassDefinition(string line, out string className)
        {
            int index = line.IndexOf(" class ", StringComparison.Ordinal);
            if (index < 0)
            {
                className = null;
                return false;
            }
            index += " class ".Length;
            int num2 = line.IndexOfAny(_CSEndOfClassDelimiters, index);
            if (num2 < 0)
            {
                className = line.Substring(index);
            }
            else
            {
                className = line.Substring(index, num2 - index);
            }
            if (className.StartsWith("@", StringComparison.Ordinal))
            {
                className = className.Substring(1);
            }
            return true;
        }

        public static bool IsLanguageSupported(LanguageOption language)
        {
            switch (language)
            {
                case LanguageOption.GenerateCSharpCode:
                case LanguageOption.GenerateVBCode:
                    return true;
            }
            return false;
        }

        private static bool LineIsVBBeginClassMethodProperty(string line, string keyword, ref string name)
        {
            int index = IndexOfKeyword(line, keyword);
            if (index < 0)
            {
                return false;
            }
            index += keyword.Length;
            if ((index >= line.Length) || !char.IsWhiteSpace(line, index))
            {
                return false;
            }
            index++;
            if (index >= line.Length)
            {
                return false;
            }
            int length = line.IndexOfAny(_VBEndOfClassDelimiters, index);
            if (length < 0)
            {
                length = line.Length;
            }
            name = line.Substring(index, length - index).Trim();
            if (name.StartsWith("[", StringComparison.Ordinal) && name.EndsWith("]", StringComparison.Ordinal))
            {
                name = name.Substring(1, name.Length - 2);
            }
            return true;
        }

        private static bool LineIsVBBeginSetterGetter(string line, string keyword) => 
            (IndexOfKeyword(line, keyword) >= 0);

        private static string NormalizeForVB(string line)
        {
            line = line.Replace('\t', ' ').Trim();
            while (line.IndexOf("  ", 0, StringComparison.Ordinal) >= 0)
            {
                line = line.Replace("  ", " ");
            }
            return line;
        }

        private Dictionary<string, List<FixUp>> ClassFixUps
        {
            get
            {
                if (this._classFixUps == null)
                {
                    this._classFixUps = new Dictionary<string, List<FixUp>>();
                }
                return this._classFixUps;
            }
        }

        private LanguageOption Language
        {
            get => 
                this._language;
            set
            {
                this._language = value;
            }
        }

        private enum CSDeclType
        {
            Method,
            Property,
            Other
        }

        public enum VBStatementType
        {
            BeginClass,
            EndClass,
            BeginProperty,
            EndProperty,
            BeginMethod,
            EndMethod,
            BeginPropertyGetter,
            EndPropertyGetter,
            BeginPropertySetter,
            EndPropertySetter,
            Other
        }
    }
}

