<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="CustomerRoles.aspx.cs" Inherits="SalonAddict.Administration.CustomerRoles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="Customer" />
        Manage Customer Roles
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new customer role" OnClientClick="location.href='CustomerRoleCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gvCustomerRoles" 
    runat="server" 
    Width="100%"
    DataKeyNames="CustomerRoleID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="30%" >
            <ItemTemplate>
                <a href="CustomerRoleDetails.aspx?CustomerRoleID=<%# Eval("CustomerRoleID")  %>" ><%# Eval("Name") %></a>
            </ItemTemplate>
            <ItemStyle Width="60%" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="System Keyword">
            <ItemTemplate>
               <%# Eval("SystemKeyword")%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display Order">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Active">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
               <a href="CustomerRoleDetails.aspx?CustomerRoleID=<%# Eval("CustomerRoleID")  %>" >Edit</a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>