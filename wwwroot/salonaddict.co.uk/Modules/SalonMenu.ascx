<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SalonMenu.ascx.cs" Inherits="SalonAddict.Modules.SalonMenu" %>
<asp:Panel ID="pnl" runat="server" CssClass="module-salonmenu1" >
   <ul>
       <li class="availability" >
          <asp:LinkButton ID="lb1" runat="server" OnClick="lb_Click" meta:resourceKey="lb1" ></asp:LinkButton>
       </li>
       <li class="directions" >
          <asp:LinkButton ID="lb2" runat="server" OnClick="lb_Click" meta:resourceKey="lb2" ></asp:LinkButton>
       </li>
       <li class="reviews" >
          <asp:LinkButton ID="lb3" runat="server" OnClick="lb_Click" meta:resourceKey="lb3" ></asp:LinkButton>
       </li>
   </ul>
</asp:Panel>