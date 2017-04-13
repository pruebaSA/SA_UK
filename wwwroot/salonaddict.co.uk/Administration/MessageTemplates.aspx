<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="MessageTemplates.aspx.cs" Inherits="SalonAddict.Administration.MessageTemplates" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-content.png" alt="Topics" />
        Manage Message Templates
    </div>
    <div class="options">
        <asp:Button ID="btnAddNew" runat="server" Text="Add new" ToolTip="Add a new template" OnClientClick="location.href='MessageTemplateCreate.aspx';return false;" />
    </div>
</div>
<table class="details" >
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblLanguage"
                runat="server"  
                Text="Select language:"
                IsRequired="true"
                ToolTip="Select a language for the template. A template can be created for each language that your store supports."
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
    ID="gvTemplates" 
    runat="server" 
    AutoGenerateColumns="false" 
    DataKeyNames="MessageTemplateID" >
    <Columns>
       <asp:TemplateField HeaderText="Language" >
          <ItemTemplate>   
             <a href="LanguageDetails.aspx?LanguageID=<%# ddlLanguage.SelectedValue %>" >
                 <%# ddlLanguage.SelectedItem.Text %>
             </a>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Name" >
          <ItemTemplate>
             <%# Eval("Name")%>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Edit info" >
          <ItemTemplate>
                <a href="MessageTemplateDetails.aspx?MessageTemplateID=<%# Eval("MessageTemplateID")%>" title="Edit template info">
                    Edit
                </a>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="" >
          <ItemTemplate>
                <a href="MessageTemplateLocalizedDetails.aspx?MessageTemplateID=<%# Eval("MessageTemplateID")%>&LanguageID=<%# ddlLanguage.SelectedValue %>" title="Edit template content">
                    Edit template content
                </a>
          </ItemTemplate>
       </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
