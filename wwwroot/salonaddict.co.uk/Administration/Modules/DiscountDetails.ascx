<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscountDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.DiscountDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/Administration/Modules/TextBoxes/DecimalTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DateTimeTextBox" Src="~/Administration/Modules/TextBoxes/DateTimeTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DiscountType" Src="~/Administration/Modules/DropDownLists/DiscountTypeDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="DiscountRequirementType" Src="~/Administration/Modules/DropDownLists/DiscountRequirementDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="DiscountLimitationType" Src="~/Administration/Modules/DropDownLists/DiscountLimitationDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="BusinessList" Src="~/Administration/Modules/DropDownLists/BusinessDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="ServiceTypeList" Src="~/Administration/Modules/DropDownLists/ServiceTypeDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="SlideType" Src="~/Administration/Modules/DropDownLists/SlideTypeDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="SlideUnitType" Src="~/Administration/Modules/DropDownLists/SlideUnitTypeDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="OverrideInitialUnitType" Src="~/Administration/Modules/DropDownLists/OverrideInitialUnitTypeDropDownList.ascx" %>

<%@ Import Namespace="SalonAddict.BusinessAccess" %>

<% if (this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Add a new discount<a href="Discounts.aspx" title="Back to discount list"> (back to discount list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save discount" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit discount details <a href="Discounts.aspx" title="Back to discount list">(back to discount list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save discount" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete discount" OnClientClick="return confirm('Are you sure?')" />
    </div>
</div>
<% } %>
<ajaxToolkit:TabContainer ID="tabs" runat="server" ActiveTabIndex="0" >
   <ajaxToolkit:TabPanel ID="tabInfo" runat="server" HeaderText="Discount info" >
      <ContentTemplate>
        <asp:UpdatePanel ID="up" runat="server" >
           <ContentTemplate>
            <table class="details">
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblDiscountType" 
                            runat="server"
                            Text="Discount Type:" 
                            IsRequired="true"
                            ToolTip="The type of discount."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DiscountType 
                            ID="ddlDiscountType" 
                            runat="server"
                            DefaultText="Choose a Discount Type"
                            DefaultValue=""
                            ErrorMessage="Discount Type is a required field." />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblDiscountRequirement" 
                            runat="server"
                            Text="Discount Requirement:" 
                            IsRequired="true"
                            ToolTip="Requirements for discount to be applied."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DiscountRequirementType 
                            ID="ddlDiscountRequirement" 
                            runat="server"
                            OnSelectedIndexChanged="ddlDiscountRequirement_SelectedIndexChanged"
                            ErrorMessage="Discount Requirement is a required field." />
                    </td>
                </tr>
                <% if (this.ddlDiscountRequirement.DiscountRequirement == DiscountRequirementType.Business)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblBusiness" 
                            runat="server"
                            Text="Business:" 
                            IsRequired="true"
                            ToolTip="The discounted business."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:BusinessList 
                            ID="ddlBusiness" 
                            runat="server" 
                            IsRequired="false"
                            DefaultText="Choose a Business"  
                            DefaultValue="" />
                    </td>
                </tr>
                <% } %>
                <% if (this.ddlDiscountRequirement.DiscountRequirement == DiscountRequirementType.ServiceType)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblServiceTypeID" 
                            runat="server"
                            Text="Service Type:" 
                            IsRequired="true"
                            ToolTip="The discounted service type."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:ServiceTypeList 
                            ID="ddlServiceType" 
                            runat="server" 
                            IsRequired="false"
                            DefaultText="Choose a Service Type"  
                            DefaultValue="" />
                    </td>
                </tr>
                <% } %>
                <% if (this.ddlDiscountRequirement.DiscountRequirement == DiscountRequirementType.HasSpentAmount)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblRequiredSpendAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Required spend amount:" 
                            ToolTip="The minimum amount required for the discount to be applied."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtRequiredSpendAmount" 
                            runat="server" 
                            RequiredErrorMessage="Required spend amount is required"
                            MinimumValue="0" 
                            Value="0"
                            IsRequired="false"
                            MaximumValue="999999" 
                            RangeErrorMessage="The value must be from 0 to 999">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblDiscountLimitation" 
                            runat="server"
                            Text="Discount Limitation:" 
                            IsRequired="true"
                            ToolTip="Limitations for discount to be applied."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DiscountLimitationType 
                            ID="ddlDiscountLimitation" 
                            runat="server"
                            OnSelectedIndexChanged="ddlDiscountLimitation_SelectedIndexChanged"
                            ErrorMessage="Discount Limitation is a required field." />
                    </td>
                </tr>
                <% if (ddlDiscountLimitation.DiscountLimitation == DiscountLimitationType.NTimesOnly)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblLimitationTimes" 
                            runat="server"
                            IsRequired="true"
                            Text="N Times:" 
                            ToolTip="Enter the number of times that the discount is limited to."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:NumericTextBox 
                            ID="txtLimitationTimes" 
                            runat="server" 
                            RequiredErrorMessage="N Times is a required field." 
                            MinimumValue="0" 
                            MaximumValue="9999999"
                            Value="0" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 9999999">
                         </SA:NumericTextBox>
                    </td>
                </tr>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblName" 
                            runat="server"
                            Text="Name:" 
                            IsRequired="true"
                            ToolTip="The discount name."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtName" 
                            runat="server" 
                            ErrorMessage="Discount name is a required field." 
                            MaxLength="100" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblDescription" 
                            runat="server"
                            Text="Description:" 
                            ToolTip="The discount description."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtDescription" runat="server" MaxLength="400" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblUsePercentage" 
                            runat="server"
                            Text="Use Percentage:" 
                            IsRequired="true"
                            ToolTip="Determines whether to apply a percentage discount. If not enabled a set value is used."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbUsePercentage" runat="server" AutoPostBack="true" OnCheckedChanged="cbUsePercentage_CheckedChanged" />
                    </td>
                </tr>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblDiscountPercentage" 
                            runat="server"
                            IsRequired="true"
                            Text="Discount Percentage:" 
                            ToolTip="The percentage discount to apply."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtDiscountPercentage" 
                            runat="server" 
                            RequiredErrorMessage="Percentage discount is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            Value="0"
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblDiscountAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Discount Amount:" 
                            ToolTip="The discount amount to apply."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtDiscountAmount" 
                            runat="server" 
                            RequiredErrorMessage="Discount amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            Value="0"
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999">
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblStartDate" 
                            runat="server"
                            IsRequired="true"
                            Text="Start Date:" 
                            ToolTip="The start of the discount period."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DateTimeTextBox 
                            ID="txtStartDate" 
                            runat="server" 
                            ErrorMessage="Start date is a required field." />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblEndDate" 
                            runat="server"
                            Text="End Date:" 
                            ToolTip="The end of the discount period."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DateTimeTextBox 
                            ID="txtEndDate" 
                            runat="server" 
                            IsRequired="false" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSlideType" 
                            runat="server"
                            IsRequired="true"
                            Text="Sliding Options:" 
                            ToolTip="Determines the discount sliding type."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:SlideType ID="ddlSlideType" runat="server" OnSelectedIndexChanged="ddlSlideType_SelectedIndexChanged" />
                    </td>
                </tr>
                <% if ((int?)ddlSlideType.SlideType > 0)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSlideOver" 
                            runat="server"
                            IsRequired="true"
                            Text="Slide Over:" 
                            ToolTip="Slide over x units"
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:NumericTextBox 
                            ID="txtSlideOver" 
                            runat="server" 
                            RequiredErrorMessage="Slide over is a required field." 
                            MinimumValue="1" 
                            MaximumValue="999999999"
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 1 to 999999999">
                         </SA:NumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSlideUnit" 
                            runat="server"
                            IsRequired="true"
                            Text="Sliding Unit:" 
                            ToolTip="The sliding unit"
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:SlideUnitType ID="ddlSlideUnit" runat="server" IsRequired="false" DefaultText="Choose a Unit" DefaultValue="" ></SA:SlideUnitType>
                    </td>
                </tr>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblMinimumSlidePercentage" 
                            runat="server"
                            IsRequired="false"
                            Text="Minimum Slide Percentage:" 
                            ToolTip="The minimum sliding percentage to apply to the discount."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtMinimumSlidePercentage" 
                            runat="server" 
                            RequiredErrorMessage="Minimum slide percentage is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblMinimumSlideAmount" 
                            runat="server"
                            IsRequired="false"
                            Text="Minimum Slide Amount:" 
                            ToolTip="The slide amount to apply to the discount."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtMinimumSlideAmount" 
                            runat="server" 
                            RequiredErrorMessage="Minimum slide amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999" >
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideInitial" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Initial:" 
                            ToolTip="Initial value to use during the first period"
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbOverrideInitial" runat="server" AutoPostBack="true" OnCheckedChanged="cbOverride_CheckedChanged" />
                    </td>
                </tr>
                <% if (cbOverrideInitial.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideInitialPeriod" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Initial Period:" 
                            ToolTip="Initial value to use that defines the first period"
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:NumericTextBox 
                            ID="txtOverrideInitialPeriod" 
                            runat="server" 
                            RequiredErrorMessage="Override initial period is a required field." 
                            MinimumValue="1" 
                            MaximumValue="99"
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 1 to 99">
                         </SA:NumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideInitialUnitType" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Initial Unit:" 
                            ToolTip="Unit of the first period"
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:OverrideInitialUnitType ID="ddlOverrideInitialUnitType" runat="server" DefaultText="Choose a Unit" DefaultValue="0" />
                    </td>
                </tr>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideInitialPercentage" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Initial Percentage:" 
                            ToolTip="The percentage value to use during the initial period."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideInitialPercentage" 
                            runat="server" 
                            RequiredErrorMessage="Override initial percentage is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideInitialAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Initial Amount:" 
                            ToolTip="The discounted amount to apply during the initial period."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideInitialAmount" 
                            runat="server" 
                            RequiredErrorMessage="Override initial amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999" >
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblMondayOverride" 
                            runat="server"
                            Text="Override Monday:" 
                            IsRequired="true"
                            ToolTip="Determines whether to use a specified value if the day falls on a monday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbMondayOverride" runat="server" AutoPostBack="true" OnCheckedChanged="cbOverride_CheckedChanged" />
                    </td>
                </tr>
                <% if(cbMondayOverride.Checked)
                   { %>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideMondayPercentage" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Monday Percentage:" 
                            ToolTip="The percentage to apply on monday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideMondayPercentage" 
                            runat="server" 
                            RequiredErrorMessage="Override monday percentage is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideMondayAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Monday Amount:" 
                            ToolTip="The amount to apply on Monday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideMondayAmount" 
                            runat="server" 
                            RequiredErrorMessage="Override monday amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999" >
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideTuesday" 
                            runat="server"
                            Text="Override Tuesday:" 
                            IsRequired="true"
                            ToolTip="Determines whether to use a specified value if the day falls on a tuesday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbTuesdayOverride" runat="server" AutoPostBack="true" OnCheckedChanged="cbOverride_CheckedChanged" />
                    </td>
                </tr>
                <% if(cbTuesdayOverride.Checked)
                   { %>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideTuesdayPercentage" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Tuesday Percentage:" 
                            ToolTip="The percentage to apply on tuesday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideTuesdayPercentage" 
                            runat="server" 
                            RequiredErrorMessage="Override tuesday percentage is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideTuesdayAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Tuesday Amount:" 
                            ToolTip="The amount to apply on Tuesday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideTuesdayAmount" 
                            runat="server" 
                            RequiredErrorMessage="Override tuesday amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999" >
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideWednesday" 
                            runat="server"
                            Text="Override Wednesday:" 
                            IsRequired="true"
                            ToolTip="Determines whether to use a specified value if the day falls on a wednesday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbWednesdayOverride" runat="server" AutoPostBack="true" OnCheckedChanged="cbOverride_CheckedChanged" />
                    </td>
                </tr>
                <% if(cbWednesdayOverride.Checked)
                   { %>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideWednesdayPercentage" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Wednesday Percentage:" 
                            ToolTip="The percentage to apply on wednesday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideWednesdayPercentage" 
                            runat="server" 
                            RequiredErrorMessage="Override wednesday percentage is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideWednesdayAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Wednesday Amount:" 
                            ToolTip="The amount to apply on Wednesday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideWednesdayAmount" 
                            runat="server" 
                            RequiredErrorMessage="Override wednesday amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999" >
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblThursdayOverride" 
                            runat="server"
                            Text="Override Thursday:" 
                            IsRequired="true"
                            ToolTip="Determines whether to use a specified value if the day falls on a thursday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbThursdayOverride" runat="server" AutoPostBack="true" OnCheckedChanged="cbOverride_CheckedChanged" />
                    </td>
                </tr>
                <% if(cbThursdayOverride.Checked)
                   { %>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideThursdayPercentage" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Thursday Percentage:" 
                            ToolTip="The percentage to apply on thursday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideThursdayPercentage" 
                            runat="server" 
                            RequiredErrorMessage="Override thursday percentage is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideThursdayAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Thursday Amount:" 
                            ToolTip="The amount to apply on Thursday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideThursdayAmount" 
                            runat="server" 
                            RequiredErrorMessage="Override thursday amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999" >
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblFridayOverride" 
                            runat="server"
                            Text="Override Friday:" 
                            IsRequired="true"
                            ToolTip="Determines whether to use a specified value if the day falls on a friday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbFridayOverride" runat="server" AutoPostBack="true" OnCheckedChanged="cbOverride_CheckedChanged" />
                    </td>
                </tr>
                <% if(cbFridayOverride.Checked)
                   { %>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideFridayPercentage" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Friday Percentage:" 
                            ToolTip="The percentage to apply on friday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideFridayPercentage" 
                            runat="server" 
                            RequiredErrorMessage="Override friday percentage is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideFridayAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Friday Amount:" 
                            ToolTip="The amount to apply on Friday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideFridayAmount" 
                            runat="server" 
                            RequiredErrorMessage="Override friday amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999" >
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSaturdayOverride" 
                            runat="server"
                            Text="Override Saturday:" 
                            IsRequired="true"
                            ToolTip="Determines whether to use a specified value if the day falls on a saturday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbSaturdayOverride" runat="server" AutoPostBack="true" OnCheckedChanged="cbOverride_CheckedChanged" />
                    </td>
                </tr>
                <% if(cbSaturdayOverride.Checked)
                   { %>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideSaturdayPercentage" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Saturday Percentage:" 
                            ToolTip="The percentage to apply on saturday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideSaturdayPercentage" 
                            runat="server" 
                            RequiredErrorMessage="Override saturday percentage is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideSaturdayAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Saturday Amount:" 
                            ToolTip="The amount to apply on Saturday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideSaturdayAmount" 
                            runat="server" 
                            RequiredErrorMessage="Override saturday amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999" >
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <% } %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSundayOverride" 
                            runat="server"
                            Text="Override Sunday:" 
                            IsRequired="true"
                            ToolTip="Determines whether to use a specified value if the day falls on a saturday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbSundayOverride" runat="server" AutoPostBack="true" OnCheckedChanged="cbOverride_CheckedChanged" />
                    </td>
                </tr>
                <% if(cbSundayOverride.Checked)
                   { %>
                <% if (cbUsePercentage.Checked)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideSundayPercentage" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Sunday Percentage:" 
                            ToolTip="The percentage to apply on sunday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideSundayPercentage" 
                            runat="server" 
                            RequiredErrorMessage="Override sunday percentage is a required field."
                            MinimumValue="0" 
                            MaximumValue="100" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 100">
                        </SA:DecimalTextBox>
                    </td>
                </tr>
                <% }
                   else
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblOverrideSundayAmount" 
                            runat="server"
                            IsRequired="true"
                            Text="Override Sunday Amount:" 
                            ToolTip="The amount to apply on Sunday."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:DecimalTextBox 
                            ID="txtOverrideSundayAmount" 
                            runat="server" 
                            RequiredErrorMessage="Override sunday amount is a required field."
                            MinimumValue="0" 
                            MaximumValue="999" 
                            IsRequired="false"
                            RangeErrorMessage="The value must be from 0 to 999" >
                        </SA:DecimalTextBox>
                        &nbsp;
                        [<%= SalonAddict.BusinessAccess.Implementation.CurrencyManager.PrimaryCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <% } %>
                <% } %>
                <% } %>
            </table>
           </ContentTemplate>
        </asp:UpdatePanel>
      </ContentTemplate>
   </ajaxToolkit:TabPanel>
   <ajaxToolkit:TabPanel ID="tabUsage" runat="server" HeaderText="Usage history" >
      <ContentTemplate>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            DataKeyNames="DiscountUsageHistoryID"
            AutoGenerateColumns="False"
            Width="100%" >
            <Columns>
                <asp:TemplateField HeaderText="Order" >
                    <ItemTemplate>
                        <a href='<%# "OrderDetails.aspx?OrderID=" + Eval("OrderID") %>' >view</a>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Used on" >
                    <ItemTemplate>
                        <%# Eval("CreatedOn") %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
      </ContentTemplate>
   </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
