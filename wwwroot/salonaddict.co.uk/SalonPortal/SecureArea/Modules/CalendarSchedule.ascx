<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarSchedule.ascx.cs" Inherits="SalonPortal.SecureArea.Modules.CalendarSchedule" %>
<%@ Register TagPrefix="SA" TagName="Header" Src="~/SecureArea/Modules/CalendarScheduleHeader.ascx" %>

<SA:Header ID="cntlHeader" runat="server" OnYearChanged="CalendarHeader_OnYearChanged" OnMonthChanged="CalendarHeader_OnMonthChanged" OnWeekChanged="CalendarHeader_OnWeekChanged" OnDayChanged="CalendarHeader_OnDayChanged" />
<div class="schedule-calendar-wrapper" >
   <table class="calendar" cellpadding="0" cellspacing="0" >
      <tr>
         <td style="border:none;">
            <% if (DateTime.Compare(this.StartOfWeek, this.SelectedDate) == 0)
               { %>
            <div class="week-selected" >
            <% }
               else
               { %>
            <div class="week" >
            <% } %>
               <asp:Literal ID="ltrWeek1" runat="server" ></asp:Literal>
            </div>
            <asp:ListView ID="lvMonday" 
                runat="server" 
                ItemPlaceholderID="phMonday" 
                OnItemCommand="lv_OnItemCommand"
                OnItemCreated="lv_OnItemCreated" >
               <LayoutTemplate>
                   <asp:PlaceHolder ID="phMonday" runat="server" ></asp:PlaceHolder>
               </LayoutTemplate>
               <ItemTemplate>
                   <asp:Literal ID="ltrDiv" runat="server" ></asp:Literal>
                   
                      <asp:Literal ID="ltrStaff" runat="server" ></asp:Literal><br />
                      <%# Convert.ToDateTime(Eval("StartDate").ToString()).ToString("HH:mm")
                          + " - " +
                          Convert.ToDateTime(Eval("EndDate").ToString()).ToString("HH:mm")%>
                    <asp:ImageButton 
                        ID="ibRemove" 
                        runat="server" 
                        CommandName="Remove"
                        CommandArgument='<%# Eval("ScheduleID") %>'
                        CssClass="schedule-calendar-delete-button" 
                        OnClientClick='<%# "return confirm(\"" + base.GetGlobalResourceObject("Modules","Calendar_Delete_Message") + "\")" %>'
                        ImageUrl="~/SecureArea/images/ico-delete.png" />
                   </div>
                </ItemTemplate>
             </asp:ListView>
         </td>
         <td>
            <% if (DateTime.Compare(this.StartOfWeek.AddDays(1), this.SelectedDate) == 0)
               { %>
            <div class="week-selected" >
            <% }
               else
               { %>
            <div class="week" >
            <% } %>
               <asp:Literal ID="ltrWeek2" runat="server" ></asp:Literal>
            </div>
            <asp:ListView ID="lvTuesday" 
                runat="server" 
                ItemPlaceholderID="phTuesday" 
                OnItemCommand="lv_OnItemCommand"
                OnItemCreated="lv_OnItemCreated" >
               <LayoutTemplate>
                   <asp:PlaceHolder ID="phTuesday" runat="server" ></asp:PlaceHolder>
               </LayoutTemplate>
               <ItemTemplate>
                   <asp:Literal ID="ltrDiv" runat="server" ></asp:Literal>
                      <asp:Literal ID="ltrStaff" runat="server" ></asp:Literal><br />
                      <%# Convert.ToDateTime(Eval("StartDate").ToString()).ToString("HH:mm")
                          + " - " +
                          Convert.ToDateTime(Eval("EndDate").ToString()).ToString("HH:mm")%>
                    <asp:ImageButton 
                        ID="ibRemove" 
                        runat="server" 
                        CommandName="Remove"
                        CommandArgument='<%# Eval("ScheduleID") %>'
                        CssClass="schedule-calendar-delete-button" 
                        OnClientClick='<%# "return confirm(\"" + base.GetGlobalResourceObject("Modules","Calendar_Delete_Message") + "\")" %>'
                        ImageUrl="~/SecureArea/images/ico-delete.png" />
                   </div>
                </ItemTemplate>
             </asp:ListView>
         </td>
         <td>
            <% if (DateTime.Compare(this.StartOfWeek.AddDays(2), this.SelectedDate) == 0)
               { %>
            <div class="week-selected" >
            <% }
               else
               { %>
            <div class="week" >
            <% } %>
               <asp:Literal ID="ltrWeek3" runat="server" ></asp:Literal>
            </div>
            <asp:ListView ID="lvWednessday" 
                runat="server" 
                ItemPlaceholderID="phWednessday" 
                OnItemCommand="lv_OnItemCommand"
                OnItemCreated="lv_OnItemCreated" >
               <LayoutTemplate>
                   <asp:PlaceHolder ID="phWednessday" runat="server" ></asp:PlaceHolder>
               </LayoutTemplate>
               <ItemTemplate>
                   <asp:Literal ID="ltrDiv" runat="server" ></asp:Literal>
                      <asp:Literal ID="ltrStaff" runat="server" ></asp:Literal><br />
                      <%# Convert.ToDateTime(Eval("StartDate").ToString()).ToString("HH:mm")
                          + " - " +
                          Convert.ToDateTime(Eval("EndDate").ToString()).ToString("HH:mm")%>
                    <asp:ImageButton 
                        ID="ibRemove" 
                        runat="server" 
                        CommandName="Remove"
                        CommandArgument='<%# Eval("ScheduleID") %>'
                        CssClass="schedule-calendar-delete-button" 
                        OnClientClick='<%# "return confirm(\"" + base.GetGlobalResourceObject("Modules","Calendar_Delete_Message") + "\")" %>'
                        ImageUrl="~/SecureArea/images/ico-delete.png" />
                   </div>
                </ItemTemplate>
             </asp:ListView>
         </td>
         <td>
            <% if (DateTime.Compare(this.StartOfWeek.AddDays(3), this.SelectedDate) == 0)
               { %>
            <div class="week-selected" >
            <% }
               else
               { %>
            <div class="week" >
            <% } %>
               <asp:Literal ID="ltrWeek4" runat="server" ></asp:Literal>
            </div>
            <asp:ListView ID="lvThursday" 
                runat="server" 
                ItemPlaceholderID="phThursday" 
                OnItemCommand="lv_OnItemCommand"
                OnItemCreated="lv_OnItemCreated" >
               <LayoutTemplate>
                   <asp:PlaceHolder ID="phThursday" runat="server" ></asp:PlaceHolder>
               </LayoutTemplate>
               <ItemTemplate>
                   <asp:Literal ID="ltrDiv" runat="server" ></asp:Literal>
                      <asp:Literal ID="ltrStaff" runat="server" ></asp:Literal><br />
                      <%# Convert.ToDateTime(Eval("StartDate").ToString()).ToString("HH:mm")
                          + " - " +
                          Convert.ToDateTime(Eval("EndDate").ToString()).ToString("HH:mm")%>
                    <asp:ImageButton 
                        ID="ibRemove" 
                        runat="server" 
                        CommandName="Remove"
                        CommandArgument='<%# Eval("ScheduleID") %>'
                        CssClass="schedule-calendar-delete-button" 
                        OnClientClick='<%# "return confirm(\"" + base.GetGlobalResourceObject("Modules","Calendar_Delete_Message") + "\")" %>'
                        ImageUrl="~/SecureArea/images/ico-delete.png" />
                   </div>
                </ItemTemplate>
             </asp:ListView>
         </td>
         <td>
            <% if (DateTime.Compare(this.StartOfWeek.AddDays(4), this.SelectedDate) == 0)
               { %>
            <div class="week-selected" >
            <% }
               else
               { %>
            <div class="week" >
            <% } %>
               <asp:Literal ID="ltrWeek5" runat="server" ></asp:Literal>
            </div>
            <asp:ListView ID="lvFriday" 
                runat="server" 
                ItemPlaceholderID="phFriday" 
                OnItemCommand="lv_OnItemCommand"
                OnItemCreated="lv_OnItemCreated" >
               <LayoutTemplate>
                   <asp:PlaceHolder ID="phFriday" runat="server" ></asp:PlaceHolder>
               </LayoutTemplate>
               <ItemTemplate>
                   <asp:Literal ID="ltrDiv" runat="server" ></asp:Literal>
                      <asp:Literal ID="ltrStaff" runat="server" ></asp:Literal><br />
                      <%# Convert.ToDateTime(Eval("StartDate").ToString()).ToString("HH:mm")
                          + " - " +
                          Convert.ToDateTime(Eval("EndDate").ToString()).ToString("HH:mm")%>
                    <asp:ImageButton 
                        ID="ibRemove" 
                        runat="server" 
                        CommandName="Remove"
                        CommandArgument='<%# Eval("ScheduleID") %>'
                        CssClass="schedule-calendar-delete-button" 
                        OnClientClick='<%# "return confirm(\"" + base.GetGlobalResourceObject("Modules","Calendar_Delete_Message") + "\")" %>'
                        ImageUrl="~/SecureArea/images/ico-delete.png" />
                   </div>
                </ItemTemplate>
             </asp:ListView>
         </td>
         <td>
            <% if (DateTime.Compare(this.StartOfWeek.AddDays(5), this.SelectedDate) == 0)
               { %>
            <div class="week-selected" >
            <% }
               else
               { %>
            <div class="week" >
            <% } %>
               <asp:Literal ID="ltrWeek6" runat="server" ></asp:Literal>
            </div>
            <asp:ListView ID="lvSaturday" 
                runat="server" 
                ItemPlaceholderID="phSaturday" 
                OnItemCommand="lv_OnItemCommand"
                OnItemCreated="lv_OnItemCreated" >
               <LayoutTemplate>
                   <asp:PlaceHolder ID="phSaturday" runat="server" ></asp:PlaceHolder>
               </LayoutTemplate>
               <ItemTemplate>
                   <asp:Literal ID="ltrDiv" runat="server" ></asp:Literal>
                      <asp:Literal ID="ltrStaff" runat="server" ></asp:Literal><br />
                      <%# Convert.ToDateTime(Eval("StartDate").ToString()).ToString("HH:mm")
                          + " - " +
                          Convert.ToDateTime(Eval("EndDate").ToString()).ToString("HH:mm")%>
                    <asp:ImageButton 
                        ID="ibRemove" 
                        runat="server" 
                        CommandName="Remove"
                        CommandArgument='<%# Eval("ScheduleID") %>'
                        CssClass="schedule-calendar-delete-button" 
                        OnClientClick='<%# "return confirm(\"" + base.GetGlobalResourceObject("Modules","Calendar_Delete_Message") + "\")" %>'
                        ImageUrl="~/SecureArea/images/ico-delete.png" />
                   </div>
                </ItemTemplate>
             </asp:ListView>
         </td>
         <td>
            <% if (DateTime.Compare(this.StartOfWeek.AddDays(6), this.SelectedDate) == 0)
               { %>
            <div class="week-selected" >
            <% }
               else
               { %>
            <div class="week" >
            <% } %>
               <asp:Literal ID="ltrWeek7" runat="server" ></asp:Literal>
            </div>
            <asp:ListView ID="lvSunday" 
                runat="server" 
                ItemPlaceholderID="phSunday" 
                OnItemCommand="lv_OnItemCommand"
                OnItemCreated="lv_OnItemCreated" >
               <LayoutTemplate>
                   <asp:PlaceHolder ID="phSunday" runat="server" ></asp:PlaceHolder>
               </LayoutTemplate>
               <ItemTemplate>
                   <asp:Literal ID="ltrDiv" runat="server" ></asp:Literal>
                      <asp:Literal ID="ltrStaff" runat="server" ></asp:Literal><br />
                      <%# Convert.ToDateTime(Eval("StartDate").ToString()).ToString("HH:mm")
                          + " - " +
                          Convert.ToDateTime(Eval("EndDate").ToString()).ToString("HH:mm")%>
                      <asp:ImageButton 
                        ID="ibRemove" 
                        runat="server" 
                        CommandName="Remove"
                        CommandArgument='<%# Eval("ScheduleID") %>'
                        CssClass="schedule-calendar-delete-button" 
                        OnClientClick='<%# "return confirm(\"" + base.GetGlobalResourceObject("Modules","Calendar_Delete_Message") + "\")" %>'
                        ImageUrl="~/SecureArea/images/ico-delete.png" />
                   </div>
                </ItemTemplate>
             </asp:ListView>
         </td>
      </tr>
   </table>
</div>