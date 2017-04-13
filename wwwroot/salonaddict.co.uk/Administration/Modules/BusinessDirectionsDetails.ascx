<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessDirectionsDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.BusinessDirectionsDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>

<% if(this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="" />
        Add a new directions <a href='<%= "BusinessDirections.aspx?BusinessGUID=" + base.BusinessGUID %>' title="Back to directions list">(back to directions list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ToolTip="Add business" />
    </div>
</div>
<% } 
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="" />
        Edit direction details <a href='<%= "BusinessDirections.aspx?BusinessGUID=" + base.BusinessGUID %>' title="Back to directions list">(back to directions list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save directions" />
        <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES") || Roles.IsUserInRole("SALES_MGR"))
           { %>
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete directions" OnClientClick="return confirm('Are you sure?')" />
        <% } %>
    </div>
</div>
<% } %>
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblInstructions" 
                runat="server" 
                Text="Instructions:" 
                IsRequired="true"
                ToolTip="The directional instructions." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <FCKeditorV2:FCKeditor ID="txtInstructions" runat="server" ToolbarSet="Basic" Width="680px" ></FCKeditorV2:FCKeditor>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server" 
                Text="Display order:" ToolTip="The display order for the directions. 1 represents the top of the list." 
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
</table>