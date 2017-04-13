<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateTextBox.ascx.cs" Inherits="IFRAME.Modules.DateTextBox" %>
<table cellpadding="0" cellspacing="0" >
   <tr>
      <td style="vertical-align:middle" >
         <asp:TextBox ID="txtValue" runat="server" SkinID="DateTextBox" MaxLength="12" />
      </td>
      <td style="padding-left:10px" >
         <asp:ImageButton ID="ibValue" runat="server" SkinID="CalendarImageButton" CausesValidation="false" meta:resourceKey="ibValue" />
      </td>
   </tr>
   <tr>
      <td colspan="2" >
        <div>
           <ajaxToolkit:CalendarExtender ID="txtValueEX" runat="server" TargetControlID="txtValue" />
           <ajaxToolkit:CalendarExtender ID="ibValueEx" runat="server" TargetControlID="txtValue" PopupButtonID="ibValue" />
        </div>
      </td>
   </tr>
</table>