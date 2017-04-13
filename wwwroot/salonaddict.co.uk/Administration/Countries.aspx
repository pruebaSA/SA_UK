<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Countries.aspx.cs" Inherits="SalonAddict.Administration.Countries" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-configuration.png" alt="Configuration" />
        Countries
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new country" OnClientClick="location.href='CountryCreate.aspx';return false" />
    </div>
</div>
<asp:GridView 
    ID="gvCountries" 
    runat="server" 
    Width="100%"
    DataKeyNames="CountryID"
    AutoGenerateColumns="False" >
    <Columns>
        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <a href="CountryDetails.aspx?CountryID=<%#Eval("CountryID")%>" title="Edit country">
                    <%#Server.HtmlEncode(Eval("Name").ToString())%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Two letter ISO Code">
            <ItemTemplate>
               <%# Eval("TwoLetterISOCode")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Three letter ISO Code">
            <ItemTemplate>
               <%# Eval("ThreeLetterISOCode")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Numeric ISO Code">
            <ItemTemplate>
               <%# Eval("NumericISOCode")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Published">
            <ItemTemplate>
               <%# Eval("Published")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Display Order">
            <ItemTemplate>
               <%# Eval("DisplayOrder")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
                <a href="CountryDetails.aspx?CountryID=<%#Eval("CountryID")%>" title="Edit country">
                    Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
