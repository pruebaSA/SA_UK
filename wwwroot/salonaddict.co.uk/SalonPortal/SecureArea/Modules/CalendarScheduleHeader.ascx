<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarScheduleHeader.ascx.cs" Inherits="SalonPortal.SecureArea.Modules.CalendarScheduleHeader" %>
<%@ Import Namespace="SalonAddict.Common" %>

<div class="clear" ></div>
<table cellpadding="0" cellspacing="0" width="950px" >
   <tr>
      <td>
        <table style="margin-left:-4px" cellpadding="0" cellspacing="4" >
           <tr>
              <td>
                 <asp:ImageButton ID="ibPreviousWeek" runat="server" ImageUrl="~/SecureArea/images/calendar_previous.jpg" OnClick="lbPreviousWeek_Click" />
              </td>
              <td>
                 <b><%= base.GetGlobalResourceObject("Modules", "ScheduleCalendar_Header_ChangeWeek") %></b>
              </td>
              <td>
                 <asp:ImageButton ID="ibNextWeek" runat="server" ImageUrl="~/SecureArea/images/calendar_next.jpg" OnClick="lbNextWeek_Click" />
              </td>
              <td>
                 &nbsp;&nbsp; <%= this.SelectedDate.StartOfWeek().ToString("dd MMM") %> - <%= this.SelectedDate.EndOfWeek().ToString("dd MMM") %>
              </td>
           </tr>
        </table>
      </td>
      <td style="width:60px;">
        <table style="margin-left:-4px" cellpadding="0" cellspacing="4" >
           <tr>
              <td>
                 <asp:ImageButton ID="ibPreviousYear" runat="server" ImageUrl="~/SecureArea/images/calendar_previous.jpg" OnClick="lbPreviousYear_Click" />
              </td>
              <td>
                 <b><%= this.SelectedDate.ToString("yyyy") %></b>
              </td>
              <td>
                 <asp:ImageButton ID="ibNextYear" runat="server" ImageUrl="~/SecureArea/images/calendar_next.jpg" OnClick="lbNextYear_Click" />
              </td>
           </tr>
        </table>
      </td>
   </tr>
