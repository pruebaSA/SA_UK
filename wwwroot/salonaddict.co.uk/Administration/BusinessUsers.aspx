<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessUsers.aspx.cs" Inherits="SalonAddict.Administration.BusinessUsers" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="Customers" />
        Manage Business Users
    </div>
    <div class="options">
        <asp:Button ID="btnNew" runat="server" Text="Add new"  ToolTip="Add a new business user" OnClick="btnNew_Click" />
    </div>
</div>
<p>
  <asp:DropDownList ID="ddlBusiness" runat="server" OnSelectedIndexChanged="ddlBusiness_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList>
</p>
<asp:GridView 
    ID="gvBusinessUsers" 
    runat="server" 
    DataKeyNames="BusinessUserID"
    AutoGenerateColumns="False"
    Width="100%"
    AllowPaging="true"
    OnRowCreated="gvBusinessUsers_RowCreated"
    PageSize="25" >
    <Columns>
        <asp:TemplateField HeaderText="Business" >
            <ItemTemplate>
                <asp:PlaceHolder ID="phBusiness" runat="server" ></asp:PlaceHolder>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Name" >
            <ItemTemplate>
                 <%#Server.HtmlEncode(Eval("FullName").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Username" >
            <ItemTemplate>
                <a href="BusinessUserDetails.aspx?BusinessUserGUID=<%#Eval("BusinessUserGUID")%>" title="Edit user details">
                    <%# Server.HtmlEncode(Eval("Username").ToString()) %>
                </a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
                <asp:TemplateField HeaderText="Send Email Notifications" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center"  >
            <ItemTemplate>
                <%# Eval("SendEmailNotifications") %>
            </ItemTemplate>
        </asp:TemplateField>
         <asp:TemplateField HeaderText="Send SMS Notifications" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center"  >
            <ItemTemplate>
                <%# Eval("SendSMSNotifications") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Is Admin" >
            <ItemTemplate>
                <%# Eval("IsAdmin") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Registration date" >
            <ItemTemplate>
                <%# ((DateTime)Eval("CreatedOn")).ToString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" ItemStyle-Width="60px" >
            <ItemTemplate>
                <a href="BusinessUserDetails.aspx?BusinessUserGUID=<%#Eval("BusinessUserGUID")%>" title="Edit user details">
                    Edit 
               </a>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
