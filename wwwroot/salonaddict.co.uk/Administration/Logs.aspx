<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Logs.aspx.cs" Inherits="SalonAddict.Administration.Logs" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-system.png" alt="System" />
        System Log
    </div>
    <div class="options">
        <asp:Button ID="btnClearLog" runat="server" Text="Clear Log" ToolTip="Clear system log" onclick="ClearLog_Click" OnClientClick="return confirm('Are you sure?')" />
    </div>
</div>
<br />
<asp:GridView 
    ID="gvLogs" 
    runat="server" 
    Width="100%"
    PageSize="15"
    AllowPaging="true"
    DataKeyNames="LogID"
    OnRowCreated="gvLogs_OnRowCreated"
    OnRowDeleting="gvLogs_OnRowDeleting"
    OnPageIndexChanging="gvLogs_OnPageIndexChanging"
    AutoGenerateColumns="False" >
    <Columns> 
        <asp:TemplateField HeaderText="Log type" ItemStyle-Width="90px" >
            <ItemTemplate>
                <%# Eval("LogType") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="User">
            <ItemTemplate>
               <asp:PlaceHolder ID="phUser" runat="server" ></asp:PlaceHolder>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Message" ItemStyle-Width="55%" >
            <ItemTemplate>
                <a href="LogDetails.aspx?LogID=<%# Eval("LogID") %>" >
                    <%# FormatExceptionDetails(Eval("Exception").ToString()) %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="CreatedOn" ItemStyle-Width="100px" >
            <ItemTemplate>
               <%# Eval("CreatedOn") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Delete" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure?')" ></asp:Button>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
