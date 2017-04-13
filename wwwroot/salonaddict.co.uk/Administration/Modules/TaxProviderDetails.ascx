<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaxProviderDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.TaxProviderDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<% if (this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Add a new tax provider <a href="TaxProviders.aspx" title="Back to tax providers list"> (back to tax providers list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save tax provider" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit tax provider details <a href="TaxProviders.aspx" title="Back to tax providers list">(back to tax providers list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save tax provider" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete tax provider" OnClientClick="return confirm('Are you sure?')" />
    </div>
</div>
<% } %>
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblName" 
                runat="server"
                Text="Name:" 
                IsRequired="true"
                ToolTip="The tax provider name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox  
                ID="txtName"  
                runat="server"
                ErrorMessage="Name is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDescription" 
                runat="server" 
                Text="Description:"
                ToolTip="A description of the tax provider." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox 
                ID="txtDescription" 
                runat="server" 
                TextMode="MultiLine"
                Height="100">
            </asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblConfigureTemplatePath"
                runat="server"  
                Text="Configuration template path:"
                ToolTip="The path to the configuration template for this provider." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtConfigureTemplatePath" runat="server" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblClassName"
                runat="server"  
                Text="Class name:" 
                IsRequired="true"
                ToolTip="The fully qualified class name for this tax provider."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtClassName"
                runat="server"
                ErrorMessage="Class name is required">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server"
                Text="Display order:"
                ToolTip="The display order of this tax provider. 1 represents the top of the list."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:NumericTextBox 
                ID="txtDisplayOrder" 
                runat="server"
                Value="1" 
                RequiredErrorMessage="Display order is required" 
                MinimumValue="1"
                MaximumValue="99999"
                RangeErrorMessage="The value must be from 1 to 99999">
            </SA:NumericTextBox>
        </td>
    </tr>
</table>