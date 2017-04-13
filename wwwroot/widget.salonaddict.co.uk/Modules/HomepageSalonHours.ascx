<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="HomepageSalonHours.ascx.cs" Inherits="IFRAME.Modules.HomepageSalonHours" %>
<div class="module-homepage-salon-hours" >
   <table class="details" cellpadding="0" cellspacing="0" >
      <tr>
         <td class="title" >
            <%= System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Monday)%>:
         </td>
         <td class="data-item" >
             <asp:Literal ID="ltrMonday" runat="server" ></asp:Literal>
         </td>
      </tr>
      <tr>
         <td class="title-alt" >
            <%= System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Tuesday)%>:
         </td>
         <td class="data-item-alt" >
             <asp:Literal ID="ltrTuesday" runat="server" ></asp:Literal>
         </td>
      </tr>
      <tr>
         <td class="title" >
            <%= System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Wednesday)%>:
         </td>
         <td class="data-item" >
             <asp:Literal ID="ltrWednesday" runat="server" ></asp:Literal>
         </td>
      </tr>
      <tr>
         <td class="title-alt" >
            <%= System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Thursday)%>:
         </td>
         <td class="data-item-alt" >
             <asp:Literal ID="ltrThursday" runat="server" ></asp:Literal>
         </td>
      </tr>
      <tr>
         <td class="title" >
            <%= System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Friday)%>:
         </td>
         <td class="data-item" >
             <asp:Literal ID="ltrFriday" runat="server" ></asp:Literal>
         </td>
      </tr>
      <tr>
         <td class="title-alt" >
            <%= System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Saturday)%>:
         </td>
         <td class="data-item-alt" >
             <asp:Literal ID="ltrSaturday" runat="server" ></asp:Literal>
         </td>
      </tr>
      <tr>
         <td class="title" >
            <%= System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedDayName(DayOfWeek.Sunday)%>:
         </td>
         <td class="data-item" >
             <asp:Literal ID="ltrSunday" runat="server" ></asp:Literal>
         </td>
      </tr>
   </table>
</div>