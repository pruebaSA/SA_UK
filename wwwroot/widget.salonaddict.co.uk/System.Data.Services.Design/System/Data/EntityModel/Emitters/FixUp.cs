namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.Data.Services.Design;
    using System.Runtime.CompilerServices;

    internal sealed class FixUp
    {
        private string _class;
        private static readonly FixMethod[] _CSFixMethods;
        private string _method;
        private string _property;
        private static readonly FixMethod[] _VBFixMethods;
        private FixUpType m_type;

        static FixUp()
        {
            FixMethod[] methodArray = new FixMethod[12];
            methodArray[1] = new FixMethod(FixUp.CSMarkOverrideMethodAsSealed);
            methodArray[2] = new FixMethod(FixUp.CSMarkPropertySetAsInternal);
            methodArray[3] = new FixMethod(FixUp.CSMarkClassAsStatic);
            methodArray[4] = new FixMethod(FixUp.CSMarkPropertyGetAsPrivate);
            methodArray[5] = new FixMethod(FixUp.CSMarkPropertyGetAsInternal);
            methodArray[6] = new FixMethod(FixUp.CSMarkPropertyGetAsPublic);
            methodArray[7] = new FixMethod(FixUp.CSMarkPropertySetAsPrivate);
            methodArray[8] = new FixMethod(FixUp.CSMarkPropertySetAsPublic);
            methodArray[9] = new FixMethod(FixUp.CSMarkMethodAsPartial);
            methodArray[10] = new FixMethod(FixUp.CSMarkPropertyGetAsProtected);
            methodArray[11] = new FixMethod(FixUp.CSMarkPropertySetAsProtected);
            _CSFixMethods = methodArray;
            FixMethod[] methodArray2 = new FixMethod[12];
            methodArray2[1] = new FixMethod(FixUp.VBMarkOverrideMethodAsSealed);
            methodArray2[2] = new FixMethod(FixUp.VBMarkPropertySetAsInternal);
            methodArray2[4] = new FixMethod(FixUp.VBMarkPropertyGetAsPrivate);
            methodArray2[5] = new FixMethod(FixUp.VBMarkPropertyGetAsInternal);
            methodArray2[6] = new FixMethod(FixUp.VBMarkPropertyGetAsPublic);
            methodArray2[7] = new FixMethod(FixUp.VBMarkPropertySetAsPrivate);
            methodArray2[8] = new FixMethod(FixUp.VBMarkPropertySetAsPublic);
            methodArray2[9] = new FixMethod(FixUp.VBMarkMethodAsPartial);
            methodArray2[10] = new FixMethod(FixUp.VBMarkPropertyGetAsProtected);
            methodArray2[11] = new FixMethod(FixUp.VBMarkPropertySetAsProtected);
            _VBFixMethods = methodArray2;
        }

        public FixUp(string fqName, FixUpType type)
        {
            this.Type = type;
            string[] strArray = Utils.SplitName(fqName);
            if (type == FixUpType.MarkClassAsStatic)
            {
                this.Class = strArray[strArray.Length - 1];
            }
            else
            {
                this.Class = strArray[strArray.Length - 2];
                string str = strArray[strArray.Length - 1];
                switch (type)
                {
                    case FixUpType.MarkOverrideMethodAsSealed:
                    case FixUpType.MarkAbstractMethodAsPartial:
                        this.Method = str;
                        return;

                    case FixUpType.MarkPropertySetAsInternal:
                    case FixUpType.MarkPropertyGetAsPrivate:
                    case FixUpType.MarkPropertyGetAsInternal:
                    case FixUpType.MarkPropertyGetAsPublic:
                    case FixUpType.MarkPropertySetAsPrivate:
                    case FixUpType.MarkPropertySetAsPublic:
                    case FixUpType.MarkPropertyGetAsProtected:
                    case FixUpType.MarkPropertySetAsProtected:
                        this.Property = str;
                        break;

                    case FixUpType.MarkClassAsStatic:
                        break;

                    default:
                        return;
                }
            }
        }

        private static string CSMarkClassAsStatic(string line)
        {
            if (IndexOfKeyword(line, "static") >= 0)
            {
                return line;
            }
            int startIndex = IndexOfKeyword(line, "class");
            if (startIndex < 0)
            {
                return line;
            }
            int num2 = IndexOfKeyword(line, "partial");
            if (num2 >= 0)
            {
                startIndex = num2;
            }
            return line.Insert(startIndex, "static ");
        }

        private static string CSMarkMethodAsPartial(string line)
        {
            line = ReplaceFirst(line, "public abstract", "partial");
            return line;
        }

        private static string CSMarkOverrideMethodAsSealed(string line) => 
            InsertBefore(line, "override", "sealed");

        private static string CSMarkPropertyGetAsInternal(string line) => 
            InsertBefore(line, "get", "internal");

        private static string CSMarkPropertyGetAsPrivate(string line) => 
            InsertBefore(line, "get", "private");

        private static string CSMarkPropertyGetAsProtected(string line) => 
            InsertBefore(line, "get", "protected");

        private static string CSMarkPropertyGetAsPublic(string line) => 
            InsertBefore(line, "get", "public");

        private static string CSMarkPropertySetAsInternal(string line) => 
            InsertBefore(line, "set", "internal");

        private static string CSMarkPropertySetAsPrivate(string line) => 
            InsertBefore(line, "set", "private");

        private static string CSMarkPropertySetAsProtected(string line) => 
            InsertBefore(line, "set", "protected");

        private static string CSMarkPropertySetAsPublic(string line) => 
            InsertBefore(line, "set", "public");

        public string Fix(LanguageOption language, string line)
        {
            FixMethod method = null;
            if (language == LanguageOption.GenerateCSharpCode)
            {
                method = _CSFixMethods[(int) this.Type];
            }
            else if (language == LanguageOption.GenerateVBCode)
            {
                method = _VBFixMethods[(int) this.Type];
            }
            if (method != null)
            {
                line = method(line);
            }
            return line;
        }

        private static int IndexOfKeyword(string line, string keyword)
        {
            int index = line.IndexOf(keyword, StringComparison.Ordinal);
            if (index < 0)
            {
                return index;
            }
            int num2 = index + keyword.Length;
            if (((index == 0) || char.IsWhiteSpace(line, index - 1)) && ((num2 == line.Length) || char.IsWhiteSpace(line, num2)))
            {
                return index;
            }
            return -1;
        }

        private static string InsertBefore(string line, string searchText, string insertText)
        {
            if (IndexOfKeyword(line, insertText) >= 0)
            {
                return line;
            }
            int startIndex = IndexOfKeyword(line, searchText);
            if (startIndex < 0)
            {
                return line;
            }
            return line.Insert(startIndex, insertText + " ");
        }

        private static string ReplaceFirst(string line, string str1, string str2)
        {
            int index = line.IndexOf(str1, StringComparison.Ordinal);
            if (index >= 0)
            {
                line = line.Remove(index, str1.Length);
                line = line.Insert(index, str2);
            }
            return line;
        }

        private static string VBMarkMethodAsPartial(string line)
        {
            line = ReplaceFirst(line, "Public MustOverride", "Partial Private");
            line = line + Environment.NewLine + "        End Sub";
            return line;
        }

        private static string VBMarkOverrideMethodAsSealed(string line) => 
            InsertBefore(line, "Overrides", "NotOverridable");

        private static string VBMarkPropertyGetAsInternal(string line) => 
            InsertBefore(line, "Get", "Friend");

        private static string VBMarkPropertyGetAsPrivate(string line) => 
            InsertBefore(line, "Get", "Private");

        private static string VBMarkPropertyGetAsProtected(string line) => 
            InsertBefore(line, "Get", "Protected");

        private static string VBMarkPropertyGetAsPublic(string line) => 
            InsertBefore(line, "Get", "Public");

        private static string VBMarkPropertySetAsInternal(string line) => 
            InsertBefore(line, "Set", "Friend");

        private static string VBMarkPropertySetAsPrivate(string line) => 
            InsertBefore(line, "Set", "Private");

        private static string VBMarkPropertySetAsProtected(string line) => 
            InsertBefore(line, "Set", "Protected");

        private static string VBMarkPropertySetAsPublic(string line) => 
            InsertBefore(line, "Set", "Public");

        public string Class
        {
            get => 
                this._class;
            private set
            {
                this._class = value;
            }
        }

        public string Method
        {
            get => 
                this._method;
            private set
            {
                this._method = value;
            }
        }

        public string Property
        {
            get => 
                this._property;
            private set
            {
                this._property = value;
            }
        }

        public FixUpType Type
        {
            get => 
                this.m_type;
            private set
            {
                this.m_type = value;
            }
        }

        internal delegate string FixMethod(string line);
    }
}

