<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderSummaryOne.ascx.cs" Inherits="SalonAddict.Templates.OrderSummaryOne" %>

<div class="template-ordersummaryone" >
<% if (this.ShowOrderDetails)
   { %>
   <table style="margin-bottom:15px;" cellpadding="0" cellspacing="0" width="100%" >
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
   <table cellpadding="0" cellspacing="0" width="100%" >
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
   <div class="subtotal" >
      <asp:Label ID="lblSubtotal" runat="server" ></asp:Label>
   </div>
   <% if (this.Discount > Decimal.Zero)
      { %>
   <div class="discount" >
      <asp:Label ID="lblDiscount" runat="server" ></asp:Label>
   </div>
   <% } %>
   <% if(this.CouponCredit > Decimal.Zero)
      { %>
   <div class="coupon" >
      <% if (this.IsShoppingCart)
         { %>
      <asp:LinkButton ID="lbRemoveCoupon" runat="server" Font-Size="11px" ForeColor="#000000" CausesValidation="false" OnClick="lbRemoveCoupon_Click" meta:resourceKey="lbRemoveCoupon" ></asp:LinkButton>
      &nbsp;
      <% } %>
      <asp:Label ID="lblCoupon" runat="server" ></asp:Label>
   </div>
   <% } %>
   <% if (this.GiftCardCredit > Decimal.Zero)
      { %>
   <div class="giftcard" >
      <% if (this.IsShoppingCart)
         { %>
      <asp:LinkButton ID="lbRemoveGiftCard" runat="server" Font-Size="11px" ForeColor="#000000" CausesValidation="false" OnClick="lbRemoveGiftCard_Click" meta:resourceKey="lbRemoveGiftCard" ></asp:LinkButton>
      &nbsp;
      <% } %>
      <asp:Label ID="lblGiftCard" runat="server" ></asp:Label>
   </div>
   <% } %>
   <div class="additional-fee" >
      <asp:Label ID="lblAdditionalFee" runat="server" ></asp:Label>
   </div>
   <div class="total" >
      <asp:Label ID="lblTotal" runat="server" ></asp:Label>
   </div>
   <div class="balance" >
      <asp:Label ID="lblBalance" runat="server" ></asp:Label>
   </div>
</div>