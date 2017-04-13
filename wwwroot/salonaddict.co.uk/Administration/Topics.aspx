<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Topics.aspx.cs" Inherits="SalonAddict.Administration.Topics" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-content.png" alt="Topics" />
        Manage Topics
    </div>
    <div class="options">
        <asp:Button ID="btnAddNew" runat="server" Text="Add new" ToolTip="Add a new topic" OnClientClick="location.href='TopicCreate.aspx';return false;" />
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
                ToolTip="Select a language for the topic. A topic can be created for each language that your store supports."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-itemF">
            <asp:DropDownList ID="ddlLanguage" AutoPostBack="True" OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gvTopics" 
    runat="server" 
    AutoGenerateColumns="false" 
    DataKeyNames="TopicID" >
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
                <a href="TopicDetails.aspx?TopicID=<%# Eval("TopicID")%>" title="Edit topic info">
                    Edit topic info
                </a>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="" >
          <ItemTemplate>
                <a href="TopicLocalizedDetails.aspx?TopicID=<%# Eval("TopicID")%>&LanguageID=<%# ddlLanguage.SelectedValue %>" title="Edit topic content">
                    Edit topic content
                </a>
          </ItemTemplate>
       </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>