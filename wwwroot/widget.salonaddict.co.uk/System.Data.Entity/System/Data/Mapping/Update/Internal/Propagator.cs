namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class Propagator : UpdateExpressionVisitor<ChangeNode>
    {
        private System.Data.Mapping.Update.Internal.UpdateTranslator m_updateTranslator;
        private static readonly string s_visitorName = typeof(Propagator).FullName;

        private Propagator(System.Data.Mapping.Update.Internal.UpdateTranslator parent)
        {
            EntityUtil.CheckArgumentNull<System.Data.Mapping.Update.Internal.UpdateTranslator>(parent, "parent");
            this.m_updateTranslator = parent;
        }

        private static ChangeNode BuildChangeNode(DbExpression node) => 
            new ChangeNode(MetadataHelper.GetElementType(node.ResultType));

        private PropagatorResult Project(DbProjectExpression node, PropagatorResult row, TypeUsage resultType)
        {
            EntityUtil.CheckArgumentNull<DbProjectExpression>(node, "node");
            DbNewInstanceExpression projection = node.Projection as DbNewInstanceExpression;
            if (projection == null)
            {
                throw EntityUtil.NotSupported(Strings.Update_UnsupportedProjection(node.Projection.ExpressionKind));
            }
            PropagatorResult[] values = new PropagatorResult[projection.Arguments.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Evaluator.Evaluate(projection.Arguments[i], row, this);
            }
            return PropagatorResult.CreateStructuralValue(values, (StructuralType) resultType.EdmType, false);
        }

        internal static ChangeNode Propagate(System.Data.Mapping.Update.Internal.UpdateTranslator parent, DbQueryCommandTree umView)
        {
            DbExpressionVisitor<ChangeNode> visitor = new Propagator(parent);
            return umView.Query.Accept<ChangeNode>(visitor);
        }

        public override ChangeNode Visit(DbCrossJoinExpression node)
        {
            throw EntityUtil.NotSupported(Strings.Update_UnsupportedJoinType(node.ExpressionKind));
        }

        public override ChangeNode Visit(DbFilterExpression node)
        {
            EntityUtil.CheckArgumentNull<DbFilterExpression>(node, "node");
            ChangeNode node2 = BuildChangeNode(node);
            ChangeNode node3 = this.Visit(node.Input.Expression);
            node2.Inserted.AddRange(Evaluator.Filter(node.Predicate, node3.Inserted, this));
            node2.Deleted.AddRange(Evaluator.Filter(node.Predicate, node3.Deleted, this));
            node2.Placeholder = node3.Placeholder;
            return node2;
        }

        public override ChangeNode Visit(DbJoinExpression node)
        {
            EntityUtil.CheckArgumentNull<DbJoinExpression>(node, "node");
            if ((DbExpressionKind.InnerJoin != node.ExpressionKind) && (DbExpressionKind.LeftOuterJoin != node.ExpressionKind))
            {
                throw EntityUtil.NotSupported(Strings.Update_UnsupportedJoinType(node.ExpressionKind));
            }
            DbExpression expression = node.Left.Expression;
            DbExpression expression2 = node.Right.Expression;
            ChangeNode left = this.Visit(expression);
            ChangeNode right = this.Visit(expression2);
            JoinPropagator propagator = new JoinPropagator(left, right, node, this);
            return propagator.ExecutePropagation();
        }

        public override ChangeNode Visit(DbProjectExpression node)
        {
            EntityUtil.CheckArgumentNull<DbProjectExpression>(node, "node");
            ChangeNode node2 = BuildChangeNode(node);
            ChangeNode node3 = this.Visit(node.Input.Expression);
            foreach (PropagatorResult result in node3.Inserted)
            {
                node2.Inserted.Add(this.Project(node, result, node2.ElementType));
            }
            foreach (PropagatorResult result2 in node3.Deleted)
            {
                node2.Deleted.Add(this.Project(node, result2, node2.ElementType));
            }
            node2.Placeholder = this.Project(node, node3.Placeholder, node2.ElementType);
            return node2;
        }

        public override ChangeNode Visit(DbScanExpression node)
        {
            EntityUtil.CheckArgumentNull<DbScanExpression>(node, "node");
            EntitySetBase target = node.Target;
            ChangeNode extentModifications = this.UpdateTranslator.GetExtentModifications(target);
            if (extentModifications.Placeholder == null)
            {
                extentModifications.Placeholder = ExtentPlaceholderCreator.CreatePlaceholder(target, this.UpdateTranslator);
            }
            return extentModifications;
        }

        public override ChangeNode Visit(DbUnionAllExpression node)
        {
            EntityUtil.CheckArgumentNull<DbUnionAllExpression>(node, "node");
            ChangeNode node2 = BuildChangeNode(node);
            ChangeNode node3 = this.Visit(node.Left);
            ChangeNode node4 = this.Visit(node.Right);
            node2.Inserted.AddRange(node3.Inserted);
            node2.Inserted.AddRange(node4.Inserted);
            node2.Deleted.AddRange(node3.Deleted);
            node2.Deleted.AddRange(node4.Deleted);
            node2.Placeholder = node3.Placeholder;
            return node2;
        }

        internal System.Data.Mapping.Update.Internal.UpdateTranslator UpdateTranslator =>
            this.m_updateTranslator;

        protected override string VisitorName =>
            s_visitorName;

        private class Evaluator : UpdateExpressionVisitor<PropagatorResult>
        {
            private Propagator m_parent;
            private PropagatorResult m_row;
            private static readonly string s_visitorName = typeof(Propagator.Evaluator).FullName;

            private Evaluator(PropagatorResult row, Propagator parent)
            {
                EntityUtil.CheckArgumentNull<PropagatorResult>(row, "row");
                EntityUtil.CheckArgumentNull<Propagator>(parent, "parent");
                this.m_row = row;
                this.m_parent = parent;
            }

            private static object Cast(object value, Type clrPrimitiveType)
            {
                IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
                if ((value == null) || (value.GetType() == clrPrimitiveType))
                {
                    return value;
                }
                if ((value is DateTime) && (clrPrimitiveType == typeof(DateTimeOffset)))
                {
                    DateTime time = (DateTime) value;
                    return new DateTimeOffset(time.Ticks, TimeSpan.Zero);
                }
                return Convert.ChangeType(value, clrPrimitiveType, invariantCulture);
            }

            private static PropagatorResult ConvertBoolToResult(bool? booleanValue, params PropagatorResult[] inputs)
            {
                object obj2;
                if (booleanValue.HasValue)
                {
                    obj2 = booleanValue.Value;
                }
                else
                {
                    obj2 = null;
                }
                return PropagatorResult.CreateSimpleValue(PropagateUnknownAndPreserveFlags(null, inputs), obj2);
            }

            private static bool? ConvertResultToBool(PropagatorResult result)
            {
                if (result.IsNull)
                {
                    return null;
                }
                return new bool?((bool) result.GetSimpleValue());
            }

            private static PropagatorResult CreatePerservedAndKnownResult(object value) => 
                PropagatorResult.CreateSimpleValue(PropagatorFlags.NoFlags | PropagatorFlags.Preserve, value);

            internal static PropagatorResult Evaluate(DbExpression node, PropagatorResult row, Propagator parent)
            {
                DbExpressionVisitor<PropagatorResult> visitor = new Propagator.Evaluator(row, parent);
                return node.Accept<PropagatorResult>(visitor);
            }

            internal static bool EvaluatePredicate(DbExpression predicate, PropagatorResult row, Propagator parent)
            {
                Propagator.Evaluator visitor = new Propagator.Evaluator(row, parent);
                bool? nullable2 = ConvertResultToBool(predicate.Accept<PropagatorResult>(visitor));
                if (!nullable2.HasValue)
                {
                    return false;
                }
                return nullable2.GetValueOrDefault();
            }

            internal static IEnumerable<PropagatorResult> Filter(DbExpression predicate, IEnumerable<PropagatorResult> rows, Propagator parent)
            {
                foreach (PropagatorResult iteratorVariable0 in rows)
                {
                    if (EvaluatePredicate(predicate, iteratorVariable0, parent))
                    {
                        yield return iteratorVariable0;
                    }
                }
            }

            private static bool PreservedAndKnown(PropagatorResult result) => 
                (1 == ((byte) (result.PropagatorFlags & (PropagatorFlags.NoFlags | PropagatorFlags.Preserve | PropagatorFlags.Unknown))));

            private static PropagatorFlags PropagateUnknownAndPreserveFlags(PropagatorResult result, IEnumerable<PropagatorResult> inputs)
            {
                bool flag = false;
                bool flag2 = true;
                bool flag3 = true;
                foreach (PropagatorResult result2 in inputs)
                {
                    flag3 = false;
                    PropagatorFlags propagatorFlags = result2.PropagatorFlags;
                    if (((byte) ((PropagatorFlags.NoFlags | PropagatorFlags.Unknown) & propagatorFlags)) != 0)
                    {
                        flag = true;
                    }
                    if (((byte) ((PropagatorFlags.NoFlags | PropagatorFlags.Preserve) & propagatorFlags)) == 0)
                    {
                        flag2 = false;
                    }
                }
                if (flag3)
                {
                    flag2 = false;
                }
                if (result != null)
                {
                    PropagatorFlags flags2 = result.PropagatorFlags;
                    if (flag)
                    {
                        flags2 = (PropagatorFlags) ((byte) (flags2 | (PropagatorFlags.NoFlags | PropagatorFlags.Unknown)));
                    }
                    if (!flag2)
                    {
                        flags2 = (PropagatorFlags) ((byte) (((int) flags2) & 0xfe));
                    }
                    return flags2;
                }
                PropagatorFlags noFlags = PropagatorFlags.NoFlags;
                if (flag)
                {
                    noFlags = (PropagatorFlags) ((byte) (noFlags | (PropagatorFlags.NoFlags | PropagatorFlags.Unknown)));
                }
                if (flag2)
                {
                    noFlags = (PropagatorFlags) ((byte) (noFlags | (PropagatorFlags.NoFlags | PropagatorFlags.Preserve)));
                }
                return noFlags;
            }

            public override PropagatorResult Visit(DbAndExpression predicate)
            {
                EntityUtil.CheckArgumentNull<DbAndExpression>(predicate, "predicate");
                PropagatorResult result = this.Visit(predicate.Left);
                PropagatorResult result2 = this.Visit(predicate.Right);
                bool? left = ConvertResultToBool(result);
                bool? right = ConvertResultToBool(result2);
                if (((left.HasValue && !left.Value) && PreservedAndKnown(result)) || ((right.HasValue && !right.Value) && PreservedAndKnown(result2)))
                {
                    return CreatePerservedAndKnownResult(false);
                }
                return ConvertBoolToResult(EntityUtil.ThreeValuedAnd(left, right), new PropagatorResult[] { result, result2 });
            }

            public override PropagatorResult Visit(DbCaseExpression node)
            {
                PropagatorResult result2;
                int num = -1;
                int num2 = 0;
                List<PropagatorResult> inputs = new List<PropagatorResult>();
                foreach (DbExpression expression in node.When)
                {
                    PropagatorResult item = this.Visit(expression);
                    inputs.Add(item);
                    bool? nullable = ConvertResultToBool(item);
                    if (nullable.HasValue ? nullable.GetValueOrDefault() : false)
                    {
                        num = num2;
                        break;
                    }
                    num2++;
                }
                if (-1 == num)
                {
                    result2 = this.Visit(node.Else);
                }
                else
                {
                    result2 = this.Visit(node.Then[num]);
                }
                inputs.Add(result2);
                PropagatorFlags flags = PropagateUnknownAndPreserveFlags(result2, inputs);
                return result2.ReplicateResultWithNewFlags(flags);
            }

            public override PropagatorResult Visit(DbCastExpression node)
            {
                object obj2;
                PropagatorResult result = this.Visit(node.Argument);
                TypeUsage resultType = node.ResultType;
                if (!result.IsSimple || (BuiltInTypeKind.PrimitiveType != resultType.EdmType.BuiltInTypeKind))
                {
                    throw EntityUtil.NotSupported(Strings.Update_UnsupportedCastArgument(resultType.EdmType.Name));
                }
                if (result.IsNull)
                {
                    obj2 = null;
                }
                else
                {
                    try
                    {
                        obj2 = Cast(result.GetSimpleValue(), ((PrimitiveType) resultType.EdmType).ClrEquivalentType);
                    }
                    catch
                    {
                        throw;
                    }
                }
                return result.ReplicateResultWithNewValue(obj2);
            }

            public override PropagatorResult Visit(DbComparisonExpression predicate)
            {
                bool? nullable;
                EntityUtil.CheckArgumentNull<DbComparisonExpression>(predicate, "predicate");
                if (DbExpressionKind.Equals != predicate.ExpressionKind)
                {
                    throw base.ConstructNotSupportedException(predicate);
                }
                PropagatorResult result = this.Visit(predicate.Left);
                PropagatorResult result2 = this.Visit(predicate.Right);
                if (result.IsNull || result2.IsNull)
                {
                    nullable = null;
                }
                else
                {
                    object simpleValue = result.GetSimpleValue();
                    object y = result2.GetSimpleValue();
                    nullable = new bool?(CdpEqualityComparer.DefaultEqualityComparer.Equals(simpleValue, y));
                }
                return ConvertBoolToResult(nullable, new PropagatorResult[] { result, result2 });
            }

            public override PropagatorResult Visit(DbConstantExpression node) => 
                PropagatorResult.CreateSimpleValue(PropagatorFlags.NoFlags | PropagatorFlags.Preserve, node.Value);

            public override PropagatorResult Visit(DbIsNullExpression node)
            {
                PropagatorResult result = this.Visit(node.Argument);
                return ConvertBoolToResult(new bool?(result.IsNull), new PropagatorResult[] { result });
            }

            public override PropagatorResult Visit(DbIsOfExpression predicate)
            {
                bool flag;
                EntityUtil.CheckArgumentNull<DbIsOfExpression>(predicate, "predicate");
                if (DbExpressionKind.IsOfOnly != predicate.ExpressionKind)
                {
                    throw base.ConstructNotSupportedException(predicate);
                }
                PropagatorResult result = this.Visit(predicate.Argument);
                if (result.IsNull)
                {
                    flag = false;
                }
                else
                {
                    flag = result.StructuralType.EdmEquals(predicate.OfType.EdmType);
                }
                return ConvertBoolToResult(new bool?(flag), new PropagatorResult[] { result });
            }

            public override PropagatorResult Visit(DbNotExpression predicate)
            {
                EntityUtil.CheckArgumentNull<DbNotExpression>(predicate, "predicate");
                PropagatorResult result = this.Visit(predicate.Argument);
                return ConvertBoolToResult(EntityUtil.ThreeValuedNot(ConvertResultToBool(result)), new PropagatorResult[] { result });
            }

            public override PropagatorResult Visit(DbNullExpression node) => 
                PropagatorResult.CreateSimpleValue(PropagatorFlags.NoFlags | PropagatorFlags.Preserve, null);

            public override PropagatorResult Visit(DbOrExpression predicate)
            {
                EntityUtil.CheckArgumentNull<DbOrExpression>(predicate, "predicate");
                PropagatorResult result = this.Visit(predicate.Left);
                PropagatorResult result2 = this.Visit(predicate.Right);
                bool? left = ConvertResultToBool(result);
                bool? right = ConvertResultToBool(result2);
                if (((left.HasValue && left.Value) && PreservedAndKnown(result)) || ((right.HasValue && right.Value) && PreservedAndKnown(result2)))
                {
                    return CreatePerservedAndKnownResult(true);
                }
                return ConvertBoolToResult(EntityUtil.ThreeValuedOr(left, right), new PropagatorResult[] { result, result2 });
            }

            public override PropagatorResult Visit(DbPropertyExpression node)
            {
                PropagatorResult result = this.Visit(node.Instance);
                if (result.IsNull)
                {
                    return PropagatorResult.CreateSimpleValue(result.PropagatorFlags, null);
                }
                return result.GetMemberValue(node.Property);
            }

            public override PropagatorResult Visit(DbRefKeyExpression node) => 
                this.Visit(node.Argument);

            public override PropagatorResult Visit(DbTreatExpression node)
            {
                PropagatorResult result = this.Visit(node.Argument);
                if (MetadataHelper.IsSuperTypeOf(node.ResultType.EdmType, result.StructuralType))
                {
                    return result;
                }
                return PropagatorResult.CreateSimpleValue(result.PropagatorFlags, null);
            }

            public override PropagatorResult Visit(DbVariableReferenceExpression node) => 
                this.m_row;

            protected override string VisitorName =>
                s_visitorName;

        }

        private class ExtentPlaceholderCreator
        {
            private UpdateTranslator m_parent;
            private static Dictionary<PrimitiveTypeKind, object> s_typeDefaultMap = InitializeTypeDefaultMap();

            private ExtentPlaceholderCreator(UpdateTranslator parent)
            {
                EntityUtil.CheckArgumentNull<UpdateTranslator>(parent, "parent");
                this.m_parent = parent;
            }

            private PropagatorResult CreateAssociationSetPlaceholder(AssociationSet associationSet)
            {
                ReadOnlyMetadataCollection<AssociationEndMember> associationEndMembers = associationSet.ElementType.AssociationEndMembers;
                PropagatorResult[] values = new PropagatorResult[associationEndMembers.Count];
                for (int i = 0; i < associationEndMembers.Count; i++)
                {
                    AssociationEndMember member = associationEndMembers[i];
                    EntityType elementType = (EntityType) ((RefType) member.TypeUsage.EdmType).ElementType;
                    PropagatorResult[] resultArray2 = new PropagatorResult[elementType.KeyMembers.Count];
                    for (int j = 0; j < elementType.KeyMembers.Count; j++)
                    {
                        EdmMember member2 = elementType.KeyMembers[j];
                        resultArray2[j] = this.CreateMemberPlaceholder(member2);
                    }
                    RowType keyRowType = elementType.GetKeyRowType(this.m_parent.MetadataWorkspace);
                    values[i] = PropagatorResult.CreateStructuralValue(resultArray2, keyRowType, false);
                }
                return PropagatorResult.CreateStructuralValue(values, associationSet.ElementType, false);
            }

            private PropagatorResult CreateEntitySetPlaceholder(EntitySet entitySet)
            {
                EntityUtil.CheckArgumentNull<EntitySet>(entitySet, "entitySet");
                ReadOnlyMetadataCollection<EdmProperty> properties = entitySet.ElementType.Properties;
                PropagatorResult[] values = new PropagatorResult[properties.Count];
                for (int i = 0; i < properties.Count; i++)
                {
                    values[i] = this.CreateMemberPlaceholder(properties[i]);
                }
                return PropagatorResult.CreateStructuralValue(values, entitySet.ElementType, false);
            }

            private PropagatorResult CreateMemberPlaceholder(EdmMember member)
            {
                EntityUtil.CheckArgumentNull<EdmMember>(member, "member");
                return this.Visit(member);
            }

            internal static PropagatorResult CreatePlaceholder(EntitySetBase extent, UpdateTranslator parent)
            {
                EntityUtil.CheckArgumentNull<EntitySetBase>(extent, "extent");
                Propagator.ExtentPlaceholderCreator creator = new Propagator.ExtentPlaceholderCreator(parent);
                AssociationSet associationSet = extent as AssociationSet;
                if (associationSet != null)
                {
                    return creator.CreateAssociationSetPlaceholder(associationSet);
                }
                EntitySet entitySet = extent as EntitySet;
                if (entitySet == null)
                {
                    throw EntityUtil.NotSupported(Strings.Update_UnsupportedExtentType(extent.Name, extent.GetType().Name));
                }
                return creator.CreateEntitySetPlaceholder(entitySet);
            }

            private static Dictionary<PrimitiveTypeKind, object> InitializeTypeDefaultMap() => 
                new Dictionary<PrimitiveTypeKind, object>(EqualityComparer<PrimitiveTypeKind>.Default) { 
                    [PrimitiveTypeKind.Binary] = new byte[0],
                    [PrimitiveTypeKind.Boolean] = false,
                    [PrimitiveTypeKind.Byte] = (byte) 0,
                    [PrimitiveTypeKind.DateTime] = new DateTime(),
                    [PrimitiveTypeKind.Time] = new TimeSpan(),
                    [PrimitiveTypeKind.DateTimeOffset] = new DateTimeOffset(),
                    [PrimitiveTypeKind.Decimal] = 0M,
                    [PrimitiveTypeKind.Double] = 0.0,
                    [PrimitiveTypeKind.Guid] = new Guid(),
                    [PrimitiveTypeKind.Int16] = (short) 0,
                    [PrimitiveTypeKind.Int32] = 0,
                    [PrimitiveTypeKind.Int64] = 0L,
                    [PrimitiveTypeKind.Single] = 0f,
                    [PrimitiveTypeKind.SByte] = (sbyte) 0,
                    [PrimitiveTypeKind.String] = string.Empty
                };

            internal PropagatorResult Visit(EdmMember node)
            {
                TypeUsage modelTypeUsage = Helper.GetModelTypeUsage(node);
                if (BuiltInTypeKind.PrimitiveType == modelTypeUsage.EdmType.BuiltInTypeKind)
                {
                    object obj2;
                    PrimitiveTypeKind primitiveTypeKind = MetadataHelper.GetPrimitiveTypeKind(modelTypeUsage);
                    if (!s_typeDefaultMap.TryGetValue(primitiveTypeKind, out obj2))
                    {
                        obj2 = (byte) 0;
                    }
                    return PropagatorResult.CreateSimpleValue(PropagatorFlags.NoFlags, obj2);
                }
                StructuralType edmType = (StructuralType) modelTypeUsage.EdmType;
                IBaseList<EdmMember> allStructuralMembers = TypeHelpers.GetAllStructuralMembers(edmType);
                PropagatorResult[] values = new PropagatorResult[allStructuralMembers.Count];
                for (int i = 0; i < allStructuralMembers.Count; i++)
                {
                    values[i] = this.Visit(allStructuralMembers[i]);
                }
                return PropagatorResult.CreateStructuralValue(values, edmType, false);
            }
        }

        private class JoinPropagator
        {
            private Set<CompositeKey> m_allKeys;
            private Dictionary<Ops, Ops> m_deleteRules;
            private Dictionary<Ops, Ops> m_insertRules;
            private DbJoinExpression m_joinExpression;
            private ChangeNode m_left;
            private Dictionary<CompositeKey, PropagatorResult> m_leftDeletes;
            private Dictionary<CompositeKey, PropagatorResult> m_leftInserts;
            private Dictionary<PropagatorResult, int> m_leftKeyMap;
            private DbExpression[] m_leftProperties;
            private Propagator m_parent;
            private ChangeNode m_right;
            private Dictionary<CompositeKey, PropagatorResult> m_rightDeletes;
            private Dictionary<CompositeKey, PropagatorResult> m_rightInserts;
            private Dictionary<PropagatorResult, int> m_rightKeyMap;
            private DbExpression[] m_rightProperties;
            private static object s_initializeRulesLockObject = new object();
            private static Dictionary<Ops, Ops> s_innerJoinDeleteRules;
            private static Dictionary<Ops, Ops> s_innerJoinInsertRules;
            private static Dictionary<Ops, Ops> s_leftOuterJoinDeleteRules;
            private static Dictionary<Ops, Ops> s_leftOuterJoinInsertRules;
            private static bool s_rulesInitialized;

            internal JoinPropagator(ChangeNode left, ChangeNode right, DbJoinExpression node, Propagator parent)
            {
                EntityUtil.CheckArgumentNull<ChangeNode>(left, "left");
                EntityUtil.CheckArgumentNull<ChangeNode>(right, "right");
                EntityUtil.CheckArgumentNull<DbJoinExpression>(node, "node");
                EntityUtil.CheckArgumentNull<Propagator>(parent, "parent");
                this.m_joinExpression = node;
                this.m_left = left;
                this.m_right = right;
                this.m_parent = parent;
                this.Initialize();
            }

            internal ChangeNode ExecutePropagation()
            {
                ChangeNode result = Propagator.BuildChangeNode(this.m_joinExpression);
                foreach (CompositeKey key in this.m_allKeys)
                {
                    this.Propagate(key, result);
                }
                result.Placeholder = Join(this.m_left.Placeholder, this.m_right.Placeholder, result);
                return result;
            }

            private PropagatorResult[] GetKeyConstants(PropagatorResult change, DbExpression[] properties)
            {
                PropagatorResult[] resultArray = new PropagatorResult[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    resultArray[i] = Propagator.Evaluator.Evaluate(properties[i], change, this.m_parent);
                }
                return resultArray;
            }

            private Dictionary<PropagatorResult, int> GetKeyMap(PropagatorResult placeholder, DbExpression[] properties)
            {
                Dictionary<PropagatorResult, int> dictionary = new Dictionary<PropagatorResult, int>();
                for (int i = 0; i < properties.Length; i++)
                {
                    DbExpression node = properties[i];
                    PropagatorResult result = Propagator.Evaluator.Evaluate(node, placeholder, this.m_parent);
                    dictionary[result] = i;
                }
                return dictionary;
            }

            private void Initialize()
            {
                lock (s_initializeRulesLockObject)
                {
                    if (!s_rulesInitialized)
                    {
                        InitializeRules();
                        s_rulesInitialized = true;
                    }
                }
                JoinPredicateVisitor.ExtractEquijoinProperties(this.m_joinExpression.JoinCondition, out this.m_leftProperties, out this.m_rightProperties);
                this.m_leftKeyMap = this.GetKeyMap(this.m_left.Placeholder, this.m_leftProperties);
                this.m_rightKeyMap = this.GetKeyMap(this.m_right.Placeholder, this.m_rightProperties);
                if (DbExpressionKind.InnerJoin == this.m_joinExpression.ExpressionKind)
                {
                    this.m_insertRules = s_innerJoinInsertRules;
                    this.m_deleteRules = s_innerJoinDeleteRules;
                }
                else
                {
                    this.m_insertRules = s_leftOuterJoinInsertRules;
                    this.m_deleteRules = s_leftOuterJoinDeleteRules;
                }
                this.m_allKeys = new Set<CompositeKey>(this.m_parent.UpdateTranslator.KeyComparer);
                this.m_leftInserts = this.ProcessKeys(this.m_left.Inserted, this.m_leftProperties);
                this.m_leftDeletes = this.ProcessKeys(this.m_left.Deleted, this.m_leftProperties);
                this.m_rightInserts = this.ProcessKeys(this.m_right.Inserted, this.m_rightProperties);
                this.m_rightDeletes = this.ProcessKeys(this.m_right.Deleted, this.m_rightProperties);
            }

            private static void InitializeRule(Ops input, Ops joinInsert, Ops joinDelete, Ops lojInsert, Ops lojDelete)
            {
                s_innerJoinInsertRules.Add(input, joinInsert);
                s_innerJoinDeleteRules.Add(input, joinDelete);
                s_leftOuterJoinInsertRules.Add(input, lojInsert);
                s_leftOuterJoinDeleteRules.Add(input, lojDelete);
            }

            private static void InitializeRules()
            {
                s_innerJoinInsertRules = new Dictionary<Ops, Ops>(EqualityComparer<Ops>.Default);
                s_innerJoinDeleteRules = new Dictionary<Ops, Ops>(EqualityComparer<Ops>.Default);
                s_leftOuterJoinInsertRules = new Dictionary<Ops, Ops>(EqualityComparer<Ops>.Default);
                s_leftOuterJoinDeleteRules = new Dictionary<Ops, Ops>(EqualityComparer<Ops>.Default);
                InitializeRule(Ops.LeftDelete | Ops.LeftInsert | Ops.RightDelete | Ops.RightInsert, Ops.LeftInsert | Ops.RightInsert, Ops.LeftDelete | Ops.RightDelete, Ops.LeftInsert | Ops.RightInsert, Ops.LeftDelete | Ops.RightDelete);
                InitializeRule(Ops.LeftDelete | Ops.RightDelete, Ops.Nothing, Ops.LeftDelete | Ops.RightDelete, Ops.Nothing, Ops.LeftDelete | Ops.RightDelete);
                InitializeRule(Ops.LeftInsert | Ops.RightInsert, Ops.LeftInsert | Ops.RightInsert, Ops.Nothing, Ops.LeftInsert | Ops.RightInsert, Ops.Nothing);
                InitializeRule(Ops.LeftDelete | Ops.LeftInsert, Ops.LeftInsert | Ops.RightUnknown, Ops.LeftDelete | Ops.RightUnknown, Ops.LeftInsert | Ops.RightUnknown, Ops.LeftDelete | Ops.RightUnknown);
                InitializeRule(Ops.Nothing | Ops.RightDelete | Ops.RightInsert, Ops.LeftUnknown | Ops.RightInsert, Ops.LeftUnknown | Ops.RightDelete, Ops.LeftUnknown | Ops.RightInsert, Ops.LeftUnknown | Ops.RightDelete);
                InitializeRule(Ops.LeftDelete | Ops.LeftInsert | Ops.RightDelete, Ops.Nothing, Ops.Nothing, Ops.LeftInsert | Ops.RightNullModified, Ops.LeftDelete | Ops.RightDelete);
                InitializeRule(Ops.LeftDelete | Ops.LeftInsert | Ops.RightInsert, Ops.Nothing, Ops.Nothing, Ops.LeftInsert | Ops.RightInsert, Ops.LeftDelete | Ops.RightNullModified);
                InitializeRule(Ops.LeftDelete, Ops.Nothing, Ops.Nothing, Ops.Nothing, Ops.LeftDelete | Ops.RightNullPreserve);
                InitializeRule(Ops.LeftInsert, Ops.Nothing, Ops.Nothing, Ops.LeftInsert | Ops.RightNullModified, Ops.Nothing);
                InitializeRule(Ops.Nothing | Ops.RightDelete, Ops.Nothing, Ops.Nothing, Ops.LeftUnknown | Ops.RightNullModified, Ops.LeftUnknown | Ops.RightDelete);
                InitializeRule(Ops.Nothing | Ops.RightInsert, Ops.Nothing, Ops.Nothing, Ops.LeftUnknown | Ops.RightInsert, Ops.LeftUnknown | Ops.RightNullModified);
                InitializeRule(Ops.LeftDelete | Ops.RightDelete | Ops.RightInsert, Ops.Nothing, Ops.Nothing, Ops.Nothing, Ops.Nothing);
                InitializeRule(Ops.LeftDelete | Ops.RightInsert, Ops.Nothing, Ops.Nothing, Ops.Nothing, Ops.Nothing);
                InitializeRule(Ops.LeftInsert | Ops.RightDelete | Ops.RightInsert, Ops.Nothing, Ops.Nothing, Ops.Nothing, Ops.Nothing);
                InitializeRule(Ops.LeftInsert | Ops.RightDelete, Ops.Nothing, Ops.Nothing, Ops.Nothing, Ops.Nothing);
            }

            private static PropagatorResult Join(PropagatorResult left, PropagatorResult right, ChangeNode result) => 
                PropagatorResult.CreateStructuralValue(new PropagatorResult[] { left, right }, (StructuralType) result.ElementType.EdmType, false);

            private PropagatorResult LeftPlaceholder(CompositeKey key, PopulateMode mode) => 
                PlaceholderPopulator.Populate(this.m_left.Placeholder, key, this.m_leftKeyMap, mode, this.m_parent.UpdateTranslator);

            private Dictionary<CompositeKey, PropagatorResult> ProcessKeys(IEnumerable<PropagatorResult> instances, DbExpression[] properties)
            {
                Dictionary<CompositeKey, PropagatorResult> dictionary = new Dictionary<CompositeKey, PropagatorResult>(this.m_parent.UpdateTranslator.KeyComparer);
                foreach (PropagatorResult result in instances)
                {
                    CompositeKey element = new CompositeKey(this.GetKeyConstants(result, properties));
                    if (!this.m_allKeys.Contains(element))
                    {
                        this.m_allKeys.Add(element);
                    }
                    dictionary[element] = result;
                }
                return dictionary;
            }

            private void Propagate(CompositeKey key, ChangeNode result)
            {
                PropagatorResult result2 = null;
                PropagatorResult result3 = null;
                PropagatorResult result4 = null;
                PropagatorResult result5 = null;
                Ops nothing = Ops.Nothing;
                if (this.m_leftInserts.TryGetValue(key, out result2))
                {
                    nothing |= Ops.LeftInsert;
                }
                if (this.m_leftDeletes.TryGetValue(key, out result3))
                {
                    nothing |= Ops.LeftDelete;
                }
                if (this.m_rightInserts.TryGetValue(key, out result4))
                {
                    nothing |= Ops.Nothing | Ops.RightInsert;
                }
                if (this.m_rightDeletes.TryGetValue(key, out result5))
                {
                    nothing |= Ops.Nothing | Ops.RightDelete;
                }
                Ops ops2 = this.m_insertRules[nothing];
                Ops ops3 = this.m_deleteRules[nothing];
                if (((Ops.Nothing | Ops.Unsupported) == ops2) || ((Ops.Nothing | Ops.Unsupported) == ops3))
                {
                    throw EntityUtil.NotSupported();
                }
                if ((Ops.LeftUnknown & ops2) != Ops.Nothing)
                {
                    result2 = this.LeftPlaceholder(key, PopulateMode.Unknown);
                }
                if ((Ops.LeftUnknown & ops3) != Ops.Nothing)
                {
                    result3 = this.LeftPlaceholder(key, PopulateMode.Unknown);
                }
                if (((Ops.Nothing | Ops.RightNullModified) & ops2) != Ops.Nothing)
                {
                    result4 = this.RightPlaceholder(key, PopulateMode.NullModified);
                }
                else if (((Ops.Nothing | Ops.RightNullPreserve) & ops2) != Ops.Nothing)
                {
                    result4 = this.RightPlaceholder(key, PopulateMode.NullPreserve);
                }
                else if (((Ops.Nothing | Ops.RightUnknown) & ops2) != Ops.Nothing)
                {
                    result4 = this.RightPlaceholder(key, PopulateMode.Unknown);
                }
                if (((Ops.Nothing | Ops.RightNullModified) & ops3) != Ops.Nothing)
                {
                    result5 = this.RightPlaceholder(key, PopulateMode.NullModified);
                }
                else if (((Ops.Nothing | Ops.RightNullPreserve) & ops3) != Ops.Nothing)
                {
                    result5 = this.RightPlaceholder(key, PopulateMode.NullPreserve);
                }
                else if (((Ops.Nothing | Ops.RightUnknown) & ops3) != Ops.Nothing)
                {
                    result5 = this.RightPlaceholder(key, PopulateMode.Unknown);
                }
                if ((result2 != null) && (result4 != null))
                {
                    result.Inserted.Add(Join(result2, result4, result));
                }
                if ((result3 != null) && (result3 != null))
                {
                    result.Deleted.Add(Join(result3, result5, result));
                }
            }

            private PropagatorResult RightPlaceholder(CompositeKey key, PopulateMode mode) => 
                PlaceholderPopulator.Populate(this.m_right.Placeholder, key, this.m_rightKeyMap, mode, this.m_parent.UpdateTranslator);

            private class JoinPredicateVisitor : UpdateExpressionVisitor<object>
            {
                private List<DbExpression> m_leftProperties;
                private List<DbExpression> m_rightProperties;
                private static readonly string s_visitorName = typeof(Propagator.JoinPropagator.JoinPredicateVisitor).FullName;

                private JoinPredicateVisitor(List<DbExpression> leftProperties, List<DbExpression> rightProperties)
                {
                    EntityUtil.CheckArgumentNull<List<DbExpression>>(leftProperties, "leftProperties");
                    EntityUtil.CheckArgumentNull<List<DbExpression>>(rightProperties, "rightProperties");
                    this.m_leftProperties = leftProperties;
                    this.m_rightProperties = rightProperties;
                }

                internal static void ExtractEquijoinProperties(DbExpression joinCondition, out DbExpression[] leftProperties, out DbExpression[] rightProperties)
                {
                    EntityUtil.CheckArgumentNull<DbExpression>(joinCondition, "joinCondition");
                    List<DbExpression> list = new List<DbExpression>();
                    List<DbExpression> list2 = new List<DbExpression>();
                    DbExpressionVisitor<object> visitor = new Propagator.JoinPropagator.JoinPredicateVisitor(list, list2);
                    joinCondition.Accept<object>(visitor);
                    leftProperties = list.ToArray();
                    rightProperties = list2.ToArray();
                }

                public override object Visit(DbAndExpression node)
                {
                    EntityUtil.CheckArgumentNull<DbAndExpression>(node, "node");
                    this.Visit(node.Left);
                    this.Visit(node.Right);
                    return null;
                }

                public override object Visit(DbComparisonExpression node)
                {
                    EntityUtil.CheckArgumentNull<DbComparisonExpression>(node, "node");
                    if (DbExpressionKind.Equals != node.ExpressionKind)
                    {
                        throw base.ConstructNotSupportedException(node);
                    }
                    this.m_leftProperties.Add(node.Left);
                    this.m_rightProperties.Add(node.Right);
                    return null;
                }

                protected override string VisitorName =>
                    s_visitorName;
            }

            [Flags]
            private enum Ops : uint
            {
                LeftDelete = 2,
                LeftDeleteJoinRightDelete = 10,
                LeftDeleteNullModifiedExtended = 130,
                LeftDeleteNullPreserveExtended = 0x102,
                LeftDeleteUnknownExtended = 0x202,
                LeftInsert = 1,
                LeftInsertJoinRightInsert = 5,
                LeftInsertNullModifiedExtended = 0x81,
                LeftInsertNullPreserveExtended = 0x101,
                LeftInsertUnknownExtended = 0x201,
                LeftUnknown = 0x20,
                LeftUnknownNullModifiedExtended = 160,
                LeftUnknownNullPreserveExtended = 0x120,
                LeftUpdate = 3,
                Nothing = 0,
                RightDelete = 8,
                RightDeleteUnknownExtended = 40,
                RightInsert = 4,
                RightInsertUnknownExtended = 0x24,
                RightNullModified = 0x80,
                RightNullPreserve = 0x100,
                RightUnknown = 0x200,
                RightUpdate = 12,
                Unsupported = 0x1000
            }

            private class PlaceholderPopulator
            {
                private readonly PropagatorFlags m_flags;
                private readonly bool m_isNull;
                private readonly CompositeKey m_key;
                private readonly Dictionary<PropagatorResult, int> m_keyMap;
                private readonly UpdateTranslator m_translator;

                private PlaceholderPopulator(CompositeKey key, Dictionary<PropagatorResult, int> keyMap, Propagator.JoinPropagator.PopulateMode mode, UpdateTranslator translator)
                {
                    this.m_key = key;
                    this.m_keyMap = keyMap;
                    this.m_isNull = (mode == Propagator.JoinPropagator.PopulateMode.NullModified) || (mode == Propagator.JoinPropagator.PopulateMode.NullPreserve);
                    bool preserve = (mode == Propagator.JoinPropagator.PopulateMode.NullPreserve) || (mode == Propagator.JoinPropagator.PopulateMode.Unknown);
                    this.m_flags = this.CreateFlags(preserve);
                    this.m_translator = translator;
                }

                private PropagatorFlags CreateFlags(bool preserve)
                {
                    PropagatorFlags noFlags = PropagatorFlags.NoFlags;
                    if (!this.m_isNull)
                    {
                        noFlags = (PropagatorFlags) ((byte) (noFlags | (PropagatorFlags.NoFlags | PropagatorFlags.Unknown)));
                    }
                    if (preserve)
                    {
                        noFlags = (PropagatorFlags) ((byte) (noFlags | (PropagatorFlags.NoFlags | PropagatorFlags.Preserve)));
                    }
                    return noFlags;
                }

                internal static PropagatorResult Populate(PropagatorResult placeholder, CompositeKey key, Dictionary<PropagatorResult, int> keyMap, Propagator.JoinPropagator.PopulateMode mode, UpdateTranslator translator)
                {
                    EntityUtil.CheckArgumentNull<PropagatorResult>(placeholder, "placeholder");
                    EntityUtil.CheckArgumentNull<CompositeKey>(key, "key");
                    EntityUtil.CheckArgumentNull<Dictionary<PropagatorResult, int>>(keyMap, "keyMap");
                    EntityUtil.CheckArgumentNull<UpdateTranslator>(translator, "translator");
                    Propagator.JoinPropagator.PlaceholderPopulator populator = new Propagator.JoinPropagator.PlaceholderPopulator(key, keyMap, mode, translator);
                    return populator.Visit(placeholder);
                }

                public PropagatorResult Visit(PropagatorResult node)
                {
                    int num;
                    EntityUtil.CheckArgumentNull<PropagatorResult>(node, "node");
                    if (this.m_keyMap.TryGetValue(node, out num))
                    {
                        return this.m_key.KeyComponents[num].Key;
                    }
                    if (node.IsSimple)
                    {
                        object obj2 = this.m_isNull ? null : node.GetSimpleValue();
                        return PropagatorResult.CreateSimpleValue(this.m_flags, obj2);
                    }
                    PropagatorResult[] values = new PropagatorResult[TypeHelpers.GetAllStructuralMembers(node.StructuralType).Count];
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = this.Visit(node.GetMemberValue(i));
                    }
                    return PropagatorResult.CreateStructuralValue(values, node.StructuralType, false);
                }
            }

            private enum PopulateMode
            {
                NullModified,
                NullPreserve,
                Unknown
            }
        }
    }
}

