<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LocalityDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.LocalityDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="LocalityDropDownList" Src="~/Administration/Modules/DropDownLists/LocalityDropDownList.ascx" %>

<div class="section-header">
<% if(this.Action == ActionType.Edit)
   { %>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Edit locality details <a href="Localities.aspx" title="Back to locality list">(back to locality list)</a>
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
        Add a new locality <a href="Localities.aspx" title="Back to locality list">(back to locality list)</a>
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
                ID="lblParentLocality" 
                runat="server" 
                Text="Parent Locality:" 
                ToolTip="The parent locality." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
           <SA:LocalityDropDownList 
                ID="ddlParentLocality" 
                runat="server" 
                IsRequired="false" 
                DefaultText="Choose a Locality"
                DefaultValue=""
                ErrorMessage="Parent locality is a required field." />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblName" 
                runat="server" 
                Text="Name:" 
                IsRequired="true"
                ToolTip="The name of the locality." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtName" 
                runat="server" 
                MaxLength="50"
                ErrorMessage="locality name is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblSEName" 
                runat="server" 
                Text="SEName:" 
                IsRequired="false"
                ToolTip="The search-engine friendly name." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtSEName" 
                runat="server" 
                MaxLength="50"
                IsRequired="false"
                ErrorMessage="SEName is a required field.">
            </SA:TextBox>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblMetaName" 
                runat="server" 
                Text="Meta Name:" 
                IsRequired="false"
                ToolTip="The meta-friendly name." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:TextBox 
                ID="txtMetaTitle" 
                runat="server" 
                MaxLength="50"
                IsRequired="false"
                ErrorMessage="Meta Name is a required field.">
            </SA:TextBox>
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
                MaxLength="5"
                RequiredErrorMessage="Display order is required" 
                RangeErrorMessage="The value must be from 1 to 99999"
                MinimumValue="1" 
                MaximumValue="99999">
             </SA:NumericTextBox>
        </td>
    </tr>
</table>