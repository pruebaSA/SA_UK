<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SalonAddict.Administration.Dashboard" %>
<%@ Register TagPrefix="SA" TagName="AppointmentReport" Src="~/Administration/Modules/AppointmentAverageReport.ascx" %>
<%@ Register TagPrefix="SA" TagName="IncomingReviewsReport" Src="~/Administration/Modules/IncomingReviewsReport.ascx" %>
<%@ Register TagPrefix="SA" TagName="QueuedMessagesReport" Src="~/Administration/Modules/QueuedMessagesReport.ascx" %>
<%@ Register TagPrefix="SA" TagName="LeadReport" Src="~/Administration/Modules/LeadReport.ascx" %>
<%@ Register TagPrefix="SA" TagName="AvailabilityReport" Src="~/Administration/Modules/AvailabilityReport.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-title">
    <img src="images/ico-dashboard.png" alt="Dashboard" />
    Dashboard
</div>

<div style="margin-bottom:25px;" >
    <SA:AppointmentReport ID="cntlAppointmentReport" runat="server" />
</div>

<table cellpadding="0" cellspacing="10" >
  <tr>
<% if (cntlLeadReport.Visible)
   { %>
      <td style="vertical-align:top;">
          <SA:LeadReport ID="cntlLeadReport" runat="server" />
      </td>
<% } %>
<% if (cntlQueuedMessagesReport.Visible)
   { %>
      <td style="vertical-align:top;">
          <SA:QueuedMessagesReport ID="cntlQueuedMessagesReport" runat="server" />
      </td>
<% } %>
<% if (cntlIncomingReviews.Visible)
   { %>
      <td style="vertical-align:top;">
          <SA:IncomingReviewsReport ID="cntlIncomingReviews" runat="server" />
      </td>
<% } %>
  </tr>
</table>
<SA:AvailabilityReport ID="cntlAvailabilityReport" runat="server" />
</asp:Content>
