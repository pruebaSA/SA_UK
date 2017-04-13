<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessLocalities.aspx.cs" Inherits="SalonAddict.Administration.BusinessLocalitiesPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="Businesses" />
        Manage Business Localities <asp:HyperLink ID="hlBack" runat="server" ></asp:HyperLink>
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new locality" OnClick="btnNew_Click" />
    </div>
</div>
<br />
<asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" >
   <Columns>
      <asp:TemplateField HeaderText="Locality" >
         <ItemTemplate>
            <%# GetLocalityFullName((SalonAddict.BusinessAccess.ModelClasses.LocalityBusinessMapping)Container.DataItem)%>
         </ItemTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Display Order" >
         <ItemTemplate>
             <%# Eval("DisplayOrder") %>
         </ItemTemplate>
         <ItemStyle Width="80px" />
      </asp:TemplateField>
      <asp:TemplateField HeaderText="" >
         <ItemTemplate>
             <a href='<%# Page.ResolveUrl("~/Administration/") + "BusinessLocalityDetails.aspx?BusinessGUID=" + this.BusinessGUID + "&LocalityBusinessMappingID=" + Eval("LocalityBusinessMappingID") %>' >
                 Edit
             </a>
         </ItemTemplate>
         <ItemStyle Width="50px" />
      </asp:TemplateField>
   </Columns>
</asp:GridView>
</asp:Content>
