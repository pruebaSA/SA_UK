namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;

    internal class Dump : BasicOpVisitor, IDisposable
    {
        private XmlWriter _writer;
        internal static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private Dump(Stream stream) : this(stream, DefaultEncoding, true)
        {
        }

        private Dump(Stream stream, Encoding encoding, bool indent)
        {
            XmlWriterSettings settings = new XmlWriterSettings {
                CheckCharacters = false,
                Indent = true,
                Encoding = encoding
            };
            this._writer = XmlWriter.Create(stream, settings);
            this._writer.WriteStartDocument(true);
        }

        internal void Begin(string name, Dictionary<string, object> attrs)
        {
            this._writer.WriteStartElement(name);
            if (attrs != null)
            {
                foreach (KeyValuePair<string, object> pair in attrs)
                {
                    this._writer.WriteAttributeString(pair.Key, pair.Value.ToString());
                }
            }
        }

        internal void BeginExpression()
        {
            this.WriteString("(");
        }

        private void DumpTable(Table table)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object> {
                { 
                    "Table",
                    table.TableId
                }
            };
            if (table.TableMetadata.Extent != null)
            {
                attrs.Add("Extent", table.TableMetadata.Extent.Name);
            }
            using (new AutoXml(this, "Table", attrs))
            {
                this.DumpVars(table.Columns);
            }
        }

        private void DumpVar(Var v)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object> {
                { 
                    "Var",
                    v.Id
                }
            };
            ColumnVar var = v as ColumnVar;
            if (var != null)
            {
                attrs.Add("Name", var.ColumnMetadata.Name);
                attrs.Add("Type", TypeHelpers.GetFullName(var.ColumnMetadata.Type));
            }
            using (new AutoXml(this, v.GetType().Name, attrs))
            {
            }
        }

        private void DumpVars(List<Var> vars)
        {
            foreach (Var var in vars)
            {
                this.DumpVar(var);
            }
        }

        internal void End(string name)
        {
            this._writer.WriteEndElement();
        }

        internal void EndExpression()
        {
            this.WriteString(")");
        }

        private static string FormatVarList(StringBuilder sb, List<System.Data.Query.InternalTrees.SortKey> varList)
        {
            sb.Length = 0;
            string str = string.Empty;
            foreach (System.Data.Query.InternalTrees.SortKey key in varList)
            {
                sb.Append(str);
                sb.Append(key.Var.Id);
                str = ",";
            }
            return sb.ToString();
        }

        private static string FormatVarList(StringBuilder sb, VarList varList)
        {
            sb.Length = 0;
            string str = string.Empty;
            foreach (Var var in varList)
            {
                sb.Append(str);
                sb.Append(var.Id);
                str = ",";
            }
            return sb.ToString();
        }

        void IDisposable.Dispose()
        {
            try
            {
                this._writer.WriteEndDocument();
                this._writer.Flush();
                this._writer.Close();
            }
            catch (Exception exception)
            {
                if (!EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw;
                }
            }
        }

        internal static string ToXml(ColumnMap columnMap)
        {
            MemoryStream stream = new MemoryStream();
            using (Dump dump = new Dump(stream))
            {
                using (new AutoXml(dump, "columnMap"))
                {
                    columnMap.Accept<Dump>(ColumnMapDumper.Instance, dump);
                }
            }
            return DefaultEncoding.GetString(stream.ToArray());
        }

        internal static string ToXml(Command itree) => 
            ToXml(itree, itree.Root);

        internal static string ToXml(Command itree, Node subtree)
        {
            MemoryStream stream = new MemoryStream();
            using (Dump dump = new Dump(stream))
            {
                using (new AutoXml(dump, "nodes"))
                {
                    dump.VisitNode(subtree);
                }
            }
            return DefaultEncoding.GetString(stream.ToArray());
        }

        public override void Visit(CaseOp op, Node n)
        {
            using (new AutoXml(this, op))
            {
                int num = 0;
                while (num < n.Children.Count)
                {
                    if ((num + 1) < n.Children.Count)
                    {
                        using (new AutoXml(this, "when"))
                        {
                            this.VisitNode(n.Children[num++]);
                        }
                        using (new AutoXml(this, "then"))
                        {
                            this.VisitNode(n.Children[num++]);
                            continue;
                        }
                    }
                    using (new AutoXml(this, "else"))
                    {
                        this.VisitNode(n.Children[num++]);
                        continue;
                    }
                }
            }
        }

        public override void Visit(CollectOp op, Node n)
        {
            using (new AutoXml(this, op))
            {
                this.VisitChildren(n);
            }
        }

        public override void Visit(ConstrainedSortOp op, Node n)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object> {
                { 
                    "WithTies",
                    op.WithTies
                }
            };
            using (new AutoXml(this, op, attrs))
            {
                base.Visit(op, n);
            }
        }

        public override void Visit(DiscriminatedNewEntityOp op, Node n)
        {
            this.VisitNewOp(op, n);
        }

        public override void Visit(DistinctOp op, Node n)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object>();
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            foreach (Var var in op.Keys)
            {
                builder.Append(str);
                builder.Append(var.Id);
                str = ",";
            }
            if (builder.Length != 0)
            {
                attrs.Add("Keys", builder.ToString());
            }
            using (new AutoXml(this, op, attrs))
            {
                this.VisitChildren(n);
            }
        }

        public override void Visit(GroupByOp op, Node n)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object>();
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            foreach (Var var in op.Keys)
            {
                builder.Append(str);
                builder.Append(var.Id);
                str = ",";
            }
            if (builder.Length != 0)
            {
                attrs.Add("Keys", builder.ToString());
            }
            using (new AutoXml(this, op, attrs))
            {
                using (new AutoXml(this, "outputs"))
                {
                    foreach (Var var2 in op.Outputs)
                    {
                        this.DumpVar(var2);
                    }
                }
                this.VisitChildren(n);
            }
        }

        public override void Visit(IsOfOp op, Node n)
        {
            using (new AutoXml(this, op.IsOfOnly ? "IsOfOnly" : "IsOf"))
            {
                string str = string.Empty;
                foreach (Node node in n.Children)
                {
                    this.WriteString(str);
                    this.VisitNode(node);
                    str = ",";
                }
            }
        }

        public override void Visit(NewEntityOp op, Node n)
        {
            this.VisitNewOp(op, n);
        }

        public override void Visit(NewInstanceOp op, Node n)
        {
            this.VisitNewOp(op, n);
        }

        public override void Visit(NewMultisetOp op, Node n)
        {
            this.VisitNewOp(op, n);
        }

        public override void Visit(NewRecordOp op, Node n)
        {
            this.VisitNewOp(op, n);
        }

        public override void Visit(PhysicalProjectOp op, Node n)
        {
            using (new AutoXml(this, op))
            {
                using (new AutoXml(this, "outputs"))
                {
                    foreach (Var var in op.Outputs)
                    {
                        this.DumpVar(var);
                    }
                }
                using (new AutoXml(this, "columnMap"))
                {
                    op.ColumnMap.Accept<Dump>(ColumnMapDumper.Instance, this);
                }
                using (new AutoXml(this, "input"))
                {
                    this.VisitChildren(n);
                }
            }
        }

        public override void Visit(ProjectOp op, Node n)
        {
            using (new AutoXml(this, op))
            {
                using (new AutoXml(this, "outputs"))
                {
                    foreach (Var var in op.Outputs)
                    {
                        this.DumpVar(var);
                    }
                }
                this.VisitChildren(n);
            }
        }

        public override void Visit(PropertyOp op, Node n)
        {
            using (new AutoString(this, op))
            {
                this.VisitChildren(n);
                this.WriteString(".");
                this.WriteString(op.PropertyInfo.Name);
            }
        }

        public override void Visit(RelPropertyOp op, Node n)
        {
            using (new AutoString(this, op))
            {
                this.VisitChildren(n);
                this.WriteString(".NAVIGATE(");
                this.WriteString(op.PropertyInfo.Relationship.Name);
                this.WriteString(",");
                this.WriteString(op.PropertyInfo.FromEnd.Name);
                this.WriteString(",");
                this.WriteString(op.PropertyInfo.ToEnd.Name);
                this.WriteString(")");
            }
        }

        public override void Visit(ScanTableOp op, Node n)
        {
            using (new AutoXml(this, op))
            {
                this.DumpTable(op.Table);
                this.VisitChildren(n);
            }
        }

        public override void Visit(ScanViewOp op, Node n)
        {
            using (new AutoXml(this, op))
            {
                this.DumpTable(op.Table);
                this.VisitChildren(n);
            }
        }

        public override void Visit(SortOp op, Node n)
        {
            using (new AutoXml(this, op))
            {
                base.Visit(op, n);
            }
        }

        public override void Visit(UnnestOp op, Node n)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object>();
            if (op.Var != null)
            {
                attrs.Add("Var", op.Var.Id);
            }
            using (new AutoXml(this, op, attrs))
            {
                this.DumpTable(op.Table);
                this.VisitChildren(n);
            }
        }

        public override void Visit(VarDefOp op, Node n)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object> {
                { 
                    "Var",
                    op.Var.Id
                }
            };
            using (new AutoXml(this, op, attrs))
            {
                this.VisitChildren(n);
            }
        }

        public override void Visit(VarRefOp op, Node n)
        {
            using (new AutoString(this, op))
            {
                this.VisitChildren(n);
                if (op.Type != null)
                {
                    this.WriteString("Type=");
                    this.WriteString(TypeHelpers.GetFullName(op.Type));
                    this.WriteString(", ");
                }
                this.WriteString("Var=");
                this.WriteString(op.Var.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        protected override void VisitConstantOp(ConstantBaseOp op, Node n)
        {
            using (new AutoString(this, op))
            {
                if (op.Value == null)
                {
                    this.WriteString("null");
                }
                else
                {
                    this.WriteString("(");
                    this.WriteString(op.Value.GetType().Name);
                    this.WriteString(")");
                    this.WriteString(string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { op.Value }));
                }
                this.VisitChildren(n);
            }
        }

        protected override void VisitDefault(Node n)
        {
            using (new AutoXml(this, n.Op))
            {
                base.VisitDefault(n);
            }
        }

        protected override void VisitJoinOp(JoinBaseOp op, Node n)
        {
            using (new AutoXml(this, op))
            {
                if (n.Children.Count > 2)
                {
                    using (new AutoXml(this, "condition"))
                    {
                        this.VisitNode(n.Child2);
                    }
                }
                using (new AutoXml(this, "input"))
                {
                    this.VisitNode(n.Child0);
                }
                using (new AutoXml(this, "input"))
                {
                    this.VisitNode(n.Child1);
                }
            }
        }

        protected override void VisitNestOp(NestBaseOp op, Node n)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object>();
            SingleStreamNestOp op2 = op as SingleStreamNestOp;
            if (op2 != null)
            {
                attrs.Add("Discriminator", (op2.Discriminator == null) ? "<null>" : op2.Discriminator.ToString());
            }
            StringBuilder sb = new StringBuilder();
            if (op2 != null)
            {
                sb.Length = 0;
                string str = string.Empty;
                foreach (Var var in op2.Keys)
                {
                    sb.Append(str);
                    sb.Append(var.Id);
                    str = ",";
                }
                if (sb.Length != 0)
                {
                    attrs.Add("Keys", sb.ToString());
                }
            }
            using (new AutoXml(this, op, attrs))
            {
                using (new AutoXml(this, "outputs"))
                {
                    foreach (Var var2 in op.Outputs)
                    {
                        this.DumpVar(var2);
                    }
                }
                foreach (CollectionInfo info in op.CollectionInfo)
                {
                    Dictionary<string, object> dictionary2 = new Dictionary<string, object> {
                        { 
                            "CollectionVar",
                            info.CollectionVar
                        }
                    };
                    if (info.DiscriminatorValue != null)
                    {
                        dictionary2.Add("DiscriminatorValue", info.DiscriminatorValue);
                    }
                    if (info.FlattenedElementVars.Count != 0)
                    {
                        dictionary2.Add("FlattenedElementVars", FormatVarList(sb, info.FlattenedElementVars));
                    }
                    if (info.Keys.Count != 0)
                    {
                        dictionary2.Add("Keys", info.Keys);
                    }
                    if (info.SortKeys.Count != 0)
                    {
                        dictionary2.Add("SortKeys", FormatVarList(sb, info.SortKeys));
                    }
                    using (new AutoXml(this, "collection", dictionary2))
                    {
                        info.ColumnMap.Accept<Dump>(ColumnMapDumper.Instance, this);
                    }
                }
                this.VisitChildren(n);
            }
        }

        private void VisitNewOp(Op op, Node n)
        {
            using (new AutoXml(this, op))
            {
                foreach (Node node in n.Children)
                {
                    using (new AutoXml(this, "argument", null))
                    {
                        this.VisitNode(node);
                    }
                }
            }
        }

        protected override void VisitScalarOpDefault(ScalarOp op, Node n)
        {
            using (new AutoString(this, op))
            {
                string str = string.Empty;
                foreach (Node node in n.Children)
                {
                    this.WriteString(str);
                    this.VisitNode(node);
                    str = ",";
                }
            }
        }

        protected override void VisitSetOp(SetOp op, Node n)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object>();
            if (OpType.UnionAll == op.OpType)
            {
                UnionAllOp op2 = (UnionAllOp) op;
                if (op2.BranchDiscriminator != null)
                {
                    attrs.Add("branchDiscriminator", op2.BranchDiscriminator);
                }
            }
            using (new AutoXml(this, op, attrs))
            {
                using (new AutoXml(this, "outputs"))
                {
                    foreach (Var var in op.Outputs)
                    {
                        this.DumpVar(var);
                    }
                }
                int num = 0;
                foreach (Node node in n.Children)
                {
                    Dictionary<string, object> dictionary2 = new Dictionary<string, object> {
                        { 
                            "VarMap",
                            op.VarMap[num++].ToString()
                        }
                    };
                    using (new AutoXml(this, "input", dictionary2))
                    {
                        this.VisitNode(node);
                    }
                }
            }
        }

        protected override void VisitSortOp(SortBaseOp op, Node n)
        {
            using (new AutoXml(this, "keys"))
            {
                foreach (System.Data.Query.InternalTrees.SortKey key in op.Keys)
                {
                    Dictionary<string, object> attrs = new Dictionary<string, object> {
                        { 
                            "Var",
                            key.Var
                        },
                        { 
                            "Ascending",
                            key.AscendingSort
                        },
                        { 
                            "Collation",
                            key.Collation
                        }
                    };
                    using (new AutoXml(this, "sortKey", attrs))
                    {
                    }
                }
            }
            this.VisitChildren(n);
        }

        internal void WriteString(string value)
        {
            this._writer.WriteString(value);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AutoString : IDisposable
        {
            private Dump _dumper;
            internal AutoString(Dump dumper, Op op)
            {
                this._dumper = dumper;
                this._dumper.WriteString(ToString(op.OpType));
                this._dumper.BeginExpression();
            }

            public void Dispose()
            {
                try
                {
                    this._dumper.EndExpression();
                }
                catch (Exception exception)
                {
                    if (!EntityUtil.IsCatchableExceptionType(exception))
                    {
                        throw;
                    }
                }
            }

            internal static string ToString(OpType op)
            {
                switch (op)
                {
                    case OpType.Constant:
                        return "Constant";

                    case OpType.InternalConstant:
                        return "InternalConstant";

                    case OpType.Null:
                        return "Null";

                    case OpType.ConstantPredicate:
                        return "ConstantPredicate";

                    case OpType.VarRef:
                        return "VarRef";

                    case OpType.GT:
                        return "GT";

                    case OpType.GE:
                        return "GE";

                    case OpType.LE:
                        return "LE";

                    case OpType.LT:
                        return "LT";

                    case OpType.EQ:
                        return "EQ";

                    case OpType.NE:
                        return "NE";

                    case OpType.Like:
                        return "Like";

                    case OpType.Plus:
                        return "Plus";

                    case OpType.Minus:
                        return "Minus";

                    case OpType.Multiply:
                        return "Multiply";

                    case OpType.Divide:
                        return "Divide";

                    case OpType.Modulo:
                        return "Modulo";

                    case OpType.UnaryMinus:
                        return "UnaryMinus";

                    case OpType.And:
                        return "And";

                    case OpType.Or:
                        return "Or";

                    case OpType.Not:
                        return "Not";

                    case OpType.IsNull:
                        return "IsNull";

                    case OpType.Case:
                        return "Case";

                    case OpType.Treat:
                        return "Treat";

                    case OpType.IsOf:
                        return "IsOf";

                    case OpType.Cast:
                        return "Cast";

                    case OpType.SoftCast:
                        return "SoftCast";

                    case OpType.Aggregate:
                        return "Aggregate";

                    case OpType.Function:
                        return "Function";

                    case OpType.RelProperty:
                        return "RelProperty";

                    case OpType.Property:
                        return "Property";

                    case OpType.NewEntity:
                        return "NewEntity";

                    case OpType.NewInstance:
                        return "NewInstance";

                    case OpType.DiscriminatedNewEntity:
                        return "DiscriminatedNewEntity";

                    case OpType.NewMultiset:
                        return "NewMultiset";

                    case OpType.NewRecord:
                        return "NewRecord";

                    case OpType.GetRefKey:
                        return "GetRefKey";

                    case OpType.GetEntityRef:
                        return "GetEntityRef";

                    case OpType.Ref:
                        return "Ref";

                    case OpType.Exists:
                        return "Exists";

                    case OpType.Element:
                        return "Element";

                    case OpType.Collect:
                        return "Collect";

                    case OpType.Deref:
                        return "Deref";

                    case OpType.Navigate:
                        return "Navigate";

                    case OpType.ScanTable:
                        return "ScanTable";

                    case OpType.ScanView:
                        return "ScanView";

                    case OpType.Filter:
                        return "Filter";

                    case OpType.Project:
                        return "Project";

                    case OpType.InnerJoin:
                        return "InnerJoin";

                    case OpType.LeftOuterJoin:
                        return "LeftOuterJoin";

                    case OpType.FullOuterJoin:
                        return "FullOuterJoin";

                    case OpType.CrossJoin:
                        return "CrossJoin";

                    case OpType.CrossApply:
                        return "CrossApply";

                    case OpType.OuterApply:
                        return "OuterApply";

                    case OpType.Unnest:
                        return "Unnest";

                    case OpType.Sort:
                        return "Sort";

                    case OpType.ConstrainedSort:
                        return "ConstrainedSort";

                    case OpType.GroupBy:
                        return "GroupBy";

                    case OpType.UnionAll:
                        return "UnionAll";

                    case OpType.Intersect:
                        return "Intersect";

                    case OpType.Except:
                        return "Except";

                    case OpType.Distinct:
                        return "Distinct";

                    case OpType.SingleRow:
                        return "SingleRow";

                    case OpType.SingleRowTable:
                        return "SingleRowTable";

                    case OpType.VarDef:
                        return "VarDef";

                    case OpType.VarDefList:
                        return "VarDefList";

                    case OpType.Leaf:
                        return "Leaf";

                    case OpType.PhysicalProject:
                        return "PhysicalProject";

                    case OpType.SingleStreamNest:
                        return "SingleStreamNest";

                    case OpType.MultiStreamNest:
                        return "MultiStreamNest";
                }
                return op.ToString();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AutoXml : IDisposable
        {
            private string _nodeName;
            private Dump _dumper;
            internal AutoXml(Dump dumper, Op op)
            {
                this._dumper = dumper;
                this._nodeName = Dump.AutoString.ToString(op.OpType);
                Dictionary<string, object> attrs = new Dictionary<string, object>();
                if (op.Type != null)
                {
                    attrs.Add("Type", TypeHelpers.GetFullName(op.Type));
                }
                this._dumper.Begin(this._nodeName, attrs);
            }

            internal AutoXml(Dump dumper, Op op, Dictionary<string, object> attrs)
            {
                this._dumper = dumper;
                this._nodeName = Dump.AutoString.ToString(op.OpType);
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                if (op.Type != null)
                {
                    dictionary.Add("Type", TypeHelpers.GetFullName(op.Type));
                }
                foreach (KeyValuePair<string, object> pair in attrs)
                {
                    dictionary.Add(pair.Key, pair.Value);
                }
                this._dumper.Begin(this._nodeName, dictionary);
            }

            internal AutoXml(Dump dumper, string nodeName) : this(dumper, nodeName, null)
            {
            }

            internal AutoXml(Dump dumper, string nodeName, Dictionary<string, object> attrs)
            {
                this._dumper = dumper;
                this._nodeName = nodeName;
                this._dumper.Begin(this._nodeName, attrs);
            }

            public void Dispose()
            {
                this._dumper.End(this._nodeName);
            }
        }

        internal class ColumnMapDumper : ColumnMapVisitor<Dump>
        {
            internal static Dump.ColumnMapDumper Instance = new Dump.ColumnMapDumper();

            private ColumnMapDumper()
            {
            }

            private void DumpCollection(CollectionColumnMap columnMap, Dump dumper)
            {
                if (columnMap.ForeignKeys.Length > 0)
                {
                    using (new Dump.AutoXml(dumper, "foreignKeys"))
                    {
                        base.VisitList<SimpleColumnMap>(columnMap.ForeignKeys, dumper);
                    }
                }
                if (columnMap.Keys.Length > 0)
                {
                    using (new Dump.AutoXml(dumper, "keys"))
                    {
                        base.VisitList<SimpleColumnMap>(columnMap.Keys, dumper);
                    }
                }
                using (new Dump.AutoXml(dumper, "element"))
                {
                    columnMap.Element.Accept<Dump>(this, dumper);
                }
            }

            private static Dictionary<string, object> GetAttributes(ColumnMap columnMap) => 
                new Dictionary<string, object> { { 
                    "Type",
                    columnMap.Type.ToString()
                } };

            internal override void Visit(ComplexTypeColumnMap columnMap, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "ComplexType", GetAttributes(columnMap)))
                {
                    if (columnMap.NullSentinel != null)
                    {
                        using (new Dump.AutoXml(dumper, "nullSentinel"))
                        {
                            columnMap.NullSentinel.Accept<Dump>(this, dumper);
                        }
                    }
                    base.VisitList<ColumnMap>(columnMap.Properties, dumper);
                }
            }

            internal override void Visit(DiscriminatedCollectionColumnMap columnMap, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "DiscriminatedCollection", GetAttributes(columnMap)))
                {
                    Dictionary<string, object> attrs = new Dictionary<string, object> {
                        { 
                            "Value",
                            columnMap.DiscriminatorValue
                        }
                    };
                    using (new Dump.AutoXml(dumper, "discriminator", attrs))
                    {
                        columnMap.Discriminator.Accept<Dump>(this, dumper);
                    }
                    this.DumpCollection(columnMap, dumper);
                }
            }

            internal override void Visit(EntityColumnMap columnMap, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "Entity", GetAttributes(columnMap)))
                {
                    using (new Dump.AutoXml(dumper, "entityIdentity"))
                    {
                        base.VisitEntityIdentity(columnMap.EntityIdentity, dumper);
                    }
                    base.VisitList<ColumnMap>(columnMap.Properties, dumper);
                }
            }

            internal override void Visit(MultipleDiscriminatorPolymorphicColumnMap columnMap, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "MultipleDiscriminatorPolymorphic", GetAttributes(columnMap)))
                {
                    using (new Dump.AutoXml(dumper, "typeDiscriminators"))
                    {
                        base.VisitList<SimpleColumnMap>(columnMap.TypeDiscriminators, dumper);
                    }
                    Dictionary<string, object> attrs = new Dictionary<string, object>();
                    foreach (KeyValuePair<EntityType, TypedColumnMap> pair in columnMap.TypeChoices)
                    {
                        attrs.Clear();
                        attrs.Add("EntityType", pair.Key);
                        using (new Dump.AutoXml(dumper, "choice", attrs))
                        {
                            pair.Value.Accept<Dump>(this, dumper);
                        }
                    }
                    using (new Dump.AutoXml(dumper, "default"))
                    {
                        base.VisitList<ColumnMap>(columnMap.Properties, dumper);
                    }
                }
            }

            internal override void Visit(RecordColumnMap columnMap, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "Record", GetAttributes(columnMap)))
                {
                    if (columnMap.NullSentinel != null)
                    {
                        using (new Dump.AutoXml(dumper, "nullSentinel"))
                        {
                            columnMap.NullSentinel.Accept<Dump>(this, dumper);
                        }
                    }
                    base.VisitList<ColumnMap>(columnMap.Properties, dumper);
                }
            }

            internal override void Visit(RefColumnMap columnMap, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "Ref", GetAttributes(columnMap)))
                {
                    using (new Dump.AutoXml(dumper, "entityIdentity"))
                    {
                        base.VisitEntityIdentity(columnMap.EntityIdentity, dumper);
                    }
                }
            }

            internal override void Visit(ScalarColumnMap columnMap, Dump dumper)
            {
                Dictionary<string, object> attributes = GetAttributes(columnMap);
                attributes.Add("CommandId", columnMap.CommandId);
                attributes.Add("ColumnPos", columnMap.ColumnPos);
                using (new Dump.AutoXml(dumper, "AssignedSimple", attributes))
                {
                }
            }

            internal override void Visit(SimpleCollectionColumnMap columnMap, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "SimpleCollection", GetAttributes(columnMap)))
                {
                    this.DumpCollection(columnMap, dumper);
                }
            }

            internal override void Visit(SimplePolymorphicColumnMap columnMap, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "SimplePolymorphic", GetAttributes(columnMap)))
                {
                    using (new Dump.AutoXml(dumper, "typeDiscriminator"))
                    {
                        columnMap.TypeDiscriminator.Accept<Dump>(this, dumper);
                    }
                    Dictionary<string, object> attrs = new Dictionary<string, object>();
                    foreach (KeyValuePair<object, TypedColumnMap> pair in columnMap.TypeChoices)
                    {
                        attrs.Clear();
                        attrs.Add("DiscriminatorValue", pair.Key);
                        using (new Dump.AutoXml(dumper, "choice", attrs))
                        {
                            pair.Value.Accept<Dump>(this, dumper);
                        }
                    }
                    using (new Dump.AutoXml(dumper, "default"))
                    {
                        base.VisitList<ColumnMap>(columnMap.Properties, dumper);
                    }
                }
            }

            internal override void Visit(VarRefColumnMap columnMap, Dump dumper)
            {
                Dictionary<string, object> attributes = GetAttributes(columnMap);
                attributes.Add("Var", columnMap.Var.Id);
                using (new Dump.AutoXml(dumper, "VarRef", attributes))
                {
                }
            }

            protected override void VisitEntityIdentity(DiscriminatedEntityIdentity entityIdentity, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "DiscriminatedEntityIdentity"))
                {
                    using (new Dump.AutoXml(dumper, "entitySetId"))
                    {
                        entityIdentity.EntitySetColumnMap.Accept<Dump>(this, dumper);
                    }
                    if (entityIdentity.Keys.Length > 0)
                    {
                        using (new Dump.AutoXml(dumper, "keys"))
                        {
                            base.VisitList<SimpleColumnMap>(entityIdentity.Keys, dumper);
                        }
                    }
                }
            }

            protected override void VisitEntityIdentity(SimpleEntityIdentity entityIdentity, Dump dumper)
            {
                using (new Dump.AutoXml(dumper, "SimpleEntityIdentity"))
                {
                    if (entityIdentity.Keys.Length > 0)
                    {
                        using (new Dump.AutoXml(dumper, "keys"))
                        {
                            base.VisitList<SimpleColumnMap>(entityIdentity.Keys, dumper);
                        }
                    }
                }
            }
        }
    }
}

