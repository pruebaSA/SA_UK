<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StaffAvailabilityOne.ascx.cs" Inherits="SalonAddict.Templates.StaffAvailabilityOne" %>
<div class="template-staffavailabilityone" >
    <div class="service" >
        <asp:Literal ID="Service" runat="server" EnableViewState="false" ></asp:Literal>
        <asp:Label ID="lblServiceID" runat="server" Visible="false" ></asp:Label>
    </div>
    <div class="staff" >
        <asp:Literal ID="Name" runat="server" EnableViewState="false" ></asp:Literal>
        <asp:Label ID="lblStaffID" runat="server" Visible="false" ></asp:Label>
    </div>
    <div class="description" >
        <asp:Literal ID="Description" runat="server" EnableViewState="false" ></asp:Literal>
    </div>
    <div class="rating" >
        <asp:Literal ID="Rating" runat="server" EnableViewState="false" ></asp:Literal>
    </div>
    <div class="price" >
        <asp:MultiView ID="mv" runat="server" EnableViewState="false" >
           <asp:View ID="v1" runat="server" >
               <div class="price-label" >
                  <%= base.GetLocalResourceObject("lblPrice.Text") %>
               </div>
               <div class="price-amount" >
                  <asp:Literal ID="Price" runat="server" EnableViewState="false" ></asp:Literal>
               </div>
               <div class="price-nextweek" >
                   <asp:LinkButton ID="lbNextWeek" runat="server" meta:resourceKey="lbNextWeek" OnClick="lbNextWeek_Click" ></asp:LinkButton>
               </div>
           </asp:View>
           <asp:View ID="v2" runat="server" >
               <div class="sprice-label">
                  <%= base.GetLocalResourceObject("lblSalonPrice.Text") %>
               </div>
               <div class="sprice-amount" >
                  <asp:Literal ID="SalonPrice" runat="server" EnableViewState="false" ></asp:Literal>
               </div>
               <div class="oprice-label" >
                  <%= base.GetLocalResourceObject("lblOnlinePrice.Text") %>
               </div>
               <div class="oprice-amount" >
                  <asp:Literal ID="OnlinePrice" runat="server" EnableViewState="false" ></asp:Literal>
               </div>
           </asp:View>
        </asp:MultiView>
    </div>
    <div class="time-label" >
       <%= base.GetLocalResourceObject("lblTime.Text") %>
    </div>
    <div class="time-choices" >
        <asp:DropDownList ID="ddlTime" runat="server" Width="70px" ></asp:DropDownList>
    </div>
    <div class="button" >
       <asp:Button ID="Submit" runat="server" SkinID="PinkButtonXSmall" OnClick="Submit_Click" meta:resourceKey="Submit" />
    </div>
</div>