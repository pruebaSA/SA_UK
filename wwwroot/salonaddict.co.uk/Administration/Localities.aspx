<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Localities.aspx.cs" Inherits="SalonAddict.Administration.LocalitiesPage" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="CountyDropDownList" Src="~/Administration/Modules/DropDownLists/CountyDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="CityTownDropDownList" Src="~/Administration/Modules/DropDownLists/CityTownDropDownList.ascx" %>

<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-configuration.png" alt="Configuration" />
        Localities
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new" ToolTip="Add a new locality" onclick="AddButton_Click" />
    </div>
</div>
<table cellpadding="0" cellspacing="4" >
    <tr>
        <td class="title">
            <sa:tooltiplabel 
                ID="lblSelectCityTown" 
                runat="server" 
                Text="Select city/town:"
                IsRequired="false"
                ToolTip="Select a city/town." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:CityTownDropDownList 
                ID="ddlCityTown"
                runat="server" 
                AutoPostback="true"
                IsRequired="false"
                DefaultText="Choose a City/Town"
                OnSelectedIndexChanged="ddlCityTown_SelectedIndexChanged"
                ErrorMessage="City/town is a required field." />
        </td>
    </tr>
</table>
<br />
<br />
<asp:GridView 
    ID="gv" 
    runat="server" 
    Width="100%"
    DataKeyNames="LocalityID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" >
            <ItemTemplate>
                <a href='<%# "LocalityDetails.aspx" + Utilities.GetQueryString("LocalityID", Eval("LocalityID"), "CityTownID", Eval("CityTownID")) %>' title="Edit locality" >
                    <%# GetLocalityFullName((SalonAddict.BusinessAccess.ModelClasses.Locality)Container.DataItem) %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display Order">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" ItemStyle-Width="10%" >
            <ItemTemplate>
                <a href='<%# "LocalityDetails.aspx" + Utilities.GetQueryString("LocalityID", Eval("LocalityID"), "CityTownID", Eval("CityTownID")) %>' title="Edit locality" >
                    Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
