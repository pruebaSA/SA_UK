<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SMSTemplates.aspx.cs" Inherits="SalonAddict.Administration.SMSTemplates" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-content.png" alt="SMS Templates" />
        Manage SMS Templates
    </div>
    <div class="options">
        <asp:Button ID="btnAddNew" runat="server" Text="Add new" ToolTip="Add a new SMS Template" OnClientClick="location.href='SMSTemplateCreate.aspx';return false;" />
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
                ToolTip="Select a language for the SMS template. A template can be created for each language supported."
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
    ID="gv" 
    runat="server" 
    AutoGenerateColumns="false" 
    DataKeyNames="SMSTemplateID" >
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
                <a href="SMSTemplateDetails.aspx?SMSTemplateID=<%# Eval("SMSTemplateID")%>" title="Edit template info">
                    Edit
                </a>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="" >
          <ItemTemplate>
                <a href="SMSTemplateLocalizedDetails.aspx?SMSTemplateID=<%# Eval("SMSTemplateID")%>&LanguageID=<%# ddlLanguage.SelectedValue %>" title="Edit template content">
                    Edit template content
                </a>
          </ItemTemplate>
       </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
