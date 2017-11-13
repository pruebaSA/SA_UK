namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultEvent("FinishButtonClick"), ToolboxData("<{0}:Wizard runat=\"server\"> <WizardSteps> <asp:WizardStep title=\"Step 1\" runat=\"server\"></asp:WizardStep> <asp:WizardStep title=\"Step 2\" runat=\"server\"></asp:WizardStep> </WizardSteps> </{0}:Wizard>"), Designer("System.Web.UI.Design.WebControls.WizardDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Bindable(false), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class Wizard : CompositeControl
    {
        private bool _activeStepIndexSet;
        private Style _cancelButtonStyle;
        private IButtonControl _commandSender;
        internal const string _customNavigationContainerIdPrefix = "__CustomNav";
        private IDictionary _customNavigationContainers;
        internal const string _customNavigationControls = "CustomNavigationControls";
        private NavigationTemplate _defaultFinishNavigationTemplate;
        private NavigationTemplate _defaultStartNavigationTemplate;
        private NavigationTemplate _defaultStepNavigationTemplate;
        private IDictionary _designModeState;
        internal bool _displaySideBar = true;
        internal bool _displaySideBarDefault = true;
        private static readonly object _eventActiveStepChanged = new object();
        private static readonly object _eventCancelButtonClick = new object();
        private static readonly object _eventFinishButtonClick = new object();
        private static readonly object _eventNextButtonClick = new object();
        private static readonly object _eventPreviousButtonClick = new object();
        private static readonly object _eventSideBarButtonClick = new object();
        private Style _finishCompleteButtonStyle;
        private ITemplate _finishNavigationTemplate;
        private FinishNavigationTemplateContainer _finishNavigationTemplateContainer;
        internal const string _finishNavigationTemplateContainerID = "FinishNavigationTemplateContainerID";
        private const string _finishNavigationTemplateName = "FinishNavigationTemplate";
        private Style _finishPreviousButtonStyle;
        private const string _headerCellID = "HeaderContainer";
        private TableItemStyle _headerStyle;
        private TableCell _headerTableCell;
        private TableRow _headerTableRow;
        private ITemplate _headerTemplate;
        private Stack _historyStack;
        private bool _isMacIE;
        private bool _isMacIESet;
        private System.Web.UI.WebControls.MultiView _multiView;
        private const string _multiViewID = "WizardMultiView";
        private Style _navigationButtonStyle;
        private TableRow _navigationRow;
        private TableItemStyle _navigationStyle;
        internal TableCell _navigationTableCell;
        private bool _renderSideBarDataList;
        private Table _renderTable;
        private Style _sideBarButtonStyle;
        private const string _sideBarCellID = "SideBarContainer";
        private DataList _sideBarDataList;
        private TableItemStyle _sideBarStyle;
        private TableCell _sideBarTableCell;
        private ITemplate _sideBarTemplate;
        private const string _sideBarTemplateName = "SideBarTemplate";
        private ITemplate _startNavigationTemplate;
        private StartNavigationTemplateContainer _startNavigationTemplateContainer;
        internal const string _startNavigationTemplateContainerID = "StartNavigationTemplateContainerID";
        private const string _startNavigationTemplateName = "StartNavigationTemplate";
        private Style _startNextButtonStyle;
        private ITemplate _stepNavigationTemplate;
        private StepNavigationTemplateContainer _stepNavigationTemplateContainer;
        internal const string _stepNavigationTemplateContainerID = "StepNavigationTemplateContainerID";
        private const string _stepNavigationTemplateName = "StepNavigationTemplate";
        private Style _stepNextButtonStyle;
        private Style _stepPreviousButtonStyle;
        private TableItemStyle _stepStyle;
        private TableCell _stepTableCell;
        private ArrayList _templatedSteps;
        internal const string _templatedStepsID = "TemplatedWizardSteps";
        private LiteralControl _titleLiteral;
        private const int _viewStateArrayLength = 15;
        private const string _wizardContentMark = "_SkipLink";
        private WizardStepCollection _wizardStepCollection;
        protected static readonly string CancelButtonID = "CancelButton";
        public static readonly string CancelCommandName = "Cancel";
        protected static readonly string CustomFinishButtonID = "CustomFinishButton";
        protected static readonly string CustomNextButtonID = "CustomNextButton";
        protected static readonly string CustomPreviousButtonID = "CustomPreviousButton";
        protected static readonly string DataListID = "SideBarList";
        protected static readonly string FinishButtonID = "FinishButton";
        protected static readonly string FinishPreviousButtonID = "FinishPreviousButton";
        public static readonly string MoveCompleteCommandName = "MoveComplete";
        public static readonly string MoveNextCommandName = "MoveNext";
        public static readonly string MovePreviousCommandName = "MovePrevious";
        public static readonly string MoveToCommandName = "Move";
        protected static readonly string SideBarButtonID = "SideBarButton";
        protected static readonly string StartNextButtonID = "StartNextButton";
        protected static readonly string StepNextButtonID = "StepNextButton";
        protected static readonly string StepPreviousButtonID = "StepPreviousButton";
        private const string StepTableCellID = "StepTableCell";

        [WebSysDescription("Wizard_ActiveStepChanged"), WebCategory("Action")]
        public event EventHandler ActiveStepChanged
        {
            add
            {
                base.Events.AddHandler(_eventActiveStepChanged, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventActiveStepChanged, value);
            }
        }

        [WebSysDescription("Wizard_CancelButtonClick"), WebCategory("Action")]
        public event EventHandler CancelButtonClick
        {
            add
            {
                base.Events.AddHandler(_eventCancelButtonClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventCancelButtonClick, value);
            }
        }

        [WebSysDescription("Wizard_FinishButtonClick"), WebCategory("Action")]
        public event WizardNavigationEventHandler FinishButtonClick
        {
            add
            {
                base.Events.AddHandler(_eventFinishButtonClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventFinishButtonClick, value);
            }
        }

        [WebSysDescription("Wizard_NextButtonClick"), WebCategory("Action")]
        public event WizardNavigationEventHandler NextButtonClick
        {
            add
            {
                base.Events.AddHandler(_eventNextButtonClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventNextButtonClick, value);
            }
        }

        [WebCategory("Action"), WebSysDescription("Wizard_PreviousButtonClick")]
        public event WizardNavigationEventHandler PreviousButtonClick
        {
            add
            {
                base.Events.AddHandler(_eventPreviousButtonClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventPreviousButtonClick, value);
            }
        }

        [WebCategory("Action"), WebSysDescription("Wizard_SideBarButtonClick")]
        public event WizardNavigationEventHandler SideBarButtonClick
        {
            add
            {
                base.Events.AddHandler(_eventSideBarButtonClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventSideBarButtonClick, value);
            }
        }

        protected virtual bool AllowNavigationToStep(int index)
        {
            if ((this._historyStack != null) && this._historyStack.Contains(index))
            {
                return this.WizardSteps[index].AllowReturn;
            }
            return true;
        }

        private void ApplyButtonProperties(ButtonType type, string text, string imageUrl, IButtonControl button)
        {
            this.ApplyButtonProperties(type, text, imageUrl, button, true);
        }

        private void ApplyButtonProperties(ButtonType type, string text, string imageUrl, IButtonControl button, bool imageButtonVisible)
        {
            if (button != null)
            {
                if (button is ImageButton)
                {
                    ImageButton button2 = (ImageButton) button;
                    button2.ImageUrl = imageUrl;
                    button2.AlternateText = text;
                    if (button is Control)
                    {
                        ((Control) button).Visible = imageButtonVisible;
                    }
                }
                else
                {
                    button.Text = text;
                }
            }
        }

        internal virtual void ApplyControlProperties()
        {
            if (base.DesignMode || (((this.ActiveStepIndex >= 0) && (this.ActiveStepIndex < this.WizardSteps.Count)) && (this.WizardSteps.Count != 0)))
            {
                if (this.SideBarEnabled && (this._sideBarStyle != null))
                {
                    this._sideBarTableCell.ApplyStyle(this._sideBarStyle);
                }
                if (this._headerTableRow != null)
                {
                    if ((this.HeaderTemplate == null) && string.IsNullOrEmpty(this.HeaderText))
                    {
                        this._headerTableRow.Visible = false;
                    }
                    else
                    {
                        this._headerTableCell.ApplyStyle(this._headerStyle);
                        if (this.HeaderTemplate != null)
                        {
                            if (this._titleLiteral != null)
                            {
                                this._titleLiteral.Visible = false;
                            }
                        }
                        else if (this._titleLiteral != null)
                        {
                            this._titleLiteral.Text = this.HeaderText;
                        }
                    }
                }
                if ((this._stepTableCell != null) && (this._stepStyle != null))
                {
                    if ((!base.DesignMode && this.IsMacIE5) && (this._stepStyle.Height == Unit.Empty))
                    {
                        this._stepStyle.Height = Unit.Pixel(1);
                    }
                    this._stepTableCell.ApplyStyle(this._stepStyle);
                }
                this.ApplyNavigationTemplateProperties();
                foreach (Control control in this.CustomNavigationContainers.Values)
                {
                    control.Visible = false;
                }
                if (this._navigationTableCell != null)
                {
                    this.NavigationTableCell.HorizontalAlign = HorizontalAlign.Right;
                    if (this._navigationStyle != null)
                    {
                        if ((!base.DesignMode && this.IsMacIE5) && (this._navigationStyle.Height == Unit.Empty))
                        {
                            this._navigationStyle.Height = Unit.Pixel(1);
                        }
                        this._navigationTableCell.ApplyStyle(this._navigationStyle);
                    }
                }
                if (this.ShowCustomNavigationTemplate)
                {
                    BaseNavigationTemplateContainer container = (BaseNavigationTemplateContainer) this._customNavigationContainers[this.ActiveStep];
                    container.Visible = true;
                    this._startNavigationTemplateContainer.Visible = false;
                    this._stepNavigationTemplateContainer.Visible = false;
                    this._finishNavigationTemplateContainer.Visible = false;
                    this._navigationRow.Visible = true;
                }
                if (this.SideBarEnabled)
                {
                    this._sideBarDataList.DataSource = this.WizardSteps;
                    this._sideBarDataList.SelectedIndex = this.ActiveStepIndex;
                    this._sideBarDataList.DataBind();
                    if (this.SideBarTemplate == null)
                    {
                        foreach (DataListItem item in this._sideBarDataList.Items)
                        {
                            WebControl control2 = item.FindControl(SideBarButtonID) as WebControl;
                            if (control2 != null)
                            {
                                control2.MergeStyle(this._sideBarButtonStyle);
                            }
                        }
                    }
                }
                if (this._renderTable != null)
                {
                    Util.CopyBaseAttributesToInnerControl(this, this._renderTable);
                    if (base.ControlStyleCreated)
                    {
                        this._renderTable.ApplyStyle(base.ControlStyle);
                    }
                    else
                    {
                        this._renderTable.CellSpacing = 0;
                        this._renderTable.CellPadding = 0;
                    }
                    if ((!base.DesignMode && this.IsMacIE5) && (!base.ControlStyleCreated || (base.ControlStyle.Height == Unit.Empty)))
                    {
                        this._renderTable.ControlStyle.Height = Unit.Pixel(1);
                    }
                }
                if ((!base.DesignMode && (this._navigationTableCell != null)) && this.IsMacIE5)
                {
                    this._navigationTableCell.ControlStyle.Height = Unit.Pixel(1);
                }
            }
        }

        private void ApplyNavigationTemplateProperties()
        {
            if (((this._finishNavigationTemplateContainer != null) && (this._startNavigationTemplateContainer != null)) && (this._stepNavigationTemplateContainer != null))
            {
                if ((this.ActiveStepIndex < this.WizardSteps.Count) && (this.ActiveStepIndex >= 0))
                {
                    bool flag = ((this.SetActiveTemplates() != WizardStepType.Finish) || (this.ActiveStepIndex != 0)) || (this.ActiveStep.StepType != WizardStepType.Auto);
                    if (this.StartNavigationTemplate == null)
                    {
                        if (base.DesignMode)
                        {
                            this._defaultStartNavigationTemplate.ResetButtonsVisibility();
                        }
                        this._startNavigationTemplateContainer.NextButton = this._defaultStartNavigationTemplate.SecondButton;
                        ((Control) this._startNavigationTemplateContainer.NextButton).Visible = true;
                        this._startNavigationTemplateContainer.CancelButton = this._defaultStartNavigationTemplate.CancelButton;
                        this.ApplyButtonProperties(this.StartNextButtonType, this.StartNextButtonText, this.StartNextButtonImageUrl, this._startNavigationTemplateContainer.NextButton);
                        this.ApplyButtonProperties(this.CancelButtonType, this.CancelButtonText, this.CancelButtonImageUrl, this._startNavigationTemplateContainer.CancelButton);
                        this.SetCancelButtonVisibility(this._startNavigationTemplateContainer);
                        this._startNavigationTemplateContainer.ApplyButtonStyle(this.FinishCompleteButtonStyle, this.StepPreviousButtonStyle, this.StartNextButtonStyle, this.CancelButtonStyle);
                    }
                    bool imageButtonVisible = true;
                    int previousStepIndex = this.GetPreviousStepIndex(false);
                    if (previousStepIndex >= 0)
                    {
                        imageButtonVisible = this.WizardSteps[previousStepIndex].AllowReturn;
                    }
                    if (this.FinishNavigationTemplate == null)
                    {
                        if (base.DesignMode)
                        {
                            this._defaultFinishNavigationTemplate.ResetButtonsVisibility();
                        }
                        this._finishNavigationTemplateContainer.PreviousButton = this._defaultFinishNavigationTemplate.FirstButton;
                        ((Control) this._finishNavigationTemplateContainer.PreviousButton).Visible = true;
                        this._finishNavigationTemplateContainer.FinishButton = this._defaultFinishNavigationTemplate.SecondButton;
                        ((Control) this._finishNavigationTemplateContainer.FinishButton).Visible = true;
                        this._finishNavigationTemplateContainer.CancelButton = this._defaultFinishNavigationTemplate.CancelButton;
                        this._finishNavigationTemplateContainer.FinishButton.CommandName = MoveCompleteCommandName;
                        this.ApplyButtonProperties(this.FinishCompleteButtonType, this.FinishCompleteButtonText, this.FinishCompleteButtonImageUrl, this._finishNavigationTemplateContainer.FinishButton);
                        this.ApplyButtonProperties(this.FinishPreviousButtonType, this.FinishPreviousButtonText, this.FinishPreviousButtonImageUrl, this._finishNavigationTemplateContainer.PreviousButton, imageButtonVisible);
                        this.ApplyButtonProperties(this.CancelButtonType, this.CancelButtonText, this.CancelButtonImageUrl, this._finishNavigationTemplateContainer.CancelButton);
                        int num2 = this.GetPreviousStepIndex(false);
                        if ((num2 != -1) && !this.WizardSteps[num2].AllowReturn)
                        {
                            ((Control) this._finishNavigationTemplateContainer.PreviousButton).Visible = false;
                        }
                        this.SetCancelButtonVisibility(this._finishNavigationTemplateContainer);
                        this._finishNavigationTemplateContainer.ApplyButtonStyle(this.FinishCompleteButtonStyle, this.FinishPreviousButtonStyle, this.StepNextButtonStyle, this.CancelButtonStyle);
                    }
                    if (this.StepNavigationTemplate == null)
                    {
                        if (base.DesignMode)
                        {
                            this._defaultStepNavigationTemplate.ResetButtonsVisibility();
                        }
                        this._stepNavigationTemplateContainer.PreviousButton = this._defaultStepNavigationTemplate.FirstButton;
                        ((Control) this._stepNavigationTemplateContainer.PreviousButton).Visible = true;
                        this._stepNavigationTemplateContainer.NextButton = this._defaultStepNavigationTemplate.SecondButton;
                        ((Control) this._stepNavigationTemplateContainer.NextButton).Visible = true;
                        this._stepNavigationTemplateContainer.CancelButton = this._defaultStepNavigationTemplate.CancelButton;
                        this.ApplyButtonProperties(this.StepNextButtonType, this.StepNextButtonText, this.StepNextButtonImageUrl, this._stepNavigationTemplateContainer.NextButton);
                        this.ApplyButtonProperties(this.StepPreviousButtonType, this.StepPreviousButtonText, this.StepPreviousButtonImageUrl, this._stepNavigationTemplateContainer.PreviousButton, imageButtonVisible);
                        this.ApplyButtonProperties(this.CancelButtonType, this.CancelButtonText, this.CancelButtonImageUrl, this._stepNavigationTemplateContainer.CancelButton);
                        int num3 = this.GetPreviousStepIndex(false);
                        if ((num3 != -1) && !this.WizardSteps[num3].AllowReturn)
                        {
                            ((Control) this._stepNavigationTemplateContainer.PreviousButton).Visible = false;
                        }
                        this.SetCancelButtonVisibility(this._stepNavigationTemplateContainer);
                        this._stepNavigationTemplateContainer.ApplyButtonStyle(this.FinishCompleteButtonStyle, this.StepPreviousButtonStyle, this.StepNextButtonStyle, this.CancelButtonStyle);
                    }
                    if (!flag)
                    {
                        Control previousButton = this._finishNavigationTemplateContainer.PreviousButton as Control;
                        if (previousButton != null)
                        {
                            if (this.FinishNavigationTemplate == null)
                            {
                                previousButton.Parent.Visible = false;
                            }
                            else
                            {
                                previousButton.Visible = false;
                            }
                        }
                    }
                }
            }
        }

        internal BaseNavigationTemplateContainer CreateBaseNavigationTemplateContainer(string id) => 
            new BaseNavigationTemplateContainer(this) { ID = id };

        protected internal override void CreateChildControls()
        {
            using (new WizardControlCollectionModifier(this))
            {
                this.Controls.Clear();
                this._customNavigationContainers = null;
                this._navigationTableCell = null;
            }
            this.CreateControlHierarchy();
            base.ClearChildViewState();
        }

        protected override ControlCollection CreateControlCollection() => 
            new WizardControlCollection(this);

        protected virtual void CreateControlHierarchy()
        {
            Table child = null;
            if (this.DisplaySideBar)
            {
                Table table2 = new WizardChildTable(this) {
                    EnableTheming = false
                };
                child = new WizardDefaultInnerTable {
                    CellSpacing = 0,
                    Height = Unit.Percentage(100.0),
                    Width = Unit.Percentage(100.0)
                };
                TableRow row = new TableRow();
                table2.Controls.Add(row);
                if (this._sideBarTableCell == null)
                {
                    TableCell cell = new AccessibleTableCell(this) {
                        ID = "SideBarContainer",
                        Height = Unit.Percentage(100.0)
                    };
                    this._sideBarTableCell = cell;
                    row.Controls.Add(cell);
                    ITemplate sideBarTemplate = this.SideBarTemplate;
                    if (sideBarTemplate == null)
                    {
                        this._sideBarTableCell.EnableViewState = false;
                        sideBarTemplate = this.CreateDefaultSideBarTemplate();
                    }
                    else
                    {
                        this._sideBarTableCell.EnableTheming = this.EnableTheming;
                    }
                    sideBarTemplate.InstantiateIn(this._sideBarTableCell);
                }
                else
                {
                    row.Controls.Add(this._sideBarTableCell);
                }
                this._renderSideBarDataList = false;
                TableCell cell2 = new TableCell {
                    Height = Unit.Percentage(100.0)
                };
                row.Controls.Add(cell2);
                cell2.Controls.Add(child);
                if (!base.DesignMode && this.IsMacIE5)
                {
                    cell2.Height = Unit.Pixel(1);
                }
                using (new WizardControlCollectionModifier(this))
                {
                    this.Controls.Add(table2);
                }
                if (this._sideBarDataList != null)
                {
                    this._sideBarDataList.ItemCommand -= new DataListCommandEventHandler(this.DataListItemCommand);
                    this._sideBarDataList.ItemDataBound -= new DataListItemEventHandler(this.DataListItemDataBound);
                }
                this._sideBarDataList = this._sideBarTableCell.FindControl(DataListID) as DataList;
                if (this._sideBarDataList != null)
                {
                    this._sideBarDataList.ItemCommand += new DataListCommandEventHandler(this.DataListItemCommand);
                    this._sideBarDataList.ItemDataBound += new DataListItemEventHandler(this.DataListItemDataBound);
                    this._sideBarDataList.DataSource = this.WizardSteps;
                    this._sideBarDataList.SelectedIndex = this.ActiveStepIndex;
                    this._sideBarDataList.DataBind();
                }
                else if (!base.DesignMode)
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("Wizard_DataList_Not_Found", new object[] { DataListID }));
                }
                this._renderTable = table2;
            }
            else
            {
                child = new WizardChildTable(this) {
                    EnableTheming = false
                };
                using (new WizardControlCollectionModifier(this))
                {
                    this.Controls.Add(child);
                }
                this._renderTable = child;
            }
            this._headerTableRow = new TableRow();
            child.Controls.Add(this._headerTableRow);
            this._headerTableCell = new InternalTableCell(this);
            this._headerTableCell.ID = "HeaderContainer";
            if (this.HeaderTemplate != null)
            {
                this._headerTableCell.EnableTheming = this.EnableTheming;
                this.HeaderTemplate.InstantiateIn(this._headerTableCell);
            }
            else
            {
                this._titleLiteral = new LiteralControl();
                this._headerTableCell.Controls.Add(this._titleLiteral);
            }
            this._headerTableRow.Controls.Add(this._headerTableCell);
            TableRow row2 = new TableRow {
                Height = Unit.Percentage(100.0)
            };
            child.Controls.Add(row2);
            this._stepTableCell = new TableCell();
            row2.Controls.Add(this._stepTableCell);
            this._navigationRow = new TableRow();
            child.Controls.Add(this._navigationRow);
            this._navigationRow.Controls.Add(this.NavigationTableCell);
            this._stepTableCell.Controls.Add(this.MultiView);
            this.InstantiateStepContentTemplates();
            this.CreateNavigationControlHierarchy();
        }

        protected override Style CreateControlStyle() => 
            new TableStyle { 
                CellSpacing = 0,
                CellPadding = 0
            };

        internal virtual void CreateCustomNavigationTemplates()
        {
            for (int i = 0; i < this.WizardSteps.Count; i++)
            {
                TemplatedWizardStep step = this.WizardSteps[i] as TemplatedWizardStep;
                if (step != null)
                {
                    this.RegisterCustomNavigationContainers(step);
                }
            }
        }

        internal virtual ITemplate CreateDefaultDataListItemTemplate() => 
            new DataListItemTemplate(this);

        internal virtual ITemplate CreateDefaultSideBarTemplate() => 
            new DefaultSideBarTemplate(this);

        private void CreateFinishNavigationTemplate()
        {
            ITemplate finishNavigationTemplate = this.FinishNavigationTemplate;
            this._finishNavigationTemplateContainer = new FinishNavigationTemplateContainer(this);
            this._finishNavigationTemplateContainer.ID = "FinishNavigationTemplateContainerID";
            if (finishNavigationTemplate == null)
            {
                this._finishNavigationTemplateContainer.EnableViewState = false;
                this._defaultFinishNavigationTemplate = NavigationTemplate.GetDefaultFinishNavigationTemplate(this);
                finishNavigationTemplate = this._defaultFinishNavigationTemplate;
            }
            else
            {
                this._finishNavigationTemplateContainer.SetEnableTheming();
            }
            finishNavigationTemplate.InstantiateIn(this._finishNavigationTemplateContainer);
            this.NavigationTableCell.Controls.Add(this._finishNavigationTemplateContainer);
        }

        internal void CreateNavigationControlHierarchy()
        {
            this.NavigationTableCell.Controls.Clear();
            this.CustomNavigationContainers.Clear();
            this.CreateCustomNavigationTemplates();
            foreach (Control control in this.CustomNavigationContainers.Values)
            {
                this.NavigationTableCell.Controls.Add(control);
            }
            this.CreateStartNavigationTemplate();
            this.CreateFinishNavigationTemplate();
            this.CreateStepNavigationTemplate();
        }

        private void CreateStartNavigationTemplate()
        {
            ITemplate startNavigationTemplate = this.StartNavigationTemplate;
            this._startNavigationTemplateContainer = new StartNavigationTemplateContainer(this);
            this._startNavigationTemplateContainer.ID = "StartNavigationTemplateContainerID";
            if (startNavigationTemplate == null)
            {
                this._startNavigationTemplateContainer.EnableViewState = false;
                this._defaultStartNavigationTemplate = NavigationTemplate.GetDefaultStartNavigationTemplate(this);
                startNavigationTemplate = this._defaultStartNavigationTemplate;
            }
            else
            {
                this._startNavigationTemplateContainer.SetEnableTheming();
            }
            startNavigationTemplate.InstantiateIn(this._startNavigationTemplateContainer);
            this.NavigationTableCell.Controls.Add(this._startNavigationTemplateContainer);
        }

        private void CreateStepNavigationTemplate()
        {
            ITemplate stepNavigationTemplate = this.StepNavigationTemplate;
            this._stepNavigationTemplateContainer = new StepNavigationTemplateContainer(this);
            this._stepNavigationTemplateContainer.ID = "StepNavigationTemplateContainerID";
            if (stepNavigationTemplate == null)
            {
                this._stepNavigationTemplateContainer.EnableViewState = false;
                this._defaultStepNavigationTemplate = NavigationTemplate.GetDefaultStepNavigationTemplate(this);
                stepNavigationTemplate = this._defaultStepNavigationTemplate;
            }
            else
            {
                this._stepNavigationTemplateContainer.SetEnableTheming();
            }
            stepNavigationTemplate.InstantiateIn(this._stepNavigationTemplateContainer);
            this.NavigationTableCell.Controls.Add(this._stepNavigationTemplateContainer);
        }

        internal virtual void DataListItemCommand(object sender, DataListCommandEventArgs e)
        {
            DataListItem item = e.Item;
            if (MoveToCommandName.Equals(e.CommandName, StringComparison.OrdinalIgnoreCase))
            {
                int activeStepIndex = this.ActiveStepIndex;
                int nextStepIndex = int.Parse((string) e.CommandArgument, CultureInfo.InvariantCulture);
                WizardNavigationEventArgs args = new WizardNavigationEventArgs(activeStepIndex, nextStepIndex);
                if (((this._commandSender != null) && !base.DesignMode) && ((this.Page != null) && !this.Page.IsValid))
                {
                    args.Cancel = true;
                }
                this._activeStepIndexSet = false;
                this.OnSideBarButtonClick(args);
                if (!args.Cancel)
                {
                    if (!this._activeStepIndexSet && this.AllowNavigationToStep(nextStepIndex))
                    {
                        this.ActiveStepIndex = nextStepIndex;
                    }
                }
                else
                {
                    this.ActiveStepIndex = activeStepIndex;
                }
            }
        }

        internal virtual void DataListItemDataBound(object sender, DataListItemEventArgs e)
        {
            DataListItem item = e.Item;
            if (((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem)) || ((item.ItemType == ListItemType.SelectedItem) || (item.ItemType == ListItemType.EditItem)))
            {
                IButtonControl button = item.FindControl(SideBarButtonID) as IButtonControl;
                if (button == null)
                {
                    if (!base.DesignMode)
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("Wizard_SideBar_Button_Not_Found", new object[] { DataListID, SideBarButtonID }));
                    }
                }
                else
                {
                    if (button is Button)
                    {
                        ((Button) button).UseSubmitBehavior = false;
                    }
                    WebControl control2 = button as WebControl;
                    if (control2 != null)
                    {
                        control2.TabIndex = this.TabIndex;
                    }
                    int index = 0;
                    WizardStepBase dataItem = item.DataItem as WizardStepBase;
                    if (dataItem != null)
                    {
                        if ((this.GetStepType(dataItem) == WizardStepType.Complete) && (control2 != null))
                        {
                            control2.Enabled = false;
                        }
                        this.RegisterSideBarDataListForRender();
                        if (dataItem.Title.Length > 0)
                        {
                            button.Text = dataItem.Title;
                        }
                        else
                        {
                            button.Text = dataItem.ID;
                        }
                        index = this.WizardSteps.IndexOf(dataItem);
                        button.CommandName = MoveToCommandName;
                        button.CommandArgument = index.ToString(NumberFormatInfo.InvariantInfo);
                        this.RegisterCommandEvents(button);
                    }
                }
            }
        }

        internal string GetCustomContainerID(int index) => 
            ("__CustomNav" + index);

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        protected override IDictionary GetDesignModeState()
        {
            IDictionary designModeState = base.GetDesignModeState();
            this._designModeState = designModeState;
            int activeStepIndex = this.ActiveStepIndex;
            try
            {
                if ((activeStepIndex == -1) && (this.WizardSteps.Count > 0))
                {
                    this.ActiveStepIndex = 0;
                }
                this.RequiresControlsRecreation();
                this.EnsureChildControls();
                this.ApplyControlProperties();
                designModeState["StepTableCell"] = this._stepTableCell;
                if (this._startNavigationTemplateContainer != null)
                {
                    designModeState[StartNextButtonID] = this._startNavigationTemplateContainer.NextButton;
                    designModeState[CancelButtonID] = this._startNavigationTemplateContainer.CancelButton;
                }
                if (this._stepNavigationTemplateContainer != null)
                {
                    designModeState[StepNextButtonID] = this._stepNavigationTemplateContainer.NextButton;
                    designModeState[StepPreviousButtonID] = this._stepNavigationTemplateContainer.PreviousButton;
                    designModeState[CancelButtonID] = this._stepNavigationTemplateContainer.CancelButton;
                }
                if (this._finishNavigationTemplateContainer != null)
                {
                    designModeState[FinishPreviousButtonID] = this._finishNavigationTemplateContainer.PreviousButton;
                    designModeState[FinishButtonID] = this._finishNavigationTemplateContainer.FinishButton;
                    designModeState[CancelButtonID] = this._finishNavigationTemplateContainer.CancelButton;
                }
                if (this.ShowCustomNavigationTemplate)
                {
                    BaseNavigationTemplateContainer container = (BaseNavigationTemplateContainer) this.CustomNavigationContainers[this.ActiveStep];
                    designModeState[CustomNextButtonID] = container.NextButton;
                    designModeState[CustomPreviousButtonID] = container.PreviousButton;
                    designModeState[CustomFinishButtonID] = container.PreviousButton;
                    designModeState[CancelButtonID] = container.CancelButton;
                    designModeState["CustomNavigationControls"] = container.Controls;
                }
                if ((this.SideBarTemplate == null) && (this._sideBarDataList != null))
                {
                    this._sideBarDataList.ItemTemplate = this.CreateDefaultDataListItemTemplate();
                }
                designModeState[DataListID] = this._sideBarDataList;
                designModeState["TemplatedWizardSteps"] = this.TemplatedSteps;
            }
            finally
            {
                this.ActiveStepIndex = activeStepIndex;
            }
            return designModeState;
        }

        public ICollection GetHistory()
        {
            ArrayList list = new ArrayList();
            foreach (int num in this.History)
            {
                list.Add(this.WizardSteps[num]);
            }
            return list;
        }

        internal int GetPreviousStepIndex(bool popStack)
        {
            int num = -1;
            int activeStepIndex = this.ActiveStepIndex;
            if ((this._historyStack != null) && (this._historyStack.Count != 0))
            {
                if (popStack)
                {
                    num = (int) this._historyStack.Pop();
                    if ((num == activeStepIndex) && (this._historyStack.Count > 0))
                    {
                        num = (int) this._historyStack.Pop();
                    }
                }
                else
                {
                    num = (int) this._historyStack.Peek();
                    if ((num == activeStepIndex) && (this._historyStack.Count > 1))
                    {
                        int num3 = (int) this._historyStack.Pop();
                        num = (int) this._historyStack.Peek();
                        this._historyStack.Push(num3);
                    }
                }
                if (num == activeStepIndex)
                {
                    return -1;
                }
            }
            return num;
        }

        internal WizardStepType GetStepType(int index)
        {
            WizardStepBase wizardStep = this.WizardSteps[index];
            return this.GetStepType(wizardStep, index);
        }

        internal WizardStepType GetStepType(WizardStepBase step)
        {
            int index = this.WizardSteps.IndexOf(step);
            return this.GetStepType(step, index);
        }

        public WizardStepType GetStepType(WizardStepBase wizardStep, int index)
        {
            if (wizardStep.StepType != WizardStepType.Auto)
            {
                return wizardStep.StepType;
            }
            if ((this.WizardSteps.Count == 1) || ((index < (this.WizardSteps.Count - 1)) && (this.WizardSteps[index + 1].StepType == WizardStepType.Complete)))
            {
                return WizardStepType.Finish;
            }
            if (index == 0)
            {
                return WizardStepType.Start;
            }
            if (index == (this.WizardSteps.Count - 1))
            {
                return WizardStepType.Finish;
            }
            return WizardStepType.Step;
        }

        internal void InstantiateStepContentTemplate(TemplatedWizardStep step)
        {
            step.Controls.Clear();
            BaseContentTemplateContainer child = new BaseContentTemplateContainer(this);
            ITemplate contentTemplate = step.ContentTemplate;
            if (contentTemplate != null)
            {
                child.SetEnableTheming();
                contentTemplate.InstantiateIn(child.InnerCell);
            }
            step.ContentTemplateContainer = child;
            step.Controls.Add(child);
        }

        internal virtual void InstantiateStepContentTemplates()
        {
            foreach (TemplatedWizardStep step in this.TemplatedSteps)
            {
                TemplatedWizardStep step2 = step;
                this.InstantiateStepContentTemplate(step2);
            }
        }

        protected internal override void LoadControlState(object state)
        {
            Triplet triplet = state as Triplet;
            if (triplet != null)
            {
                base.LoadControlState(triplet.First);
                Array second = triplet.Second as Array;
                if (second != null)
                {
                    Array.Reverse(second);
                    this._historyStack = new Stack(second);
                }
                this.ActiveStepIndex = (int) triplet.Third;
            }
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState == null)
            {
                base.LoadViewState(null);
            }
            else
            {
                object[] objArray = (object[]) savedState;
                if (objArray.Length != 15)
                {
                    throw new ArgumentException(System.Web.SR.GetString("ViewState_InvalidViewState"));
                }
                base.LoadViewState(objArray[0]);
                if (objArray[1] != null)
                {
                    ((IStateManager) this.NavigationButtonStyle).LoadViewState(objArray[1]);
                }
                if (objArray[2] != null)
                {
                    ((IStateManager) this.SideBarButtonStyle).LoadViewState(objArray[2]);
                }
                if (objArray[3] != null)
                {
                    ((IStateManager) this.HeaderStyle).LoadViewState(objArray[3]);
                }
                if (objArray[4] != null)
                {
                    ((IStateManager) this.NavigationStyle).LoadViewState(objArray[4]);
                }
                if (objArray[5] != null)
                {
                    ((IStateManager) this.SideBarStyle).LoadViewState(objArray[5]);
                }
                if (objArray[6] != null)
                {
                    ((IStateManager) this.StepStyle).LoadViewState(objArray[6]);
                }
                if (objArray[7] != null)
                {
                    ((IStateManager) this.StartNextButtonStyle).LoadViewState(objArray[7]);
                }
                if (objArray[8] != null)
                {
                    ((IStateManager) this.StepPreviousButtonStyle).LoadViewState(objArray[8]);
                }
                if (objArray[9] != null)
                {
                    ((IStateManager) this.StepNextButtonStyle).LoadViewState(objArray[9]);
                }
                if (objArray[10] != null)
                {
                    ((IStateManager) this.FinishPreviousButtonStyle).LoadViewState(objArray[10]);
                }
                if (objArray[11] != null)
                {
                    ((IStateManager) this.FinishCompleteButtonStyle).LoadViewState(objArray[11]);
                }
                if (objArray[12] != null)
                {
                    ((IStateManager) this.CancelButtonStyle).LoadViewState(objArray[12]);
                }
                if (objArray[13] != null)
                {
                    ((IStateManager) base.ControlStyle).LoadViewState(objArray[13]);
                }
                if (objArray[14] != null)
                {
                    this.DisplaySideBar = (bool) objArray[14];
                }
            }
        }

        public void MoveTo(WizardStepBase wizardStep)
        {
            if (wizardStep == null)
            {
                throw new ArgumentNullException("wizardStep");
            }
            int index = this.WizardSteps.IndexOf(wizardStep);
            if (index == -1)
            {
                throw new ArgumentException(System.Web.SR.GetString("Wizard_Step_Not_In_Wizard"));
            }
            this.ActiveStepIndex = index;
        }

        private void MultiViewActiveViewChanged(object source, EventArgs e)
        {
            this.OnActiveStepChanged(this, EventArgs.Empty);
        }

        protected virtual void OnActiveStepChanged(object source, EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[_eventActiveStepChanged];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            bool flag = false;
            if (e is CommandEventArgs)
            {
                CommandEventArgs args = (CommandEventArgs) e;
                if (string.Equals(CancelCommandName, args.CommandName, StringComparison.OrdinalIgnoreCase))
                {
                    this.OnCancelButtonClick(EventArgs.Empty);
                    return true;
                }
                int activeStepIndex = this.ActiveStepIndex;
                int nextStepIndex = activeStepIndex;
                bool flag2 = true;
                WizardStepType auto = WizardStepType.Auto;
                WizardStepBase step = this.WizardSteps[activeStepIndex];
                if (step is TemplatedWizardStep)
                {
                    flag2 = false;
                }
                else
                {
                    auto = this.GetStepType(step);
                }
                WizardNavigationEventArgs args2 = new WizardNavigationEventArgs(activeStepIndex, nextStepIndex);
                if (((this._commandSender != null) && (this.Page != null)) && !this.Page.IsValid)
                {
                    args2.Cancel = true;
                }
                bool flag3 = false;
                this._activeStepIndexSet = false;
                if (string.Equals(MoveNextCommandName, args.CommandName, StringComparison.OrdinalIgnoreCase))
                {
                    if ((flag2 && (auto != WizardStepType.Start)) && (auto != WizardStepType.Step))
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("Wizard_InvalidBubbleEvent", new object[] { MoveNextCommandName }));
                    }
                    if (activeStepIndex < (this.WizardSteps.Count - 1))
                    {
                        args2.SetNextStepIndex(activeStepIndex + 1);
                    }
                    this.OnNextButtonClick(args2);
                    flag = true;
                }
                else if (string.Equals(MovePreviousCommandName, args.CommandName, StringComparison.OrdinalIgnoreCase))
                {
                    if ((flag2 && (auto != WizardStepType.Step)) && (auto != WizardStepType.Finish))
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("Wizard_InvalidBubbleEvent", new object[] { MovePreviousCommandName }));
                    }
                    flag3 = true;
                    int previousStepIndex = this.GetPreviousStepIndex(false);
                    if (previousStepIndex != -1)
                    {
                        args2.SetNextStepIndex(previousStepIndex);
                    }
                    this.OnPreviousButtonClick(args2);
                    flag = true;
                }
                else if (string.Equals(MoveCompleteCommandName, args.CommandName, StringComparison.OrdinalIgnoreCase))
                {
                    if (flag2 && (auto != WizardStepType.Finish))
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("Wizard_InvalidBubbleEvent", new object[] { MoveCompleteCommandName }));
                    }
                    if (activeStepIndex < (this.WizardSteps.Count - 1))
                    {
                        args2.SetNextStepIndex(activeStepIndex + 1);
                    }
                    this.OnFinishButtonClick(args2);
                    flag = true;
                }
                else if (string.Equals(MoveToCommandName, args.CommandName, StringComparison.OrdinalIgnoreCase))
                {
                    nextStepIndex = int.Parse((string) args.CommandArgument, CultureInfo.InvariantCulture);
                    args2.SetNextStepIndex(nextStepIndex);
                    flag = true;
                }
                if (flag)
                {
                    if (!args2.Cancel)
                    {
                        if (!this._activeStepIndexSet && this.AllowNavigationToStep(args2.NextStepIndex))
                        {
                            if (flag3)
                            {
                                this.GetPreviousStepIndex(true);
                            }
                            this.ActiveStepIndex = args2.NextStepIndex;
                        }
                        return flag;
                    }
                    this.ActiveStepIndex = activeStepIndex;
                }
            }
            return flag;
        }

        protected virtual void OnCancelButtonClick(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[_eventCancelButtonClick];
            if (handler != null)
            {
                handler(this, e);
            }
            string cancelDestinationPageUrl = this.CancelDestinationPageUrl;
            if (!string.IsNullOrEmpty(cancelDestinationPageUrl))
            {
                this.Page.Response.Redirect(base.ResolveClientUrl(cancelDestinationPageUrl), false);
            }
        }

        private void OnCommand(object sender, CommandEventArgs e)
        {
            this._commandSender = sender as IButtonControl;
        }

        protected virtual void OnFinishButtonClick(WizardNavigationEventArgs e)
        {
            WizardNavigationEventHandler handler = (WizardNavigationEventHandler) base.Events[_eventFinishButtonClick];
            if (handler != null)
            {
                handler(this, e);
            }
            string finishDestinationPageUrl = this.FinishDestinationPageUrl;
            if (!string.IsNullOrEmpty(finishDestinationPageUrl))
            {
                this.Page.Response.Redirect(base.ResolveClientUrl(finishDestinationPageUrl), false);
            }
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (((this.ActiveStepIndex == -1) && (this.WizardSteps.Count > 0)) && !base.DesignMode)
            {
                this.ActiveStepIndex = 0;
            }
            this.EnsureChildControls();
            if (this.Page != null)
            {
                this.Page.RegisterRequiresControlState(this);
            }
        }

        protected virtual void OnNextButtonClick(WizardNavigationEventArgs e)
        {
            WizardNavigationEventHandler handler = (WizardNavigationEventHandler) base.Events[_eventNextButtonClick];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnPreviousButtonClick(WizardNavigationEventArgs e)
        {
            WizardNavigationEventHandler handler = (WizardNavigationEventHandler) base.Events[_eventPreviousButtonClick];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSideBarButtonClick(WizardNavigationEventArgs e)
        {
            WizardNavigationEventHandler handler = (WizardNavigationEventHandler) base.Events[_eventSideBarButtonClick];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void OnWizardStepsChanged()
        {
            if (this._sideBarDataList != null)
            {
                this._sideBarDataList.DataSource = this.WizardSteps;
                this._sideBarDataList.SelectedIndex = this.ActiveStepIndex;
                this._sideBarDataList.DataBind();
            }
        }

        protected internal void RegisterCommandEvents(IButtonControl button)
        {
            if ((button != null) && button.CausesValidation)
            {
                button.Command += new CommandEventHandler(this.OnCommand);
            }
        }

        internal void RegisterCustomNavigationContainers(TemplatedWizardStep step)
        {
            this.InstantiateStepContentTemplate(step);
            if (!this.CustomNavigationContainers.Contains(step))
            {
                BaseNavigationTemplateContainer container = null;
                string customContainerID = this.GetCustomContainerID(this.WizardSteps.IndexOf(step));
                if (step.CustomNavigationTemplate != null)
                {
                    container = this.CreateBaseNavigationTemplateContainer(customContainerID);
                    step.CustomNavigationTemplate.InstantiateIn(container);
                    step.CustomNavigationTemplateContainer = container;
                    container.RegisterButtonCommandEvents();
                }
                else
                {
                    container = this.CreateBaseNavigationTemplateContainer(customContainerID);
                    container.RegisterButtonCommandEvents();
                }
                this.CustomNavigationContainers[step] = container;
            }
        }

        internal void RegisterSideBarDataListForRender()
        {
            this._renderSideBarDataList = true;
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            if (this.Page != null)
            {
                this.Page.VerifyRenderingInServerForm(this);
            }
            this.EnsureChildControls();
            this.ApplyControlProperties();
            if ((this.ActiveStepIndex != -1) && (this.WizardSteps.Count != 0))
            {
                this.RenderContents(writer);
            }
        }

        internal void RequiresControlsRecreation()
        {
            if (base.ChildControlsCreated)
            {
                using (new WizardControlCollectionModifier(this))
                {
                    base.ChildControlsCreated = false;
                }
            }
        }

        protected internal override object SaveControlState()
        {
            int activeStepIndex = this.ActiveStepIndex;
            if (((this._historyStack == null) || (this._historyStack.Count == 0)) || (((int) this._historyStack.Peek()) != activeStepIndex))
            {
                this.History.Push(this.ActiveStepIndex);
            }
            object x = base.SaveControlState();
            bool flag = (this._historyStack != null) && (this._historyStack.Count > 0);
            if (((x != null) || flag) || (activeStepIndex != -1))
            {
                return new Triplet(x, flag ? this._historyStack.ToArray() : null, activeStepIndex);
            }
            return null;
        }

        protected override object SaveViewState()
        {
            object[] objArray = new object[15];
            objArray[0] = base.SaveViewState();
            objArray[1] = (this._navigationButtonStyle != null) ? ((IStateManager) this._navigationButtonStyle).SaveViewState() : null;
            objArray[2] = (this._sideBarButtonStyle != null) ? ((IStateManager) this._sideBarButtonStyle).SaveViewState() : null;
            objArray[3] = (this._headerStyle != null) ? ((IStateManager) this._headerStyle).SaveViewState() : null;
            objArray[4] = (this._navigationStyle != null) ? ((IStateManager) this._navigationStyle).SaveViewState() : null;
            objArray[5] = (this._sideBarStyle != null) ? ((IStateManager) this._sideBarStyle).SaveViewState() : null;
            objArray[6] = (this._stepStyle != null) ? ((IStateManager) this._stepStyle).SaveViewState() : null;
            objArray[7] = (this._startNextButtonStyle != null) ? ((IStateManager) this._startNextButtonStyle).SaveViewState() : null;
            objArray[8] = (this._stepNextButtonStyle != null) ? ((IStateManager) this._stepNextButtonStyle).SaveViewState() : null;
            objArray[9] = (this._stepPreviousButtonStyle != null) ? ((IStateManager) this._stepPreviousButtonStyle).SaveViewState() : null;
            objArray[10] = (this._finishPreviousButtonStyle != null) ? ((IStateManager) this._finishPreviousButtonStyle).SaveViewState() : null;
            objArray[11] = (this._finishCompleteButtonStyle != null) ? ((IStateManager) this._finishCompleteButtonStyle).SaveViewState() : null;
            objArray[12] = (this._cancelButtonStyle != null) ? ((IStateManager) this._cancelButtonStyle).SaveViewState() : null;
            objArray[13] = base.ControlStyleCreated ? ((IStateManager) base.ControlStyle).SaveViewState() : null;
            if (this.DisplaySideBar != this._displaySideBarDefault)
            {
                objArray[14] = this.DisplaySideBar;
            }
            for (int i = 0; i < 15; i++)
            {
                if (objArray[i] != null)
                {
                    return objArray;
                }
            }
            return null;
        }

        private WizardStepType SetActiveTemplates()
        {
            WizardStepType stepType = this.GetStepType(this.ActiveStepIndex);
            if (stepType == WizardStepType.Complete)
            {
                if (this._headerTableRow != null)
                {
                    this._headerTableRow.Visible = false;
                }
                if (this._sideBarTableCell != null)
                {
                    this._sideBarTableCell.Visible = false;
                }
                this._navigationRow.Visible = false;
            }
            else if (this._sideBarTableCell != null)
            {
                this._sideBarTableCell.Visible = this.SideBarEnabled && this._renderSideBarDataList;
            }
            this._startNavigationTemplateContainer.Visible = stepType == WizardStepType.Start;
            this._stepNavigationTemplateContainer.Visible = stepType == WizardStepType.Step;
            this._finishNavigationTemplateContainer.Visible = stepType == WizardStepType.Finish;
            return stepType;
        }

        private void SetCancelButtonVisibility(BaseNavigationTemplateContainer container)
        {
            Control cancelButton = container.CancelButton as Control;
            if (cancelButton != null)
            {
                Control parent = cancelButton.Parent;
                if (parent != null)
                {
                    parent.Visible = this.DisplayCancelButton;
                }
                cancelButton.Visible = this.DisplayCancelButton;
            }
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (this._navigationButtonStyle != null)
            {
                ((IStateManager) this._navigationButtonStyle).TrackViewState();
            }
            if (this._sideBarButtonStyle != null)
            {
                ((IStateManager) this._sideBarButtonStyle).TrackViewState();
            }
            if (this._headerStyle != null)
            {
                ((IStateManager) this._headerStyle).TrackViewState();
            }
            if (this._navigationStyle != null)
            {
                ((IStateManager) this._navigationStyle).TrackViewState();
            }
            if (this._sideBarStyle != null)
            {
                ((IStateManager) this._sideBarStyle).TrackViewState();
            }
            if (this._stepStyle != null)
            {
                ((IStateManager) this._stepStyle).TrackViewState();
            }
            if (this._startNextButtonStyle != null)
            {
                ((IStateManager) this._startNextButtonStyle).TrackViewState();
            }
            if (this._stepPreviousButtonStyle != null)
            {
                ((IStateManager) this._stepPreviousButtonStyle).TrackViewState();
            }
            if (this._stepNextButtonStyle != null)
            {
                ((IStateManager) this._stepNextButtonStyle).TrackViewState();
            }
            if (this._finishPreviousButtonStyle != null)
            {
                ((IStateManager) this._finishPreviousButtonStyle).TrackViewState();
            }
            if (this._finishCompleteButtonStyle != null)
            {
                ((IStateManager) this._finishCompleteButtonStyle).TrackViewState();
            }
            if (this._cancelButtonStyle != null)
            {
                ((IStateManager) this._cancelButtonStyle).TrackViewState();
            }
            if (base.ControlStyleCreated)
            {
                ((IStateManager) base.ControlStyle).TrackViewState();
            }
        }

        private void ValidateButtonType(ButtonType value)
        {
            if ((value < ButtonType.Button) || (value > ButtonType.Link))
            {
                throw new ArgumentOutOfRangeException("value");
            }
        }

        [WebSysDescription("Wizard_ActiveStep"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public WizardStepBase ActiveStep
        {
            get
            {
                if ((this.ActiveStepIndex < -1) || (this.ActiveStepIndex >= this.WizardSteps.Count))
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("Wizard_ActiveStepIndex_out_of_range"));
                }
                return (this.MultiView.GetActiveView() as WizardStepBase);
            }
        }

        [DefaultValue(-1), Themeable(false), WebCategory("Behavior"), WebSysDescription("Wizard_ActiveStepIndex")]
        public virtual int ActiveStepIndex
        {
            get => 
                this.MultiView.ActiveViewIndex;
            set
            {
                if ((value < -1) || ((value >= this.WizardSteps.Count) && (base.ControlState >= ControlState.FrameworkInitialized)))
                {
                    throw new ArgumentOutOfRangeException("value", System.Web.SR.GetString("Wizard_ActiveStepIndex_out_of_range"));
                }
                if (this.MultiView.ActiveViewIndex != value)
                {
                    this.MultiView.ActiveViewIndex = value;
                    this._activeStepIndexSet = true;
                    if ((this._sideBarDataList != null) && (this.SideBarTemplate != null))
                    {
                        this._sideBarDataList.SelectedIndex = this.ActiveStepIndex;
                        this._sideBarDataList.DataBind();
                    }
                }
            }
        }

        [WebSysDescription("Wizard_CancelButtonImageUrl"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("Appearance"), UrlProperty]
        public virtual string CancelButtonImageUrl
        {
            get
            {
                object obj2 = this.ViewState["CancelButtonImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["CancelButtonImageUrl"] = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue((string) null), WebCategory("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("Wizard_CancelButtonStyle")]
        public Style CancelButtonStyle
        {
            get
            {
                if (this._cancelButtonStyle == null)
                {
                    this._cancelButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._cancelButtonStyle).TrackViewState();
                    }
                }
                return this._cancelButtonStyle;
            }
        }

        [Localizable(true), WebCategory("Appearance"), WebSysDefaultValue("Wizard_Default_CancelButtonText"), WebSysDescription("Wizard_CancelButtonText")]
        public virtual string CancelButtonText
        {
            get
            {
                string str = this.ViewState["CancelButtonText"] as string;
                if (str != null)
                {
                    return str;
                }
                return System.Web.SR.GetString("Wizard_Default_CancelButtonText");
            }
            set
            {
                if (value != this.CancelButtonText)
                {
                    this.ViewState["CancelButtonText"] = value;
                }
            }
        }

        [DefaultValue(0), WebCategory("Appearance"), WebSysDescription("Wizard_CancelButtonType")]
        public virtual ButtonType CancelButtonType
        {
            get
            {
                object obj2 = this.ViewState["CancelButtonType"];
                if (obj2 != null)
                {
                    return (ButtonType) obj2;
                }
                return ButtonType.Button;
            }
            set
            {
                this.ValidateButtonType(value);
                this.ViewState["CancelButtonType"] = value;
            }
        }

        [WebSysDescription("Wizard_CancelDestinationPageUrl"), Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Themeable(false), WebCategory("Behavior"), DefaultValue(""), UrlProperty]
        public virtual string CancelDestinationPageUrl
        {
            get
            {
                string str = this.ViewState["CancelDestinationPageUrl"] as string;
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["CancelDestinationPageUrl"] = value;
            }
        }

        [WebCategory("Layout"), DefaultValue(0), WebSysDescription("Wizard_CellPadding")]
        public virtual int CellPadding
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return 0;
                }
                return ((TableStyle) base.ControlStyle).CellPadding;
            }
            set
            {
                ((TableStyle) base.ControlStyle).CellPadding = value;
            }
        }

        [WebCategory("Layout"), DefaultValue(0), WebSysDescription("Wizard_CellSpacing")]
        public virtual int CellSpacing
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return 0;
                }
                return ((TableStyle) base.ControlStyle).CellSpacing;
            }
            set
            {
                ((TableStyle) base.ControlStyle).CellSpacing = value;
            }
        }

        internal IDictionary CustomNavigationContainers
        {
            get
            {
                if (this._customNavigationContainers == null)
                {
                    this._customNavigationContainers = new Hashtable();
                }
                return this._customNavigationContainers;
            }
        }

        internal ITemplate CustomNavigationTemplate
        {
            get
            {
                if ((this.ActiveStep != null) && (this.ActiveStep is TemplatedWizardStep))
                {
                    return ((TemplatedWizardStep) this.ActiveStep).CustomNavigationTemplate;
                }
                return null;
            }
        }

        [WebSysDescription("Wizard_DisplayCancelButton"), DefaultValue(false), Themeable(false), WebCategory("Behavior")]
        public virtual bool DisplayCancelButton
        {
            get
            {
                object obj2 = this.ViewState["DisplayCancelButton"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                this.ViewState["DisplayCancelButton"] = value;
            }
        }

        [DefaultValue(true), Themeable(false), WebCategory("Behavior"), WebSysDescription("Wizard_DisplaySideBar")]
        public virtual bool DisplaySideBar
        {
            get => 
                this._displaySideBar;
            set
            {
                if (value != this._displaySideBar)
                {
                    this._displaySideBar = value;
                    this._sideBarTableCell = null;
                    this.RequiresControlsRecreation();
                }
            }
        }

        [WebSysDescription("Wizard_FinishCompleteButtonImageUrl"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("Appearance"), UrlProperty]
        public virtual string FinishCompleteButtonImageUrl
        {
            get
            {
                object obj2 = this.ViewState["FinishCompleteButtonImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["FinishCompleteButtonImageUrl"] = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), WebCategory("Styles"), WebSysDescription("Wizard_FinishCompleteButtonStyle")]
        public Style FinishCompleteButtonStyle
        {
            get
            {
                if (this._finishCompleteButtonStyle == null)
                {
                    this._finishCompleteButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._finishCompleteButtonStyle).TrackViewState();
                    }
                }
                return this._finishCompleteButtonStyle;
            }
        }

        [Localizable(true), WebCategory("Appearance"), WebSysDefaultValue("Wizard_Default_FinishButtonText"), WebSysDescription("Wizard_FinishCompleteButtonText")]
        public virtual string FinishCompleteButtonText
        {
            get
            {
                string str = this.ViewState["FinishCompleteButtonText"] as string;
                if (str != null)
                {
                    return str;
                }
                return System.Web.SR.GetString("Wizard_Default_FinishButtonText");
            }
            set
            {
                this.ViewState["FinishCompleteButtonText"] = value;
            }
        }

        [WebCategory("Appearance"), DefaultValue(0), WebSysDescription("Wizard_FinishCompleteButtonType")]
        public virtual ButtonType FinishCompleteButtonType
        {
            get
            {
                object obj2 = this.ViewState["FinishCompleteButtonType"];
                if (obj2 != null)
                {
                    return (ButtonType) obj2;
                }
                return ButtonType.Button;
            }
            set
            {
                this.ValidateButtonType(value);
                this.ViewState["FinishCompleteButtonType"] = value;
            }
        }

        [Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), Themeable(false), WebCategory("Behavior"), WebSysDescription("Wizard_FinishDestinationPageUrl"), UrlProperty]
        public virtual string FinishDestinationPageUrl
        {
            get
            {
                object obj2 = this.ViewState["FinishDestinationPageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["FinishDestinationPageUrl"] = value;
            }
        }

        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(Wizard)), WebSysDescription("Wizard_FinishNavigationTemplate"), DefaultValue((string) null)]
        public virtual ITemplate FinishNavigationTemplate
        {
            get => 
                this._finishNavigationTemplate;
            set
            {
                this._finishNavigationTemplate = value;
                this.RequiresControlsRecreation();
            }
        }

        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, WebCategory("Appearance"), WebSysDescription("Wizard_FinishPreviousButtonImageUrl"), DefaultValue("")]
        public virtual string FinishPreviousButtonImageUrl
        {
            get
            {
                object obj2 = this.ViewState["FinishPreviousButtonImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["FinishPreviousButtonImageUrl"] = value;
            }
        }

        [DefaultValue((string) null), WebCategory("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), WebSysDescription("Wizard_FinishPreviousButtonStyle")]
        public Style FinishPreviousButtonStyle
        {
            get
            {
                if (this._finishPreviousButtonStyle == null)
                {
                    this._finishPreviousButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._finishPreviousButtonStyle).TrackViewState();
                    }
                }
                return this._finishPreviousButtonStyle;
            }
        }

        [WebSysDescription("Wizard_FinishPreviousButtonText"), Localizable(true), WebCategory("Appearance"), WebSysDefaultValue("Wizard_Default_StepPreviousButtonText")]
        public virtual string FinishPreviousButtonText
        {
            get
            {
                string str = this.ViewState["FinishPreviousButtonText"] as string;
                if (str != null)
                {
                    return str;
                }
                return System.Web.SR.GetString("Wizard_Default_StepPreviousButtonText");
            }
            set
            {
                this.ViewState["FinishPreviousButtonText"] = value;
            }
        }

        [WebCategory("Appearance"), DefaultValue(0), WebSysDescription("Wizard_FinishPreviousButtonType")]
        public virtual ButtonType FinishPreviousButtonType
        {
            get
            {
                object obj2 = this.ViewState["FinishPreviousButtonType"];
                if (obj2 != null)
                {
                    return (ButtonType) obj2;
                }
                return ButtonType.Button;
            }
            set
            {
                this.ValidateButtonType(value);
                this.ViewState["FinishPreviousButtonType"] = value;
            }
        }

        [NotifyParentProperty(true), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), WebCategory("Styles"), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("WebControl_HeaderStyle")]
        public TableItemStyle HeaderStyle
        {
            get
            {
                if (this._headerStyle == null)
                {
                    this._headerStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._headerStyle).TrackViewState();
                    }
                }
                return this._headerStyle;
            }
        }

        [Browsable(false), TemplateContainer(typeof(Wizard)), WebSysDescription("WebControl_HeaderTemplate"), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate HeaderTemplate
        {
            get => 
                this._headerTemplate;
            set
            {
                this._headerTemplate = value;
                this.RequiresControlsRecreation();
            }
        }

        [DefaultValue(""), WebSysDescription("Wizard_HeaderText"), WebCategory("Appearance"), Localizable(true)]
        public virtual string HeaderText
        {
            get
            {
                string str = this.ViewState["HeaderText"] as string;
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["HeaderText"] = value;
            }
        }

        private Stack History
        {
            get
            {
                if (this._historyStack == null)
                {
                    this._historyStack = new Stack();
                }
                return this._historyStack;
            }
        }

        internal bool IsMacIE5
        {
            get
            {
                if (!this._isMacIESet && !base.DesignMode)
                {
                    HttpBrowserCapabilities browser = null;
                    if (this.Page != null)
                    {
                        browser = this.Page.Request.Browser;
                    }
                    else
                    {
                        HttpContext current = HttpContext.Current;
                        if (current != null)
                        {
                            browser = current.Request.Browser;
                        }
                    }
                    if (browser != null)
                    {
                        this._isMacIE = (browser.Type == "IE5") && (browser.Platform == "MacPPC");
                    }
                    this._isMacIESet = true;
                }
                return this._isMacIE;
            }
        }

        internal System.Web.UI.WebControls.MultiView MultiView
        {
            get
            {
                if (this._multiView == null)
                {
                    this._multiView = new System.Web.UI.WebControls.MultiView();
                    this._multiView.EnableTheming = true;
                    this._multiView.ID = "WizardMultiView";
                    this._multiView.ActiveViewChanged += new EventHandler(this.MultiViewActiveViewChanged);
                    this._multiView.IgnoreBubbleEvents();
                }
                return this._multiView;
            }
        }

        [NotifyParentProperty(true), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), WebCategory("Styles"), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("Wizard_NavigationButtonStyle")]
        public Style NavigationButtonStyle
        {
            get
            {
                if (this._navigationButtonStyle == null)
                {
                    this._navigationButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._navigationButtonStyle).TrackViewState();
                    }
                }
                return this._navigationButtonStyle;
            }
        }

        [NotifyParentProperty(true), WebSysDescription("Wizard_NavigationStyle"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), WebCategory("Styles"), PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle NavigationStyle
        {
            get
            {
                if (this._navigationStyle == null)
                {
                    this._navigationStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._navigationStyle).TrackViewState();
                    }
                }
                return this._navigationStyle;
            }
        }

        internal TableCell NavigationTableCell
        {
            get
            {
                if (this._navigationTableCell == null)
                {
                    this._navigationTableCell = new TableCell();
                }
                return this._navigationTableCell;
            }
        }

        internal bool ShouldRenderChildControl
        {
            get
            {
                if (base.DesignMode)
                {
                    if (this._designModeState == null)
                    {
                        return true;
                    }
                    object obj2 = this._designModeState["ShouldRenderWizardSteps"];
                    if (obj2 != null)
                    {
                        return (bool) obj2;
                    }
                }
                return true;
            }
        }

        internal virtual bool ShowCustomNavigationTemplate =>
            (this.CustomNavigationTemplate != null);

        [WebCategory("Styles"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("Wizard_SideBarButtonStyle")]
        public Style SideBarButtonStyle
        {
            get
            {
                if (this._sideBarButtonStyle == null)
                {
                    this._sideBarButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._sideBarButtonStyle).TrackViewState();
                    }
                }
                return this._sideBarButtonStyle;
            }
        }

        internal DataList SideBarDataList =>
            this._sideBarDataList;

        internal bool SideBarEnabled =>
            ((this._sideBarDataList != null) && this.DisplaySideBar);

        [PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Styles"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), WebSysDescription("Wizard_SideBarStyle")]
        public TableItemStyle SideBarStyle
        {
            get
            {
                if (this._sideBarStyle == null)
                {
                    this._sideBarStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._sideBarStyle).TrackViewState();
                    }
                }
                return this._sideBarStyle;
            }
        }

        [TemplateContainer(typeof(Wizard)), Browsable(false), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("Wizard_SideBarTemplate")]
        public virtual ITemplate SideBarTemplate
        {
            get => 
                this._sideBarTemplate;
            set
            {
                this._sideBarTemplate = value;
                this._sideBarTableCell = null;
                this.RequiresControlsRecreation();
            }
        }

        [WebCategory("Appearance"), Localizable(true), WebSysDescription("WebControl_SkipLinkText"), WebSysDefaultValue("Wizard_Default_SkipToContentText")]
        public virtual string SkipLinkText
        {
            get
            {
                string skipLinkTextInternal = this.SkipLinkTextInternal;
                if (skipLinkTextInternal != null)
                {
                    return skipLinkTextInternal;
                }
                return System.Web.SR.GetString("Wizard_Default_SkipToContentText");
            }
            set
            {
                this.ViewState["SkipLinkText"] = value;
            }
        }

        internal string SkipLinkTextInternal =>
            (this.ViewState["SkipLinkText"] as string);

        [WebSysDescription("Wizard_StartNavigationTemplate"), Browsable(false), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(Wizard))]
        public virtual ITemplate StartNavigationTemplate
        {
            get => 
                this._startNavigationTemplate;
            set
            {
                this._startNavigationTemplate = value;
                this.RequiresControlsRecreation();
            }
        }

        [WebCategory("Appearance"), UrlProperty, Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), WebSysDescription("Wizard_StartNextButtonImageUrl")]
        public virtual string StartNextButtonImageUrl
        {
            get
            {
                object obj2 = this.ViewState["StartNextButtonImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["StartNextButtonImageUrl"] = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), WebCategory("Styles"), WebSysDescription("Wizard_StartNextButtonStyle")]
        public Style StartNextButtonStyle
        {
            get
            {
                if (this._startNextButtonStyle == null)
                {
                    this._startNextButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._startNextButtonStyle).TrackViewState();
                    }
                }
                return this._startNextButtonStyle;
            }
        }

        [WebCategory("Appearance"), WebSysDefaultValue("Wizard_Default_StepNextButtonText"), Localizable(true), WebSysDescription("Wizard_StartNextButtonText")]
        public virtual string StartNextButtonText
        {
            get
            {
                string str = this.ViewState["StartNextButtonText"] as string;
                if (str != null)
                {
                    return str;
                }
                return System.Web.SR.GetString("Wizard_Default_StepNextButtonText");
            }
            set
            {
                this.ViewState["StartNextButtonText"] = value;
            }
        }

        [WebSysDescription("Wizard_StartNextButtonType"), WebCategory("Appearance"), DefaultValue(0)]
        public virtual ButtonType StartNextButtonType
        {
            get
            {
                object obj2 = this.ViewState["StartNextButtonType"];
                if (obj2 != null)
                {
                    return (ButtonType) obj2;
                }
                return ButtonType.Button;
            }
            set
            {
                this.ValidateButtonType(value);
                this.ViewState["StartNextButtonType"] = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("Wizard_StepNavigationTemplate"), Browsable(false), DefaultValue((string) null), TemplateContainer(typeof(Wizard))]
        public virtual ITemplate StepNavigationTemplate
        {
            get => 
                this._stepNavigationTemplate;
            set
            {
                this._stepNavigationTemplate = value;
                this.RequiresControlsRecreation();
            }
        }

        [WebSysDescription("Wizard_StepNextButtonImageUrl"), WebCategory("Appearance"), DefaultValue(""), UrlProperty, Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual string StepNextButtonImageUrl
        {
            get
            {
                object obj2 = this.ViewState["StepNextButtonImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["StepNextButtonImageUrl"] = value;
            }
        }

        [WebCategory("Styles"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("Wizard_StepNextButtonStyle")]
        public Style StepNextButtonStyle
        {
            get
            {
                if (this._stepNextButtonStyle == null)
                {
                    this._stepNextButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._stepNextButtonStyle).TrackViewState();
                    }
                }
                return this._stepNextButtonStyle;
            }
        }

        [WebSysDescription("Wizard_StepNextButtonText"), Localizable(true), WebSysDefaultValue("Wizard_Default_StepNextButtonText"), WebCategory("Appearance")]
        public virtual string StepNextButtonText
        {
            get
            {
                string str = this.ViewState["StepNextButtonText"] as string;
                if (str != null)
                {
                    return str;
                }
                return System.Web.SR.GetString("Wizard_Default_StepNextButtonText");
            }
            set
            {
                this.ViewState["StepNextButtonText"] = value;
            }
        }

        [WebSysDescription("Wizard_StepNextButtonType"), DefaultValue(0), WebCategory("Appearance")]
        public virtual ButtonType StepNextButtonType
        {
            get
            {
                object obj2 = this.ViewState["StepNextButtonType"];
                if (obj2 != null)
                {
                    return (ButtonType) obj2;
                }
                return ButtonType.Button;
            }
            set
            {
                this.ValidateButtonType(value);
                this.ViewState["StepNextButtonType"] = value;
            }
        }

        [WebCategory("Appearance"), DefaultValue(""), WebSysDescription("Wizard_StepPreviousButtonImageUrl"), UrlProperty, Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual string StepPreviousButtonImageUrl
        {
            get
            {
                object obj2 = this.ViewState["StepPreviousButtonImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["StepPreviousButtonImageUrl"] = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), WebCategory("Styles"), WebSysDescription("Wizard_StepPreviousButtonStyle"), DefaultValue((string) null)]
        public Style StepPreviousButtonStyle
        {
            get
            {
                if (this._stepPreviousButtonStyle == null)
                {
                    this._stepPreviousButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._stepPreviousButtonStyle).TrackViewState();
                    }
                }
                return this._stepPreviousButtonStyle;
            }
        }

        [Localizable(true), WebSysDefaultValue("Wizard_Default_StepPreviousButtonText"), WebSysDescription("Wizard_StepPreviousButtonText"), WebCategory("Appearance")]
        public virtual string StepPreviousButtonText
        {
            get
            {
                string str = this.ViewState["StepPreviousButtonText"] as string;
                if (str != null)
                {
                    return str;
                }
                return System.Web.SR.GetString("Wizard_Default_StepPreviousButtonText");
            }
            set
            {
                this.ViewState["StepPreviousButtonText"] = value;
            }
        }

        [WebCategory("Appearance"), DefaultValue(0), WebSysDescription("Wizard_StepPreviousButtonType")]
        public virtual ButtonType StepPreviousButtonType
        {
            get
            {
                object obj2 = this.ViewState["StepPreviousButtonType"];
                if (obj2 != null)
                {
                    return (ButtonType) obj2;
                }
                return ButtonType.Button;
            }
            set
            {
                this.ValidateButtonType(value);
                this.ViewState["StepPreviousButtonType"] = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), WebCategory("Styles"), WebSysDescription("Wizard_StepStyle"), DefaultValue((string) null), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle StepStyle
        {
            get
            {
                if (this._stepStyle == null)
                {
                    this._stepStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._stepStyle).TrackViewState();
                    }
                }
                return this._stepStyle;
            }
        }

        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Table;

        internal ArrayList TemplatedSteps
        {
            get
            {
                if (this._templatedSteps == null)
                {
                    this._templatedSteps = new ArrayList();
                }
                return this._templatedSteps;
            }
        }

        [Editor("System.Web.UI.Design.WebControls.WizardStepCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Themeable(false), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), WebSysDescription("Wizard_WizardSteps")]
        public virtual WizardStepCollection WizardSteps
        {
            get
            {
                if (this._wizardStepCollection == null)
                {
                    this._wizardStepCollection = new WizardStepCollection(this);
                }
                return this._wizardStepCollection;
            }
        }

        private class AccessibleTableCell : Wizard.InternalTableCell
        {
            internal AccessibleTableCell(Wizard owner) : base(owner)
            {
            }

            protected internal override void RenderChildren(HtmlTextWriter writer)
            {
                bool flag = !string.IsNullOrEmpty(base._owner.SkipLinkText) && !base._owner.DesignMode;
                string str = base._owner.ClientID + "_SkipLink";
                if (flag)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, "#" + str);
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.AddAttribute(HtmlTextWriterAttribute.Alt, base._owner.SkipLinkText);
                    writer.AddAttribute(HtmlTextWriterAttribute.Height, "0");
                    writer.AddAttribute(HtmlTextWriterAttribute.Width, "0");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, base.SpacerImageUrl);
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }
                base.RenderChildren(writer);
                if (flag)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, str);
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.RenderEndTag();
                }
            }
        }

        internal class BaseContentTemplateContainer : Wizard.BlockControl
        {
            internal BaseContentTemplateContainer(Wizard owner) : base(owner)
            {
                base.Table.Width = Unit.Percentage(100.0);
                base.Table.Height = Unit.Percentage(100.0);
            }
        }

        internal class BaseNavigationTemplateContainer : WebControl, INonBindingContainer, INamingContainer
        {
            private IButtonControl _cancelButton;
            private IButtonControl _finishButton;
            private IButtonControl _nextButton;
            private Wizard _owner;
            private IButtonControl _previousButton;

            internal BaseNavigationTemplateContainer(Wizard owner)
            {
                this._owner = owner;
            }

            internal virtual void ApplyButtonStyle(Style finishStyle, Style prevStyle, Style nextStyle, Style cancelStyle)
            {
                if (this.FinishButton != null)
                {
                    this.ApplyButtonStyleInternal(this.FinishButton, finishStyle);
                }
                if (this.PreviousButton != null)
                {
                    this.ApplyButtonStyleInternal(this.PreviousButton, prevStyle);
                }
                if (this.NextButton != null)
                {
                    this.ApplyButtonStyleInternal(this.NextButton, nextStyle);
                }
                if (this.CancelButton != null)
                {
                    this.ApplyButtonStyleInternal(this.CancelButton, cancelStyle);
                }
            }

            protected void ApplyButtonStyleInternal(IButtonControl control, Style buttonStyle)
            {
                WebControl control2 = control as WebControl;
                if (control2 != null)
                {
                    control2.ApplyStyle(buttonStyle);
                    control2.ControlStyle.MergeWith(this.Owner.NavigationButtonStyle);
                }
            }

            public override void Focus()
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoFocusSupport", new object[] { base.GetType().Name }));
            }

            internal virtual void RegisterButtonCommandEvents()
            {
                this.Owner.RegisterCommandEvents(this.NextButton);
                this.Owner.RegisterCommandEvents(this.FinishButton);
                this.Owner.RegisterCommandEvents(this.PreviousButton);
                this.Owner.RegisterCommandEvents(this.CancelButton);
            }

            protected internal override void Render(HtmlTextWriter writer)
            {
                this.RenderContents(writer);
            }

            internal void SetEnableTheming()
            {
                this.EnableTheming = this._owner.EnableTheming;
            }

            internal virtual IButtonControl CancelButton
            {
                get
                {
                    if (this._cancelButton == null)
                    {
                        this._cancelButton = this.FindControl(Wizard.CancelButtonID) as IButtonControl;
                    }
                    return this._cancelButton;
                }
                set
                {
                    this._cancelButton = value;
                }
            }

            internal virtual IButtonControl FinishButton
            {
                get
                {
                    if (this._finishButton == null)
                    {
                        this._finishButton = this.FindControl(Wizard.FinishButtonID) as IButtonControl;
                    }
                    return this._finishButton;
                }
                set
                {
                    this._finishButton = value;
                }
            }

            internal virtual IButtonControl NextButton
            {
                get
                {
                    if (this._nextButton == null)
                    {
                        this._nextButton = this.FindControl(Wizard.StepNextButtonID) as IButtonControl;
                    }
                    return this._nextButton;
                }
                set
                {
                    this._nextButton = value;
                }
            }

            internal Wizard Owner =>
                this._owner;

            internal virtual IButtonControl PreviousButton
            {
                get
                {
                    if (this._previousButton == null)
                    {
                        this._previousButton = this.FindControl(Wizard.StepPreviousButtonID) as IButtonControl;
                    }
                    return this._previousButton;
                }
                set
                {
                    this._previousButton = value;
                }
            }
        }

        internal abstract class BlockControl : WebControl, INonBindingContainer, INamingContainer
        {
            internal TableCell _cell;
            internal Wizard _owner;
            private System.Web.UI.WebControls.Table _table;

            internal BlockControl(Wizard owner)
            {
                this._owner = owner;
                this._table = new WizardDefaultInnerTable();
                this._table.EnableTheming = false;
                this.Controls.Add(this._table);
                TableRow child = new TableRow();
                this._table.Controls.Add(child);
                this._cell = new TableCell();
                this._cell.Height = Unit.Percentage(100.0);
                this._cell.Width = Unit.Percentage(100.0);
                child.Controls.Add(this._cell);
                this.HandleMacIECellHeight();
                base.PreventAutoID();
            }

            protected override Style CreateControlStyle() => 
                new TableItemStyle(this.ViewState);

            public override void Focus()
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoFocusSupport", new object[] { base.GetType().Name }));
            }

            internal void HandleMacIECellHeight()
            {
                if (!this._owner.DesignMode && this._owner.IsMacIE5)
                {
                    this._cell.Height = Unit.Pixel(1);
                }
            }

            protected internal override void Render(HtmlTextWriter writer)
            {
                this.RenderContents(writer);
            }

            internal void SetEnableTheming()
            {
                this._cell.EnableTheming = this._owner.EnableTheming;
            }

            internal TableCell InnerCell =>
                this._cell;

            protected System.Web.UI.WebControls.Table Table =>
                this._table;
        }

        private class DataListItemTemplate : ITemplate
        {
            private Wizard _owner;

            internal DataListItemTemplate(Wizard owner)
            {
                this._owner = owner;
            }

            public void InstantiateIn(Control container)
            {
                LinkButton child = new LinkButton();
                container.Controls.Add(child);
                child.ID = Wizard.SideBarButtonID;
                if (this._owner.DesignMode)
                {
                    child.MergeStyle(this._owner.SideBarButtonStyle);
                }
            }
        }

        private class DefaultSideBarTemplate : ITemplate
        {
            private Wizard _owner;

            internal DefaultSideBarTemplate(Wizard owner)
            {
                this._owner = owner;
            }

            public void InstantiateIn(Control container)
            {
                DataList child = null;
                if (this._owner.SideBarDataList == null)
                {
                    child = new DataList {
                        ID = Wizard.DataListID,
                        SelectedItemStyle = { Font = { Bold = true } },
                        ItemTemplate = this._owner.CreateDefaultDataListItemTemplate()
                    };
                }
                else
                {
                    child = this._owner.SideBarDataList;
                }
                container.Controls.Add(child);
            }
        }

        internal class FinishNavigationTemplateContainer : Wizard.BaseNavigationTemplateContainer
        {
            private IButtonControl _previousButton;

            internal FinishNavigationTemplateContainer(Wizard owner) : base(owner)
            {
            }

            internal override IButtonControl PreviousButton
            {
                get
                {
                    if (this._previousButton == null)
                    {
                        this._previousButton = this.FindControl(Wizard.FinishPreviousButtonID) as IButtonControl;
                    }
                    return this._previousButton;
                }
                set
                {
                    this._previousButton = value;
                }
            }
        }

        private class InternalTableCell : TableCell, INonBindingContainer, INamingContainer
        {
            protected Wizard _owner;

            internal InternalTableCell(Wizard owner)
            {
                this._owner = owner;
            }

            protected override void AddAttributesToRender(HtmlTextWriter writer)
            {
                if (base.ControlStyleCreated && !base.ControlStyle.IsEmpty)
                {
                    base.ControlStyle.AddAttributesToRender(writer, this);
                }
            }
        }

        private sealed class NavigationTemplate : ITemplate
        {
            private bool _button1CausesValidation;
            private string _button1ID;
            private string _button2ID;
            private string _button3ID;
            private IButtonControl[][] _buttons;
            private const string _cancelButtonID = "Cancel";
            private const string _finishButtonID = "Finish";
            private const string _finishPreviousButtonID = "FinishPrevious";
            private TableRow _row;
            private const string _startNextButtonID = "StartNext";
            private const string _stepNextButtonID = "StepNext";
            private const string _stepPreviousButtonID = "StepPrevious";
            private Wizard.WizardTemplateType _templateType;
            private Wizard _wizard;

            private NavigationTemplate(Wizard wizard, Wizard.WizardTemplateType templateType, bool button1CausesValidation, string label1ID, string label2ID, string label3ID)
            {
                this._wizard = wizard;
                this._button1ID = label1ID;
                this._button2ID = label2ID;
                this._button3ID = label3ID;
                this._templateType = templateType;
                this._buttons = new IButtonControl[][] { new IButtonControl[3], new IButtonControl[3], new IButtonControl[3] };
                this._button1CausesValidation = button1CausesValidation;
            }

            private void CreateButtonControl(IButtonControl[] buttons, string id, bool causesValidation, string commandName)
            {
                LinkButton button = new LinkButton {
                    CausesValidation = causesValidation,
                    ID = id + "LinkButton",
                    Visible = false,
                    CommandName = commandName,
                    TabIndex = this._wizard.TabIndex
                };
                this._wizard.RegisterCommandEvents(button);
                buttons[0] = button;
                ImageButton button2 = new ImageButton {
                    CausesValidation = causesValidation,
                    ID = id + "ImageButton",
                    Visible = true,
                    CommandName = commandName,
                    TabIndex = this._wizard.TabIndex
                };
                this._wizard.RegisterCommandEvents(button2);
                button2.PreRender += new EventHandler(this.OnPreRender);
                buttons[1] = button2;
                Button button3 = new Button {
                    CausesValidation = causesValidation,
                    ID = id + "Button",
                    Visible = false,
                    CommandName = commandName,
                    TabIndex = this._wizard.TabIndex
                };
                this._wizard.RegisterCommandEvents(button3);
                buttons[2] = button3;
                TableCell cell = new TableCell {
                    HorizontalAlign = HorizontalAlign.Right
                };
                this._row.Cells.Add(cell);
                cell.Controls.Add(button);
                cell.Controls.Add(button2);
                cell.Controls.Add(button3);
            }

            private IButtonControl GetButtonBasedOnType(int pos, ButtonType type)
            {
                switch (type)
                {
                    case ButtonType.Button:
                        return this._buttons[pos][2];

                    case ButtonType.Image:
                        return this._buttons[pos][1];

                    case ButtonType.Link:
                        return this._buttons[pos][0];
                }
                return null;
            }

            internal static Wizard.NavigationTemplate GetDefaultFinishNavigationTemplate(Wizard wizard) => 
                new Wizard.NavigationTemplate(wizard, Wizard.WizardTemplateType.FinishNavigationTemplate, false, "FinishPrevious", "Finish", "Cancel");

            internal static Wizard.NavigationTemplate GetDefaultStartNavigationTemplate(Wizard wizard) => 
                new Wizard.NavigationTemplate(wizard, Wizard.WizardTemplateType.StartNavigationTemplate, true, null, "StartNext", "Cancel");

            internal static Wizard.NavigationTemplate GetDefaultStepNavigationTemplate(Wizard wizard) => 
                new Wizard.NavigationTemplate(wizard, Wizard.WizardTemplateType.StepNavigationTemplate, false, "StepPrevious", "StepNext", "Cancel");

            private void OnPreRender(object source, EventArgs e)
            {
                ((ImageButton) source).Visible = false;
            }

            internal void ResetButtonsVisibility()
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Control control = this._buttons[i][j] as Control;
                        if (control != null)
                        {
                            control.Visible = false;
                        }
                    }
                }
            }

            void ITemplate.InstantiateIn(Control container)
            {
                Table child = new WizardDefaultInnerTable {
                    CellSpacing = 5,
                    CellPadding = 5
                };
                container.Controls.Add(child);
                this._row = new TableRow();
                child.Rows.Add(this._row);
                if (this._button1ID != null)
                {
                    this.CreateButtonControl(this._buttons[0], this._button1ID, this._button1CausesValidation, Wizard.MovePreviousCommandName);
                }
                if (this._button2ID != null)
                {
                    this.CreateButtonControl(this._buttons[1], this._button2ID, true, (this._templateType == Wizard.WizardTemplateType.FinishNavigationTemplate) ? Wizard.MoveCompleteCommandName : Wizard.MoveNextCommandName);
                }
                this.CreateButtonControl(this._buttons[2], this._button3ID, false, Wizard.CancelCommandName);
            }

            internal IButtonControl CancelButton
            {
                get
                {
                    ButtonType cancelButtonType = this._wizard.CancelButtonType;
                    return this.GetButtonBasedOnType(2, cancelButtonType);
                }
            }

            internal IButtonControl FirstButton
            {
                get
                {
                    ButtonType button = ButtonType.Button;
                    switch (this._templateType)
                    {
                        case Wizard.WizardTemplateType.StartNavigationTemplate:
                            break;

                        case Wizard.WizardTemplateType.StepNavigationTemplate:
                            button = this._wizard.StepPreviousButtonType;
                            break;

                        default:
                            button = this._wizard.FinishPreviousButtonType;
                            break;
                    }
                    return this.GetButtonBasedOnType(0, button);
                }
            }

            internal IButtonControl SecondButton
            {
                get
                {
                    ButtonType button = ButtonType.Button;
                    switch (this._templateType)
                    {
                        case Wizard.WizardTemplateType.StartNavigationTemplate:
                            button = this._wizard.StartNextButtonType;
                            break;

                        case Wizard.WizardTemplateType.StepNavigationTemplate:
                            button = this._wizard.StepNextButtonType;
                            break;

                        default:
                            button = this._wizard.FinishCompleteButtonType;
                            break;
                    }
                    return this.GetButtonBasedOnType(1, button);
                }
            }
        }

        internal class StartNavigationTemplateContainer : Wizard.BaseNavigationTemplateContainer
        {
            private IButtonControl _nextButton;

            internal StartNavigationTemplateContainer(Wizard owner) : base(owner)
            {
            }

            internal override IButtonControl NextButton
            {
                get
                {
                    if (this._nextButton == null)
                    {
                        this._nextButton = this.FindControl(Wizard.StartNextButtonID) as IButtonControl;
                    }
                    return this._nextButton;
                }
                set
                {
                    this._nextButton = value;
                }
            }
        }

        internal class StepNavigationTemplateContainer : Wizard.BaseNavigationTemplateContainer
        {
            internal StepNavigationTemplateContainer(Wizard owner) : base(owner)
            {
            }
        }

        [SupportsEventValidation]
        private class WizardChildTable : ChildTable
        {
            private Wizard _owner;

            internal WizardChildTable(Wizard owner)
            {
                this._owner = owner;
            }

            protected override bool OnBubbleEvent(object source, EventArgs args) => 
                this._owner.OnBubbleEvent(source, args);
        }

        internal class WizardControlCollection : ControlCollection
        {
            public WizardControlCollection(Wizard wizard) : base(wizard)
            {
                if (!wizard.DesignMode)
                {
                    base.SetCollectionReadOnly("Wizard_Cannot_Modify_ControlCollection");
                }
            }
        }

        internal class WizardControlCollectionModifier : IDisposable
        {
            private ControlCollection _controls;
            private string _originalError;
            private Wizard _wizard;

            public WizardControlCollectionModifier(Wizard wizard)
            {
                this._wizard = wizard;
                if (!this._wizard.DesignMode)
                {
                    this._controls = this._wizard.Controls;
                    this._originalError = this._controls.SetCollectionReadOnly(null);
                }
            }

            void IDisposable.Dispose()
            {
                if (!this._wizard.DesignMode)
                {
                    this._controls.SetCollectionReadOnly(this._originalError);
                }
            }
        }

        private enum WizardTemplateType
        {
            StartNavigationTemplate,
            StepNavigationTemplate,
            FinishNavigationTemplate
        }
    }
}

