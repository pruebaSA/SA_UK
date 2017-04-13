<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="LocaleStringResources.aspx.cs" Inherits="SalonAddict.Administration.LocaleStringResources" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.ModelClasses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-content.png" alt="Topics" />
        Localization
    </div>
    <div class="options">
        <asp:Button ID="btnAddNew" runat="server" Text="Add new" ToolTip="Add a new topic" OnClientClick="location.href='LocaleStringResourceCreate.aspx';return false;" />
    </div>
</div>
<table>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblLanguage"
                runat="server"  
                Text="Select language:"
                IsRequired="true"
                ToolTip="Select a language for the resource. A locale string resource can be created for each language that your store supports."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:DropDownList ID="ddlLanguage" AutoPostBack="True" OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gvLocalizations" 
    runat="server" 
    PageSize="25"
    AllowPaging="true"
    OnPageIndexChanging="gvLocalizations_PageIndexChanging"
    AutoGenerateColumns="false"  >
    <Columns>
       <asp:TemplateField HeaderText="Language" >
          <ItemTemplate>   
             <a href="LanguageDetails.aspx?LanguageID=<%# ddlLanguage.SelectedValue %>" >
                 <%# ddlLanguage.SelectedItem.Text %>
             </a>
          </ItemTemplate>
          <ItemStyle Width="100px" />
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Resource name" >
          <ItemTemplate>
            <%#  ((LocaleStringResource)Eval("Value")).ResourceName %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Resource value" >
          <ItemTemplate>
            <%#  ((LocaleStringResource)Eval("Value")).ResourceValue %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Edit" >
          <ItemTemplate>
                <a href="LocaleStringResourceDetails.aspx?LocaleStringResourceID=<%# ((LocaleStringResource)Eval("Value")).LocaleStringResourceID %>&LanguageID=<%# ddlLanguage.SelectedValue %>" >
                    Edit
                </a>
          </ItemTemplate>
          <ItemStyle Width="50px" />
       </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
