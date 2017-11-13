namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class ErrorLog : InternalBase
    {
        private List<Record> m_log = new List<Record>();

        internal ErrorLog()
        {
        }

        internal void AddEntry(Record record)
        {
            EntityUtil.CheckArgumentNull<Record>(record, "record");
            this.m_log.Add(record);
        }

        internal void Merge(ErrorLog log)
        {
            foreach (Record record in log.m_log)
            {
                this.m_log.Add(record);
            }
        }

        internal void PrintTrace()
        {
            StringBuilder builder = new StringBuilder();
            this.ToCompactString(builder);
            Helpers.StringTraceLine(builder.ToString());
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            foreach (Record record in this.m_log)
            {
                record.ToCompactString(builder);
            }
        }

        internal string ToUserString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Record record in this.m_log)
            {
                string str = record.ToUserString();
                builder.AppendLine(str);
            }
            return builder.ToString();
        }

        internal int Count =>
            this.m_log.Count;

        internal IEnumerable<EdmSchemaError> Errors
        {
            get
            {
                foreach (Record iteratorVariable0 in this.m_log)
                {
                    yield return iteratorVariable0.Error;
                }
            }
        }


        internal class Record : InternalBase
        {
            private string m_debugMessage;
            private EdmSchemaError m_mappingError;
            private List<Cell> m_sourceCells;

            internal Record(EdmSchemaError error)
            {
                this.m_debugMessage = error.ToString();
                this.m_mappingError = error;
            }

            internal Record(bool isError, ViewGenErrorCode errorCode, string message, IEnumerable<Cell> sourceCells, string debugMessage)
            {
                this.Init(isError, errorCode, message, sourceCells, debugMessage);
            }

            internal Record(bool isError, ViewGenErrorCode errorCode, string message, IEnumerable<LeftCellWrapper> wrappers, string debugMessage)
            {
                IEnumerable<Cell> inputCellsForWrappers = LeftCellWrapper.GetInputCellsForWrappers(wrappers);
                this.Init(isError, errorCode, message, inputCellsForWrappers, debugMessage);
            }

            internal Record(bool isError, ViewGenErrorCode errorCode, string message, Cell sourceCell, string debugMessage)
            {
                this.Init(isError, errorCode, message, new Cell[] { sourceCell }, debugMessage);
            }

            private static void GetUserLinesFromCells(IEnumerable<Cell> sourceCells, StringBuilder lineBuilder, bool isInvariant)
            {
                IOrderedEnumerable<Cell> enumerable = sourceCells.OrderBy<Cell, int>(cell => cell.CellLabel.StartLineNumber, Comparer<int>.Default);
                bool flag = true;
                foreach (Cell cell in enumerable)
                {
                    if (!flag)
                    {
                        lineBuilder.Append(isInvariant ? EntityRes.GetString("ViewGen_CommaBlank") : ", ");
                    }
                    flag = false;
                    lineBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", new object[] { cell.CellLabel.StartLineNumber });
                }
            }

            private void Init(bool isError, ViewGenErrorCode errorCode, string message, IEnumerable<Cell> sourceCells, string debugMessage)
            {
                this.m_sourceCells = new List<Cell>(sourceCells);
                CellLabel cellLabel = this.m_sourceCells[0].CellLabel;
                string sourceLocation = cellLabel.SourceLocation;
                int startLineNumber = cellLabel.StartLineNumber;
                int startLinePosition = cellLabel.StartLinePosition;
                string str2 = InternalToString(message, debugMessage, this.m_sourceCells, sourceLocation, errorCode, isError, false);
                this.m_debugMessage = InternalToString(message, debugMessage, this.m_sourceCells, sourceLocation, errorCode, isError, true);
                this.m_mappingError = new EdmSchemaError(str2, (int) errorCode, EdmSchemaErrorSeverity.Error, sourceLocation, startLineNumber, startLinePosition);
            }

            private static string InternalToString(string message, string debugMessage, List<Cell> sourceCells, string sourceLocation, ViewGenErrorCode errorCode, bool isError, bool isInvariant)
            {
                StringBuilder builder = new StringBuilder();
                if (isInvariant)
                {
                    builder.AppendLine(debugMessage);
                    builder.Append(isInvariant ? "ERROR" : System.Data.Entity.Strings.ViewGen_Error);
                    StringUtil.FormatStringBuilder(builder, " ({0}): ", new object[] { (int) errorCode });
                }
                StringBuilder lineBuilder = new StringBuilder();
                GetUserLinesFromCells(sourceCells, lineBuilder, isInvariant);
                if (isInvariant)
                {
                    if (sourceCells.Count > 1)
                    {
                        StringUtil.FormatStringBuilder(builder, "Problem in Mapping Fragments starting at lines {0}: ", new object[] { lineBuilder.ToString() });
                    }
                    else
                    {
                        StringUtil.FormatStringBuilder(builder, "Problem in Mapping Fragment starting at line {0}: ", new object[] { lineBuilder.ToString() });
                    }
                }
                else if (sourceCells.Count > 1)
                {
                    builder.Append(System.Data.Entity.Strings.ViewGen_ErrorLog_1(lineBuilder.ToString()));
                }
                else
                {
                    builder.Append(System.Data.Entity.Strings.ViewGen_ErrorLog_0(lineBuilder.ToString()));
                }
                builder.AppendLine(message);
                return builder.ToString();
            }

            internal override void ToCompactString(StringBuilder builder)
            {
                builder.Append(this.m_debugMessage);
            }

            internal string ToUserString() => 
                this.m_mappingError.ToString();

            internal EdmSchemaError Error =>
                this.m_mappingError;
        }
    }
}

