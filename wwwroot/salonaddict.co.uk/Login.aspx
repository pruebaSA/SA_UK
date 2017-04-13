<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SalonAddict.Login" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <SA:Topic ID="ContentTopic" runat="server" meta:resourceKey="ContentTopic" />
    <SA:Message ID="lblMessage" runat="server" IsError="true" />
    <asp:Panel ID="pnlLogin" runat="server" Width="380px" Height="150px" CssClass="grey-panel" DefaultButton="btn" >
        <h2><%= base.GetLocalResourceObject("lblHeader.Text") %></h2>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" >
                 <%= base.GetLocalResourceObject("lblUsername.Text") %>
              </td>
              <td class="data-item" >
                 <SA:TextBox 
                    ID="txtUsername" 
                    runat="server" 
                    Width="250px"
                    MaxLength="50"
                    EnableViewState="false"
                    ValidationGroup="login_form" 
                    meta:resourceKey="txtUsername" />
              </td>
          </tr>
          <tr>
              <td class="title" >
                 <%= base.GetLocalResourceObject("lblPassword.Text") %>
              </td>
              <td class="data-item" >
                 <SA:TextBox 
                    ID="txtPassword" 
                    runat="server" 
                    MaxLength="50" 
                    TextMode="Password"
                    Width="250px"
                    EnableViewState="false"
                    ValidationGroup="login_form"
                    meta:resourceKey="txtPassword" />
              </td>
          </tr>
        </table>
        <div style="text-align:right;padding-top:10px;" >
           <asp:Button ID="btn" runat="server" ValidationGroup="login_form" SkinID="BlackButtonSmall" OnClick="btn_Click" meta:resourceKey="btn" />
        </div>
    </asp:Panel>
</asp:Content>