<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CouponDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.CouponDetails" %>
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
        Add a new coupon<a href="Coupons.aspx" title="Back to coupon list"> (back to coupon list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save coupon" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit coupon details <a href="Coupons.aspx" title="Back to coupon list">(back to coupon list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save coupon" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete coupon" OnClientClick="return confirm('Are you sure?')" />
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
                            ID="lblCouponCode" 
                            runat="server"
                            Text="Coupon code:" 
                            IsRequired="true"
                            ToolTip="The coupon code."
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtCouponCode" 
                            runat="server" 
                            ErrorMessage="Coupon code is a required field." 
                            AutoPostBack="true"
                            OnTextChanged="txtCouponCode_TextChanged"
                            MaxLength="50" />
                        <asp:CustomValidator ID="cvCouponCode" runat="server" ControlToValidate="txtCouponCode" EnableClientScript="false" OnServerValidate="cvCouponCode_Validate" ErrorMessage="Coupon code is not available." ></asp:CustomValidator>
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
