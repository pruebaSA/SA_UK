namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;

    [ToolboxData("<{0}:CreateUserWizard runat=\"server\"> <WizardSteps> <asp:CreateUserWizardStep runat=\"server\"/> <asp:CompleteWizardStep runat=\"server\"/> </WizardSteps> </{0}:CreateUserWizard>"), Bindable(false), DefaultEvent("CreatedUser"), Designer("System.Web.UI.Design.WebControls.CreateUserWizardDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CreateUserWizard : Wizard
    {
        private string _answer;
        private const string _answerID = "Answer";
        private const string _answerRequiredID = "AnswerRequired";
        private TableRow _answerRow;
        private const ValidatorDisplay _compareFieldValidatorDisplay = ValidatorDisplay.Dynamic;
        private CompleteWizardStep _completeStep;
        private CompleteStepContainer _completeStepContainer;
        private const string _completeStepContainerID = "CompleteStepContainer";
        private TableItemStyle _completeSuccessTextStyle;
        private string _confirmPassword;
        private const string _confirmPasswordID = "ConfirmPassword";
        private const string _confirmPasswordRequiredID = "ConfirmPasswordRequired";
        private TableRow _confirmPasswordTableRow;
        private const string _continueButtonID = "ContinueButton";
        private Style _continueButtonStyle;
        private bool _convertingToTemplate;
        private Style _createUserButtonStyle;
        private const string _createUserNavigationTemplateName = "CreateUserNavigationTemplate";
        private CreateUserWizardStep _createUserStep;
        private CreateUserStepContainer _createUserStepContainer;
        private const string _createUserStepContainerID = "CreateUserStepContainer";
        private DefaultCreateUserNavigationTemplate _defaultCreateUserNavigationTemplate;
        private const string _editProfileLinkID = "EditProfileLink";
        private const string _emailID = "Email";
        private const string _emailRegExpID = "EmailRegExp";
        private TableRow _emailRegExpRow;
        private const string _emailRequiredID = "EmailRequired";
        private TableRow _emailRow;
        private const string _errorMessageID = "ErrorMessage";
        private TableItemStyle _errorMessageStyle;
        private bool _failure;
        private const string _helpLinkID = "HelpLink";
        private TableItemStyle _hyperLinkStyle;
        private TableItemStyle _instructionTextStyle;
        private TableItemStyle _labelStyle;
        private System.Web.UI.WebControls.MailDefinition _mailDefinition;
        private string _password;
        private const string _passwordCompareID = "PasswordCompare";
        private TableRow _passwordCompareRow;
        private TableItemStyle _passwordHintStyle;
        private TableRow _passwordHintTableRow;
        private const string _passwordID = "Password";
        private const string _passwordRegExpID = "PasswordRegExp";
        private TableRow _passwordRegExpRow;
        private const string _passwordReplacementKey = @"<%\s*Password\s*%>";
        private const string _passwordRequiredID = "PasswordRequired";
        private TableRow _passwordTableRow;
        private const string _questionID = "Question";
        private const string _questionRequiredID = "QuestionRequired";
        private TableRow _questionRow;
        private const ValidatorDisplay _regexpFieldValidatorDisplay = ValidatorDisplay.Dynamic;
        private const ValidatorDisplay _requiredFieldValidatorDisplay = ValidatorDisplay.Static;
        private const string _sideBarLabelID = "SideBarLabel";
        private Style _textBoxStyle;
        private TableItemStyle _titleTextStyle;
        private string _unknownErrorMessage;
        private const string _userNameID = "UserName";
        private const string _userNameReplacementKey = @"<%\s*UserName\s*%>";
        private const string _userNameRequiredID = "UserNameRequired";
        private string _validationGroup;
        private Style _validatorTextStyle;
        private const int _viewStateArrayLength = 13;
        public static readonly string ContinueButtonCommandName = "Continue";
        private static readonly object EventButtonContinueClick = new object();
        private static readonly object EventCancelClick = new object();
        private static readonly object EventCreatedUser = new object();
        private static readonly object EventCreateUserError = new object();
        private static readonly object EventCreatingUser = new object();
        private static readonly object EventSendingMail = new object();
        private static readonly object EventSendMailError = new object();

        [WebSysDescription("CreateUserWizard_ContinueButtonClick"), WebCategory("Action")]
        public event EventHandler ContinueButtonClick
        {
            add
            {
                base.Events.AddHandler(EventButtonContinueClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventButtonContinueClick, value);
            }
        }

        [WebSysDescription("CreateUserWizard_CreatedUser"), WebCategory("Action")]
        public event EventHandler CreatedUser
        {
            add
            {
                base.Events.AddHandler(EventCreatedUser, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventCreatedUser, value);
            }
        }

        [WebCategory("Action"), WebSysDescription("CreateUserWizard_CreateUserError")]
        public event CreateUserErrorEventHandler CreateUserError
        {
            add
            {
                base.Events.AddHandler(EventCreateUserError, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventCreateUserError, value);
            }
        }

        [WebCategory("Action"), WebSysDescription("CreateUserWizard_CreatingUser")]
        public event LoginCancelEventHandler CreatingUser
        {
            add
            {
                base.Events.AddHandler(EventCreatingUser, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventCreatingUser, value);
            }
        }

        [WebSysDescription("ChangePassword_SendingMail"), WebCategory("Action")]
        public event MailMessageEventHandler SendingMail
        {
            add
            {
                base.Events.AddHandler(EventSendingMail, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventSendingMail, value);
            }
        }

        [WebSysDescription("CreateUserWizard_SendMailError"), WebCategory("Action")]
        public event SendMailErrorEventHandler SendMailError
        {
            add
            {
                base.Events.AddHandler(EventSendMailError, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventSendMailError, value);
            }
        }

        public CreateUserWizard()
        {
            base._displaySideBarDefault = false;
            base._displaySideBar = base._displaySideBarDefault;
        }

        private void AnswerTextChanged(object source, EventArgs e)
        {
            this.Answer = ((ITextControl) source).Text;
        }

        private void ApplyCommonCreateUserValues()
        {
            if (!string.IsNullOrEmpty(this.UserNameInternal))
            {
                ITextControl userNameTextBox = (ITextControl) this._createUserStepContainer.UserNameTextBox;
                if (userNameTextBox != null)
                {
                    userNameTextBox.Text = this.UserNameInternal;
                }
            }
            if (!string.IsNullOrEmpty(this.EmailInternal))
            {
                ITextControl emailTextBox = (ITextControl) this._createUserStepContainer.EmailTextBox;
                if (emailTextBox != null)
                {
                    emailTextBox.Text = this.EmailInternal;
                }
            }
            if (!string.IsNullOrEmpty(this.QuestionInternal))
            {
                ITextControl questionTextBox = (ITextControl) this._createUserStepContainer.QuestionTextBox;
                if (questionTextBox != null)
                {
                    questionTextBox.Text = this.QuestionInternal;
                }
            }
            if (!string.IsNullOrEmpty(this.AnswerInternal))
            {
                ITextControl answerTextBox = (ITextControl) this._createUserStepContainer.AnswerTextBox;
                if (answerTextBox != null)
                {
                    answerTextBox.Text = this.AnswerInternal;
                }
            }
        }

        private void ApplyCompleteValues()
        {
            LoginUtil.ApplyStyleToLiteral(this._completeStepContainer.SuccessTextLabel, this.CompleteSuccessText, this._completeSuccessTextStyle, true);
            switch (this.ContinueButtonType)
            {
                case ButtonType.Button:
                    this._completeStepContainer.ContinueLinkButton.Visible = false;
                    this._completeStepContainer.ContinueImageButton.Visible = false;
                    this._completeStepContainer.ContinuePushButton.Text = this.ContinueButtonText;
                    this._completeStepContainer.ContinuePushButton.ValidationGroup = this.ValidationGroup;
                    this._completeStepContainer.ContinuePushButton.TabIndex = this.TabIndex;
                    this._completeStepContainer.ContinuePushButton.AccessKey = this.AccessKey;
                    break;

                case ButtonType.Image:
                    this._completeStepContainer.ContinueLinkButton.Visible = false;
                    this._completeStepContainer.ContinuePushButton.Visible = false;
                    this._completeStepContainer.ContinueImageButton.ImageUrl = this.ContinueButtonImageUrl;
                    this._completeStepContainer.ContinueImageButton.AlternateText = this.ContinueButtonText;
                    this._completeStepContainer.ContinueImageButton.ValidationGroup = this.ValidationGroup;
                    this._completeStepContainer.ContinueImageButton.TabIndex = this.TabIndex;
                    this._completeStepContainer.ContinueImageButton.AccessKey = this.AccessKey;
                    break;

                case ButtonType.Link:
                    this._completeStepContainer.ContinuePushButton.Visible = false;
                    this._completeStepContainer.ContinueImageButton.Visible = false;
                    this._completeStepContainer.ContinueLinkButton.Text = this.ContinueButtonText;
                    this._completeStepContainer.ContinueLinkButton.ValidationGroup = this.ValidationGroup;
                    this._completeStepContainer.ContinueLinkButton.TabIndex = this.TabIndex;
                    this._completeStepContainer.ContinueLinkButton.AccessKey = this.AccessKey;
                    break;
            }
            if (!base.NavigationButtonStyle.IsEmpty)
            {
                this._completeStepContainer.ContinuePushButton.ApplyStyle(base.NavigationButtonStyle);
                this._completeStepContainer.ContinueImageButton.ApplyStyle(base.NavigationButtonStyle);
                this._completeStepContainer.ContinueLinkButton.ApplyStyle(base.NavigationButtonStyle);
            }
            if (this._continueButtonStyle != null)
            {
                this._completeStepContainer.ContinuePushButton.ApplyStyle(this._continueButtonStyle);
                this._completeStepContainer.ContinueImageButton.ApplyStyle(this._continueButtonStyle);
                this._completeStepContainer.ContinueLinkButton.ApplyStyle(this._continueButtonStyle);
            }
            LoginUtil.ApplyStyleToLiteral(this._completeStepContainer.Title, this.CompleteStep.Title, this._titleTextStyle, true);
            string editProfileText = this.EditProfileText;
            bool flag = editProfileText.Length > 0;
            HyperLink editProfileLink = this._completeStepContainer.EditProfileLink;
            editProfileLink.Visible = flag;
            if (flag)
            {
                editProfileLink.Text = editProfileText;
                editProfileLink.NavigateUrl = this.EditProfileUrl;
                editProfileLink.TabIndex = this.TabIndex;
                if (this._hyperLinkStyle != null)
                {
                    Style style = new TableItemStyle();
                    style.CopyFrom(this._hyperLinkStyle);
                    style.Font.Reset();
                    LoginUtil.SetTableCellStyle(editProfileLink, style);
                    editProfileLink.Font.CopyFrom(this._hyperLinkStyle.Font);
                    editProfileLink.ForeColor = this._hyperLinkStyle.ForeColor;
                }
            }
            string editProfileIconUrl = this.EditProfileIconUrl;
            bool flag2 = editProfileIconUrl.Length > 0;
            Image editProfileIcon = this._completeStepContainer.EditProfileIcon;
            editProfileIcon.Visible = flag2;
            if (flag2)
            {
                editProfileIcon.ImageUrl = editProfileIconUrl;
                editProfileIcon.AlternateText = this.EditProfileText;
            }
            LoginUtil.SetTableCellVisible(editProfileLink, flag || flag2);
            Table layoutTable = ((CompleteStepContainer) this.CompleteStep.ContentTemplateContainer).LayoutTable;
            layoutTable.Height = this.Height;
            layoutTable.Width = this.Width;
        }

        internal override void ApplyControlProperties()
        {
            this.SetChildProperties();
            if (this.CreateUserStep.CustomNavigationTemplate == null)
            {
                this.SetDefaultCreateUserNavigationTemplateProperties();
            }
            base.ApplyControlProperties();
        }

        private void ApplyDefaultCreateUserValues()
        {
            this._createUserStepContainer.UserNameLabel.Text = this.UserNameLabelText;
            WebControl userNameTextBox = (WebControl) this._createUserStepContainer.UserNameTextBox;
            userNameTextBox.TabIndex = this.TabIndex;
            userNameTextBox.AccessKey = this.AccessKey;
            this._createUserStepContainer.PasswordLabel.Text = this.PasswordLabelText;
            WebControl passwordTextBox = (WebControl) this._createUserStepContainer.PasswordTextBox;
            passwordTextBox.TabIndex = this.TabIndex;
            this._createUserStepContainer.ConfirmPasswordLabel.Text = this.ConfirmPasswordLabelText;
            WebControl confirmPasswordTextBox = (WebControl) this._createUserStepContainer.ConfirmPasswordTextBox;
            confirmPasswordTextBox.TabIndex = this.TabIndex;
            if (this._textBoxStyle != null)
            {
                userNameTextBox.ApplyStyle(this._textBoxStyle);
                passwordTextBox.ApplyStyle(this._textBoxStyle);
                confirmPasswordTextBox.ApplyStyle(this._textBoxStyle);
            }
            LoginUtil.ApplyStyleToLiteral(this._createUserStepContainer.Title, this.CreateUserStep.Title, this.TitleTextStyle, true);
            LoginUtil.ApplyStyleToLiteral(this._createUserStepContainer.InstructionLabel, this.InstructionText, this.InstructionTextStyle, true);
            LoginUtil.ApplyStyleToLiteral(this._createUserStepContainer.UserNameLabel, this.UserNameLabelText, this.LabelStyle, false);
            LoginUtil.ApplyStyleToLiteral(this._createUserStepContainer.PasswordLabel, this.PasswordLabelText, this.LabelStyle, false);
            LoginUtil.ApplyStyleToLiteral(this._createUserStepContainer.ConfirmPasswordLabel, this.ConfirmPasswordLabelText, this.LabelStyle, false);
            if (!string.IsNullOrEmpty(this.PasswordHintText) && !this.AutoGeneratePassword)
            {
                LoginUtil.ApplyStyleToLiteral(this._createUserStepContainer.PasswordHintLabel, this.PasswordHintText, this.PasswordHintStyle, false);
            }
            else
            {
                this._passwordHintTableRow.Visible = false;
            }
            bool flag = true;
            WebControl emailTextBox = null;
            if (this.RequireEmail)
            {
                LoginUtil.ApplyStyleToLiteral(this._createUserStepContainer.EmailLabel, this.EmailLabelText, this.LabelStyle, false);
                emailTextBox = (WebControl) this._createUserStepContainer.EmailTextBox;
                ((ITextControl) emailTextBox).Text = this.Email;
                RequiredFieldValidator emailRequired = this._createUserStepContainer.EmailRequired;
                emailRequired.ToolTip = this.EmailRequiredErrorMessage;
                emailRequired.ErrorMessage = this.EmailRequiredErrorMessage;
                emailRequired.Enabled = flag;
                emailRequired.Visible = flag;
                if (this._validatorTextStyle != null)
                {
                    emailRequired.ApplyStyle(this._validatorTextStyle);
                }
                emailTextBox.TabIndex = this.TabIndex;
                if (this._textBoxStyle != null)
                {
                    emailTextBox.ApplyStyle(this._textBoxStyle);
                }
            }
            else
            {
                this._emailRow.Visible = false;
            }
            WebControl questionTextBox = null;
            WebControl answerTextBox = null;
            RequiredFieldValidator questionRequired = this._createUserStepContainer.QuestionRequired;
            RequiredFieldValidator answerRequired = this._createUserStepContainer.AnswerRequired;
            bool flag2 = flag && this.QuestionAndAnswerRequired;
            questionRequired.Enabled = flag2;
            questionRequired.Visible = flag2;
            answerRequired.Enabled = flag2;
            answerRequired.Visible = flag2;
            if (this.QuestionAndAnswerRequired)
            {
                LoginUtil.ApplyStyleToLiteral(this._createUserStepContainer.QuestionLabel, this.QuestionLabelText, this.LabelStyle, false);
                questionTextBox = (WebControl) this._createUserStepContainer.QuestionTextBox;
                ((ITextControl) questionTextBox).Text = this.Question;
                questionTextBox.TabIndex = this.TabIndex;
                LoginUtil.ApplyStyleToLiteral(this._createUserStepContainer.AnswerLabel, this.AnswerLabelText, this.LabelStyle, false);
                answerTextBox = (WebControl) this._createUserStepContainer.AnswerTextBox;
                ((ITextControl) answerTextBox).Text = this.Answer;
                answerTextBox.TabIndex = this.TabIndex;
                if (this._textBoxStyle != null)
                {
                    questionTextBox.ApplyStyle(this._textBoxStyle);
                    answerTextBox.ApplyStyle(this._textBoxStyle);
                }
                questionRequired.ToolTip = this.QuestionRequiredErrorMessage;
                questionRequired.ErrorMessage = this.QuestionRequiredErrorMessage;
                answerRequired.ToolTip = this.AnswerRequiredErrorMessage;
                answerRequired.ErrorMessage = this.AnswerRequiredErrorMessage;
                if (this._validatorTextStyle != null)
                {
                    questionRequired.ApplyStyle(this._validatorTextStyle);
                    answerRequired.ApplyStyle(this._validatorTextStyle);
                }
            }
            else
            {
                this._questionRow.Visible = false;
                this._answerRow.Visible = false;
            }
            if (this._defaultCreateUserNavigationTemplate != null)
            {
                ((Wizard.BaseNavigationTemplateContainer) this.CreateUserStep.CustomNavigationTemplateContainer).NextButton = this._defaultCreateUserNavigationTemplate.CreateUserButton;
                ((Wizard.BaseNavigationTemplateContainer) this.CreateUserStep.CustomNavigationTemplateContainer).CancelButton = this._defaultCreateUserNavigationTemplate.CancelButton;
            }
            RequiredFieldValidator passwordRequired = this._createUserStepContainer.PasswordRequired;
            RequiredFieldValidator confirmPasswordRequired = this._createUserStepContainer.ConfirmPasswordRequired;
            CompareValidator passwordCompareValidator = this._createUserStepContainer.PasswordCompareValidator;
            RegularExpressionValidator passwordRegExpValidator = this._createUserStepContainer.PasswordRegExpValidator;
            bool flag3 = flag && !this.AutoGeneratePassword;
            passwordRequired.Enabled = flag3;
            passwordRequired.Visible = flag3;
            confirmPasswordRequired.Enabled = flag3;
            confirmPasswordRequired.Visible = flag3;
            passwordCompareValidator.Enabled = flag3;
            passwordCompareValidator.Visible = flag3;
            bool flag4 = flag3 && (this.PasswordRegularExpression.Length > 0);
            passwordRegExpValidator.Enabled = flag4;
            passwordRegExpValidator.Visible = flag4;
            if (!flag)
            {
                this._passwordRegExpRow.Visible = false;
                this._passwordCompareRow.Visible = false;
                this._emailRegExpRow.Visible = false;
            }
            if (this.AutoGeneratePassword)
            {
                this._passwordTableRow.Visible = false;
                this._confirmPasswordTableRow.Visible = false;
                this._passwordRegExpRow.Visible = false;
                this._passwordCompareRow.Visible = false;
            }
            else
            {
                passwordRequired.ErrorMessage = this.PasswordRequiredErrorMessage;
                passwordRequired.ToolTip = this.PasswordRequiredErrorMessage;
                confirmPasswordRequired.ErrorMessage = this.ConfirmPasswordRequiredErrorMessage;
                confirmPasswordRequired.ToolTip = this.ConfirmPasswordRequiredErrorMessage;
                passwordCompareValidator.ErrorMessage = this.ConfirmPasswordCompareErrorMessage;
                if (this._validatorTextStyle != null)
                {
                    passwordRequired.ApplyStyle(this._validatorTextStyle);
                    confirmPasswordRequired.ApplyStyle(this._validatorTextStyle);
                    passwordCompareValidator.ApplyStyle(this._validatorTextStyle);
                }
                if (flag4)
                {
                    passwordRegExpValidator.ValidationExpression = this.PasswordRegularExpression;
                    passwordRegExpValidator.ErrorMessage = this.PasswordRegularExpressionErrorMessage;
                    if (this._validatorTextStyle != null)
                    {
                        passwordRegExpValidator.ApplyStyle(this._validatorTextStyle);
                    }
                }
                else
                {
                    this._passwordRegExpRow.Visible = false;
                }
            }
            RequiredFieldValidator userNameRequired = this._createUserStepContainer.UserNameRequired;
            userNameRequired.ErrorMessage = this.UserNameRequiredErrorMessage;
            userNameRequired.ToolTip = this.UserNameRequiredErrorMessage;
            userNameRequired.Enabled = flag;
            userNameRequired.Visible = flag;
            if (this._validatorTextStyle != null)
            {
                userNameRequired.ApplyStyle(this._validatorTextStyle);
            }
            bool flag5 = (flag && (this.EmailRegularExpression.Length > 0)) && this.RequireEmail;
            RegularExpressionValidator emailRegExpValidator = this._createUserStepContainer.EmailRegExpValidator;
            emailRegExpValidator.Enabled = flag5;
            emailRegExpValidator.Visible = flag5;
            if ((this.EmailRegularExpression.Length > 0) && this.RequireEmail)
            {
                emailRegExpValidator.ValidationExpression = this.EmailRegularExpression;
                emailRegExpValidator.ErrorMessage = this.EmailRegularExpressionErrorMessage;
                if (this._validatorTextStyle != null)
                {
                    emailRegExpValidator.ApplyStyle(this._validatorTextStyle);
                }
            }
            else
            {
                this._emailRegExpRow.Visible = false;
            }
            string helpPageText = this.HelpPageText;
            bool flag6 = helpPageText.Length > 0;
            HyperLink helpPageLink = this._createUserStepContainer.HelpPageLink;
            Image helpPageIcon = this._createUserStepContainer.HelpPageIcon;
            helpPageLink.Visible = flag6;
            if (flag6)
            {
                helpPageLink.Text = helpPageText;
                helpPageLink.NavigateUrl = this.HelpPageUrl;
                helpPageLink.TabIndex = this.TabIndex;
            }
            string helpPageIconUrl = this.HelpPageIconUrl;
            bool flag7 = helpPageIconUrl.Length > 0;
            helpPageIcon.Visible = flag7;
            if (flag7)
            {
                helpPageIcon.ImageUrl = helpPageIconUrl;
                helpPageIcon.AlternateText = helpPageText;
            }
            LoginUtil.SetTableCellVisible(helpPageLink, flag6 || flag7);
            if ((this._hyperLinkStyle != null) && (flag6 || flag7))
            {
                TableItemStyle style = new TableItemStyle();
                style.CopyFrom(this._hyperLinkStyle);
                style.Font.Reset();
                LoginUtil.SetTableCellStyle(helpPageLink, style);
                helpPageLink.Font.CopyFrom(this._hyperLinkStyle.Font);
                helpPageLink.ForeColor = this._hyperLinkStyle.ForeColor;
            }
            Control errorMessageLabel = this._createUserStepContainer.ErrorMessageLabel;
            if (errorMessageLabel != null)
            {
                if (this._failure && !string.IsNullOrEmpty(this._unknownErrorMessage))
                {
                    ((ITextControl) errorMessageLabel).Text = this._unknownErrorMessage;
                    LoginUtil.SetTableCellStyle(errorMessageLabel, this.ErrorMessageStyle);
                    LoginUtil.SetTableCellVisible(errorMessageLabel, true);
                }
                else
                {
                    LoginUtil.SetTableCellVisible(errorMessageLabel, false);
                }
            }
        }

        private bool AttemptCreateUser()
        {
            if ((this.Page == null) || this.Page.IsValid)
            {
                MembershipCreateStatus status;
                LoginCancelEventArgs e = new LoginCancelEventArgs();
                this.OnCreatingUser(e);
                if (e.Cancel)
                {
                    return false;
                }
                System.Web.Security.MembershipProvider provider = LoginUtil.GetProvider(this.MembershipProvider);
                if (this.AutoGeneratePassword)
                {
                    int length = Math.Max(10, Membership.MinRequiredPasswordLength);
                    this._password = Membership.GeneratePassword(length, Membership.MinRequiredNonAlphanumericCharacters);
                }
                provider.CreateUser(this.UserNameInternal, this.PasswordInternal, this.EmailInternal, this.QuestionInternal, this.AnswerInternal, !this.DisableCreatedUser, null, out status);
                if (status == MembershipCreateStatus.Success)
                {
                    this.OnCreatedUser(EventArgs.Empty);
                    if ((this._mailDefinition != null) && !string.IsNullOrEmpty(this.EmailInternal))
                    {
                        LoginUtil.SendPasswordMail(this.EmailInternal, this.UserNameInternal, this.PasswordInternal, this.MailDefinition, null, null, new LoginUtil.OnSendingMailDelegate(this.OnSendingMail), new LoginUtil.OnSendMailErrorDelegate(this.OnSendMailError), this);
                    }
                    this.CreateUserStep.AllowReturnInternal = false;
                    if (this.LoginCreatedUser)
                    {
                        this.AttemptLogin();
                    }
                    return true;
                }
                this.OnCreateUserError(new CreateUserErrorEventArgs(status));
                switch (status)
                {
                    case MembershipCreateStatus.InvalidPassword:
                    {
                        string invalidPasswordErrorMessage = this.InvalidPasswordErrorMessage;
                        if (!string.IsNullOrEmpty(invalidPasswordErrorMessage))
                        {
                            invalidPasswordErrorMessage = string.Format(CultureInfo.InvariantCulture, invalidPasswordErrorMessage, new object[] { provider.MinRequiredPasswordLength, provider.MinRequiredNonAlphanumericCharacters });
                        }
                        this._unknownErrorMessage = invalidPasswordErrorMessage;
                        break;
                    }
                    case MembershipCreateStatus.InvalidQuestion:
                        this._unknownErrorMessage = this.InvalidQuestionErrorMessage;
                        break;

                    case MembershipCreateStatus.InvalidAnswer:
                        this._unknownErrorMessage = this.InvalidAnswerErrorMessage;
                        break;

                    case MembershipCreateStatus.InvalidEmail:
                        this._unknownErrorMessage = this.InvalidEmailErrorMessage;
                        break;

                    case MembershipCreateStatus.DuplicateUserName:
                        this._unknownErrorMessage = this.DuplicateUserNameErrorMessage;
                        break;

                    case MembershipCreateStatus.DuplicateEmail:
                        this._unknownErrorMessage = this.DuplicateEmailErrorMessage;
                        break;

                    default:
                        this._unknownErrorMessage = this.UnknownErrorMessage;
                        break;
                }
            }
            return false;
        }

        private void AttemptLogin()
        {
            if (LoginUtil.GetProvider(this.MembershipProvider).ValidateUser(this.UserName, this.Password))
            {
                FormsAuthentication.SetAuthCookie(this.UserNameInternal, false);
            }
        }

        private void ConfirmPasswordTextChanged(object source, EventArgs e)
        {
            if (!this.AutoGeneratePassword)
            {
                this._confirmPassword = ((ITextControl) source).Text;
            }
        }

        protected internal override void CreateChildControls()
        {
            this._createUserStep = null;
            this._completeStep = null;
            base.CreateChildControls();
            this.UpdateValidators();
        }

        protected override void CreateControlHierarchy()
        {
            this.EnsureCreateUserSteps();
            base.CreateControlHierarchy();
            IEditableTextControl userNameTextBox = this._createUserStepContainer.UserNameTextBox as IEditableTextControl;
            if (userNameTextBox != null)
            {
                userNameTextBox.TextChanged += new EventHandler(this.UserNameTextChanged);
            }
            IEditableTextControl emailTextBox = this._createUserStepContainer.EmailTextBox as IEditableTextControl;
            if (emailTextBox != null)
            {
                emailTextBox.TextChanged += new EventHandler(this.EmailTextChanged);
            }
            IEditableTextControl questionTextBox = this._createUserStepContainer.QuestionTextBox as IEditableTextControl;
            if (questionTextBox != null)
            {
                questionTextBox.TextChanged += new EventHandler(this.QuestionTextChanged);
            }
            IEditableTextControl answerTextBox = this._createUserStepContainer.AnswerTextBox as IEditableTextControl;
            if (answerTextBox != null)
            {
                answerTextBox.TextChanged += new EventHandler(this.AnswerTextChanged);
            }
            IEditableTextControl passwordTextBox = this._createUserStepContainer.PasswordTextBox as IEditableTextControl;
            if (passwordTextBox != null)
            {
                passwordTextBox.TextChanged += new EventHandler(this.PasswordTextChanged);
            }
            passwordTextBox = this._createUserStepContainer.ConfirmPasswordTextBox as IEditableTextControl;
            if (passwordTextBox != null)
            {
                passwordTextBox.TextChanged += new EventHandler(this.ConfirmPasswordTextChanged);
            }
            this.ApplyCommonCreateUserValues();
        }

        internal override void CreateCustomNavigationTemplates()
        {
            for (int i = 0; i < this.WizardSteps.Count; i++)
            {
                TemplatedWizardStep step = this.WizardSteps[i] as TemplatedWizardStep;
                if (step != null)
                {
                    string customContainerID = base.GetCustomContainerID(i);
                    Wizard.BaseNavigationTemplateContainer container = base.CreateBaseNavigationTemplateContainer(customContainerID);
                    if (step.CustomNavigationTemplate != null)
                    {
                        step.CustomNavigationTemplate.InstantiateIn(container);
                        step.CustomNavigationTemplateContainer = container;
                        container.SetEnableTheming();
                    }
                    else if (step == this.CreateUserStep)
                    {
                        ITemplate template = new DefaultCreateUserNavigationTemplate(this);
                        template.InstantiateIn(container);
                        step.CustomNavigationTemplateContainer = container;
                        container.RegisterButtonCommandEvents();
                    }
                    base.CustomNavigationContainers[step] = container;
                }
            }
        }

        internal override ITemplate CreateDefaultDataListItemTemplate() => 
            new DataListItemTemplate();

        internal override ITemplate CreateDefaultSideBarTemplate() => 
            new DefaultSideBarTemplate();

        private static LabelLiteral CreateLabelLiteral(Control control)
        {
            LabelLiteral literal = new LabelLiteral(control);
            literal.PreventAutoID();
            return literal;
        }

        private static Literal CreateLiteral()
        {
            Literal literal = new Literal();
            literal.PreventAutoID();
            return literal;
        }

        private static RequiredFieldValidator CreateRequiredFieldValidator(string id, string validationGroup, TextBox textBox, bool enableValidation) => 
            new RequiredFieldValidator { 
                ID = id,
                ControlToValidate = textBox.ID,
                ValidationGroup = validationGroup,
                Display = ValidatorDisplay.Static,
                Text = System.Web.SR.GetString("LoginControls_DefaultRequiredFieldValidatorText"),
                Enabled = enableValidation,
                Visible = enableValidation
            };

        private static Table CreateTable()
        {
            Table table = new Table {
                Width = Unit.Percentage(100.0),
                Height = Unit.Percentage(100.0)
            };
            table.PreventAutoID();
            return table;
        }

        private static TableCell CreateTableCell()
        {
            TableCell cell = new TableCell();
            cell.PreventAutoID();
            return cell;
        }

        private static TableRow CreateTableRow()
        {
            TableRow row = new LoginUtil.DisappearingTableRow();
            row.PreventAutoID();
            return row;
        }

        internal override void DataListItemDataBound(object sender, DataListItemEventArgs e)
        {
            DataListItem item = e.Item;
            if (((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem)) || ((item.ItemType == ListItemType.SelectedItem) || (item.ItemType == ListItemType.EditItem)))
            {
                if (item.FindControl(Wizard.SideBarButtonID) is IButtonControl)
                {
                    base.DataListItemDataBound(sender, e);
                }
                else
                {
                    Label label = item.FindControl("SideBarLabel") as Label;
                    if (label == null)
                    {
                        if (!base.DesignMode)
                        {
                            throw new InvalidOperationException(System.Web.SR.GetString("CreateUserWizard_SideBar_Label_Not_Found", new object[] { Wizard.DataListID, "SideBarLabel" }));
                        }
                    }
                    else
                    {
                        label.MergeStyle(base.SideBarButtonStyle);
                        WizardStepBase dataItem = item.DataItem as WizardStepBase;
                        if (dataItem != null)
                        {
                            base.RegisterSideBarDataListForRender();
                            if (dataItem.Title.Length > 0)
                            {
                                label.Text = dataItem.Title;
                            }
                            else
                            {
                                label.Text = dataItem.ID;
                            }
                        }
                    }
                }
            }
        }

        private void EmailTextChanged(object source, EventArgs e)
        {
            this.Email = ((ITextControl) source).Text;
        }

        private void EnsureCreateUserSteps()
        {
            bool flag = false;
            bool flag2 = false;
            foreach (WizardStepBase base2 in this.WizardSteps)
            {
                if (base2 is CreateUserWizardStep)
                {
                    if (flag)
                    {
                        throw new HttpException(System.Web.SR.GetString("CreateUserWizard_DuplicateCreateUserWizardStep"));
                    }
                    flag = true;
                    this._createUserStep = (CreateUserWizardStep) base2;
                }
                else if (base2 is CompleteWizardStep)
                {
                    if (flag2)
                    {
                        throw new HttpException(System.Web.SR.GetString("CreateUserWizard_DuplicateCompleteWizardStep"));
                    }
                    flag2 = true;
                    this._completeStep = (CompleteWizardStep) base2;
                }
            }
            if (!flag)
            {
                this._createUserStep = new CreateUserWizardStep();
                this._createUserStep.ApplyStyleSheetSkin(this.Page);
                this.WizardSteps.AddAt(0, this._createUserStep);
                this._createUserStep.Active = true;
            }
            if (!flag2)
            {
                this._completeStep = new CompleteWizardStep();
                this._completeStep.ApplyStyleSheetSkin(this.Page);
                this.WizardSteps.Add(this._completeStep);
            }
            if (this.ActiveStepIndex == -1)
            {
                this.ActiveStepIndex = 0;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        protected override IDictionary GetDesignModeState()
        {
            IDictionary designModeState = base.GetDesignModeState();
            WizardStepBase activeStep = base.ActiveStep;
            if ((activeStep != null) && (activeStep == this.CreateUserStep))
            {
                designModeState["CustomNavigationControls"] = ((Wizard.BaseNavigationTemplateContainer) base.CustomNavigationContainers[base.ActiveStep]).Controls;
            }
            Control errorMessageLabel = this._createUserStepContainer.ErrorMessageLabel;
            if (errorMessageLabel != null)
            {
                LoginUtil.SetTableCellVisible(errorMessageLabel, true);
            }
            return designModeState;
        }

        internal override void InstantiateStepContentTemplates()
        {
            foreach (WizardStepBase base2 in this.WizardSteps)
            {
                if (base2 == this.CreateUserStep)
                {
                    base2.Controls.Clear();
                    this._createUserStepContainer = new CreateUserStepContainer(this);
                    this._createUserStepContainer.ID = "CreateUserStepContainer";
                    ITemplate contentTemplate = ((CreateUserWizardStep) base2).ContentTemplate;
                    if (contentTemplate == null)
                    {
                        contentTemplate = new DefaultCreateUserContentTemplate(this);
                    }
                    else
                    {
                        this._createUserStepContainer.SetEnableTheming();
                    }
                    contentTemplate.InstantiateIn(this._createUserStepContainer.InnerCell);
                    ((CreateUserWizardStep) base2).ContentTemplateContainer = this._createUserStepContainer;
                    base2.Controls.Add(this._createUserStepContainer);
                }
                else if (base2 == this.CompleteStep)
                {
                    base2.Controls.Clear();
                    this._completeStepContainer = new CompleteStepContainer(this);
                    this._completeStepContainer.ID = "CompleteStepContainer";
                    ITemplate template2 = ((CompleteWizardStep) base2).ContentTemplate;
                    if (template2 == null)
                    {
                        template2 = new DefaultCompleteStepContentTemplate(this);
                    }
                    else
                    {
                        this._completeStepContainer.SetEnableTheming();
                    }
                    template2.InstantiateIn(this._completeStepContainer.InnerCell);
                    ((CompleteWizardStep) base2).ContentTemplateContainer = this._completeStepContainer;
                    base2.Controls.Add(this._completeStepContainer);
                }
                else if (base2 is TemplatedWizardStep)
                {
                    base.InstantiateStepContentTemplate((TemplatedWizardStep) base2);
                }
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
                if (objArray.Length != 13)
                {
                    throw new ArgumentException(System.Web.SR.GetString("ViewState_InvalidViewState"));
                }
                base.LoadViewState(objArray[0]);
                if (objArray[1] != null)
                {
                    ((IStateManager) this.CreateUserButtonStyle).LoadViewState(objArray[1]);
                }
                if (objArray[2] != null)
                {
                    ((IStateManager) this.LabelStyle).LoadViewState(objArray[2]);
                }
                if (objArray[3] != null)
                {
                    ((IStateManager) this.TextBoxStyle).LoadViewState(objArray[3]);
                }
                if (objArray[4] != null)
                {
                    ((IStateManager) this.HyperLinkStyle).LoadViewState(objArray[4]);
                }
                if (objArray[5] != null)
                {
                    ((IStateManager) this.InstructionTextStyle).LoadViewState(objArray[5]);
                }
                if (objArray[6] != null)
                {
                    ((IStateManager) this.TitleTextStyle).LoadViewState(objArray[6]);
                }
                if (objArray[7] != null)
                {
                    ((IStateManager) this.ErrorMessageStyle).LoadViewState(objArray[7]);
                }
                if (objArray[8] != null)
                {
                    ((IStateManager) this.PasswordHintStyle).LoadViewState(objArray[8]);
                }
                if (objArray[9] != null)
                {
                    ((IStateManager) this.MailDefinition).LoadViewState(objArray[9]);
                }
                if (objArray[10] != null)
                {
                    ((IStateManager) this.ContinueButtonStyle).LoadViewState(objArray[10]);
                }
                if (objArray[11] != null)
                {
                    ((IStateManager) this.CompleteSuccessTextStyle).LoadViewState(objArray[11]);
                }
                if (objArray[12] != null)
                {
                    ((IStateManager) this.ValidatorTextStyle).LoadViewState(objArray[12]);
                }
            }
            this.UpdateValidators();
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            if (e is CommandEventArgs)
            {
                CommandEventArgs args = (CommandEventArgs) e;
                if (args.CommandName.Equals(ContinueButtonCommandName, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.OnContinueButtonClick(EventArgs.Empty);
                    return true;
                }
            }
            return base.OnBubbleEvent(source, e);
        }

        protected virtual void OnContinueButtonClick(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventButtonContinueClick];
            if (handler != null)
            {
                handler(this, e);
            }
            string continueDestinationPageUrl = this.ContinueDestinationPageUrl;
            if (!string.IsNullOrEmpty(continueDestinationPageUrl))
            {
                this.Page.Response.Redirect(base.ResolveClientUrl(continueDestinationPageUrl), false);
            }
        }

        protected virtual void OnCreatedUser(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventCreatedUser];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnCreateUserError(CreateUserErrorEventArgs e)
        {
            CreateUserErrorEventHandler handler = (CreateUserErrorEventHandler) base.Events[EventCreateUserError];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnCreatingUser(LoginCancelEventArgs e)
        {
            LoginCancelEventHandler handler = (LoginCancelEventHandler) base.Events[EventCreatingUser];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnNextButtonClick(WizardNavigationEventArgs e)
        {
            if (this.WizardSteps[e.CurrentStepIndex] == this._createUserStep)
            {
                e.Cancel = (this.Page != null) && !this.Page.IsValid;
                if (!e.Cancel)
                {
                    this._failure = !this.AttemptCreateUser();
                    if (this._failure)
                    {
                        e.Cancel = true;
                        ITextControl errorMessageLabel = (ITextControl) this._createUserStepContainer.ErrorMessageLabel;
                        if ((errorMessageLabel != null) && !string.IsNullOrEmpty(this._unknownErrorMessage))
                        {
                            errorMessageLabel.Text = this._unknownErrorMessage;
                            if (errorMessageLabel is Control)
                            {
                                ((Control) errorMessageLabel).Visible = true;
                            }
                        }
                    }
                }
            }
            base.OnNextButtonClick(e);
        }

        protected internal override void OnPreRender(EventArgs e)
        {
            this.EnsureCreateUserSteps();
            base.OnPreRender(e);
            string membershipProvider = this.MembershipProvider;
            if (!string.IsNullOrEmpty(membershipProvider) && (Membership.Providers[membershipProvider] == null))
            {
                throw new HttpException(System.Web.SR.GetString("WebControl_CantFindProvider"));
            }
        }

        protected virtual void OnSendingMail(MailMessageEventArgs e)
        {
            MailMessageEventHandler handler = (MailMessageEventHandler) base.Events[EventSendingMail];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSendMailError(SendMailErrorEventArgs e)
        {
            SendMailErrorEventHandler handler = (SendMailErrorEventHandler) base.Events[EventSendMailError];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void PasswordTextChanged(object source, EventArgs e)
        {
            if (!this.AutoGeneratePassword)
            {
                this._password = ((ITextControl) source).Text;
            }
        }

        private void QuestionTextChanged(object source, EventArgs e)
        {
            this.Question = ((ITextControl) source).Text;
        }

        protected override object SaveViewState()
        {
            object[] objArray = new object[] { base.SaveViewState(), (this._createUserButtonStyle != null) ? ((IStateManager) this._createUserButtonStyle).SaveViewState() : null, (this._labelStyle != null) ? ((IStateManager) this._labelStyle).SaveViewState() : null, (this._textBoxStyle != null) ? ((IStateManager) this._textBoxStyle).SaveViewState() : null, (this._hyperLinkStyle != null) ? ((IStateManager) this._hyperLinkStyle).SaveViewState() : null, (this._instructionTextStyle != null) ? ((IStateManager) this._instructionTextStyle).SaveViewState() : null, (this._titleTextStyle != null) ? ((IStateManager) this._titleTextStyle).SaveViewState() : null, (this._errorMessageStyle != null) ? ((IStateManager) this._errorMessageStyle).SaveViewState() : null, (this._passwordHintStyle != null) ? ((IStateManager) this._passwordHintStyle).SaveViewState() : null, (this._mailDefinition != null) ? ((IStateManager) this._mailDefinition).SaveViewState() : null, (this._continueButtonStyle != null) ? ((IStateManager) this._continueButtonStyle).SaveViewState() : null, (this._completeSuccessTextStyle != null) ? ((IStateManager) this._completeSuccessTextStyle).SaveViewState() : null, (this._validatorTextStyle != null) ? ((IStateManager) this._validatorTextStyle).SaveViewState() : null };
            for (int i = 0; i < 13; i++)
            {
                if (objArray[i] != null)
                {
                    return objArray;
                }
            }
            return null;
        }

        internal void SetChildProperties()
        {
            this.ApplyCommonCreateUserValues();
            if (this.DefaultCreateUserStep)
            {
                this.ApplyDefaultCreateUserValues();
            }
            if (this.DefaultCompleteStep)
            {
                this.ApplyCompleteValues();
            }
            Control errorMessageLabel = this._createUserStepContainer.ErrorMessageLabel;
            if (errorMessageLabel != null)
            {
                if (this._failure && !string.IsNullOrEmpty(this._unknownErrorMessage))
                {
                    ((ITextControl) errorMessageLabel).Text = this._unknownErrorMessage;
                    errorMessageLabel.Visible = true;
                }
                else
                {
                    errorMessageLabel.Visible = false;
                }
            }
        }

        private void SetDefaultCreateUserNavigationTemplateProperties()
        {
            WebControl createUserButton = (WebControl) this._defaultCreateUserNavigationTemplate.CreateUserButton;
            WebControl previousButton = (WebControl) this._defaultCreateUserNavigationTemplate.PreviousButton;
            WebControl cancelButton = (WebControl) this._defaultCreateUserNavigationTemplate.CancelButton;
            this._defaultCreateUserNavigationTemplate.ApplyLayoutStyleToInnerCells(base.NavigationStyle);
            this.WizardSteps.IndexOf(this.CreateUserStep);
            ((IButtonControl) createUserButton).CausesValidation = true;
            ((IButtonControl) createUserButton).Text = this.CreateUserButtonText;
            ((IButtonControl) createUserButton).ValidationGroup = this.ValidationGroup;
            ((IButtonControl) previousButton).CausesValidation = false;
            ((IButtonControl) previousButton).Text = this.StepPreviousButtonText;
            ((IButtonControl) cancelButton).Text = this.CancelButtonText;
            if (this._createUserButtonStyle != null)
            {
                createUserButton.ApplyStyle(this._createUserButtonStyle);
            }
            createUserButton.ControlStyle.MergeWith(base.NavigationButtonStyle);
            createUserButton.TabIndex = this.TabIndex;
            createUserButton.Visible = true;
            if (createUserButton is ImageButton)
            {
                ((ImageButton) createUserButton).ImageUrl = this.CreateUserButtonImageUrl;
                ((ImageButton) createUserButton).AlternateText = this.CreateUserButtonText;
            }
            previousButton.ApplyStyle(base.StepPreviousButtonStyle);
            previousButton.ControlStyle.MergeWith(base.NavigationButtonStyle);
            previousButton.TabIndex = this.TabIndex;
            int previousStepIndex = base.GetPreviousStepIndex(false);
            if ((previousStepIndex != -1) && this.WizardSteps[previousStepIndex].AllowReturn)
            {
                previousButton.Visible = true;
            }
            else
            {
                previousButton.Parent.Visible = false;
            }
            if (previousButton is ImageButton)
            {
                ((ImageButton) previousButton).AlternateText = this.StepPreviousButtonText;
                ((ImageButton) previousButton).ImageUrl = this.StepPreviousButtonImageUrl;
            }
            if (this.DisplayCancelButton)
            {
                cancelButton.ApplyStyle(base.CancelButtonStyle);
                cancelButton.ControlStyle.MergeWith(base.NavigationButtonStyle);
                cancelButton.TabIndex = this.TabIndex;
                cancelButton.Visible = true;
                if (cancelButton is ImageButton)
                {
                    ((ImageButton) cancelButton).ImageUrl = this.CancelButtonImageUrl;
                    ((ImageButton) cancelButton).AlternateText = this.CancelButtonText;
                }
            }
            else
            {
                cancelButton.Parent.Visible = false;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        protected override void SetDesignModeState(IDictionary data)
        {
            if (data != null)
            {
                object obj2 = data["ConvertToTemplate"];
                if (obj2 != null)
                {
                    this._convertingToTemplate = (bool) obj2;
                }
            }
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (this._createUserButtonStyle != null)
            {
                ((IStateManager) this._createUserButtonStyle).TrackViewState();
            }
            if (this._labelStyle != null)
            {
                ((IStateManager) this._labelStyle).TrackViewState();
            }
            if (this._textBoxStyle != null)
            {
                ((IStateManager) this._textBoxStyle).TrackViewState();
            }
            if (this._hyperLinkStyle != null)
            {
                ((IStateManager) this._hyperLinkStyle).TrackViewState();
            }
            if (this._instructionTextStyle != null)
            {
                ((IStateManager) this._instructionTextStyle).TrackViewState();
            }
            if (this._titleTextStyle != null)
            {
                ((IStateManager) this._titleTextStyle).TrackViewState();
            }
            if (this._errorMessageStyle != null)
            {
                ((IStateManager) this._errorMessageStyle).TrackViewState();
            }
            if (this._passwordHintStyle != null)
            {
                ((IStateManager) this._passwordHintStyle).TrackViewState();
            }
            if (this._mailDefinition != null)
            {
                ((IStateManager) this._mailDefinition).TrackViewState();
            }
            if (this._continueButtonStyle != null)
            {
                ((IStateManager) this._continueButtonStyle).TrackViewState();
            }
            if (this._completeSuccessTextStyle != null)
            {
                ((IStateManager) this._completeSuccessTextStyle).TrackViewState();
            }
            if (this._validatorTextStyle != null)
            {
                ((IStateManager) this._validatorTextStyle).TrackViewState();
            }
        }

        private void UpdateValidators()
        {
            if (!base.DesignMode && (this.DefaultCreateUserStep && (this._createUserStepContainer != null)))
            {
                if (this.AutoGeneratePassword)
                {
                    BaseValidator confirmPasswordRequired = this._createUserStepContainer.ConfirmPasswordRequired;
                    if (confirmPasswordRequired != null)
                    {
                        this.Page.Validators.Remove(confirmPasswordRequired);
                        confirmPasswordRequired.Enabled = false;
                    }
                    BaseValidator passwordRequired = this._createUserStepContainer.PasswordRequired;
                    if (passwordRequired != null)
                    {
                        this.Page.Validators.Remove(passwordRequired);
                        passwordRequired.Enabled = false;
                    }
                    BaseValidator passwordRegExpValidator = this._createUserStepContainer.PasswordRegExpValidator;
                    if (passwordRegExpValidator != null)
                    {
                        this.Page.Validators.Remove(passwordRegExpValidator);
                        passwordRegExpValidator.Enabled = false;
                    }
                }
                else if (this.PasswordRegularExpression.Length <= 0)
                {
                    BaseValidator validator = this._createUserStepContainer.PasswordRegExpValidator;
                    if (validator != null)
                    {
                        if (this.Page != null)
                        {
                            this.Page.Validators.Remove(validator);
                        }
                        validator.Enabled = false;
                    }
                }
                if (!this.RequireEmail)
                {
                    BaseValidator emailRequired = this._createUserStepContainer.EmailRequired;
                    if (emailRequired != null)
                    {
                        if (this.Page != null)
                        {
                            this.Page.Validators.Remove(emailRequired);
                        }
                        emailRequired.Enabled = false;
                    }
                    BaseValidator emailRegExpValidator = this._createUserStepContainer.EmailRegExpValidator;
                    if (emailRegExpValidator != null)
                    {
                        if (this.Page != null)
                        {
                            this.Page.Validators.Remove(emailRegExpValidator);
                        }
                        emailRegExpValidator.Enabled = false;
                    }
                }
                else if (this.EmailRegularExpression.Length <= 0)
                {
                    BaseValidator validator7 = this._createUserStepContainer.EmailRegExpValidator;
                    if (validator7 != null)
                    {
                        if (this.Page != null)
                        {
                            this.Page.Validators.Remove(validator7);
                        }
                        validator7.Enabled = false;
                    }
                }
                if (!this.QuestionAndAnswerRequired)
                {
                    BaseValidator questionRequired = this._createUserStepContainer.QuestionRequired;
                    if (questionRequired != null)
                    {
                        if (this.Page != null)
                        {
                            this.Page.Validators.Remove(questionRequired);
                        }
                        questionRequired.Enabled = false;
                    }
                    BaseValidator answerRequired = this._createUserStepContainer.AnswerRequired;
                    if (answerRequired != null)
                    {
                        if (this.Page != null)
                        {
                            this.Page.Validators.Remove(answerRequired);
                        }
                        answerRequired.Enabled = false;
                    }
                }
            }
        }

        private void UserNameTextChanged(object source, EventArgs e)
        {
            this.UserName = ((ITextControl) source).Text;
        }

        [DefaultValue(0)]
        public override int ActiveStepIndex
        {
            get => 
                base.ActiveStepIndex;
            set
            {
                base.ActiveStepIndex = value;
            }
        }

        [Themeable(false), DefaultValue(""), Localizable(true), WebSysDescription("CreateUserWizard_Answer"), WebCategory("Appearance")]
        public virtual string Answer
        {
            get
            {
                if (this._answer != null)
                {
                    return this._answer;
                }
                return string.Empty;
            }
            set
            {
                this._answer = value;
            }
        }

        private string AnswerInternal
        {
            get
            {
                string answer = this.Answer;
                if (string.IsNullOrEmpty(this.Answer) && (this._createUserStepContainer != null))
                {
                    ITextControl answerTextBox = (ITextControl) this._createUserStepContainer.AnswerTextBox;
                    if (answerTextBox != null)
                    {
                        answer = answerTextBox.Text;
                    }
                }
                if (string.IsNullOrEmpty(answer))
                {
                    answer = null;
                }
                return answer;
            }
        }

        [WebSysDefaultValue("CreateUserWizard_DefaultAnswerLabelText"), WebCategory("Appearance"), Localizable(true), WebSysDescription("CreateUserWizard_AnswerLabelText")]
        public virtual string AnswerLabelText
        {
            get
            {
                object obj2 = this.ViewState["AnswerLabelText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultAnswerLabelText");
            }
            set
            {
                this.ViewState["AnswerLabelText"] = value;
            }
        }

        [Localizable(true), WebCategory("Validation"), WebSysDefaultValue("CreateUserWizard_DefaultAnswerRequiredErrorMessage"), WebSysDescription("LoginControls_AnswerRequiredErrorMessage")]
        public virtual string AnswerRequiredErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["AnswerRequiredErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultAnswerRequiredErrorMessage");
            }
            set
            {
                this.ViewState["AnswerRequiredErrorMessage"] = value;
            }
        }

        [WebCategory("Behavior"), WebSysDescription("CreateUserWizard_AutoGeneratePassword"), DefaultValue(false), Themeable(false)]
        public virtual bool AutoGeneratePassword
        {
            get
            {
                object obj2 = this.ViewState["AutoGeneratePassword"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                if (this.AutoGeneratePassword != value)
                {
                    this.ViewState["AutoGeneratePassword"] = value;
                    base.RequiresControlsRecreation();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), WebSysDescription("CreateUserWizard_CompleteStep"), WebCategory("Appearance")]
        public CompleteWizardStep CompleteStep
        {
            get
            {
                this.EnsureChildControls();
                return this._completeStep;
            }
        }

        [Localizable(true), WebSysDescription("CreateUserWizard_CompleteSuccessText"), WebCategory("Appearance"), WebSysDefaultValue("CreateUserWizard_DefaultCompleteSuccessText")]
        public virtual string CompleteSuccessText
        {
            get
            {
                object obj2 = this.ViewState["CompleteSuccessText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultCompleteSuccessText");
            }
            set
            {
                this.ViewState["CompleteSuccessText"] = value;
            }
        }

        [WebCategory("Styles"), NotifyParentProperty(true), WebSysDescription("CreateUserWizard_CompleteSuccessTextStyle"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle CompleteSuccessTextStyle
        {
            get
            {
                if (this._completeSuccessTextStyle == null)
                {
                    this._completeSuccessTextStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._completeSuccessTextStyle).TrackViewState();
                    }
                }
                return this._completeSuccessTextStyle;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual string ConfirmPassword
        {
            get
            {
                if (this._confirmPassword != null)
                {
                    return this._confirmPassword;
                }
                return string.Empty;
            }
        }

        [WebCategory("Validation"), WebSysDescription("ChangePassword_ConfirmPasswordCompareErrorMessage"), Localizable(true), WebSysDefaultValue("CreateUserWizard_DefaultConfirmPasswordCompareErrorMessage")]
        public virtual string ConfirmPasswordCompareErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["ConfirmPasswordCompareErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultConfirmPasswordCompareErrorMessage");
            }
            set
            {
                this.ViewState["ConfirmPasswordCompareErrorMessage"] = value;
            }
        }

        [WebCategory("Appearance"), WebSysDescription("CreateUserWizard_ConfirmPasswordLabelText"), WebSysDefaultValue("CreateUserWizard_DefaultConfirmPasswordLabelText"), Localizable(true)]
        public virtual string ConfirmPasswordLabelText
        {
            get
            {
                object obj2 = this.ViewState["ConfirmPasswordLabelText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultConfirmPasswordLabelText");
            }
            set
            {
                this.ViewState["ConfirmPasswordLabelText"] = value;
            }
        }

        [Localizable(true), WebCategory("Validation"), WebSysDefaultValue("CreateUserWizard_DefaultConfirmPasswordRequiredErrorMessage"), WebSysDescription("LoginControls_ConfirmPasswordRequiredErrorMessage")]
        public virtual string ConfirmPasswordRequiredErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["ConfirmPasswordRequiredErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultConfirmPasswordRequiredErrorMessage");
            }
            set
            {
                this.ViewState["ConfirmPasswordRequiredErrorMessage"] = value;
            }
        }

        [WebSysDescription("ChangePassword_ContinueButtonImageUrl"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, WebCategory("Appearance"), DefaultValue("")]
        public virtual string ContinueButtonImageUrl
        {
            get
            {
                object obj2 = this.ViewState["ContinueButtonImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ContinueButtonImageUrl"] = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("CreateUserWizard_ContinueButtonStyle"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), WebCategory("Styles")]
        public Style ContinueButtonStyle
        {
            get
            {
                if (this._continueButtonStyle == null)
                {
                    this._continueButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._continueButtonStyle).TrackViewState();
                    }
                }
                return this._continueButtonStyle;
            }
        }

        [WebSysDefaultValue("CreateUserWizard_DefaultContinueButtonText"), WebCategory("Appearance"), WebSysDescription("CreateUserWizard_ContinueButtonText"), Localizable(true)]
        public virtual string ContinueButtonText
        {
            get
            {
                object obj2 = this.ViewState["ContinueButtonText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultContinueButtonText");
            }
            set
            {
                this.ViewState["ContinueButtonText"] = value;
            }
        }

        [WebCategory("Appearance"), WebSysDescription("CreateUserWizard_ContinueButtonType"), DefaultValue(0)]
        public virtual ButtonType ContinueButtonType
        {
            get
            {
                object obj2 = this.ViewState["ContinueButtonType"];
                if (obj2 != null)
                {
                    return (ButtonType) obj2;
                }
                return ButtonType.Button;
            }
            set
            {
                if ((value < ButtonType.Button) || (value > ButtonType.Link))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this.ContinueButtonType)
                {
                    this.ViewState["ContinueButtonType"] = value;
                }
            }
        }

        [WebCategory("Behavior"), DefaultValue(""), Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Themeable(false), UrlProperty, WebSysDescription("LoginControls_ContinueDestinationPageUrl")]
        public virtual string ContinueDestinationPageUrl
        {
            get
            {
                object obj2 = this.ViewState["ContinueDestinationPageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ContinueDestinationPageUrl"] = value;
            }
        }

        private bool ConvertingToTemplate =>
            (base.DesignMode && this._convertingToTemplate);

        [DefaultValue(""), UrlProperty, WebCategory("Appearance"), WebSysDescription("CreateUserWizard_CreateUserButtonImageUrl"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual string CreateUserButtonImageUrl
        {
            get
            {
                object obj2 = this.ViewState["CreateUserButtonImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["CreateUserButtonImageUrl"] = value;
            }
        }

        [WebSysDescription("CreateUserWizard_CreateUserButtonStyle"), WebCategory("Styles"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public Style CreateUserButtonStyle
        {
            get
            {
                if (this._createUserButtonStyle == null)
                {
                    this._createUserButtonStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._createUserButtonStyle).TrackViewState();
                    }
                }
                return this._createUserButtonStyle;
            }
        }

        [WebSysDefaultValue("CreateUserWizard_DefaultCreateUserButtonText"), Localizable(true), WebCategory("Appearance"), WebSysDescription("CreateUserWizard_CreateUserButtonText")]
        public virtual string CreateUserButtonText
        {
            get
            {
                object obj2 = this.ViewState["CreateUserButtonText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultCreateUserButtonText");
            }
            set
            {
                this.ViewState["CreateUserButtonText"] = value;
            }
        }

        [DefaultValue(0), WebCategory("Appearance"), WebSysDescription("CreateUserWizard_CreateUserButtonType")]
        public virtual ButtonType CreateUserButtonType
        {
            get
            {
                object obj2 = this.ViewState["CreateUserButtonType"];
                if (obj2 != null)
                {
                    return (ButtonType) obj2;
                }
                return ButtonType.Button;
            }
            set
            {
                if ((value < ButtonType.Button) || (value > ButtonType.Link))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this.CreateUserButtonType)
                {
                    this.ViewState["CreateUserButtonType"] = value;
                }
            }
        }

        [WebCategory("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebSysDescription("CreateUserWizard_CreateUserStep"), Browsable(false)]
        public CreateUserWizardStep CreateUserStep
        {
            get
            {
                this.EnsureChildControls();
                return this._createUserStep;
            }
        }

        private bool DefaultCompleteStep
        {
            get
            {
                CompleteWizardStep completeStep = this.CompleteStep;
                return ((completeStep != null) && (completeStep.ContentTemplate == null));
            }
        }

        private bool DefaultCreateUserStep
        {
            get
            {
                CreateUserWizardStep createUserStep = this.CreateUserStep;
                return ((createUserStep != null) && (createUserStep.ContentTemplate == null));
            }
        }

        [WebCategory("Behavior"), DefaultValue(false), Themeable(false), WebSysDescription("CreateUserWizard_DisableCreatedUser")]
        public virtual bool DisableCreatedUser
        {
            get
            {
                object obj2 = this.ViewState["DisableCreatedUser"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                this.ViewState["DisableCreatedUser"] = value;
            }
        }

        [DefaultValue(false)]
        public override bool DisplaySideBar
        {
            get => 
                base.DisplaySideBar;
            set
            {
                base.DisplaySideBar = value;
            }
        }

        [Localizable(true), WebSysDescription("CreateUserWizard_DuplicateEmailErrorMessage"), WebCategory("Appearance"), WebSysDefaultValue("CreateUserWizard_DefaultDuplicateEmailErrorMessage")]
        public virtual string DuplicateEmailErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["DuplicateEmailErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultDuplicateEmailErrorMessage");
            }
            set
            {
                this.ViewState["DuplicateEmailErrorMessage"] = value;
            }
        }

        [Localizable(true), WebSysDefaultValue("CreateUserWizard_DefaultDuplicateUserNameErrorMessage"), WebSysDescription("CreateUserWizard_DuplicateUserNameErrorMessage"), WebCategory("Appearance")]
        public virtual string DuplicateUserNameErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["DuplicateUserNameErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultDuplicateUserNameErrorMessage");
            }
            set
            {
                this.ViewState["DuplicateUserNameErrorMessage"] = value;
            }
        }

        [UrlProperty, WebCategory("Links"), DefaultValue(""), WebSysDescription("LoginControls_EditProfileIconUrl"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual string EditProfileIconUrl
        {
            get
            {
                object obj2 = this.ViewState["EditProfileIconUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["EditProfileIconUrl"] = value;
            }
        }

        [Localizable(true), WebSysDescription("CreateUserWizard_EditProfileText"), WebCategory("Links"), DefaultValue("")]
        public virtual string EditProfileText
        {
            get
            {
                object obj2 = this.ViewState["EditProfileText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["EditProfileText"] = value;
            }
        }

        [WebCategory("Links"), WebSysDescription("CreateUserWizard_EditProfileUrl"), Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), UrlProperty]
        public virtual string EditProfileUrl
        {
            get
            {
                object obj2 = this.ViewState["EditProfileUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["EditProfileUrl"] = value;
            }
        }

        [WebSysDescription("CreateUserWizard_Email"), WebCategory("Appearance"), DefaultValue("")]
        public virtual string Email
        {
            get
            {
                object obj2 = this.ViewState["Email"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["Email"] = value;
            }
        }

        private string EmailInternal
        {
            get
            {
                string email = this.Email;
                if (string.IsNullOrEmpty(email) && (this._createUserStepContainer != null))
                {
                    ITextControl emailTextBox = (ITextControl) this._createUserStepContainer.EmailTextBox;
                    if (emailTextBox != null)
                    {
                        return emailTextBox.Text;
                    }
                }
                return email;
            }
        }

        [WebCategory("Appearance"), WebSysDescription("CreateUserWizard_EmailLabelText"), WebSysDefaultValue("CreateUserWizard_DefaultEmailLabelText"), Localizable(true)]
        public virtual string EmailLabelText
        {
            get
            {
                object obj2 = this.ViewState["EmailLabelText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultEmailLabelText");
            }
            set
            {
                this.ViewState["EmailLabelText"] = value;
            }
        }

        [WebSysDescription("CreateUserWizard_EmailRegularExpression"), WebSysDefaultValue(""), WebCategory("Validation")]
        public virtual string EmailRegularExpression
        {
            get
            {
                object obj2 = this.ViewState["EmailRegularExpression"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["EmailRegularExpression"] = value;
            }
        }

        [WebCategory("Validation"), WebSysDescription("CreateUserWizard_EmailRegularExpressionErrorMessage"), WebSysDefaultValue("CreateUserWizard_DefaultEmailRegularExpressionErrorMessage")]
        public virtual string EmailRegularExpressionErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["EmailRegularExpressionErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultEmailRegularExpressionErrorMessage");
            }
            set
            {
                this.ViewState["EmailRegularExpressionErrorMessage"] = value;
            }
        }

        [WebSysDescription("CreateUserWizard_EmailRequiredErrorMessage"), Localizable(true), WebCategory("Validation"), WebSysDefaultValue("CreateUserWizard_DefaultEmailRequiredErrorMessage")]
        public virtual string EmailRequiredErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["EmailRequiredErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultEmailRequiredErrorMessage");
            }
            set
            {
                this.ViewState["EmailRequiredErrorMessage"] = value;
            }
        }

        [DefaultValue((string) null), WebSysDescription("CreateUserWizard_ErrorMessageStyle"), WebCategory("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle ErrorMessageStyle
        {
            get
            {
                if (this._errorMessageStyle == null)
                {
                    this._errorMessageStyle = new ErrorTableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._errorMessageStyle).TrackViewState();
                    }
                }
                return this._errorMessageStyle;
            }
        }

        [WebSysDescription("LoginControls_HelpPageIconUrl"), UrlProperty, WebCategory("Links"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual string HelpPageIconUrl
        {
            get
            {
                object obj2 = this.ViewState["HelpPageIconUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["HelpPageIconUrl"] = value;
            }
        }

        [Localizable(true), DefaultValue(""), WebSysDescription("ChangePassword_HelpPageText"), WebCategory("Links")]
        public virtual string HelpPageText
        {
            get
            {
                object obj2 = this.ViewState["HelpPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["HelpPageText"] = value;
            }
        }

        [UrlProperty, Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("Links"), DefaultValue(""), WebSysDescription("LoginControls_HelpPageUrl")]
        public virtual string HelpPageUrl
        {
            get
            {
                object obj2 = this.ViewState["HelpPageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["HelpPageUrl"] = value;
            }
        }

        [WebCategory("Styles"), NotifyParentProperty(true), WebSysDescription("WebControl_HyperLinkStyle"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
        public TableItemStyle HyperLinkStyle
        {
            get
            {
                if (this._hyperLinkStyle == null)
                {
                    this._hyperLinkStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._hyperLinkStyle).TrackViewState();
                    }
                }
                return this._hyperLinkStyle;
            }
        }

        [WebSysDescription("WebControl_InstructionText"), Localizable(true), WebCategory("Appearance"), DefaultValue("")]
        public virtual string InstructionText
        {
            get
            {
                object obj2 = this.ViewState["InstructionText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["InstructionText"] = value;
            }
        }

        [DefaultValue((string) null), WebCategory("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("WebControl_InstructionTextStyle")]
        public TableItemStyle InstructionTextStyle
        {
            get
            {
                if (this._instructionTextStyle == null)
                {
                    this._instructionTextStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._instructionTextStyle).TrackViewState();
                    }
                }
                return this._instructionTextStyle;
            }
        }

        [Localizable(true), WebSysDescription("CreateUserWizard_InvalidAnswerErrorMessage"), WebCategory("Appearance"), WebSysDefaultValue("CreateUserWizard_DefaultInvalidAnswerErrorMessage")]
        public virtual string InvalidAnswerErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["InvalidAnswerErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultInvalidAnswerErrorMessage");
            }
            set
            {
                this.ViewState["InvalidAnswerErrorMessage"] = value;
            }
        }

        [WebSysDescription("CreateUserWizard_InvalidEmailErrorMessage"), Localizable(true), WebCategory("Appearance"), WebSysDefaultValue("CreateUserWizard_DefaultInvalidEmailErrorMessage")]
        public virtual string InvalidEmailErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["InvalidEmailErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultInvalidEmailErrorMessage");
            }
            set
            {
                this.ViewState["InvalidEmailErrorMessage"] = value;
            }
        }

        [WebSysDefaultValue("CreateUserWizard_DefaultInvalidPasswordErrorMessage"), Localizable(true), WebSysDescription("CreateUserWizard_InvalidPasswordErrorMessage"), WebCategory("Appearance")]
        public virtual string InvalidPasswordErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["InvalidPasswordErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultInvalidPasswordErrorMessage");
            }
            set
            {
                this.ViewState["InvalidPasswordErrorMessage"] = value;
            }
        }

        [Localizable(true), WebSysDefaultValue("CreateUserWizard_DefaultInvalidQuestionErrorMessage"), WebSysDescription("CreateUserWizard_InvalidQuestionErrorMessage"), WebCategory("Appearance")]
        public virtual string InvalidQuestionErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["InvalidQuestionErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultInvalidQuestionErrorMessage");
            }
            set
            {
                this.ViewState["InvalidQuestionErrorMessage"] = value;
            }
        }

        [DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("LoginControls_LabelStyle"), WebCategory("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
        public TableItemStyle LabelStyle
        {
            get
            {
                if (this._labelStyle == null)
                {
                    this._labelStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._labelStyle).TrackViewState();
                    }
                }
                return this._labelStyle;
            }
        }

        [WebSysDescription("CreateUserWizard_LoginCreatedUser"), Themeable(false), WebCategory("Behavior"), DefaultValue(true)]
        public virtual bool LoginCreatedUser
        {
            get
            {
                object obj2 = this.ViewState["LoginCreatedUser"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["LoginCreatedUser"] = value;
            }
        }

        [Themeable(false), WebCategory("Behavior"), WebSysDescription("CreateUserWizard_MailDefinition"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public System.Web.UI.WebControls.MailDefinition MailDefinition
        {
            get
            {
                if (this._mailDefinition == null)
                {
                    this._mailDefinition = new System.Web.UI.WebControls.MailDefinition();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._mailDefinition).TrackViewState();
                    }
                }
                return this._mailDefinition;
            }
        }

        [WebSysDescription("MembershipProvider_Name"), WebCategory("Data"), DefaultValue(""), Themeable(false)]
        public virtual string MembershipProvider
        {
            get
            {
                object obj2 = this.ViewState["MembershipProvider"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (this.MembershipProvider != value)
                {
                    this.ViewState["MembershipProvider"] = value;
                    base.RequiresControlsRecreation();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string Password
        {
            get
            {
                if (this._password != null)
                {
                    return this._password;
                }
                return string.Empty;
            }
        }

        [NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), WebCategory("Styles"), DefaultValue((string) null), WebSysDescription("CreateUserWizard_PasswordHintStyle")]
        public TableItemStyle PasswordHintStyle
        {
            get
            {
                if (this._passwordHintStyle == null)
                {
                    this._passwordHintStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._passwordHintStyle).TrackViewState();
                    }
                }
                return this._passwordHintStyle;
            }
        }

        [WebSysDescription("ChangePassword_PasswordHintText"), Localizable(true), WebCategory("Appearance"), WebSysDefaultValue("")]
        public virtual string PasswordHintText
        {
            get
            {
                object obj2 = this.ViewState["PasswordHintText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["PasswordHintText"] = value;
            }
        }

        private string PasswordInternal
        {
            get
            {
                string password = this.Password;
                if ((string.IsNullOrEmpty(password) && !this.AutoGeneratePassword) && (this._createUserStepContainer != null))
                {
                    ITextControl passwordTextBox = (ITextControl) this._createUserStepContainer.PasswordTextBox;
                    if (passwordTextBox != null)
                    {
                        return passwordTextBox.Text;
                    }
                }
                return password;
            }
        }

        [WebSysDefaultValue("LoginControls_DefaultPasswordLabelText"), WebCategory("Appearance"), WebSysDescription("LoginControls_PasswordLabelText"), Localizable(true)]
        public virtual string PasswordLabelText
        {
            get
            {
                object obj2 = this.ViewState["PasswordLabelText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("LoginControls_DefaultPasswordLabelText");
            }
            set
            {
                this.ViewState["PasswordLabelText"] = value;
            }
        }

        [WebSysDefaultValue(""), WebCategory("Validation"), WebSysDescription("CreateUserWizard_PasswordRegularExpression")]
        public virtual string PasswordRegularExpression
        {
            get
            {
                object obj2 = this.ViewState["PasswordRegularExpression"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["PasswordRegularExpression"] = value;
            }
        }

        [WebSysDescription("CreateUserWizard_PasswordRegularExpressionErrorMessage"), WebCategory("Validation"), WebSysDefaultValue("Password_InvalidPasswordErrorMessage")]
        public virtual string PasswordRegularExpressionErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["PasswordRegularExpressionErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("Password_InvalidPasswordErrorMessage");
            }
            set
            {
                this.ViewState["PasswordRegularExpressionErrorMessage"] = value;
            }
        }

        [WebSysDefaultValue("CreateUserWizard_DefaultPasswordRequiredErrorMessage"), Localizable(true), WebCategory("Validation"), WebSysDescription("CreateUserWizard_PasswordRequiredErrorMessage")]
        public virtual string PasswordRequiredErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["PasswordRequiredErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultPasswordRequiredErrorMessage");
            }
            set
            {
                this.ViewState["PasswordRequiredErrorMessage"] = value;
            }
        }

        [WebCategory("Appearance"), Localizable(true), Themeable(false), DefaultValue(""), WebSysDescription("CreateUserWizard_Question")]
        public virtual string Question
        {
            get
            {
                object obj2 = this.ViewState["Question"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["Question"] = value;
            }
        }

        [WebSysDescription("CreateUserWizard_QuestionAndAnswerRequired"), WebCategory("Validation"), DefaultValue(true)]
        protected internal bool QuestionAndAnswerRequired
        {
            get
            {
                if (!base.DesignMode)
                {
                    return LoginUtil.GetProvider(this.MembershipProvider).RequiresQuestionAndAnswer;
                }
                if ((this.CreateUserStep != null) && (this.CreateUserStep.ContentTemplate != null))
                {
                    return false;
                }
                return true;
            }
        }

        private string QuestionInternal
        {
            get
            {
                string question = this.Question;
                if (string.IsNullOrEmpty(question) && (this._createUserStepContainer != null))
                {
                    ITextControl questionTextBox = (ITextControl) this._createUserStepContainer.QuestionTextBox;
                    if (questionTextBox != null)
                    {
                        question = questionTextBox.Text;
                    }
                }
                if (string.IsNullOrEmpty(question))
                {
                    question = null;
                }
                return question;
            }
        }

        [WebSysDescription("CreateUserWizard_QuestionLabelText"), WebCategory("Appearance"), WebSysDefaultValue("CreateUserWizard_DefaultQuestionLabelText"), Localizable(true)]
        public virtual string QuestionLabelText
        {
            get
            {
                object obj2 = this.ViewState["QuestionLabelText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultQuestionLabelText");
            }
            set
            {
                this.ViewState["QuestionLabelText"] = value;
            }
        }

        [WebSysDefaultValue("CreateUserWizard_DefaultQuestionRequiredErrorMessage"), WebSysDescription("CreateUserWizard_QuestionRequiredErrorMessage"), WebCategory("Validation"), Localizable(true)]
        public virtual string QuestionRequiredErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["QuestionRequiredErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultQuestionRequiredErrorMessage");
            }
            set
            {
                this.ViewState["QuestionRequiredErrorMessage"] = value;
            }
        }

        [WebSysDescription("CreateUserWizard_RequireEmail"), WebCategory("Behavior"), DefaultValue(true), Themeable(false)]
        public virtual bool RequireEmail
        {
            get
            {
                object obj2 = this.ViewState["RequireEmail"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                if (this.RequireEmail != value)
                {
                    this.ViewState["RequireEmail"] = value;
                }
            }
        }

        internal override bool ShowCustomNavigationTemplate =>
            (base.ShowCustomNavigationTemplate || (base.ActiveStep == this.CreateUserStep));

        [DefaultValue("")]
        public override string SkipLinkText
        {
            get
            {
                string skipLinkTextInternal = base.SkipLinkTextInternal;
                if (skipLinkTextInternal != null)
                {
                    return skipLinkTextInternal;
                }
                return string.Empty;
            }
            set
            {
                base.SkipLinkText = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), WebSysDescription("LoginControls_TextBoxStyle"), WebCategory("Styles"), DefaultValue((string) null), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public Style TextBoxStyle
        {
            get
            {
                if (this._textBoxStyle == null)
                {
                    this._textBoxStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._textBoxStyle).TrackViewState();
                    }
                }
                return this._textBoxStyle;
            }
        }

        [DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), WebCategory("Styles"), WebSysDescription("LoginControls_TitleTextStyle")]
        public TableItemStyle TitleTextStyle
        {
            get
            {
                if (this._titleTextStyle == null)
                {
                    this._titleTextStyle = new TableItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._titleTextStyle).TrackViewState();
                    }
                }
                return this._titleTextStyle;
            }
        }

        [WebSysDescription("CreateUserWizard_UnknownErrorMessage"), Localizable(true), WebCategory("Appearance"), WebSysDefaultValue("CreateUserWizard_DefaultUnknownErrorMessage")]
        public virtual string UnknownErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["UnknownErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultUnknownErrorMessage");
            }
            set
            {
                this.ViewState["UnknownErrorMessage"] = value;
            }
        }

        [DefaultValue(""), WebSysDescription("UserName_InitialValue"), WebCategory("Appearance")]
        public virtual string UserName
        {
            get
            {
                object obj2 = this.ViewState["UserName"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["UserName"] = value;
            }
        }

        private string UserNameInternal
        {
            get
            {
                string userName = this.UserName;
                if (string.IsNullOrEmpty(userName) && (this._createUserStepContainer != null))
                {
                    ITextControl userNameTextBox = (ITextControl) this._createUserStepContainer.UserNameTextBox;
                    if (userNameTextBox != null)
                    {
                        return userNameTextBox.Text;
                    }
                }
                return userName;
            }
        }

        [WebSysDefaultValue("CreateUserWizard_DefaultUserNameLabelText"), WebCategory("Appearance"), Localizable(true), WebSysDescription("LoginControls_UserNameLabelText")]
        public virtual string UserNameLabelText
        {
            get
            {
                object obj2 = this.ViewState["UserNameLabelText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultUserNameLabelText");
            }
            set
            {
                this.ViewState["UserNameLabelText"] = value;
            }
        }

        [WebSysDescription("ChangePassword_UserNameRequiredErrorMessage"), Localizable(true), WebCategory("Validation"), WebSysDefaultValue("CreateUserWizard_DefaultUserNameRequiredErrorMessage")]
        public virtual string UserNameRequiredErrorMessage
        {
            get
            {
                object obj2 = this.ViewState["UserNameRequiredErrorMessage"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return System.Web.SR.GetString("CreateUserWizard_DefaultUserNameRequiredErrorMessage");
            }
            set
            {
                this.ViewState["UserNameRequiredErrorMessage"] = value;
            }
        }

        internal string ValidationGroup
        {
            get
            {
                if (this._validationGroup == null)
                {
                    base.EnsureID();
                    this._validationGroup = this.ID;
                }
                return this._validationGroup;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), WebSysDescription("CreateUserWizard_ValidatorTextStyle"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), NotifyParentProperty(true), WebCategory("Styles")]
        public Style ValidatorTextStyle
        {
            get
            {
                if (this._validatorTextStyle == null)
                {
                    this._validatorTextStyle = new ErrorStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._validatorTextStyle).TrackViewState();
                    }
                }
                return this._validatorTextStyle;
            }
        }

        [Editor("System.Web.UI.Design.WebControls.CreateUserWizardStepCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public override WizardStepCollection WizardSteps =>
            base.WizardSteps;

        internal sealed class CompleteStepContainer : Wizard.BaseContentTemplateContainer
        {
            private ImageButton _continueImageButton;
            private LinkButton _continueLinkButton;
            private Button _continuePushButton;
            private CreateUserWizard _createUserWizard;
            private Image _editProfileIcon;
            private HyperLink _editProfileLink;
            private Table _layoutTable;
            private Literal _successTextLabel;
            private Literal _title;

            internal CompleteStepContainer(CreateUserWizard wizard) : base(wizard)
            {
                this._createUserWizard = wizard;
            }

            internal ImageButton ContinueImageButton
            {
                get => 
                    this._continueImageButton;
                set
                {
                    this._continueImageButton = value;
                }
            }

            internal LinkButton ContinueLinkButton
            {
                get => 
                    this._continueLinkButton;
                set
                {
                    this._continueLinkButton = value;
                }
            }

            internal Button ContinuePushButton
            {
                get => 
                    this._continuePushButton;
                set
                {
                    this._continuePushButton = value;
                }
            }

            internal Image EditProfileIcon
            {
                get => 
                    this._editProfileIcon;
                set
                {
                    this._editProfileIcon = value;
                }
            }

            internal HyperLink EditProfileLink
            {
                get => 
                    this._editProfileLink;
                set
                {
                    this._editProfileLink = value;
                }
            }

            internal Table LayoutTable
            {
                get => 
                    this._layoutTable;
                set
                {
                    this._layoutTable = value;
                }
            }

            internal Literal SuccessTextLabel
            {
                get => 
                    this._successTextLabel;
                set
                {
                    this._successTextLabel = value;
                }
            }

            internal Literal Title
            {
                get => 
                    this._title;
                set
                {
                    this._title = value;
                }
            }
        }

        internal sealed class CreateUserStepContainer : Wizard.BaseContentTemplateContainer
        {
            private LabelLiteral _answerLabel;
            private RequiredFieldValidator _answerRequired;
            private Control _answerTextBox;
            private LabelLiteral _confirmPasswordLabel;
            private RequiredFieldValidator _confirmPasswordRequired;
            private Control _confirmPasswordTextBox;
            private CreateUserWizard _createUserWizard;
            private LabelLiteral _emailLabel;
            private RegularExpressionValidator _emailRegExpValidator;
            private RequiredFieldValidator _emailRequired;
            private Control _emailTextBox;
            private Image _helpPageIcon;
            private HyperLink _helpPageLink;
            private Literal _instructionLabel;
            private CompareValidator _passwordCompareValidator;
            private Literal _passwordHintLabel;
            private LabelLiteral _passwordLabel;
            private RegularExpressionValidator _passwordRegExpValidator;
            private RequiredFieldValidator _passwordRequired;
            private Control _passwordTextBox;
            private LabelLiteral _questionLabel;
            private RequiredFieldValidator _questionRequired;
            private Control _questionTextBox;
            private Literal _title;
            private Control _unknownErrorMessageLabel;
            private LabelLiteral _userNameLabel;
            private RequiredFieldValidator _userNameRequired;
            private Control _userNameTextBox;

            internal CreateUserStepContainer(CreateUserWizard wizard) : base(wizard)
            {
                this._createUserWizard = wizard;
            }

            internal LabelLiteral AnswerLabel
            {
                get => 
                    this._answerLabel;
                set
                {
                    this._answerLabel = value;
                }
            }

            internal RequiredFieldValidator AnswerRequired
            {
                get => 
                    this._answerRequired;
                set
                {
                    this._answerRequired = value;
                }
            }

            internal Control AnswerTextBox
            {
                get
                {
                    if (this._answerTextBox != null)
                    {
                        return this._answerTextBox;
                    }
                    Control control = this.FindControl("Answer");
                    if (control is IEditableTextControl)
                    {
                        return control;
                    }
                    if (!this._createUserWizard.DesignMode && this._createUserWizard.QuestionAndAnswerRequired)
                    {
                        throw new HttpException(System.Web.SR.GetString("CreateUserWizard_NoAnswerTextBox", new object[] { this._createUserWizard.ID, "Answer" }));
                    }
                    return null;
                }
                set
                {
                    this._answerTextBox = value;
                }
            }

            internal LabelLiteral ConfirmPasswordLabel
            {
                get => 
                    this._confirmPasswordLabel;
                set
                {
                    this._confirmPasswordLabel = value;
                }
            }

            internal RequiredFieldValidator ConfirmPasswordRequired
            {
                get => 
                    this._confirmPasswordRequired;
                set
                {
                    this._confirmPasswordRequired = value;
                }
            }

            internal Control ConfirmPasswordTextBox
            {
                get
                {
                    if (this._confirmPasswordTextBox != null)
                    {
                        return this._confirmPasswordTextBox;
                    }
                    Control control = this.FindControl("ConfirmPassword");
                    if (control is IEditableTextControl)
                    {
                        return control;
                    }
                    return null;
                }
                set
                {
                    this._confirmPasswordTextBox = value;
                }
            }

            internal LabelLiteral EmailLabel
            {
                get => 
                    this._emailLabel;
                set
                {
                    this._emailLabel = value;
                }
            }

            internal RegularExpressionValidator EmailRegExpValidator
            {
                get => 
                    this._emailRegExpValidator;
                set
                {
                    this._emailRegExpValidator = value;
                }
            }

            internal RequiredFieldValidator EmailRequired
            {
                get => 
                    this._emailRequired;
                set
                {
                    this._emailRequired = value;
                }
            }

            internal Control EmailTextBox
            {
                get
                {
                    if (this._emailTextBox != null)
                    {
                        return this._emailTextBox;
                    }
                    Control control = this.FindControl("Email");
                    if (control is IEditableTextControl)
                    {
                        return control;
                    }
                    if (!this._createUserWizard.DesignMode && this._createUserWizard.RequireEmail)
                    {
                        throw new HttpException(System.Web.SR.GetString("CreateUserWizard_NoEmailTextBox", new object[] { this._createUserWizard.ID, "Email" }));
                    }
                    return null;
                }
                set
                {
                    this._emailTextBox = value;
                }
            }

            internal Control ErrorMessageLabel
            {
                get
                {
                    if (this._unknownErrorMessageLabel != null)
                    {
                        return this._unknownErrorMessageLabel;
                    }
                    Control control = this.FindControl("ErrorMessage");
                    if (control is ITextControl)
                    {
                        return control;
                    }
                    return null;
                }
                set
                {
                    this._unknownErrorMessageLabel = value;
                }
            }

            internal Image HelpPageIcon
            {
                get => 
                    this._helpPageIcon;
                set
                {
                    this._helpPageIcon = value;
                }
            }

            internal HyperLink HelpPageLink
            {
                get => 
                    this._helpPageLink;
                set
                {
                    this._helpPageLink = value;
                }
            }

            internal Literal InstructionLabel
            {
                get => 
                    this._instructionLabel;
                set
                {
                    this._instructionLabel = value;
                }
            }

            internal CompareValidator PasswordCompareValidator
            {
                get => 
                    this._passwordCompareValidator;
                set
                {
                    this._passwordCompareValidator = value;
                }
            }

            internal Literal PasswordHintLabel
            {
                get => 
                    this._passwordHintLabel;
                set
                {
                    this._passwordHintLabel = value;
                }
            }

            internal LabelLiteral PasswordLabel
            {
                get => 
                    this._passwordLabel;
                set
                {
                    this._passwordLabel = value;
                }
            }

            internal RegularExpressionValidator PasswordRegExpValidator
            {
                get => 
                    this._passwordRegExpValidator;
                set
                {
                    this._passwordRegExpValidator = value;
                }
            }

            internal RequiredFieldValidator PasswordRequired
            {
                get => 
                    this._passwordRequired;
                set
                {
                    this._passwordRequired = value;
                }
            }

            internal Control PasswordTextBox
            {
                get
                {
                    if (this._passwordTextBox != null)
                    {
                        return this._passwordTextBox;
                    }
                    Control control = this.FindControl("Password");
                    if (control is IEditableTextControl)
                    {
                        return control;
                    }
                    if (!this._createUserWizard.DesignMode && !this._createUserWizard.AutoGeneratePassword)
                    {
                        throw new HttpException(System.Web.SR.GetString("CreateUserWizard_NoPasswordTextBox", new object[] { this._createUserWizard.ID, "Password" }));
                    }
                    return null;
                }
                set
                {
                    this._passwordTextBox = value;
                }
            }

            internal LabelLiteral QuestionLabel
            {
                get => 
                    this._questionLabel;
                set
                {
                    this._questionLabel = value;
                }
            }

            internal RequiredFieldValidator QuestionRequired
            {
                get => 
                    this._questionRequired;
                set
                {
                    this._questionRequired = value;
                }
            }

            internal Control QuestionTextBox
            {
                get
                {
                    if (this._questionTextBox != null)
                    {
                        return this._questionTextBox;
                    }
                    Control control = this.FindControl("Question");
                    if (control is IEditableTextControl)
                    {
                        return control;
                    }
                    if (!this._createUserWizard.DesignMode && this._createUserWizard.QuestionAndAnswerRequired)
                    {
                        throw new HttpException(System.Web.SR.GetString("CreateUserWizard_NoQuestionTextBox", new object[] { this._createUserWizard.ID, "Question" }));
                    }
                    return null;
                }
                set
                {
                    this._questionTextBox = value;
                }
            }

            internal Literal Title
            {
                get => 
                    this._title;
                set
                {
                    this._title = value;
                }
            }

            internal LabelLiteral UserNameLabel
            {
                get => 
                    this._userNameLabel;
                set
                {
                    this._userNameLabel = value;
                }
            }

            internal RequiredFieldValidator UserNameRequired
            {
                get => 
                    this._userNameRequired;
                set
                {
                    this._userNameRequired = value;
                }
            }

            internal Control UserNameTextBox
            {
                get
                {
                    if (this._userNameTextBox != null)
                    {
                        return this._userNameTextBox;
                    }
                    Control control = this.FindControl("UserName");
                    if (control is IEditableTextControl)
                    {
                        return control;
                    }
                    if (!this._createUserWizard.DesignMode)
                    {
                        throw new HttpException(System.Web.SR.GetString("CreateUserWizard_NoUserNameTextBox", new object[] { this._createUserWizard.ID, "UserName" }));
                    }
                    return null;
                }
                set
                {
                    this._userNameTextBox = value;
                }
            }
        }

        private sealed class DataListItemTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Label child = new Label();
                child.PreventAutoID();
                child.ID = "SideBarLabel";
                container.Controls.Add(child);
            }
        }

        internal sealed class DefaultCompleteStepContentTemplate : ITemplate
        {
            private CreateUserWizard _wizard;

            internal DefaultCompleteStepContentTemplate(CreateUserWizard wizard)
            {
                this._wizard = wizard;
            }

            private void ConstructControls(CreateUserWizard.CompleteStepContainer container)
            {
                container.Title = CreateUserWizard.CreateLiteral();
                container.SuccessTextLabel = CreateUserWizard.CreateLiteral();
                container.EditProfileLink = new HyperLink();
                container.EditProfileLink.ID = "EditProfileLink";
                container.EditProfileIcon = new Image();
                container.EditProfileIcon.PreventAutoID();
                LinkButton button = new LinkButton {
                    ID = "ContinueButtonLinkButton",
                    CommandName = CreateUserWizard.ContinueButtonCommandName,
                    CausesValidation = false
                };
                ImageButton button2 = new ImageButton {
                    ID = "ContinueButtonImageButton",
                    CommandName = CreateUserWizard.ContinueButtonCommandName,
                    CausesValidation = false
                };
                Button button3 = new Button {
                    ID = "ContinueButtonButton",
                    CommandName = CreateUserWizard.ContinueButtonCommandName,
                    CausesValidation = false
                };
                container.ContinueLinkButton = button;
                container.ContinuePushButton = button3;
                container.ContinueImageButton = button2;
            }

            private void LayoutControls(CreateUserWizard.CompleteStepContainer container)
            {
                Table child = CreateUserWizard.CreateTable();
                child.EnableViewState = false;
                TableRow row = CreateUserWizard.CreateTableRow();
                TableCell cell = CreateUserWizard.CreateTableCell();
                cell.ColumnSpan = 2;
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.Controls.Add(container.Title);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                cell = CreateUserWizard.CreateTableCell();
                cell.Controls.Add(container.SuccessTextLabel);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                cell = CreateUserWizard.CreateTableCell();
                cell.ColumnSpan = 2;
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Controls.Add(container.ContinuePushButton);
                cell.Controls.Add(container.ContinueLinkButton);
                cell.Controls.Add(container.ContinueImageButton);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                cell = CreateUserWizard.CreateTableCell();
                cell.ColumnSpan = 2;
                cell.Controls.Add(container.EditProfileIcon);
                cell.Controls.Add(container.EditProfileLink);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                container.LayoutTable = child;
                container.InnerCell.Controls.Add(child);
            }

            void ITemplate.InstantiateIn(Control container)
            {
                CreateUserWizard.CompleteStepContainer parent = (CreateUserWizard.CompleteStepContainer) container.Parent.Parent.Parent;
                this.ConstructControls(parent);
                this.LayoutControls(parent);
            }
        }

        internal sealed class DefaultCreateUserContentTemplate : ITemplate
        {
            private CreateUserWizard _wizard;

            internal DefaultCreateUserContentTemplate(CreateUserWizard wizard)
            {
                this._wizard = wizard;
            }

            private void ConstructControls(CreateUserWizard.CreateUserStepContainer container)
            {
                string validationGroup = this._wizard.ValidationGroup;
                container.Title = CreateUserWizard.CreateLiteral();
                container.InstructionLabel = CreateUserWizard.CreateLiteral();
                container.PasswordHintLabel = CreateUserWizard.CreateLiteral();
                TextBox textBox = new TextBox {
                    ID = "UserName"
                };
                container.UserNameTextBox = textBox;
                TextBox control = new TextBox {
                    ID = "Password",
                    TextMode = TextBoxMode.Password
                };
                container.PasswordTextBox = control;
                TextBox box3 = new TextBox {
                    ID = "ConfirmPassword",
                    TextMode = TextBoxMode.Password
                };
                container.ConfirmPasswordTextBox = box3;
                bool enableValidation = true;
                container.UserNameRequired = CreateUserWizard.CreateRequiredFieldValidator("UserNameRequired", validationGroup, textBox, enableValidation);
                container.UserNameLabel = CreateUserWizard.CreateLabelLiteral(textBox);
                container.PasswordLabel = CreateUserWizard.CreateLabelLiteral(control);
                container.ConfirmPasswordLabel = CreateUserWizard.CreateLabelLiteral(box3);
                Image image = new Image();
                image.PreventAutoID();
                container.HelpPageIcon = image;
                HyperLink link = new HyperLink {
                    ID = "HelpLink"
                };
                container.HelpPageLink = link;
                Literal literal = new Literal {
                    ID = "ErrorMessage"
                };
                container.ErrorMessageLabel = literal;
                TextBox box4 = new TextBox {
                    ID = "Email"
                };
                container.EmailRequired = CreateUserWizard.CreateRequiredFieldValidator("EmailRequired", validationGroup, box4, enableValidation);
                container.EmailTextBox = box4;
                container.EmailLabel = CreateUserWizard.CreateLabelLiteral(box4);
                RegularExpressionValidator validator = new RegularExpressionValidator {
                    ID = "EmailRegExp",
                    ControlToValidate = "Email",
                    ErrorMessage = this._wizard.EmailRegularExpressionErrorMessage,
                    ValidationExpression = this._wizard.EmailRegularExpression,
                    ValidationGroup = validationGroup,
                    Display = ValidatorDisplay.Dynamic,
                    Enabled = enableValidation,
                    Visible = enableValidation
                };
                container.EmailRegExpValidator = validator;
                container.PasswordRequired = CreateUserWizard.CreateRequiredFieldValidator("PasswordRequired", validationGroup, control, enableValidation);
                container.ConfirmPasswordRequired = CreateUserWizard.CreateRequiredFieldValidator("ConfirmPasswordRequired", validationGroup, box3, enableValidation);
                RegularExpressionValidator validator2 = new RegularExpressionValidator {
                    ID = "PasswordRegExp",
                    ControlToValidate = "Password",
                    ErrorMessage = this._wizard.PasswordRegularExpressionErrorMessage,
                    ValidationExpression = this._wizard.PasswordRegularExpression,
                    ValidationGroup = validationGroup,
                    Display = ValidatorDisplay.Dynamic,
                    Enabled = enableValidation,
                    Visible = enableValidation
                };
                container.PasswordRegExpValidator = validator2;
                CompareValidator validator3 = new CompareValidator {
                    ID = "PasswordCompare",
                    ControlToValidate = "ConfirmPassword",
                    ControlToCompare = "Password",
                    Operator = ValidationCompareOperator.Equal,
                    ErrorMessage = this._wizard.ConfirmPasswordCompareErrorMessage,
                    ValidationGroup = validationGroup,
                    Display = ValidatorDisplay.Dynamic,
                    Enabled = enableValidation,
                    Visible = enableValidation
                };
                container.PasswordCompareValidator = validator3;
                TextBox box5 = new TextBox {
                    ID = "Question"
                };
                container.QuestionTextBox = box5;
                TextBox box6 = new TextBox {
                    ID = "Answer"
                };
                container.AnswerTextBox = box6;
                container.QuestionRequired = CreateUserWizard.CreateRequiredFieldValidator("QuestionRequired", validationGroup, box5, enableValidation);
                container.AnswerRequired = CreateUserWizard.CreateRequiredFieldValidator("AnswerRequired", validationGroup, box6, enableValidation);
                container.QuestionLabel = CreateUserWizard.CreateLabelLiteral(box5);
                container.AnswerLabel = CreateUserWizard.CreateLabelLiteral(box6);
            }

            private void LayoutControls(CreateUserWizard.CreateUserStepContainer container)
            {
                Table child = CreateUserWizard.CreateTable();
                child.EnableViewState = false;
                TableRow row = CreateUserWizard.CreateTableRow();
                TableCell cell = CreateUserWizard.CreateTableCell();
                cell.ColumnSpan = 2;
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.Controls.Add(container.Title);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                row.PreventAutoID();
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.ColumnSpan = 2;
                cell.Controls.Add(container.InstructionLabel);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                if (this._wizard.ConvertingToTemplate)
                {
                    container.UserNameLabel.RenderAsLabel = true;
                }
                cell.Controls.Add(container.UserNameLabel);
                row.Cells.Add(cell);
                cell = CreateUserWizard.CreateTableCell();
                cell.Controls.Add(container.UserNameTextBox);
                cell.Controls.Add(container.UserNameRequired);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                this._wizard._passwordTableRow = row;
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                if (this._wizard.ConvertingToTemplate)
                {
                    container.PasswordLabel.RenderAsLabel = true;
                }
                cell.Controls.Add(container.PasswordLabel);
                row.Cells.Add(cell);
                cell = CreateUserWizard.CreateTableCell();
                cell.Controls.Add(container.PasswordTextBox);
                if (!this._wizard.AutoGeneratePassword)
                {
                    cell.Controls.Add(container.PasswordRequired);
                }
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                this._wizard._passwordHintTableRow = row;
                cell = CreateUserWizard.CreateTableCell();
                row.Cells.Add(cell);
                cell = CreateUserWizard.CreateTableCell();
                cell.Controls.Add(container.PasswordHintLabel);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                this._wizard._confirmPasswordTableRow = row;
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                if (this._wizard.ConvertingToTemplate)
                {
                    container.ConfirmPasswordLabel.RenderAsLabel = true;
                }
                cell.Controls.Add(container.ConfirmPasswordLabel);
                row.Cells.Add(cell);
                cell = CreateUserWizard.CreateTableCell();
                cell.Controls.Add(container.ConfirmPasswordTextBox);
                if (!this._wizard.AutoGeneratePassword)
                {
                    cell.Controls.Add(container.ConfirmPasswordRequired);
                }
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                this._wizard._emailRow = row;
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Controls.Add(container.EmailLabel);
                if (this._wizard.ConvertingToTemplate)
                {
                    container.EmailLabel.RenderAsLabel = true;
                }
                row.Cells.Add(cell);
                cell = CreateUserWizard.CreateTableCell();
                cell.Controls.Add(container.EmailTextBox);
                cell.Controls.Add(container.EmailRequired);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                this._wizard._questionRow = row;
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Controls.Add(container.QuestionLabel);
                if (this._wizard.ConvertingToTemplate)
                {
                    container.QuestionLabel.RenderAsLabel = true;
                }
                row.Cells.Add(cell);
                cell = CreateUserWizard.CreateTableCell();
                cell.Controls.Add(container.QuestionTextBox);
                cell.Controls.Add(container.QuestionRequired);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                this._wizard._answerRow = row;
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Controls.Add(container.AnswerLabel);
                if (this._wizard.ConvertingToTemplate)
                {
                    container.AnswerLabel.RenderAsLabel = true;
                }
                row.Cells.Add(cell);
                cell = CreateUserWizard.CreateTableCell();
                cell.Controls.Add(container.AnswerTextBox);
                cell.Controls.Add(container.AnswerRequired);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                this._wizard._passwordCompareRow = row;
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.ColumnSpan = 2;
                cell.Controls.Add(container.PasswordCompareValidator);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                this._wizard._passwordRegExpRow = row;
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.ColumnSpan = 2;
                cell.Controls.Add(container.PasswordRegExpValidator);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                this._wizard._emailRegExpRow = row;
                cell = CreateUserWizard.CreateTableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.ColumnSpan = 2;
                cell.Controls.Add(container.EmailRegExpValidator);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                cell = CreateUserWizard.CreateTableCell();
                cell.ColumnSpan = 2;
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.Controls.Add(container.ErrorMessageLabel);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                row = CreateUserWizard.CreateTableRow();
                cell = CreateUserWizard.CreateTableCell();
                cell.ColumnSpan = 2;
                cell.Controls.Add(container.HelpPageIcon);
                cell.Controls.Add(container.HelpPageLink);
                row.Cells.Add(cell);
                child.Rows.Add(row);
                container.InnerCell.Controls.Add(child);
            }

            void ITemplate.InstantiateIn(Control container)
            {
                CreateUserWizard.CreateUserStepContainer parent = (CreateUserWizard.CreateUserStepContainer) container.Parent.Parent.Parent;
                this.ConstructControls(parent);
                this.LayoutControls(parent);
            }
        }

        internal sealed class DefaultCreateUserNavigationTemplate : ITemplate
        {
            private IButtonControl[][] _buttons;
            private TableCell[] _innerCells;
            private TableRow _row;
            private CreateUserWizard _wizard;

            internal DefaultCreateUserNavigationTemplate(CreateUserWizard wizard)
            {
                this._wizard = wizard;
            }

            internal void ApplyLayoutStyleToInnerCells(TableItemStyle tableItemStyle)
            {
                for (int i = 0; i < this._innerCells.Length; i++)
                {
                    if (tableItemStyle.IsSet(0x10000))
                    {
                        this._innerCells[i].HorizontalAlign = tableItemStyle.HorizontalAlign;
                    }
                    if (tableItemStyle.IsSet(0x20000))
                    {
                        this._innerCells[i].VerticalAlign = tableItemStyle.VerticalAlign;
                    }
                }
            }

            private TableCell CreateButtonControl(IButtonControl[] buttons, string validationGroup, string id, bool causesValidation, string commandName)
            {
                LinkButton child = new LinkButton {
                    CausesValidation = causesValidation,
                    ID = id + "LinkButton",
                    Visible = false,
                    CommandName = commandName,
                    ValidationGroup = validationGroup
                };
                buttons[0] = child;
                ImageButton button2 = new ImageButton {
                    CausesValidation = causesValidation,
                    ID = id + "ImageButton",
                    Visible = !this._wizard.DesignMode,
                    CommandName = commandName,
                    ValidationGroup = validationGroup
                };
                button2.PreRender += new EventHandler(this.OnPreRender);
                buttons[1] = button2;
                Button button3 = new Button {
                    CausesValidation = causesValidation,
                    ID = id + "Button",
                    Visible = false,
                    CommandName = commandName,
                    ValidationGroup = validationGroup
                };
                buttons[2] = button3;
                TableCell cell = new TableCell {
                    HorizontalAlign = HorizontalAlign.Right
                };
                this._row.Cells.Add(cell);
                cell.Controls.Add(child);
                cell.Controls.Add(button2);
                cell.Controls.Add(button3);
                return cell;
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

            private void OnPreRender(object source, EventArgs e)
            {
                ((ImageButton) source).Visible = false;
            }

            void ITemplate.InstantiateIn(Control container)
            {
                this._wizard._defaultCreateUserNavigationTemplate = this;
                container.EnableViewState = false;
                Table child = CreateUserWizard.CreateTable();
                child.CellSpacing = 5;
                child.CellPadding = 5;
                container.Controls.Add(child);
                TableRow row = new TableRow();
                this._row = row;
                row.PreventAutoID();
                row.HorizontalAlign = HorizontalAlign.Right;
                child.Rows.Add(row);
                this._buttons = new IButtonControl[][] { new IButtonControl[3], new IButtonControl[3], new IButtonControl[3] };
                this._innerCells = new TableCell[] { this.CreateButtonControl(this._buttons[0], this._wizard.ValidationGroup, Wizard.StepPreviousButtonID, false, Wizard.MovePreviousCommandName), this.CreateButtonControl(this._buttons[1], this._wizard.ValidationGroup, Wizard.StepNextButtonID, true, Wizard.MoveNextCommandName), this.CreateButtonControl(this._buttons[2], this._wizard.ValidationGroup, Wizard.CancelButtonID, false, Wizard.CancelCommandName) };
            }

            internal IButtonControl CancelButton =>
                this.GetButtonBasedOnType(2, this._wizard.CancelButtonType);

            internal IButtonControl CreateUserButton =>
                this.GetButtonBasedOnType(1, this._wizard.CreateUserButtonType);

            internal IButtonControl PreviousButton =>
                this.GetButtonBasedOnType(0, this._wizard.StepPreviousButtonType);
        }

        private sealed class DefaultSideBarTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                DataList child = new DataList {
                    ID = Wizard.DataListID
                };
                container.Controls.Add(child);
                child.SelectedItemStyle.Font.Bold = true;
                child.ItemTemplate = new CreateUserWizard.DataListItemTemplate();
            }
        }
    }
}

