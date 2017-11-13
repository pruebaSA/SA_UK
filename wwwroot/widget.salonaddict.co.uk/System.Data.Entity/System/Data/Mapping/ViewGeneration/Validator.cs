namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Mapping.ViewGeneration.Validation;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal class Validator
    {
        private IEnumerable<Cell> m_cells;
        private ConfigViewGenerator m_config;
        private SchemaConstraints<ViewKeyConstraint> m_cViewConstraints;
        private ErrorLog m_errorLog;
        private SchemaConstraints<ViewKeyConstraint> m_sViewConstraints;

        internal Validator(IEnumerable<Cell> cells, ConfigViewGenerator config)
        {
            this.m_cells = cells;
            this.m_config = config;
            this.m_errorLog = new ErrorLog();
        }

        [Conditional("DEBUG")]
        private static void CheckConstraintSanity(SchemaConstraints<BasicKeyConstraint> cConstraints, SchemaConstraints<BasicKeyConstraint> sConstraints, SchemaConstraints<ViewKeyConstraint> cViewConstraints, SchemaConstraints<ViewKeyConstraint> sViewConstraints)
        {
        }

        private void CheckImplication(SchemaConstraints<ViewKeyConstraint> cViewConstraints, SchemaConstraints<ViewKeyConstraint> sViewConstraints, MetadataWorkspace workspace)
        {
            this.CheckImplicationKeyConstraints(cViewConstraints, sViewConstraints, workspace);
            KeyToListMap<ExtentPair, ViewKeyConstraint> map = new KeyToListMap<ExtentPair, ViewKeyConstraint>(EqualityComparer<ExtentPair>.Default);
            foreach (ViewKeyConstraint constraint in cViewConstraints.KeyConstraints)
            {
                ExtentPair key = new ExtentPair(constraint.Cell.CQuery.Extent, constraint.Cell.SQuery.Extent);
                map.Add(key, constraint);
            }
            foreach (ExtentPair pair2 in map.Keys)
            {
                ReadOnlyCollection<ViewKeyConstraint> rightKeyConstraints = map.ListForKey(pair2);
                bool flag = false;
                foreach (ViewKeyConstraint constraint2 in rightKeyConstraints)
                {
                    foreach (ViewKeyConstraint constraint3 in sViewConstraints.KeyConstraints)
                    {
                        if (constraint3.Implies(constraint2))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    this.m_errorLog.AddEntry(ViewKeyConstraint.GetErrorRecord(rightKeyConstraints, workspace));
                }
            }
        }

        private void CheckImplicationKeyConstraints(SchemaConstraints<ViewKeyConstraint> leftViewConstraints, SchemaConstraints<ViewKeyConstraint> rightViewConstraints, MetadataWorkspace workspace)
        {
            foreach (ViewKeyConstraint constraint in rightViewConstraints.KeyConstraints)
            {
                bool flag = false;
                foreach (ViewKeyConstraint constraint2 in leftViewConstraints.KeyConstraints)
                {
                    if (constraint2.Implies(constraint))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    this.m_errorLog.AddEntry(ViewKeyConstraint.GetErrorRecord(constraint, workspace));
                }
            }
        }

        private void ConstructCellRelationsWithConstraints(SchemaConstraints<BasicKeyConstraint> cConstraints, SchemaConstraints<BasicKeyConstraint> sConstraints, MetadataWorkspace workspace)
        {
            int cellNumber = 0;
            foreach (Cell cell in this.m_cells)
            {
                cell.CreateViewCellRelation(cellNumber);
                BasicCellRelation basicCellRelation = cell.CQuery.BasicCellRelation;
                BasicCellRelation baseRelation = cell.SQuery.BasicCellRelation;
                PopulateBaseConstraints(basicCellRelation, cConstraints, workspace);
                PopulateBaseConstraints(baseRelation, sConstraints, workspace);
                cellNumber++;
            }
            foreach (Cell cell2 in this.m_cells)
            {
                foreach (Cell cell3 in this.m_cells)
                {
                    object.ReferenceEquals(cell2, cell3);
                }
            }
        }

        private bool PerformSingleCellChecks()
        {
            int count = this.m_errorLog.Count;
            foreach (Cell cell in this.m_cells)
            {
                ErrorLog.Record record = cell.SQuery.CheckForDuplicateFields(cell.CQuery, cell);
                if (record != null)
                {
                    this.m_errorLog.AddEntry(record);
                }
                record = cell.CQuery.VerifyKeysPresent(cell, new Func<object, object, string>(Strings.ViewGen_EntitySetKey_Missing_1), new Func<object, object, object, string>(Strings.ViewGen_AssociationSetKey_Missing_2), ViewGenErrorCode.KeyNotMappedForCSideExtent);
                if (record != null)
                {
                    this.m_errorLog.AddEntry(record);
                }
                record = cell.SQuery.VerifyKeysPresent(cell, new Func<object, object, string>(Strings.ViewGen_TableKey_Missing_1), null, ViewGenErrorCode.KeyNotMappedForTable);
                if (record != null)
                {
                    this.m_errorLog.AddEntry(record);
                }
                record = cell.CQuery.CheckForProjectedNotNullSlots(cell);
                if (record != null)
                {
                    this.m_errorLog.AddEntry(record);
                }
                record = cell.SQuery.CheckForProjectedNotNullSlots(cell);
                if (record != null)
                {
                    this.m_errorLog.AddEntry(record);
                }
            }
            return (this.m_errorLog.Count == count);
        }

        private static void PopulateBaseConstraints(BasicCellRelation baseRelation, SchemaConstraints<BasicKeyConstraint> constraints, MetadataWorkspace workspace)
        {
            baseRelation.PopulateKeyConstraints(constraints, workspace);
        }

        private static SchemaConstraints<ViewKeyConstraint> PropagateConstraints(SchemaConstraints<BasicKeyConstraint> baseConstraints)
        {
            SchemaConstraints<ViewKeyConstraint> constraints = new SchemaConstraints<ViewKeyConstraint>();
            foreach (BasicKeyConstraint constraint in baseConstraints.KeyConstraints)
            {
                ViewKeyConstraint constraint2 = constraint.Propagate();
                if (constraint2 != null)
                {
                    constraints.Add(constraint2);
                }
            }
            return constraints;
        }

        internal ErrorLog Validate(MetadataWorkspace workspace)
        {
            if (this.PerformSingleCellChecks())
            {
                SchemaConstraints<BasicKeyConstraint> cConstraints = new SchemaConstraints<BasicKeyConstraint>();
                SchemaConstraints<BasicKeyConstraint> sConstraints = new SchemaConstraints<BasicKeyConstraint>();
                this.ConstructCellRelationsWithConstraints(cConstraints, sConstraints, workspace);
                if (this.m_config.IsVerboseTracing)
                {
                    Trace.WriteLine(string.Empty);
                    Trace.WriteLine("C-Level Basic Constraints");
                    Trace.WriteLine(cConstraints);
                    Trace.WriteLine("S-Level Basic Constraints");
                    Trace.WriteLine(sConstraints);
                }
                this.m_cViewConstraints = PropagateConstraints(cConstraints);
                this.m_sViewConstraints = PropagateConstraints(sConstraints);
                if (this.m_config.IsVerboseTracing)
                {
                    Trace.WriteLine(string.Empty);
                    Trace.WriteLine("C-Level View Constraints");
                    Trace.WriteLine(this.m_cViewConstraints);
                    Trace.WriteLine("S-Level View Constraints");
                    Trace.WriteLine(this.m_sViewConstraints);
                }
                this.CheckImplication(this.m_cViewConstraints, this.m_sViewConstraints, workspace);
            }
            return this.m_errorLog;
        }

        private class ExtentPair
        {
            internal EntitySetBase cExtent;
            internal EntitySetBase sExtent;

            internal ExtentPair(EntitySetBase acExtent, EntitySetBase asExtent)
            {
                this.cExtent = acExtent;
                this.sExtent = asExtent;
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }
                Validator.ExtentPair pair = obj as Validator.ExtentPair;
                if (pair == null)
                {
                    return false;
                }
                return (pair.cExtent.Equals(this.cExtent) && pair.sExtent.Equals(this.sExtent));
            }

            public override int GetHashCode() => 
                (this.cExtent.GetHashCode() ^ this.sExtent.GetHashCode());
        }
    }
}

