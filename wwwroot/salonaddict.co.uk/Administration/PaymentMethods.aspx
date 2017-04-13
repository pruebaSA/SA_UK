<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="PaymentMethods.aspx.cs" Inherits="SalonAddict.Administration.PaymentMethods" Title="Manage Payment Methods" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Sales" />
        Manage Payment Methods
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new payment provider" OnClientClick="location.href='PaymentMethodCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gvPaymentMethods" 
    runat="server" 
    Width="100%"
    DataKeyNames="PaymentMethodID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="80px" >
            <ItemTemplate>
                <asp:Image ID="imgLogo" runat="server" ImageUrl='<%# SalonAddict.BusinessAccess.Implementation.MediaManager.GetPaymentMethodPictureURL((int)Eval("PictureID")) %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="30%" >
            <ItemTemplate>
                <a href="PaymentMethodDetails.aspx?PaymentMethodID=<%# Eval("PaymentMethodID")  %>" ><%# Eval("Name") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="System Keyword" ItemStyle-Width="30%" >
            <ItemTemplate>
                <%# Eval("SystemKeyword") %>
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
                <a href="PaymentMethodDetails.aspx?PaymentMethodID=<%# Eval("PaymentMethodID")  %>" >
                   Edit
                </a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>