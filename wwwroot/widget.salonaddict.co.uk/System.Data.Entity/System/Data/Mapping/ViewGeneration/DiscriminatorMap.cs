namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal class DiscriminatorMap
    {
        internal readonly DbPropertyExpression Discriminator;
        internal readonly System.Data.Metadata.Edm.EntitySet EntitySet;
        internal readonly ReadOnlyCollection<KeyValuePair<EdmProperty, DbExpression>> PropertyMap;
        internal readonly ReadOnlyCollection<KeyValuePair<RelProperty, DbExpression>> RelPropertyMap;
        internal readonly ReadOnlyCollection<KeyValuePair<object, EntityType>> TypeMap;

        private DiscriminatorMap(DbPropertyExpression discriminator, List<KeyValuePair<object, EntityType>> typeMap, Dictionary<EdmProperty, DbExpression> propertyMap, Dictionary<RelProperty, DbExpression> relPropertyMap, System.Data.Metadata.Edm.EntitySet entitySet)
        {
            this.Discriminator = discriminator;
            this.TypeMap = typeMap.AsReadOnly();
            this.PropertyMap = propertyMap.ToList<KeyValuePair<EdmProperty, DbExpression>>().AsReadOnly();
            this.RelPropertyMap = relPropertyMap.ToList<KeyValuePair<RelProperty, DbExpression>>().AsReadOnly();
            this.EntitySet = entitySet;
        }

        private static bool CheckForMissingRelProperties(Dictionary<RelProperty, DbExpression> relPropertyMap, Dictionary<EntityType, List<RelProperty>> typeToRelPropertyMap)
        {
            foreach (RelProperty property in relPropertyMap.Keys)
            {
                foreach (KeyValuePair<EntityType, List<RelProperty>> pair in typeToRelPropertyMap)
                {
                    if (pair.Key.IsSubtypeOf(property.FromEnd.TypeUsage.EdmType) && !pair.Value.Contains(property))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool ExpressionsCompatible(DbExpression x, DbExpression y)
        {
            if (x.ExpressionKind == y.ExpressionKind)
            {
                switch (x.ExpressionKind)
                {
                    case DbExpressionKind.Property:
                    {
                        DbPropertyExpression expression = (DbPropertyExpression) x;
                        DbPropertyExpression expression2 = (DbPropertyExpression) y;
                        if (expression.Property != expression2.Property)
                        {
                            return false;
                        }
                        return ExpressionsCompatible(expression.Instance, expression2.Instance);
                    }
                    case DbExpressionKind.Ref:
                    {
                        DbRefExpression expression5 = (DbRefExpression) x;
                        DbRefExpression expression6 = (DbRefExpression) y;
                        if (!expression5.EntitySet.EdmEquals(expression6.EntitySet))
                        {
                            return false;
                        }
                        return ExpressionsCompatible(expression5.Argument, expression6.Argument);
                    }
                    case DbExpressionKind.VariableReference:
                        return (((DbVariableReferenceExpression) x).VariableName == ((DbVariableReferenceExpression) y).VariableName);

                    case DbExpressionKind.NewInstance:
                    {
                        DbNewInstanceExpression expression3 = (DbNewInstanceExpression) x;
                        DbNewInstanceExpression expression4 = (DbNewInstanceExpression) y;
                        if (!expression3.ResultType.EdmType.EdmEquals(expression4.ResultType.EdmType))
                        {
                            return false;
                        }
                        for (int i = 0; i < expression3.Arguments.Count; i++)
                        {
                            if (!ExpressionsCompatible(expression3.Arguments[i], expression4.Arguments[i]))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private static IEnumerable<DbExpression> FlattenOr(DbExpression expression)
        {
            if (expression.ExpressionKind == DbExpressionKind.Or)
            {
                return CommandTreeUtils.FlattenAssociativeExpression(expression);
            }
            return new DbExpression[] { expression };
        }

        internal static bool TryCreateDiscriminatorMap(System.Data.Metadata.Edm.EntitySet entitySet, DbExpression queryView, out DiscriminatorMap discriminatorMap)
        {
            EntityType type2;
            discriminatorMap = null;
            if (queryView.ExpressionKind != DbExpressionKind.Project)
            {
                return false;
            }
            DbProjectExpression expression = (DbProjectExpression) queryView;
            if (expression.Projection.ExpressionKind != DbExpressionKind.Case)
            {
                return false;
            }
            DbCaseExpression projection = (DbCaseExpression) expression.Projection;
            if (expression.Projection.ResultType.EdmType.BuiltInTypeKind != BuiltInTypeKind.EntityType)
            {
                return false;
            }
            EdmProperty property = null;
            HashSet<object> source = new HashSet<object>();
            if (expression.Input.Expression.ExpressionKind != DbExpressionKind.Filter)
            {
                return false;
            }
            DbFilterExpression expression3 = (DbFilterExpression) expression.Input.Expression;
            foreach (DbExpression expression4 in FlattenOr(expression3.Predicate))
            {
                DbPropertyExpression expression5;
                object obj2;
                if (!TryMatchPropertyEqualsValue(expression4, expression3.Input.VariableName, out expression5, out obj2))
                {
                    return false;
                }
                if (property == null)
                {
                    property = (EdmProperty) expression5.Property;
                }
                else if (property != expression5.Property)
                {
                    return false;
                }
                source.Add(obj2);
            }
            List<KeyValuePair<object, EntityType>> typeMap = new List<KeyValuePair<object, EntityType>>();
            Dictionary<EdmProperty, DbExpression> propertyMap = new Dictionary<EdmProperty, DbExpression>();
            Dictionary<RelProperty, DbExpression> relPropertyMap = new Dictionary<RelProperty, DbExpression>();
            Dictionary<EntityType, List<RelProperty>> typeToRelPropertyMap = new Dictionary<EntityType, List<RelProperty>>();
            DbPropertyExpression discriminator = null;
            for (int i = 0; i < projection.When.Count; i++)
            {
                DbPropertyExpression expression9;
                object obj3;
                EntityType type;
                DbExpression expression7 = projection.When[i];
                DbExpression then = projection.Then[i];
                string variableName = expression.Input.VariableName;
                if (!TryMatchPropertyEqualsValue(expression7, variableName, out expression9, out obj3))
                {
                    return false;
                }
                if (property == null)
                {
                    property = (EdmProperty) expression9.Property;
                }
                else if (property != expression9.Property)
                {
                    return false;
                }
                discriminator = expression9;
                if (!TryMatchEntityTypeConstructor(then, propertyMap, relPropertyMap, typeToRelPropertyMap, out type))
                {
                    return false;
                }
                typeMap.Add(new KeyValuePair<object, EntityType>(obj3, type));
                source.Remove(obj3);
            }
            if (1 != source.Count)
            {
                return false;
            }
            if ((projection.Else == null) || !TryMatchEntityTypeConstructor(projection.Else, propertyMap, relPropertyMap, typeToRelPropertyMap, out type2))
            {
                return false;
            }
            typeMap.Add(new KeyValuePair<object, EntityType>(source.Single<object>(), type2));
            if (!CheckForMissingRelProperties(relPropertyMap, typeToRelPropertyMap))
            {
                return false;
            }
            int num2 = (from map in typeMap select map.Key).Distinct<object>(TrailingSpaceComparer.Instance).Count<object>();
            int count = typeMap.Count;
            if (num2 != count)
            {
                return false;
            }
            discriminatorMap = new DiscriminatorMap(discriminator, typeMap, propertyMap, relPropertyMap, entitySet);
            return true;
        }

        private static bool TryMatchConstant(DbExpression expression, out object value)
        {
            if (expression.ExpressionKind == DbExpressionKind.Constant)
            {
                value = ((DbConstantExpression) expression).Value;
                return true;
            }
            if ((expression.ExpressionKind == DbExpressionKind.Cast) && (expression.ResultType.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType))
            {
                DbCastExpression expression2 = (DbCastExpression) expression;
                if (TryMatchConstant(expression2.Argument, out value))
                {
                    PrimitiveType edmType = (PrimitiveType) expression.ResultType.EdmType;
                    value = Convert.ChangeType(value, edmType.ClrEquivalentType, CultureInfo.InvariantCulture);
                    return true;
                }
            }
            value = null;
            return false;
        }

        private static bool TryMatchEntityTypeConstructor(DbExpression then, Dictionary<EdmProperty, DbExpression> propertyMap, Dictionary<RelProperty, DbExpression> relPropertyMap, Dictionary<EntityType, List<RelProperty>> typeToRelPropertyMap, out EntityType entityType)
        {
            if (then.ExpressionKind != DbExpressionKind.NewInstance)
            {
                entityType = null;
                return false;
            }
            DbNewInstanceExpression expression = (DbNewInstanceExpression) then;
            entityType = (EntityType) expression.ResultType.EdmType;
            for (int i = 0; i < entityType.Properties.Count; i++)
            {
                DbExpression expression3;
                EdmProperty key = entityType.Properties[i];
                DbExpression x = expression.Arguments[i];
                if (propertyMap.TryGetValue(key, out expression3))
                {
                    if (!ExpressionsCompatible(x, expression3))
                    {
                        return false;
                    }
                }
                else
                {
                    propertyMap.Add(key, x);
                }
            }
            if (expression.HasRelatedEntityReferences)
            {
                List<RelProperty> list;
                if (!typeToRelPropertyMap.TryGetValue(entityType, out list))
                {
                    list = new List<RelProperty>();
                    typeToRelPropertyMap[entityType] = list;
                }
                foreach (DbRelatedEntityRef ref2 in expression.RelatedEntityReferences)
                {
                    DbExpression expression5;
                    RelProperty property2 = new RelProperty((RelationshipType) ref2.TargetEnd.DeclaringType, ref2.SourceEnd, ref2.TargetEnd);
                    DbExpression targetEntityReference = ref2.TargetEntityReference;
                    if (relPropertyMap.TryGetValue(property2, out expression5))
                    {
                        if (!ExpressionsCompatible(targetEntityReference, expression5))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        relPropertyMap.Add(property2, targetEntityReference);
                    }
                    list.Add(property2);
                }
            }
            return true;
        }

        private static bool TryMatchPropertyEqualsValue(DbExpression expression, string propertyVariable, out DbPropertyExpression property, out object value)
        {
            property = null;
            value = null;
            if (expression.ExpressionKind != DbExpressionKind.Equals)
            {
                return false;
            }
            DbBinaryExpression expression2 = (DbBinaryExpression) expression;
            if (expression2.Left.ExpressionKind != DbExpressionKind.Property)
            {
                return false;
            }
            property = (DbPropertyExpression) expression2.Left;
            if (!TryMatchConstant(expression2.Right, out value))
            {
                return false;
            }
            return ((property.Instance.ExpressionKind == DbExpressionKind.VariableReference) && (((DbVariableReferenceExpression) property.Instance).VariableName == propertyVariable));
        }
    }
}

