<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="ServiceCategories.aspx.cs" Inherits="SalonAddict.Administration.ServiceCategories" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.Implementation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-catalog.png" alt="Configuration" />
        Service Categories
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new service category" />
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
                ToolTip="Select a language for the service. A service category can be created for each language that your store supports."
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
    ID="gvServiceCategories" 
    runat="server" 
    DataKeyNames="ServiceCategoryID"
    AutoGenerateColumns="false" onrowcreated="gvServiceCategories_RowCreated"  >
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
            <a href="ServiceCategoryDetails.aspx?ServiceCategoryID=<%# Eval("ServiceCategoryID")%>&LanguageID=<%# ddlLanguage.SelectedValue %>" >
               <%# Eval("Name") %>
            </a>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Service Type" >
          <ItemTemplate>
            <asp:PlaceHolder ID="phServiceType" runat="server" ></asp:PlaceHolder>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Published" >
          <ItemTemplate>
            <%# Eval("Published") %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Display Order" >
          <ItemTemplate>
            <%# Eval("DisplayOrder") %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Edit (info)" >
          <ItemTemplate>
                <a href="ServiceCategoryDetails.aspx?ServiceCategoryID=<%# Eval("ServiceCategoryID")%>&LanguageID=<%# ddlLanguage.SelectedValue %>" >
                    Edit
                </a>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Edit (content)" >
          <ItemTemplate>
                <a href="ServiceCategoryLocalizedDetails.aspx?ServiceCategoryID=<%# Eval("ServiceCategoryID")%>&LanguageID=<%# ddlLanguage.SelectedValue %>" >
                    Edit
                </a>
          </ItemTemplate>
       </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
