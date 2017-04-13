<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CurrencyDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.CurrencyDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/Administration/Modules/TextBoxes/DecimalTextBox.ascx" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.Implementation" %>

<div class="section-header">
<% if(this.Action == ActionType.Edit)
   { %>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Edit currency details <a href="Currencies.aspx" title="Back to currency list">(back to currency list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save currency" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete currency" OnClientClick="return confirm('Are you sure?')" />
    </div>
<% } 
   else
   {%>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Add a new currency <a href="Currencies.aspx" title="Back to currency list">(back to currency list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save currency" />
    </div>
<% } %>
</div>
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblName" 
                runat="server" 
                Text="Name:" 
                IsRequired="true"
                ToolTip="The name of the currency." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtName" 
                runat="server" 
                MaxLength="50"
                ErrorMessage="Currency name is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCurrencyCode" 
                runat="server" 
                Text="Currency code:" 
                IsRequired="true"
                ToolTip="The currency code. For a list of currency codes, go to: http://www.iso.org/iso/support/currency_codes_list-1.htm" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtCurrencyCode" 
                runat="server" 
                MaxLength="5"
                ErrorMessage="Currency code is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblRate" 
                runat="server" 
                Text="Rate to primary exchange currency " 
                IsRequired="true"
                ToolTip="The exchange rate against the primary exchange rate currency." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
                [<%= CurrencyManager.PrimaryExchangeRateCurrency.CurrencyCode %>]:
        </td>
        <td class="data-item">
            <SA:DecimalTextBox 
                ID="txtRate" 
                runat="server" 
                RequiredErrorMessage="Rate is required"
                MinimumValue="0" 
                MaximumValue="100" 
                RangeErrorMessage="The value must be from 0 to 100">
            </SA:DecimalTextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayLocale" 
                runat="server" 
                Text="Display locale:" 
                IsRequired="true"
                ToolTip="The display locale for currency values." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList ID="ddlDisplayLocale" runat="server" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCustomFormatting" 
                runat="server" 
                Text="Custom formatting:" 
                ToolTip="Custom formatting to be applied to the currency values." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtCustomFormatting" runat="server" MaxLength="50" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblPublished" 
                runat="server" 
                Text="Published:" 
                ToolTip="Determines whether this currency is published and can therefore be selected by visitors to your store." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbPublished" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server" 
                Text="Display order:" ToolTip="The display order for this currency. 1 represents the top of the list." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox  
                ID="txtDisplayOrder" 
                runat="server"
                Value="1" 
                MaxLength="4"
                RequiredErrorMessage="Display order is required" 
                RangeErrorMessage="The value must be from 1 to 99999"
                MinimumValue="1" 
                MaximumValue="99999">
             </SA:NumericTextBox>
        </td>
    </tr>
    <% if (this.Action == ActionType.Edit)
       { %>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCreatedOnTitle" 
                runat="server" 
                Text="Created on:"
                ToolTip="The date/time the currency was created." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrCreatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel    
                ID="lblUpdateOnTitle" 
                runat="server" 
                Text="Updated on:"
                ToolTip="The date/time the currency was updated." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrUpdatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <% } %>
</table>