</table>
<div class="clear" ></div>
<div class="calendar-schedule-header-wrapper" >
    <table class="calendar-schedule-header" cellpadding="0" cellspacing="0" >
       <tr>
          <td>
             <% if (this.SelectedDate.Month == 1)
                { %>
             <asp:LinkButton ID="lbM1_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="1" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM1_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="1" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
             <% if (this.SelectedDate.Month == 2)
                { %>
             <asp:LinkButton ID="lbM2_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="2" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM2_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="2" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 3)
                { %>
             <asp:LinkButton ID="lbM3_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="3" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM3_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="3" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 4)
                { %>
             <asp:LinkButton ID="lbM4_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="4" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM4_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="4" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 5)
                { %>
             <asp:LinkButton ID="lbM5_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="5" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM5_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="5" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 6)
                { %>
             <asp:LinkButton ID="lbM6_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="6" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM6_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="6" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 7)
                { %>
             <asp:LinkButton ID="lbM7_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="7" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM7_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="7" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 8)
                { %>
             <asp:LinkButton ID="lbM8_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="8" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM8_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="8" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 9)
                { %>
             <asp:LinkButton ID="lbM9_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="9" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM9_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="9" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 10)
                { %>
             <asp:LinkButton ID="lbM10_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="10" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM10_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="10" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 11)
                { %>
             <asp:LinkButton ID="lbM11_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="11" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM11_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="11" ></asp:LinkButton>
             <% } %>
          </td>
          <td>
            <% if (this.SelectedDate.Month == 12)
                { %>
             <asp:LinkButton ID="lbM12_1" runat="server" CssClass="schedule-header-month-selected" OnCommand="lbMX_Click" CommandArgument="12" ></asp:LinkButton>
             <% }
                else
                { %>
             <asp:LinkButton ID="lbM12_0" runat="server" CssClass="schedule-header-month" OnCommand="lbMX_Click" CommandArgument="12" ></asp:LinkButton>
             <% } %>
          </td>
       </tr>
    </table>
    <div class="calendar-schedule-daymenu-wrapper" >
    <table class="calendar-schedule-daymenu" cellpadding="0" cellspacing="0" >
      <tr>
         <td>
            <% if (this.SelectedDate.Day == 1)
               { %>
            <asp:LinkButton ID="lbD1_1" runat="server" Text="1" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="1" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 1)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD1_0w" runat="server" Text="1" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="1" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD1_0" runat="server" Text="1" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="1" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 2)
               { %>
            <asp:LinkButton ID="lbD2_1" runat="server" Text="2" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="2" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 2)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD2_0w" runat="server" Text="2" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="2" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD2_0" runat="server" Text="2" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="2" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 3)
               { %>
            <asp:LinkButton ID="lbD3_1" runat="server" Text="3" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="3" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 3)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD3_0w" runat="server" Text="3" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="3" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD3_0" runat="server" Text="3" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="3" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 4)
               { %>
            <asp:LinkButton ID="lbD4_1" runat="server" Text="4" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="4" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 4)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD4_0w" runat="server" Text="4" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="4" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD4_0" runat="server" Text="4" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="4" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 5)
               { %>
            <asp:LinkButton ID="lbD5_1" runat="server" Text="5" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="5" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 4)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD5_0w" runat="server" Text="5" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="5" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD5_0" runat="server" Text="5" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="5" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 6)
               { %>
            <asp:LinkButton ID="lbD6_1" runat="server" Text="6" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="6" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 6)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD6_0w" runat="server" Text="6" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="6" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD6_0" runat="server" Text="6" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="6" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 7)
               { %>
            <asp:LinkButton ID="lbD7_1" runat="server" Text="7" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="7" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 7)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD7_0w" runat="server" Text="7" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="7" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD7_0" runat="server" Text="7" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="7" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 8)
               { %>
            <asp:LinkButton ID="lbD8_1" runat="server" Text="8" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="8" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 8)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD8_0w" runat="server" Text="8" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="8" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD8_0" runat="server" Text="8" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="8" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 9)
               { %>
            <asp:LinkButton ID="lbD9_1" runat="server" Text="9" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="9" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 9)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD9_0w" runat="server" Text="9" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="9" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD9_0" runat="server" Text="9" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="9" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <%if (this.SelectedDate.Day == 10)
               { %>
            <asp:LinkButton ID="lbD10_1" runat="server" Text="10" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="10" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 10)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD10_0w" runat="server" Text="10" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="10" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD10_0" runat="server" Text="10" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="10" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 11)
               { %>
            <asp:LinkButton ID="lbD11_1" runat="server" Text="11" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="11" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 11)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD11_0w" runat="server" Text="11" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="11" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD11_0" runat="server" Text="11" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="11" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 12)
               { %>
            <asp:LinkButton ID="lbD12_1" runat="server" Text="12" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="12" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 12)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD12_0w" runat="server" Text="12" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="12" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD12_0" runat="server" Text="12" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="12" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 13)
               { %>
            <asp:LinkButton ID="lbD13_1" runat="server" Text="13" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="13" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 13)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD13_0w" runat="server" Text="13" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="13" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD13_0" runat="server" Text="13" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="13" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 14)
               { %>
            <asp:LinkButton ID="lbD14_1" runat="server" Text="14" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="14" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 14)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD14_0w" runat="server" Text="14" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="14" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD14_0" runat="server" Text="14" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="14" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 15)
               { %>
            <asp:LinkButton ID="lbD15_1" runat="server" Text="15" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="15" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 15)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD15_0w" runat="server" Text="15" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="15" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD15_0" runat="server" Text="15" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="15" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 16)
               { %>
            <asp:LinkButton ID="lbD16_1" runat="server" Text="16" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="16" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 16)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD16_0w" runat="server" Text="16" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="16" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD16_0" runat="server" Text="16" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="16" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 17)
               { %>
            <asp:LinkButton ID="lbD17_1" runat="server" Text="17" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="17" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 17)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD17_0w" runat="server" Text="17" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="17" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD17_0" runat="server" Text="17" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="17" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 18)
               { %>
            <asp:LinkButton ID="lbD18_1" runat="server" Text="18" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="18" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 18)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD18_0w" runat="server" Text="18" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="18" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD18_0" runat="server" Text="18" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="18" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 19)
               { %>
            <asp:LinkButton ID="lbD19_1" runat="server" Text="19" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="19" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 19)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD19_0w" runat="server" Text="19" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="19" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD19_0" runat="server" Text="19" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="19" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 20)
               { %>
            <asp:LinkButton ID="lbD20_1" runat="server" Text="20" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="20" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 20)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD20_0w" runat="server" Text="20" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="20" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD20_0" runat="server" Text="20" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="20" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 21)
               { %>
            <asp:LinkButton ID="lbD21_1" runat="server" Text="21" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="21" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 21)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD21_0w" runat="server" Text="21" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="21" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD21_0" runat="server" Text="21" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="21" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 22)
               { %>
            <asp:LinkButton ID="lbD22_1" runat="server" Text="22" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="22" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 22)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD22_0w" runat="server" Text="22" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="22" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD22_0" runat="server" Text="22" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="22" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 23)
               { %>
            <asp:LinkButton ID="lbD23_1" runat="server" Text="23" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="23" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 23)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD23_0w" runat="server" Text="23" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="23" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD23_0" runat="server" Text="23" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="23" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 24)
               { %>
            <asp:LinkButton ID="lbD24_1" runat="server" Text="24" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="24" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 24)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD24_0w" runat="server" Text="24" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="24" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD24_0" runat="server" Text="24" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="24" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 25)
               { %>
            <asp:LinkButton ID="lbD25_1" runat="server" Text="25" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="25" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 25)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD25_0w" runat="server" Text="25" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="25" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD25_0" runat="server" Text="25" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="25" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 26)
               { %>
            <asp:LinkButton ID="lbD26_1" runat="server" Text="26" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="26" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 26)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD26_0w" runat="server" Text="26" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="26" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD26_0" runat="server" Text="26" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="26" ></asp:LinkButton>
            <% } %>
         </td>
         <td>
            <% if (this.SelectedDate.Day == 27)
               { %>
            <asp:LinkButton ID="lbD27_1" runat="server" Text="27" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="27" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 27)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD27_0w" runat="server" Text="27" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="27" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD27_0" runat="server" Text="27" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="27" ></asp:LinkButton>
            <% } %>
         </td>
         <% if (DateTime.DaysInMonth(this.SelectedDate.Year, this.SelectedDate.Month) > 27)
            { %>
         <td>
            <% if (this.SelectedDate.Day == 28)
               { %>
            <asp:LinkButton ID="lbD28_1" runat="server" Text="28" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="28" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 28)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD28_0w" runat="server" Text="28" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="28" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD28_0" runat="server" Text="28" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="28" ></asp:LinkButton>
            <% } %>
         </td>
         <% } %>
         <% if (DateTime.DaysInMonth(this.SelectedDate.Year, this.SelectedDate.Month) > 28)
            { %>
         <td>
            <% if (this.SelectedDate.Day == 29)
               { %>
            <asp:LinkButton ID="lbD29_1" runat="server" Text="29" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="29" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 29)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD29_0w" runat="server" Text="29" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="29" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD29_0" runat="server" Text="29" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="29" ></asp:LinkButton>
            <% } %>
         </td>
         <% } %>
         <% if (DateTime.DaysInMonth(this.SelectedDate.Year, this.SelectedDate.Month) > 29)
            { %>
         <td>
            <% if (this.SelectedDate.Day == 30)
               { %>
            <asp:LinkButton ID="lbD30_1" runat="server" Text="30" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="30" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 30)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD30_0w" runat="server" Text="30" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="30" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD30_0" runat="server" Text="30" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="30" ></asp:LinkButton>
            <% } %>
         </td>
         <% } %>
         <% if (DateTime.DaysInMonth(this.SelectedDate.Year, this.SelectedDate.Month) > 30)
            { %>
         <td>
            <% if (this.SelectedDate.Day == 31)
               { %>
            <asp:LinkButton ID="lbD31_1" runat="server" Text="31" CssClass="schedule-header-day-selected" OnClick="lbDX_Click" CommandArgument="31" ></asp:LinkButton>
            <% }
               else if ((new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, 31)).IsWeekend())
               { %>
            <asp:LinkButton ID="lbD31_0w" runat="server" Text="31" CssClass="schedule-header-weekend" OnClick="lbDX_Click" CommandArgument="31" ></asp:LinkButton>
            <% }
               else
               { %>
            <asp:LinkButton ID="lbD31_0" runat="server" Text="31" CssClass="schedule-header-day" OnClick="lbDX_Click" CommandArgument="31" ></asp:LinkButton>
            <% } %>
         </td>
         <% } %>
      </tr>
    </table>
    </div>
</div>