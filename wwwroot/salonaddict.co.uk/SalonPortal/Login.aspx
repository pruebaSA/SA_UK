<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SalonPortal.Login" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="OneColumnContentPlaceHolder" runat="server">
    <SA:Topic ID="cntlTopic" runat="server" />
    <asp:Panel ID="pnlLoginForm" runat="server" DefaultButton="btnLogin" >
       <table cellpadding="0" cellspacing="0" >
          <tr>
             <td>
                <table class="details" cellpadding="0" cellspacing="4" >
                   <tr>
                     <td class="title" >
                        <asp:Label ID="lblUsername" runat="server" meta:resourcekey="lblUsername" ></asp:Label>
                     </td>
                     <td class="data-item" >
                        <SA:TextBox 
                            ID="txtUsername" 
                            runat="server" 
                            MaxLength="50"
                            meta:resourcekey="txtUsername"  />
                     </td>
                   </tr>
                   <tr>
                      <td class="title" >
                         <asp:Label ID="lblPassword" runat="server" meta:resourcekey="lblPassword" ></asp:Label>
                      </td>
                      <td class="data-item" >
                         <SA:TextBox 
                            ID="txtPassword" 
                            runat="server" 
                            TextMode="Password" 
                            MaxLength="50"
                            meta:resourcekey="txtPassword"  />
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >&nbsp;</td>
                      <td class="data-item" >
                         <asp:CheckBox ID="cbStaySignedIn" runat="server" Checked="false" meta:resourcekey="cbStaySignedIn" />
                      </td>
                   </tr>
                   <tr>
                       <td class="title" >&nbsp;</td>
                       <td class="data-item" >
                         <asp:Button ID="btnLogin" runat="server" SkinID="ButtonLogin" Onclick="btnLogin_Click" meta:resourcekey="btnLogin" />
                      </td>
                   </tr>
                </table>
             </td>
             <td style="padding-left:30px;vertical-align:top;text-align:left;width:500px;">
                  <SA:Message ID="lblMessage" runat="server" IsError="true" />
             </td>
          </tr>
       </table>
    </asp:Panel>
</asp:Content>
