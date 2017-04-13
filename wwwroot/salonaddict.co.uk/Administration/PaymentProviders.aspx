<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="PaymentProviders.aspx.cs" Inherits="SalonAddict.Administration.PaymentProviders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Sales" />
        Manage Payment Providers
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new payment provider" OnClientClick="location.href='PaymentProviderCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gvPaymentProviders" 
    runat="server" 
    Width="100%"
    DataKeyNames="PaymentProviderID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="30%" >
            <ItemTemplate>
                <a href="PaymentProviderDetails.aspx?PaymentProviderID=<%# Eval("PaymentProviderID")  %>" ><%# Eval("Name") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Visible Name" ItemStyle-Width="30%" >
            <ItemTemplate>
                <a href="PaymentProviderDetails.aspx?PaymentProviderID=<%# Eval("PaymentProviderID")  %>" ><%# Eval("VisibleName") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display order">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Active">
            <ItemTemplate>
               <%# Eval("Active") %>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
                <a href="PaymentProviderDetails.aspx?PaymentProviderID=<%# Eval("PaymentProviderID")  %>" >
                   Edit
                </a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
