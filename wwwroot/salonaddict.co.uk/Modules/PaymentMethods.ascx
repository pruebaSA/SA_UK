<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="PaymentMethods.ascx.cs" Inherits="SalonAddict.Modules.PaymentMethods" %>
<div class="payment-method-logos" >
   <asp:ListView 
        ID="lv" 
        runat="server" 
        GroupItemCount="10"
        GroupPlaceholderID="GroupPlaceholder" 
        ItemPlaceholderID="ItemPlaceholder" 
        EnableViewState="false"
        OnItemCreated="lv_ItemCreated" >
     <LayoutTemplate>
        <table cellpadding="0" cellspacing="2" >
            <asp:PlaceHolder ID="GroupPlaceholder" runat="server" EnableViewState="false" ></asp:PlaceHolder>
        </table>
     </LayoutTemplate>
     <GroupTemplate>
         <tr>
            <asp:PlaceHolder ID="ItemPlaceholder" runat="server" EnableViewState="false" ></asp:PlaceHolder>
         </tr>
     </GroupTemplate>
     <ItemTemplate>
         <asp:PlaceHolder ID="HTML" runat="server" EnableViewState="false" ></asp:PlaceHolder>
     </ItemTemplate>
   </asp:ListView>
</div>