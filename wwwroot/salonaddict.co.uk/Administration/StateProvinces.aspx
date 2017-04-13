<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="StateProvinces.aspx.cs" Inherits="SalonAddict.Administration.StateProvinces" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="CountryDropDownList" Src="~/Administration/Modules/DropDownLists/CountryDropDownList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-configuration.png" alt="Configuration" />
        States / Provinces
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new" ToolTip="Add a new state/province" onclick="AddButton_Click" />
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
<asp:GridView 
    ID="gvStateProvinces" 
    runat="server" 
    Width="100%"
    DataKeyNames="StateProvinceID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="60%" >
            <ItemTemplate>
                <a href="StateProvinceDetails.aspx?StateProvinceID=<%#Eval("StateProvinceID")%>&CountryID=<%# Eval("CountryID") %>" title="Edit state/province">
                    <%#Server.HtmlEncode(Eval("Name").ToString())%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Abbreviation">
            <ItemTemplate>
               <%# Eval("Abbreviation")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display Order">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" ItemStyle-Width="10%" >
            <ItemTemplate>
                <a href="StateProvinceDetails.aspx?StateProvinceID=<%#Eval("StateProvinceID")%>&CountryID=<%# Eval("CountryID") %>" title="Edit state/province">
                    Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
