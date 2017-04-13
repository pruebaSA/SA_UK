<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SalonMenu.ascx.cs" Inherits="IFRAME.Admin.Modules.SalonMenu" %>
<div class="module-salonmenu" >
   <ul>
      <li><a class='<%= (this.SelectedItem == MenuItem.Overview)? "option-overview selected" : "option-overview" %>' href='<%= IFRMHelper.GetURL("default.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><%= base.GetLocaleResourceString("hlOverview.Text") %></a></li>
      <li>
         <asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
             <div style="position:relative;" >
                <a class='<%= (this.SelectedItem == MenuItem.Appointments)? "option-appointments selected" : "option-appointments" %>' href='<%= IFRMHelper.GetURL("appointments.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><%= base.GetLocaleResourceString("hlAppointments.Text") %></a>
                <asp:Panel ID="pnlAppointentBadge" runat="server" CssClass="appointment-badge" Visible="false" >
                   <asp:Literal ID="ltrAppointmentCount" runat="server" ></asp:Literal>
                </asp:Panel>
             </div>
            </ContentTemplate>
            <Triggers>
               <asp:AsyncPostBackTrigger ControlID="Timer" EventName="Tick" />
            </Triggers>
         </asp:UpdatePanel>
      </li>
      <li><a class='<%= (this.SelectedItem == MenuItem.Scheduling)? "option-scheduling selected" : "option-scheduling" %>' href='<%= IFRMHelper.GetURL("blockedtimes.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><%= base.GetLocaleResourceString("hlScheduling.Text") %></a></li>
      <li><a class='<%= (this.SelectedItem == MenuItem.Services)? "option-services selected" : "option-services" %>' href='<%= IFRMHelper.GetURL("services.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><%= base.GetLocaleResourceString("hlServices.Text") %></a></li>
      <li><a class='<%= (this.SelectedItem == MenuItem.Employees)? "option-employees selected" : "option-employees" %>' href='<%= IFRMHelper.GetURL("employees.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><%= base.GetLocaleResourceString("hlEmployees.Text") %></a></li>
      <li><a class='<%= (this.SelectedItem == MenuItem.Account)? "option-account selected" : "option-account" %>'  href='<%= IFRMHelper.GetURL("wsp.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><%= base.GetLocaleResourceString("hlAccount.Text") %></a></li>
      <li><a class='<%= (this.SelectedItem == MenuItem.Settings)? "option-settings selected" : "option-settings" %>'  href='<%= IFRMHelper.GetURL("settings.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><%= base.GetLocaleResourceString("hlSettings.Text") %></a></li>
   </ul>
</div>
<asp:Timer ID="Timer" runat="server" OnTick="Timer_Tick" ></asp:Timer>