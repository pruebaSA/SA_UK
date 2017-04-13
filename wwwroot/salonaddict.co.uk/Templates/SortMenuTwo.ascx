<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SortMenuTwo.ascx.cs" EnableViewState="false" Inherits="SalonAddict.Templates.SortMenuTwo" %>
<div class="tempate-sortmenutwo" >
<ul>
   <li>
      <%= base.GetLocalResourceObject("lblSortBy.Text") %>
   </li>
   <li>&nbsp;</li>
   <li>
      <asp:LinkButton ID="Item1" runat="server" meta:resourceKey="Item1" OnClick="Item_Click"></asp:LinkButton>
   </li>
   <li>|</li>
   <li>
      <asp:LinkButton ID="Item2" runat="server" meta:resourceKey="Item2" OnClick="Item_Click" ></asp:LinkButton>
   </li>
   <li>|</li>
   <li>
      <asp:LinkButton ID="Item3" runat="server" meta:resourceKey="Item3" OnClick="Item_Click" ></asp:LinkButton>
   </li>
   <li>|</li>
   <li>
      <asp:LinkButton ID="Item4" runat="server" meta:resourceKey="Item4" OnClick="Item_Click" ></asp:LinkButton>
   </li>
   <li>|</li>
   <li>
      <asp:LinkButton ID="Item5" runat="server" meta:resourceKey="Item5" OnClick="Item_Click" ></asp:LinkButton>
   </li>
</ul>
</div>