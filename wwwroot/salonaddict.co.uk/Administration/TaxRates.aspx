<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="TaxRates.aspx.cs" Inherits="SalonAddict.Administration.TaxRates" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-sales.png" alt="Sales" />
        Manage Tax Rates
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new tax rate" OnClientClick="location.href='TaxRateCreate.aspx';return false" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gvTaxRates" 
    runat="server" 
    Width="100%"
    DataKeyNames="TaxRateID"
    OnRowCreated="gvTaxRates_RowCreated"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Tax Category" >
            <ItemTemplate>
                <asp:PlaceHolder ID="phTaxCategory" runat="server" ></asp:PlaceHolder>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Country">
            <ItemTemplate>
               <asp:PlaceHolder ID="phCountry" runat="server" ></asp:PlaceHolder>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="State/Province">
            <ItemTemplate>
               <asp:PlaceHolder ID="phStateProvince" runat="server" ></asp:PlaceHolder>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Percentage">
            <ItemTemplate>
               <%# Eval("Percentage") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
               <a href="TaxRateDetails.aspx?TaxRateID=<%# Eval("TaxRateID") %>" >Edit</a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
