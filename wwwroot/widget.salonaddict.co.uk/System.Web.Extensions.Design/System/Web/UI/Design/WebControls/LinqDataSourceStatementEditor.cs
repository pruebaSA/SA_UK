namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing.Design;
    using System.Web.Resources.Design;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    internal sealed class LinqDataSourceStatementEditor : UITypeEditor
    {
        internal static bool ContainsParameter(List<Parameter> list, string name)
        {
            foreach (Parameter parameter in list)
            {
                if (string.Equals(name, parameter.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool EditQueryChangeCallback(object pair)
        {
            ITypeDescriptorContext first = (ITypeDescriptorContext) ((Pair) pair).First;
            string second = (string) ((Pair) pair).Second;
            LinqDataSource instance = (LinqDataSource) first.Instance;
            IServiceProvider site = instance.Site;
            IDesignerHost service = (IDesignerHost) site.GetService(typeof(IDesignerHost));
            LinqDataSourceDesigner designer = (LinqDataSourceDesigner) service.GetDesigner(instance);
            ILinqDataSourceDesignerHelper designerHelper = designer.Helper;
            return this.SetUp(designerHelper, instance, first.PropertyDescriptor.Name, site, second);
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ControlDesigner.InvokeTransactedChange((IComponent) context.Instance, new TransactedChangeCallback(this.EditQueryChangeCallback), new Pair(context, value), AtlasWebDesign.LinqDataSourceStatementEditor_EditQuery);
            return value;
        }

        internal static bool GetAutoGen(string operation, ILinqDataSourceDesignerHelper linqDataSourceHelper)
        {
            if (string.Equals("Where", operation, StringComparison.Ordinal))
            {
                return linqDataSourceHelper.AutoGenerateWhereClause;
            }
            return (string.Equals("OrderBy", operation, StringComparison.Ordinal) && linqDataSourceHelper.AutoGenerateOrderByClause);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => 
            UITypeEditorEditStyle.Modal;

        internal static bool GetIsInsertUpdateDelete(string operation)
        {
            if ((!string.Equals("Insert", operation, StringComparison.Ordinal) && !string.Equals("Update", operation, StringComparison.Ordinal)) && !string.Equals("Delete", operation, StringComparison.Ordinal))
            {
                return false;
            }
            return true;
        }

        internal static List<Parameter> GetNeededParameters(string statement)
        {
            List<Parameter> list = new List<Parameter>();
            if (!string.IsNullOrEmpty(statement))
            {
                bool flag = false;
                bool flag2 = false;
                int num = statement.LastIndexOf('@');
                for (int i = 0; i <= num; i++)
                {
                    switch (statement[i])
                    {
                        case '"':
                            if (!flag)
                            {
                                flag2 = !flag2;
                            }
                            break;

                        case '\'':
                            if (!flag2)
                            {
                                flag = !flag;
                            }
                            break;

                        case '@':
                            if (!flag && !flag2)
                            {
                                int nextParameterNameLength = GetNextParameterNameLength(statement, i);
                                if (nextParameterNameLength > 0)
                                {
                                    string name = statement.Substring(i + 1, nextParameterNameLength);
                                    if (!ContainsParameter(list, name))
                                    {
                                        list.Add(new Parameter(name));
                                    }
                                }
                                i += nextParameterNameLength;
                            }
                            break;
                    }
                }
            }
            return list;
        }

        internal static int GetNextParameterNameLength(string statement, int at)
        {
            if (at >= (statement.Length - 1))
            {
                return 0;
            }
            int num = at + 1;
            int num2 = num;
            if ((!char.IsLetter(statement[num2]) && (statement[num2] != '_')) && (statement[num2] != '@'))
            {
                return 0;
            }
            num2++;
            while ((num2 < statement.Length) && (char.IsLetterOrDigit(statement[num2]) || (statement[num2] == '_')))
            {
                num2++;
            }
            return (num2 - num);
        }

        internal static string GetOperationAutoGenerateProperty(string operation)
        {
            if (string.Equals("Where", operation, StringComparison.Ordinal))
            {
                return "AutoGenerateWhereClause";
            }
            if (string.Equals("OrderBy", operation, StringComparison.Ordinal))
            {
                return "AutoGenerateOrderByClause";
            }
            return null;
        }

        internal static string GetOperationLabel(string operation)
        {
            if (string.Equals("Where", operation, StringComparison.Ordinal))
            {
                return "Where";
            }
            if (string.Equals("OrderBy", operation, StringComparison.Ordinal))
            {
                return "OrderBy";
            }
            if (string.Equals("GroupBy", operation, StringComparison.Ordinal))
            {
                return "GroupBy";
            }
            if (string.Equals("OrderGroupsBy", operation, StringComparison.Ordinal))
            {
                return "OrderGroupsBy";
            }
            if (string.Equals("Select", operation, StringComparison.Ordinal))
            {
                return "Select";
            }
            if (string.Equals("Insert", operation, StringComparison.Ordinal))
            {
                return "Insert";
            }
            if (string.Equals("Update", operation, StringComparison.Ordinal))
            {
                return "Update";
            }
            if (string.Equals("Delete", operation, StringComparison.Ordinal))
            {
                return "Delete";
            }
            return null;
        }

        internal static string GetOperationParameterProperty(string operation)
        {
            if (string.Equals("Where", operation, StringComparison.Ordinal))
            {
                return "WhereParameters";
            }
            if (string.Equals("OrderBy", operation, StringComparison.Ordinal))
            {
                return "OrderByParameters";
            }
            if (string.Equals("GroupBy", operation, StringComparison.Ordinal))
            {
                return "GroupByParameters";
            }
            if (string.Equals("OrderGroupsBy", operation, StringComparison.Ordinal))
            {
                return "OrderGroupsByParameters";
            }
            if (string.Equals("Select", operation, StringComparison.Ordinal))
            {
                return "SelectParameters";
            }
            if (string.Equals("Insert", operation, StringComparison.Ordinal))
            {
                return "InsertParameters";
            }
            if (string.Equals("Update", operation, StringComparison.Ordinal))
            {
                return "UpdateParameters";
            }
            if (string.Equals("Delete", operation, StringComparison.Ordinal))
            {
                return "DeleteParameters";
            }
            return null;
        }

        internal static string GetOperationProperty(string operation)
        {
            if (string.Equals("Where", operation, StringComparison.Ordinal))
            {
                return "Where";
            }
            if (string.Equals("OrderBy", operation, StringComparison.Ordinal))
            {
                return "OrderBy";
            }
            if (string.Equals("GroupBy", operation, StringComparison.Ordinal))
            {
                return "GroupBy";
            }
            if (string.Equals("OrderGroupsBy", operation, StringComparison.Ordinal))
            {
                return "OrderGroupsBy";
            }
            if (string.Equals("Select", operation, StringComparison.Ordinal))
            {
                return "Select";
            }
            return null;
        }

        internal static ParameterCollection GetParameters(string operation, ILinqDataSourceDesignerHelper designerHelper)
        {
            if (string.Equals("Where", operation, StringComparison.Ordinal))
            {
                return designerHelper.CloneWhereParameters();
            }
            if (string.Equals("OrderBy", operation, StringComparison.Ordinal))
            {
                return designerHelper.CloneOrderByParameters();
            }
            if (string.Equals("GroupBy", operation, StringComparison.Ordinal))
            {
                return designerHelper.CloneGroupByParameters();
            }
            if (string.Equals("OrderGroupsBy", operation, StringComparison.Ordinal))
            {
                return designerHelper.CloneOrderGroupsByParameters();
            }
            if (string.Equals("Select", operation, StringComparison.Ordinal))
            {
                return designerHelper.CloneSelectParameters();
            }
            if (string.Equals("Insert", operation, StringComparison.Ordinal))
            {
                return designerHelper.CloneInsertParameters();
            }
            if (string.Equals("Update", operation, StringComparison.Ordinal))
            {
                return designerHelper.CloneUpdateParameters();
            }
            if (string.Equals("Delete", operation, StringComparison.Ordinal))
            {
                return designerHelper.CloneDeleteParameters();
            }
            return null;
        }

        internal static void SetParameters(string operation, ILinqDataSourceDesignerHelper designerHelper, ParameterCollection parameters)
        {
            if (string.Equals("Where", operation, StringComparison.Ordinal))
            {
                designerHelper.SetWhereParameterContents(parameters);
            }
            else if (string.Equals("OrderBy", operation, StringComparison.Ordinal))
            {
                designerHelper.SetOrderByParameterContents(parameters);
            }
            else if (string.Equals("GroupBy", operation, StringComparison.Ordinal))
            {
                designerHelper.SetGroupByParameterContents(parameters);
            }
            else if (string.Equals("OrderGroupsBy", operation, StringComparison.Ordinal))
            {
                designerHelper.SetOrderGroupsByParameterContents(parameters);
            }
            else if (string.Equals("Select", operation, StringComparison.Ordinal))
            {
                designerHelper.SetSelectParameterContents(parameters);
            }
            else if (string.Equals("Insert", operation, StringComparison.Ordinal))
            {
                designerHelper.SetInsertParameterContents(parameters);
            }
            else if (string.Equals("Update", operation, StringComparison.Ordinal))
            {
                designerHelper.SetUpdateParameterContents(parameters);
            }
            else if (string.Equals("Delete", operation, StringComparison.Ordinal))
            {
                designerHelper.SetDeleteParameterContents(parameters);
            }
        }

        public bool SetUp(ILinqDataSourceDesignerHelper designerHelper, LinqDataSource linqDataSource, string operation, IServiceProvider serviceProvider, string statement)
        {
            string operationProperty = GetOperationProperty(operation);
            string operationParameterProperty = GetOperationParameterProperty(operation);
            string operationAutoGenerateProperty = GetOperationAutoGenerateProperty(operation);
            bool hasAutoGen = operationAutoGenerateProperty != null;
            bool autoGen = GetAutoGen(operation, designerHelper);
            bool isInsertUpdateDelete = GetIsInsertUpdateDelete(operation);
            ParameterCollection parameters = GetParameters(operation, designerHelper);
            string operationLabel = GetOperationLabel(operation);
            LinqDataSourceStatementEditorForm form = new LinqDataSourceStatementEditorForm(linqDataSource, serviceProvider, hasAutoGen, isInsertUpdateDelete, autoGen, statement, parameters, operationLabel);
            if (UIServiceHelper.ShowDialog(serviceProvider, form) != DialogResult.OK)
            {
                return false;
            }
            PropertyDescriptor descriptor = null;
            if (operationAutoGenerateProperty != null)
            {
                descriptor = TypeDescriptor.GetProperties(linqDataSource)[operationAutoGenerateProperty];
                descriptor.ResetValue(linqDataSource);
                descriptor.SetValue(linqDataSource, form.AutoGen);
            }
            if (operationProperty != null)
            {
                descriptor = TypeDescriptor.GetProperties(linqDataSource)[operationProperty];
                descriptor.ResetValue(linqDataSource);
                descriptor.SetValue(linqDataSource, form.Statement);
            }
            if (operationParameterProperty != null)
            {
                SetParameters(operation, designerHelper, form.Parameters);
            }
            return true;
        }
    }
}

