<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeSearchDateOne.ascx.cs" Inherits="SalonAddict.Templates.ChangeSearchDateOne" %>
<div class="template-changesearchdateone" >
   <ul>
      <li>
         <asp:LinkButton ID="lbPreviousWeek" runat="server" meta:resourceKey="lbPreviousWeek" OnClick="lbPreviousWeek_Click" ></asp:LinkButton>
      </li>
      <li>
         <asp:LinkButton ID="lbPreviousDay" runat="server" meta:resourceKey="lbPreviousDay" OnClick="lbPreviousDay_Click" ></asp:LinkButton>
      </li>
      <li><span>|</span></li>
      <li>
         <asp:LinkButton ID="lbNextDay" runat="server" meta:resourceKey="lbNextDay" OnClick="lbNextDay_Click" ></asp:LinkButton>
      </li>
      <li>
         <asp:LinkButton ID="lbNextWeek" runat="server" meta:resourceKey="lbNextWeek" OnClick="lbNextWeek_Click" ></asp:LinkButton>
      </li>
   </ul>
</div>