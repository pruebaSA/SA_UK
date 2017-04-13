<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="PasswordRecovery.aspx.cs" Inherits="SalonPortal.PasswordRecovery" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/SecureArea/Modules/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="OneColumnContentPlaceHolder" runat="server">
    <SA:Topic ID="Topic" runat="server" meta:resourceKey="Topic" />
    <SA:Message ID="lblMessage" runat="server" />
    <table class="details" >
       <tr>
         <td class="details" ></td>
         <td class="data-item" >
            <asp:Image ID="CaptchaImage" runat="server" ImageUrl="~/CaptchaImage.ashx" BorderColor="#cccccc" BorderWidth="1px" BorderStyle="Solid" />
         </td>
      </tr>
       <tr>
          <td class="title" >
            <%= base.GetLocalResourceObject("lblSecurityCode.Text").ToString() %>
          </td>
          <td class="data-item" >
             <SA:TextBox ID="txtSecurityCode" runat="server" MaxLength="50" meta:resourceKey="txtSecurityCode" />
          </td>
       </tr>
       <tr>
          <td class="title" >
            <%= base.GetLocalResourceObject("lblUsername.Text").ToString() %>
          </td>
          <td class="data-item" >
             <SA:TextBox ID="txtUsername" runat="server" MaxLength="50" meta:resourceKey="txtUsername" />
          </td>
       </tr>
      <tr>
         <td class="details" >
          
         </td>
         <td class="data-item">
            <asp:Button ID="btnSubmit" runat="server" meta:resourceKey="btnSubmit" OnClick="btnSubmit_Click" />
         </td>
      </tr>
    </table>
</asp:Content>
