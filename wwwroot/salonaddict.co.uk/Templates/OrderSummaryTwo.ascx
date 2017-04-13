<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderSummaryTwo.ascx.cs" Inherits="SalonAddict.Templates.OrderSummaryTwo" %>
<%@ Import Namespace="SalonAddict.BusinessAccess.Implementation" %>

<% if (this.ShowOrderDetails)
   { %>
<table cellpadding="0" cellspacing="10" width="100%" >
  <tr>
    <td width="33%">
      <p>
          <b><asp:Label ID="lblConfirmationNumber" runat="server" ></asp:Label></b><br />
          <asp:Label ID="lblOrderDate" runat="server" ></asp:Label>
      </p>
    </td>
    <td width="33%">
      <p>
          <b><asp:Label ID="lblCustomerName" runat="server" ></asp:Label></b><br />
          <asp:Label ID="lblPaymentInfo" runat="server" ></asp:Label><br />
          <asp:Label ID="lblCardholderName" runat="server" ></asp:Label>
      </p>
    </td>
    <td width="33%">
      <p>
          <b><asp:Label ID="lblBusinessName" runat="server" ></asp:Label></b><br />
          <asp:Label ID="lblBusinessAddress" runat="server" ></asp:Label><br />
          <b class="pink"><asp:Label ID="lblBusinessPhone" runat="server" ></asp:Label></b>
      </p>
    </td>
  </tr>
</table>
<% } %>
<br /><br />
<table cellpadding="0" cellspacing="10" width="100%" >
  <thead>
     <tr>
        <th><%= base.GetLocalResourceObject("lblDate.Text") %></th>
        <th><%= base.GetLocalResourceObject("lblAppointment.Text")%></th>
        <th><%= base.GetLocalResourceObject("lblStaff.Text")%></th>
        <th><%= base.GetLocalResourceObject("lblTime.Text")%></th>
     </tr>
  </thead>
  <asp:ListView ID="lv" runat="server" GroupItemCount="1" GroupPlaceholderID="Group" ItemPlaceholderID="Item" >
     <LayoutTemplate>
        <tbody>
           <asp:PlaceHolder ID="Group" runat="server" ></asp:PlaceHolder>
        </tbody>
     </LayoutTemplate>
     <GroupTemplate>
         <tr>
            <asp:PlaceHolder ID="Item" runat="server" ></asp:PlaceHolder>
         </tr>
     </GroupTemplate>
     <ItemTemplate>
         <td><%# ((DateTime)Eval("StartsOn")).ToString("MMM dd, yyyy") %></td>
         <td><%# Eval("ServiceDescription") %></td>
         <td><%# Eval("StaffDescription") %></td>
         <td><%# ((DateTime)Eval("StartsOn")).ToString("HH:mm")%></td>
     </ItemTemplate>
  </asp:ListView>
</table>
<br /><br />
<table cellpadding="0" cellspacing="10" width="100%" 
  <tr>
     <td>
        <asp:Label ID="lblSubtotal" runat="server" ></asp:Label>
     </td>
  </tr>
  <% if (this.Discount > Decimal.Zero)
     {%>
  <tr>
     <td>
        <asp:Label ID="lblDiscount" runat="server" ></asp:Label>
     </td>
  </tr>
  <% } %>
  <% if (this.CouponCredit > Decimal.Zero)
     {%>
  <tr>
     <td>
        <asp:Label ID="lblCouponCredit" runat="server" ></asp:Label>
     </td>
  </tr>
  <% } %>
  <% if (this.GiftCardCredit > Decimal.Zero)
     { %>
   <tr>
     <td>
      <asp:Label ID="lblGiftCard" runat="server" ></asp:Label>
     </td>
   </tr>
  <% } %>
  <tr>
     <td>
        <asp:Label ID="lblAdditionalFee" runat="server" ></asp:Label>
     </td>
  </tr>
  <tr>
     <td>
        <asp:Label ID="lblTotal" runat="server" ></asp:Label>
     </td>
  </tr>
  <tr>
     <td>
        <asp:Label ID="lblBalance" runat="server" ></asp:Label>
     </td>
  </tr>
</table>