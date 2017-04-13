<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Print-Booking-Confirmation.aspx.cs" Inherits="SalonAddict.Print_Booking_Confirmation" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ Register TagPrefix="SA" TagName="OrderSummary" Src="~/Templates/OrderSummaryTwo.ascx" %>
<%@ Import Namespace="SalonAddict.Common" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Salon Addict</title>
</head>
<body onload="window.print()">
    <asp:Image ID="logo" runat="server" ImageUrl="~/images/salonaddict.png" />
    <br /><br />
    <SA:Topic ID="ContentTopic" runat="server" meta:resourceKey="ContentTopic" />
    <br />
    <SA:OrderSummary ID="OrderSummary" runat="server" />
</body>
</html>
