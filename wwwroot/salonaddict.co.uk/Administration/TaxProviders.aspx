<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TaxProviders.aspx.cs" Inherits="SalonAddict.Administration.TaxProviders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Sales" />
        Manage Tax Providers
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new tax provider" OnClientClick="location.href='TaxProviderCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gvTaxProviders" 
    runat="server" 
    Width="100%"
    DataKeyNames="TaxProviderID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="30%" >
            <ItemTemplate>
                <a href="TaxProviderDetails.aspx?TaxProviderID=<%# Eval("TaxProviderID")  %>" ><%# Eval("Name") %></a>
            </ItemTemplate>
            <ItemStyle Width="60%" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display order">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Is Default">
            <ItemTemplate>
               <asp:RadioButton runat="server" ID="rbIsDefault" OnCheckedChanged="rdbIsDefault_CheckedChanged" AutoPostBack="true" ToolTip="Click to make this tax provider the default" />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
               <a href="TaxProviderDetails.aspx?TaxProviderID=<%# Eval("TaxProviderID")  %>" >Edit</a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
