<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomerRoleDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.CustomerRoleDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<% if (this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="" />
        Add a new customer role <a href="CustomerRoles.aspx" title="Back to customer role list"> (back to customer role list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save customer role" />
    </div>
</div>
<% }
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit customer role details <a href="CustomerRoles.aspx" title="Back to customer roles list">(back to customer roles list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save customer roles" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete customer role" OnClientClick="return confirm('Are you sure?')" />
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
                ToolTip="The customer role name."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox  
                ID="txtName"  
                runat="server"
                MaxLength="100"
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
                ToolTip="A description of the customer role." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtDescription" runat="server" MaxLength="400" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSystemKeyword"
                runat="server"  
                Text="System keyword:"
                IsRequired="true"
                ToolTip="The customer role key used by the system." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtSystemKeyword" runat="server" MaxLength="100" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblActive"
                runat="server"  
                Text="Active:"
                ToolTip="Determines whether this role is active." 
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