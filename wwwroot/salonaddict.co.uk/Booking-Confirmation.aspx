<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Booking-Confirmation.aspx.cs" Inherits="SalonAddict.Booking_Confirmation" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ Register TagPrefix="SA" TagName="OrderSummary" Src="~/Templates/OrderSummaryOne.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>
<%@ Import Namespace="SalonAddict.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
<div style="position:relative;" >
    <div style="position:absolute;right:10px;top:10px;" >
       <asp:HyperLink ID="btnPrint" runat="server" SkinID="BlackButtonMedium" Target="_blank" meta:resourceKey="btnPrint" />
    </div>
    <SA:Topic ID="ContentTopic" runat="server" meta:resourceKey="ContentTopic" />
    <SA:OrderSummary ID="OrderSummary" runat="server" Paid="true" />
    <p>
        <asp:Button ID="btnHomepage" runat="server" SkinID="BlackButtonLarge" OnClick="btnHomepage_Click" meta:resourceKey="btnHomepage" />
    </p>
</div>
</asp:Content>
