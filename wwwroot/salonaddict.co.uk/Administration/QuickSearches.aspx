<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="QuickSearches.aspx.cs" Inherits="SalonAddict.Administration.QuickSearches" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="LangaugeDropDownList" Src="~/Administration/Modules/DropDownLists/LanguageDropDownList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-content.png" alt="Quick Searches" />
        Manage Quick Searches
    </div>
    <div class="options">
        <asp:Button ID="btnAddNew" runat="server" Text="Add new" ToolTip="Add a new quick search" OnClientClick="location.href='QuickSearchCreate.aspx';return false;" />
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
                ToolTip="Select a language for the quick search. A quick search can be created for each language that your store supports."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <SA:LangaugeDropDownList 
                ID="ddlLanguage" 
                runat="server"
                AutoPostback="true"
                OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged" />
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gv" 
    runat="server" 
    AutoGenerateColumns="false" 
    DataKeyNames="QuickSearchID" >
    <Columns>
       <asp:TemplateField HeaderText="Language" ItemStyle-Width="100px" >
          <ItemTemplate>   
             <a href="LanguageDetails.aspx?LanguageID=<%# ddlLanguage.SelectedValue %>" >
                <%= ddlLanguage.SelectedText %>
             </a>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Title" >
          <ItemTemplate>
             <%# Eval("Title")%>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Category" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" >
          <ItemTemplate>
             <%# Eval("QuickSearchCategoryType")%>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Display Order" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" >
          <ItemTemplate>
             <%# Eval("DisplayOrder")%>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="" ItemStyle-Width="60px" >
          <ItemTemplate>
                <a href="QuickSearchDetails.aspx?QuickSearchID=<%# Eval("QuickSearchID")%>&LanguageID=<%# ddlLanguage.SelectedValue %>" title="Edit quick search">
                    Edit
                </a>
          </ItemTemplate>
       </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
