namespace System.Data.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal sealed class StorageFunctionMapping
    {
        internal readonly EdmFunction Function;
        internal readonly ReadOnlyCollection<StorageFunctionParameterBinding> ParameterBindings;
        internal readonly ReadOnlyCollection<StorageFunctionResultBinding> ResultBindings;
        internal readonly FunctionParameter RowsAffectedParameter;

        internal StorageFunctionMapping(EdmFunction function, IEnumerable<StorageFunctionParameterBinding> parameterBindings, FunctionParameter rowsAffectedParameter, IEnumerable<StorageFunctionResultBinding> resultBindings)
        {
            this.RowsAffectedParameter = rowsAffectedParameter;
            this.Function = EntityUtil.CheckArgumentNull<EdmFunction>(function, "function");
            this.ParameterBindings = new ReadOnlyCollection<StorageFunctionParameterBinding>(new List<StorageFunctionParameterBinding>(EntityUtil.CheckArgumentNull<IEnumerable<StorageFunctionParameterBinding>>(parameterBindings, "parameterBindings")));
            if (resultBindings != null)
            {
                List<StorageFunctionResultBinding> list = new List<StorageFunctionResultBinding>(resultBindings);
                if (0 < list.Count)
                {
                    this.ResultBindings = new ReadOnlyCollection<StorageFunctionResultBinding>(list);
                }
            }
        }

        internal void AddReferencedAssociationSetEnds(EntitySet entitySet, List<AssociationSetEnd> associationSetEnds)
        {
            associationSetEnds.AddRange(this.GetReferencedAssociationSetEnds(entitySet));
        }

        internal IEnumerable<AssociationSetEnd> GetReferencedAssociationSetEnds(EntitySet entitySet)
        {
            foreach (StorageFunctionParameterBinding iteratorVariable0 in this.ParameterBindings)
            {
                AssociationSetEnd associationSetEnd = iteratorVariable0.MemberPath.AssociationSetEnd;
                if (associationSetEnd != null)
                {
                    yield return associationSetEnd;
                }
            }
            foreach (AssociationSet iteratorVariable2 in MetadataHelper.GetAssociationsForEntitySet(entitySet))
            {
                ReadOnlyMetadataCollection<ReferentialConstraint> referentialConstraints = iteratorVariable2.ElementType.ReferentialConstraints;
                if (referentialConstraints != null)
                {
                    foreach (ReferentialConstraint iteratorVariable4 in referentialConstraints)
                    {
                        if (iteratorVariable2.AssociationSetEnds[iteratorVariable4.ToRole.Name].EntitySet != entitySet)
                        {
                            continue;
                        }
                        yield return iteratorVariable2.AssociationSetEnds[iteratorVariable4.FromRole.Name];
                    }
                }
            }
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "Func{{{0}}}: Prm={{{1}}}, Result={{{2}}}", new object[] { this.Function, StringUtil.ToCommaSeparatedStringSorted(this.ParameterBindings), StringUtil.ToCommaSeparatedStringSorted(this.ResultBindings) });

    }
}

