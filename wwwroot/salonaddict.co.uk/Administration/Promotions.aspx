<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Promotions.aspx.cs" Inherits="SalonAddict.Administration.Promotions" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="LangaugeDropDownList" Src="~/Administration/Modules/DropDownLists/LanguageDropDownList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-content.png" alt="Quick Searches" />
        Manage Promotions
    </div>
    <div class="options">
        <asp:Button ID="btnAddNew" runat="server" Text="Add new" ToolTip="Add a new promotion" OnClientClick="location.href='PromotionCreate.aspx';return false;" />
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
                ToolTip="Select a language for the promotion. A promotion can be created for each language that your store supports."
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
    OnRowCreated="gv_RowCreated"
    DataKeyNames="PromotionID" >
    <Columns>
       <asp:TemplateField HeaderText="Language" ItemStyle-Width="100px" >
          <ItemTemplate>   
             <a href="LanguageDetails.aspx?LanguageID=<%# ddlLanguage.SelectedValue %>" >
                <%= ddlLanguage.SelectedText %>
             </a>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Business" ItemStyle-Width="130px" >
          <ItemTemplate>   
              <asp:HyperLink ID="hlBusiness" runat="server" NavigateUrl="BusinessDetails.aspx?BusinessGUID={0}" Visible='<%# (int)Eval("BusinessID") > 0 %>' ></asp:HyperLink>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Title" >
          <ItemTemplate>
             <%# Eval("Title") %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Category" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center" >
          <ItemTemplate>
             <%# Eval("PromotionCategoryType") %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Special Offer" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" >
          <ItemTemplate>
             <%# Eval("IsSpecialOffer") %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Display Order" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" >
          <ItemTemplate>
             <%# Eval("DisplayOrder") %>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="Published" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center" >
          <ItemTemplate>
             <%# Eval("Published")%>
          </ItemTemplate>
       </asp:TemplateField>
       <asp:TemplateField HeaderText="" ItemStyle-Width="50px" >
          <ItemTemplate>
                <a href="PromotionDetails.aspx?PromotionID=<%# Eval("PromotionID")%>&LanguageID=<%# ddlLanguage.SelectedValue %>" title="Edit promotion">
                    Edit
                </a>
          </ItemTemplate>
       </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
