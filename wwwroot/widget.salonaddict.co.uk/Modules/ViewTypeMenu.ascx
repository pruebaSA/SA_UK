<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="ViewTypeMenu.ascx.cs" Inherits="IFRAME.Modules.ViewTypeMenu" %>
<div class="module-viewtypes" >
   <ul>
      <li>
         <asp:HyperLink ID="hlDayView" runat="server" meta:resourceKey="hlDayView" ></asp:HyperLink>
      </li>
      <li>
         <asp:HyperLink ID="hl3DayView" runat="server" meta:resourceKey="hl3DayView" ></asp:HyperLink>
      </li>
      <li>
         <asp:HyperLink ID="hlWeekView" runat="server" meta:resourceKey="hlWeekView" ></asp:HyperLink>
      </li>
   </ul>
</div>