namespace System.Data.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.Provider;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal abstract class ChangeDirector
    {
        protected ChangeDirector()
        {
        }

        internal abstract void AppendDeleteText(TrackedObject item, StringBuilder appendTo);
        internal abstract void AppendInsertText(TrackedObject item, StringBuilder appendTo);
        internal abstract void AppendUpdateText(TrackedObject item, StringBuilder appendTo);
        internal static ChangeDirector CreateChangeDirector(DataContext context) => 
            new StandardChangeDirector(context);

        internal abstract int Delete(TrackedObject item);
        internal abstract int DynamicDelete(TrackedObject item);
        internal abstract int DynamicInsert(TrackedObject item);
        internal abstract int DynamicUpdate(TrackedObject item);
        internal abstract int Insert(TrackedObject item);
        internal abstract int Update(TrackedObject item);

        internal class StandardChangeDirector : ChangeDirector
        {
            private DataContext context;

            internal StandardChangeDirector(DataContext context)
            {
                this.context = context;
            }

            internal override void AppendDeleteText(TrackedObject item, StringBuilder appendTo)
            {
                if (item.Type.Table.DeleteMethod != null)
                {
                    appendTo.Append(System.Data.Linq.Strings.DeleteCallbackComment);
                }
                else
                {
                    Expression deleteCommand = this.GetDeleteCommand(item);
                    appendTo.Append(this.context.Provider.GetQueryText(deleteCommand));
                    appendTo.AppendLine();
                }
            }

            internal override void AppendInsertText(TrackedObject item, StringBuilder appendTo)
            {
                if (item.Type.Table.InsertMethod != null)
                {
                    appendTo.Append(System.Data.Linq.Strings.InsertCallbackComment);
                }
                else
                {
                    Expression insertCommand = this.GetInsertCommand(item);
                    appendTo.Append(this.context.Provider.GetQueryText(insertCommand));
                    appendTo.AppendLine();
                }
            }

            internal override void AppendUpdateText(TrackedObject item, StringBuilder appendTo)
            {
                if (item.Type.Table.UpdateMethod != null)
                {
                    appendTo.Append(System.Data.Linq.Strings.UpdateCallbackComment);
                }
                else
                {
                    Expression updateCommand = this.GetUpdateCommand(item);
                    appendTo.Append(this.context.Provider.GetQueryText(updateCommand));
                    appendTo.AppendLine();
                }
            }

            private static void AutoSyncMembers(object[] syncResults, TrackedObject item, UpdateType updateType)
            {
                if (syncResults != null)
                {
                    int num = 0;
                    foreach (MetaDataMember member in GetAutoSyncMembers(item.Type, updateType))
                    {
                        object obj2 = syncResults[num++];
                        object current = item.Current;
                        if ((member.Member is PropertyInfo) && ((PropertyInfo) member.Member).CanWrite)
                        {
                            member.MemberAccessor.SetBoxedValue(ref current, DBConvert.ChangeType(obj2, member.Type));
                        }
                        else
                        {
                            member.StorageAccessor.SetBoxedValue(ref current, DBConvert.ChangeType(obj2, member.Type));
                        }
                    }
                }
            }

            private Expression CreateAutoSync(List<MetaDataMember> membersToSync, Expression source)
            {
                int num = 0;
                Expression[] initializers = new Expression[membersToSync.Count];
                foreach (MetaDataMember member in membersToSync)
                {
                    initializers[num++] = Expression.Convert(this.GetMemberExpression(source, member.Member), typeof(object));
                }
                return Expression.NewArrayInit(typeof(object), initializers);
            }

            internal override int Delete(TrackedObject item)
            {
                if (item.Type.Table.DeleteMethod == null)
                {
                    return this.DynamicDelete(item);
                }
                try
                {
                    item.Type.Table.DeleteMethod.Invoke(this.context, new object[] { item.Current });
                }
                catch (TargetInvocationException exception)
                {
                    if (exception.InnerException != null)
                    {
                        throw exception.InnerException;
                    }
                    throw;
                }
                return 1;
            }

            internal override int DynamicDelete(TrackedObject item)
            {
                Expression deleteCommand = this.GetDeleteCommand(item);
                int returnValue = (int) this.context.Provider.Execute(deleteCommand).ReturnValue;
                if (returnValue == 0)
                {
                    deleteCommand = this.GetDeleteVerificationCommand(item);
                    int? nullable = (int?) this.context.Provider.Execute(deleteCommand).ReturnValue;
                    returnValue = nullable.HasValue ? nullable.GetValueOrDefault() : -1;
                }
                return returnValue;
            }

            internal override int DynamicInsert(TrackedObject item)
            {
                Expression insertCommand = this.GetInsertCommand(item);
                if (insertCommand.Type == typeof(int))
                {
                    return (int) this.context.Provider.Execute(insertCommand).ReturnValue;
                }
                IEnumerable<object> returnValue = (IEnumerable<object>) this.context.Provider.Execute(insertCommand).ReturnValue;
                object[] syncResults = (object[]) returnValue.FirstOrDefault<object>();
                if (syncResults == null)
                {
                    throw System.Data.Linq.Error.InsertAutoSyncFailure();
                }
                AutoSyncMembers(syncResults, item, UpdateType.Insert);
                return 1;
            }

            internal override int DynamicUpdate(TrackedObject item)
            {
                Expression updateCommand = this.GetUpdateCommand(item);
                if (updateCommand.Type == typeof(int))
                {
                    return (int) this.context.Provider.Execute(updateCommand).ReturnValue;
                }
                IEnumerable<object> returnValue = (IEnumerable<object>) this.context.Provider.Execute(updateCommand).ReturnValue;
                object[] syncResults = (object[]) returnValue.FirstOrDefault<object>();
                if (syncResults != null)
                {
                    AutoSyncMembers(syncResults, item, UpdateType.Update);
                    return 1;
                }
                return 0;
            }

            internal static List<MetaDataMember> GetAutoSyncMembers(MetaType metaType, UpdateType updateType)
            {
                List<MetaDataMember> list = new List<MetaDataMember>();
                foreach (MetaDataMember member in from m in metaType.PersistentDataMembers
                    orderby m.Ordinal
                    select m)
                {
                    if ((((updateType == UpdateType.Insert) && (member.AutoSync == AutoSync.OnInsert)) || ((updateType == UpdateType.Update) && (member.AutoSync == AutoSync.OnUpdate))) || (member.AutoSync == AutoSync.Always))
                    {
                        list.Add(member);
                    }
                }
                return list;
            }

            private Expression GetDeleteCommand(TrackedObject tracked)
            {
                MetaType type = tracked.Type;
                MetaType inheritanceRoot = type.InheritanceRoot;
                ParameterExpression expression = Expression.Parameter(inheritanceRoot.Type, "p");
                Expression serverItem = expression;
                if (type != inheritanceRoot)
                {
                    serverItem = Expression.Convert(expression, type.Type);
                }
                object obj2 = tracked.CreateDataCopy(tracked.Original);
                Expression updateCheck = this.GetUpdateCheck(serverItem, tracked);
                if (updateCheck != null)
                {
                    updateCheck = Expression.Lambda(updateCheck, new ParameterExpression[] { expression });
                    return Expression.Call(typeof(DataManipulation), "Delete", new Type[] { inheritanceRoot.Type }, new Expression[] { Expression.Constant(obj2), updateCheck });
                }
                return Expression.Call(typeof(DataManipulation), "Delete", new Type[] { inheritanceRoot.Type }, new Expression[] { Expression.Constant(obj2) });
            }

            private Expression GetDeleteVerificationCommand(TrackedObject tracked)
            {
                ITable table = this.context.GetTable(tracked.Type.InheritanceRoot.Type);
                ParameterExpression left = Expression.Parameter(table.ElementType, "p");
                Expression expression2 = Expression.Lambda(Expression.Equal(left, Expression.Constant(tracked.Current)), new ParameterExpression[] { left });
                Expression expression3 = Expression.Call(typeof(Queryable), "Where", new Type[] { table.ElementType }, new Expression[] { table.Expression, expression2 });
                Expression expression4 = Expression.Lambda(Expression.Constant(0, typeof(int?)), new ParameterExpression[] { left });
                Expression expression5 = Expression.Call(typeof(Queryable), "Select", new Type[] { table.ElementType, typeof(int?) }, new Expression[] { expression3, expression4 });
                return Expression.Call(typeof(Queryable), "SingleOrDefault", new Type[] { typeof(int?) }, new Expression[] { expression5 });
            }

            private Expression GetInsertCommand(TrackedObject item)
            {
                List<MetaDataMember> autoSyncMembers = GetAutoSyncMembers(item.Type, UpdateType.Insert);
                ParameterExpression source = Expression.Parameter(item.Type.Table.RowType.Type, "p");
                if (autoSyncMembers.Count > 0)
                {
                    LambdaExpression expression3 = Expression.Lambda(this.CreateAutoSync(autoSyncMembers, source), new ParameterExpression[] { source });
                    return Expression.Call(typeof(DataManipulation), "Insert", new Type[] { item.Type.InheritanceRoot.Type, expression3.Body.Type }, new Expression[] { Expression.Constant(item.Current), expression3 });
                }
                return Expression.Call(typeof(DataManipulation), "Insert", new Type[] { item.Type.InheritanceRoot.Type }, new Expression[] { Expression.Constant(item.Current) });
            }

            private Expression GetMemberExpression(Expression exp, MemberInfo mi)
            {
                FieldInfo field = mi as FieldInfo;
                if (field != null)
                {
                    return Expression.Field(exp, field);
                }
                PropertyInfo property = (PropertyInfo) mi;
                return Expression.Property(exp, property);
            }

            private Expression GetUpdateCheck(Expression serverItem, TrackedObject tracked)
            {
                MetaType type = tracked.Type;
                if (type.VersionMember != null)
                {
                    return Expression.Equal(this.GetMemberExpression(serverItem, type.VersionMember.Member), this.GetMemberExpression(Expression.Constant(tracked.Current), type.VersionMember.Member));
                }
                Expression left = null;
                foreach (MetaDataMember member in type.PersistentDataMembers)
                {
                    if (!member.IsPrimaryKey)
                    {
                        UpdateCheck updateCheck = member.UpdateCheck;
                        if ((updateCheck == UpdateCheck.Always) || ((updateCheck == UpdateCheck.WhenChanged) && tracked.HasChangedValue(member)))
                        {
                            object boxedValue = member.MemberAccessor.GetBoxedValue(tracked.Original);
                            Expression right = Expression.Equal(this.GetMemberExpression(serverItem, member.Member), Expression.Constant(boxedValue, member.Type));
                            left = (left != null) ? Expression.And(left, right) : right;
                        }
                    }
                }
                return left;
            }

            private Expression GetUpdateCommand(TrackedObject tracked)
            {
                object original = tracked.Original;
                MetaType inheritanceType = tracked.Type.GetInheritanceType(original.GetType());
                MetaType inheritanceRoot = inheritanceType.InheritanceRoot;
                ParameterExpression expression = Expression.Parameter(inheritanceRoot.Type, "p");
                Expression serverItem = expression;
                if (inheritanceType != inheritanceRoot)
                {
                    serverItem = Expression.Convert(expression, inheritanceType.Type);
                }
                Expression updateCheck = this.GetUpdateCheck(serverItem, tracked);
                if (updateCheck != null)
                {
                    updateCheck = Expression.Lambda(updateCheck, new ParameterExpression[] { expression });
                }
                List<MetaDataMember> autoSyncMembers = GetAutoSyncMembers(inheritanceType, UpdateType.Update);
                if (autoSyncMembers.Count > 0)
                {
                    LambdaExpression expression5 = Expression.Lambda(this.CreateAutoSync(autoSyncMembers, serverItem), new ParameterExpression[] { expression });
                    if (updateCheck != null)
                    {
                        return Expression.Call(typeof(DataManipulation), "Update", new Type[] { inheritanceRoot.Type, expression5.Body.Type }, new Expression[] { Expression.Constant(tracked.Current), updateCheck, expression5 });
                    }
                    return Expression.Call(typeof(DataManipulation), "Update", new Type[] { inheritanceRoot.Type, expression5.Body.Type }, new Expression[] { Expression.Constant(tracked.Current), expression5 });
                }
                if (updateCheck != null)
                {
                    return Expression.Call(typeof(DataManipulation), "Update", new Type[] { inheritanceRoot.Type }, new Expression[] { Expression.Constant(tracked.Current), updateCheck });
                }
                return Expression.Call(typeof(DataManipulation), "Update", new Type[] { inheritanceRoot.Type }, new Expression[] { Expression.Constant(tracked.Current) });
            }

            internal override int Insert(TrackedObject item)
            {
                if (item.Type.Table.InsertMethod == null)
                {
                    return this.DynamicInsert(item);
                }
                try
                {
                    item.Type.Table.InsertMethod.Invoke(this.context, new object[] { item.Current });
                }
                catch (TargetInvocationException exception)
                {
                    if (exception.InnerException != null)
                    {
                        throw exception.InnerException;
                    }
                    throw;
                }
                return 1;
            }

            internal override int Update(TrackedObject item)
            {
                if (item.Type.Table.UpdateMethod == null)
                {
                    return this.DynamicUpdate(item);
                }
                try
                {
                    item.Type.Table.UpdateMethod.Invoke(this.context, new object[] { item.Current });
                }
                catch (TargetInvocationException exception)
                {
                    if (exception.InnerException != null)
                    {
                        throw exception.InnerException;
                    }
                    throw;
                }
                return 1;
            }

            internal enum UpdateType
            {
                Insert,
                Update,
                Delete
            }
        }
    }
}

