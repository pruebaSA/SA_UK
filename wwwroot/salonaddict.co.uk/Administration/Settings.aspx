<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="SalonAddict.Administration.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-configuration.png" alt="Configuration" />
        All Settings
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new setting" OnClientClick="location.href='SettingCreate.aspx';return false" />
    </div>
</div>
<asp:GridView 
    ID="gvSetting" 
    runat="server" 
    Width="100%"
    PageSize="15"
    AllowPaging="true"
    DataKeyNames="SettingID"
    OnPageIndexChanging="gvSetting_OnPageIndexChanging"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Name" >
            <ItemTemplate>
                <a href="SettingDetails.aspx?SettingID=<%# Eval("SettingID")  %>" ><%# Eval("Key") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Value" ItemStyle-Wrap="true">
            <ItemTemplate>
               <%# Server.HtmlEncode(Eval("Value").ToString()) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
               <a href="SettingDetails.aspx?SettingID=<%# Eval("SettingID")  %>" >Edit</a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
