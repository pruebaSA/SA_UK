<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Areas.aspx.cs" Inherits="SalonAddict.Administration.Areas" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="CountyDropDownList" Src="~/Administration/Modules/DropDownLists/CountyDropDownList.ascx" %>

<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-configuration.png" alt="Configuration" />
        Areas
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new" ToolTip="Add a new area" onclick="AddButton_Click" />
    </div>
</div>
<table cellpadding="0" cellspacing="4" >
    <tr>
        <td class="title">
            <sa:tooltiplabel 
                ID="lblSelectCounty" 
                runat="server" 
                Text="Select county:"
                IsRequired="true"
                ToolTip="Select a county to view it's areas." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:CountyDropDownList
                ID="ddlCounty"
                runat="server" 
                DefaultText="Choose a County"
                AutoPostback="true"
                OnSelectedIndexChanged="ddlCounty_SelectedIndexChanged"
                ErrorMessage="County is a required field." />
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gvAreas" 
    runat="server" 
    Width="100%"
    DataKeyNames="AreaID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="60%" >
            <ItemTemplate>
                <a href='<%# "AreaDetails.aspx" + Utilities.GetQueryString("AreaID", Eval("AreaID"), "CountyID", Eval("CountyID")) %>' title="Edit area" >
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
                <a href='<%# "AreaDetails.aspx" + Utilities.GetQueryString("AreaID", Eval("AreaID"), "CountyID", Eval("CountyID")) %>' title="Edit area" >
                    Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
