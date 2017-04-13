<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Counties.aspx.cs" Inherits="SalonAddict.Administration.Counties" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="CountryDropDownList" Src="~/Administration/Modules/DropDownLists/CountryDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="StateProvinceDropDownList" Src="~/Administration/Modules/DropDownLists/StateProvinceDropDownList.ascx" %>

<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-configuration.png" alt="Configuration" />
        Counties
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new" ToolTip="Add a new county" onclick="AddButton_Click" />
    </div>
</div>
<table cellpadding="0" cellspacing="4" >
    <tr>
        <td class="title">
            <sa:tooltiplabel 
                ID="lblSelectCountry" 
                runat="server" 
                Text="Select country:"
                IsRequired="true"
                ToolTip="Select a country to view it's states/provinces." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:CountryDropDownList
                ID="ddlCountry"
                runat="server" 
                DefaultText="Choose a Country"
                AutoPostback="true"
                OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged"
                ErrorMessage="Country is a required field." />
        </td>
    </tr>
</table>
<br />
<table cellpadding="0" cellspacing="4" >
    <tr>
        <td class="title">
            <sa:tooltiplabel 
                ID="lblSelectStateProvince" 
                runat="server" 
                Text="Select state/province:"
                IsRequired="true"
                ToolTip="Select a state/province to view it's counties." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:StateProvinceDropDownList 
                ID="ddlStateProvince"
                runat="server" 
                AutoPostback="true"
                DefaultText="Choose a State/Province"
                ErrorMessage="State/Province is a required field." />
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gvCounties" 
    runat="server" 
    Width="100%"
    DataKeyNames="CountyID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="60%" >
            <ItemTemplate>
                <a href='<%# "CountyDetails.aspx" + Utilities.GetQueryString("CountyID", Eval("CountyID"), "StateProvinceID", Eval("StateProvinceID"), "CountryID", Eval("CountryID")) %>' title="Edit county" >
                    <%# Eval("Name").ToString().HtmlEncode() %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Abbreviation">
            <ItemTemplate>
               <%# Eval("Abbreviation").ToString().HtmlEncode() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display Order">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" ItemStyle-Width="10%" >
            <ItemTemplate>
                <a href='<%# "CountyDetails.aspx" + Utilities.GetQueryString("CountyID", Eval("CountyID"), "StateProvinceID", Eval("StateProvinceID"), "CountryID", Eval("CountryID")) %>' title="Edit county" >
                    Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>