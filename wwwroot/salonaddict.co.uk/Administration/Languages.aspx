<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Languages.aspx.cs" Inherits="SalonAddict.Administration.Languages" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-configuration.png" alt="Configuration" />
        Languages
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new language" OnClientClick="location.href='LanguageCreate.aspx';return false" />
    </div>
</div>
<asp:GridView 
    ID="gvLanguages" 
    runat="server" 
    Width="100%"
    DataKeyNames="LanguageID"
    AutoGenerateColumns="False" >
    <Columns>
        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <a href="LanguageDetails.aspx?LanguageID=<%#Eval("LanguageID")%>" title="Edit language">
                    <%#Server.HtmlEncode(Eval("Name").ToString())%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Language Culture">
            <ItemTemplate>
               <%# Eval("LanguageCulture")%>
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
        <asp:TemplateField HeaderText="View String Resources">
            <ItemTemplate>
               <a href="LocaleStringResources.aspx?LanguageID=<%#Eval("LanguageID")%>" title="View locale string resources">
                   view string resources
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
                <a href="LanguageDetails.aspx?LanguageID=<%#Eval("LanguageID")%>" title="Edit country">
                    Edit
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
