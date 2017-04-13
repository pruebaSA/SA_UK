<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="SMSProviders.aspx.cs" Inherits="SalonAddict.Administration.SMSProviders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Sales" />
        Manage SMS Providers
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new SMS provider" OnClientClick="location.href='SMSProviderCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gv" 
    runat="server" 
    Width="100%"
    DataKeyNames="SMSProviderID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="30%" >
            <ItemTemplate>
                <a href="SMSProviderDetails.aspx?SMSProviderID=<%# Eval("SMSProviderID")  %>" ><%# Eval("Name") %></a>
            </ItemTemplate>
            <ItemStyle Width="60%" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="System Keyword" >
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
               <%# Eval("Active")%>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
               <a href="SMSProviderDetails.aspx?SMSProviderID=<%# Eval("SMSProviderID")  %>" >Edit</a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
