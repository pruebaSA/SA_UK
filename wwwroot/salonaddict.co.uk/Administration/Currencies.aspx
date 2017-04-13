<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Currencies.aspx.cs" Inherits="SalonAddict.Administration.Currencies" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-configuration.png" alt="Configuration" />
        Currencies
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new currency" OnClientClick="location.href='CurrencyCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gvCurrency" 
    runat="server" 
    Width="100%"
    DataKeyNames="CurrencyID"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" ItemStyle-Width="30%" >
            <ItemTemplate>
                <a href="CurrencyDetails.aspx?CurrencyID=<%# Eval("CurrencyID")  %>" ><%# Eval("Name") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Currency code">
            <ItemTemplate>
               <%# Eval("CurrencyCode") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display locale">
            <ItemTemplate>
               <%# Eval("DisplayLocale")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Rate">
            <ItemTemplate>
               <%# Eval("Rate")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Published">
            <ItemTemplate>
               <%# Eval("Published")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display order">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Primary exchange rate currency" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="14%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:RadioButton runat="server" ID="rbIsPrimaryExchangeRateCurrency" Checked='<%#Eval("IsPrimaryExchangeRateCurrency")%>'
                    OnCheckedChanged="rdbIsPrimaryExchangeRateCurrency_CheckedChanged" AutoPostBack="true"
                    ToolTip="Set as primary exchange rate currency" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Primary store currency" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="14%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:RadioButton runat="server" ID="rbIsPrimaryStoreCurrency" Checked='<%#Eval("IsPrimaryCurrency")%>'
                    OnCheckedChanged="rdbIsPrimaryStoreCurrency_CheckedChanged" AutoPostBack="true"
                    ToolTip="Set as primary store currency" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
               <a href="CurrencyDetails.aspx?CurrencyID=<%# Eval("CurrencyID")  %>" >Edit</a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>

