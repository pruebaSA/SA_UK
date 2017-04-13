<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TaxCategories.aspx.cs" Inherits="SalonAddict.Administration.TaxCategories" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Sales" />
        Manage Tax Categories
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new tax category" OnClientClick="location.href='TaxCategoryCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gvTaxCategories" 
    runat="server" 
    Width="100%"
    DataKeyNames="TaxCategoryID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="30%" >
            <ItemTemplate>
                <a href="TaxCategoryDetails.aspx?TaxCategoryID=<%# Eval("TaxCategoryID")  %>" ><%# Eval("Name") %></a>
            </ItemTemplate>
            <ItemStyle Width="60%" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display order">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
               <a href="TaxCategoryDetails.aspx?TaxCategoryID=<%# Eval("TaxCategoryID")  %>" >Edit</a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
