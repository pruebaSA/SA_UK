<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessLocalityDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.BusinessLocalityDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="LocalityDropDownList" Src="~/Administration/Modules/DropDownLists/LocalityDropDownList.ascx" %>

<div class="section-header">
<% if(this.Action == ActionType.Edit)
   { %>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Edit locality details <a href='<%= "BusinessLocalities.aspx?BusinessGUID=" + base.BusinessGUID %>' title="Back to locality list">(back to locality list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save locality" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete locality" OnClientClick="return confirm('Are you sure?')" />
    </div>
<% } 
   else
   {%>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Add a new locality <a href='<%= "BusinessLocalities.aspx?BusinessGUID=" + base.BusinessGUID %>' title="Back to locality list">(back to locality list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save locality" />
    </div>
<% } %>
</div>
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblLocality" 
                runat="server" 
                Text="Locality:" 
                ToolTip="The locality." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
           <SA:LocalityDropDownList 
                ID="ddlLocality" 
                runat="server" 
                IsRequired="true" 
                DefaultText="Choose a Locality"
                DefaultValue=""
                ErrorMessage="Locality is a required field." />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblDisplayOrder" 
                runat="server" 
                Text="Display order:" ToolTip="The display order for this locality. 1 represents the top of the list." 
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