<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentProviderDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.PaymentProviderDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<% if (this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Add a new payment provider <a href="PaymentProviders.aspx" title="Back to payment providers list"> (back to payment provider list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save payment provider" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit payment provider details <a href="PaymentProviders.aspx" title="Back to payment provider list">(back to payment provider list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save payment provider" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete payment provider" OnClientClick="return confirm('Are you sure?')" />
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
                ToolTip="The payment provider name."
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
                ID="lblVisibleName" 
                runat="server"
                Text="Visible Name:" 
                IsRequired="true"
                ToolTip="The payment provider visible name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtVisibleName"
                runat="server"
                MaxLength="50"
                ErrorMessage="Visible name is required">
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
                ID="lblUserTemplatePath"
                runat="server"  
                Text="User template path:"
                ToolTip="The path to the user template for this provider." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtUserTemplatePath" runat="server" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblClassName"
                runat="server"  
                Text="Class name:" 
                IsRequired="true"
                ToolTip="The fully qualified class name for this payment provider."
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
                ID="lblSystemKeyword"
                runat="server"  
                Text="System Keyword:" 
                IsRequired="true"
                ToolTip="The system name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtSystemKeyword"
                runat="server"
                ErrorMessage="System keyword is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblActive"
                runat="server"  
                Text="Active:" 
                ToolTip="Value indicating whether the provider is active."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:CheckBox ID="cbActive" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server"
                Text="Display order:"
                ToolTip="The display order of this payment provider. 1 represents the top of the list."
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