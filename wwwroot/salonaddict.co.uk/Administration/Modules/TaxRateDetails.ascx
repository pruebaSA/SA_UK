<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaxRateDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.TaxRateDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/Administration/Modules/TextBoxes/DecimalTextBox.ascx" %>

<% if (this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Add a new tax rate <a href="TaxRates.aspx" title="Back to tax rate list"> (back to tax rate list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save tax rate" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit tax rate details <a href="TaxRates.aspx" title="Back to tax rate list">(back to tax rate list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save tax rate" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete tax rate" OnClientClick="return confirm('Are you sure?')" />
    </div>
</div>
<% } %>
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblTaxCategory" 
                runat="server"
                Text="Tax Category:" 
                IsRequired="true"
                ToolTip="The tax category."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList 
                ID="ddlTaxCategories" 
                runat="server" 
                Width="220px" >
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblCountries" 
                runat="server"
                Text="Country:" 
                ToolTip="The tax rate country (optional)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList 
                ID="ddlCountries" 
                runat="server" 
                Width="220px" 
                AutoPostBack="true" 
                OnSelectedIndexChanged="ddlTaxCategories_SelectedIndexChanged" >
            </asp:DropDownList>
        </td>
    </tr>
        <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblStateProvince" 
                runat="server"
                Text="State/Province:" 
                ToolTip="The tax rate state/province (optional)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList ID="ddlStateProvinces" runat="server" Width="220px" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblZipPostalCode" 
                runat="server" 
                Text="Zip/Postal code:"
                ToolTip="A zip/postal code of the tax rate (optional)." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox  ID="txtZipPostalCode" runat="server" MaxLength="10" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblPercentage" 
                runat="server"
                Text="Percentage:"
                IsRequired="true"
                ToolTip="The percentage of the tax rate (#.0000)"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:DecimalTextBox 
                ID="txtPercentage" 
                runat="server"
                MinimumValue="0"
                MaximumValue="99999"
                RequiredErrorMessage="Tax rate percentage is a requried field." 
                Value="0.0000"
                RangeErrorMessage="The value must be from 0 to 99999">
            </SA:DecimalTextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblEffectiveOn" 
                runat="server" 
                Text="Effective on:"
                IsRequired="true"
                ToolTip="The date the tax rate is in effect."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox 
                ID="txtEffectiveOn" 
                runat="server" 
                CausesValidation="true"
                MaxLength="10" >
            </asp:TextBox>
            <asp:ImageButton ID="iEffectiveOn" runat="Server" ImageUrl="~/Administration/images/ico-calendar.png" AlternateText="Click to show calendar" /><br />
            <ajaxToolkit:CalendarExtender ID="cEffectiveOnButtonExtender" runat="server" TargetControlID="txtEffectiveOn" PopupButtonID="iEffectiveOn" />
        </td>
    </tr>
</table>