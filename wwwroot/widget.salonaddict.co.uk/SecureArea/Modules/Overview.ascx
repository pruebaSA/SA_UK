<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Overview.ascx.cs" EnableViewState="false" Inherits="IFRAME.SecureArea.Modules.Overview" %>
<div class="module-overview" >
    <table style="margin:-20px" cellpadding="0" cellspacing="20" >
      <tr>
         <td>
            <a href='<%= IFRMHelper.GetURL(Page.ResolveUrl("~/")) %>' class="option-home" ><%= base.GetLocalResourceObject("hlHome.Text") %></a>
         </td>
         <td>
            <div style="position:relative;">
            <asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional" >
                <ContentTemplate>
                    <a href='<%= IFRMHelper.GetURL("appointments.aspx") %>' class="option-appointments" ><%= base.GetLocalResourceObject("hlAppointments.Text") %></a>
                    <asp:Panel ID="pnlAppointentBadge" runat="server" CssClass="appointment-badge" Visible="false" > 
                        <asp:Literal ID="ltrAppointmentCount" runat="server" ></asp:Literal>
                    </asp:Panel>
                </ContentTemplate>
                   <Triggers>
                     <asp:AsyncPostBackTrigger ControlID="Timer" EventName="Tick" />
                   </Triggers>
                </asp:UpdatePanel>
            </div>
         </td>
         <td>
            <a href='<%= IFRMHelper.GetURL("blockedtimes.aspx") %>' class="option-scheduling" ><%= base.GetLocalResourceObject("hlScheduling.Text") %></a>
         </td>
         <td>
            <a href='<%= IFRMHelper.GetURL("services.aspx") %>' class="option-services" ><%= base.GetLocalResourceObject("hlServices.Text") %></a>
         </td>
         <td>
            <a href='<%= IFRMHelper.GetURL("employees.aspx") %>' class="option-employees" ><%= base.GetLocalResourceObject("hlEmployees.Text") %></a>
         </td>
      </tr>
    </table>
    <div class="horizontal-line" ></div>
    <table style="margin:-20px" cellpadding="0" cellspacing="20" >
      <tr>
         <td>
            <a href='<%= IFRMHelper.GetURL("account.aspx") %>' class="option-account" ><%= base.GetLocalResourceObject("hlAccount.Text") %></a>
         </td>
         <td>
            <a href='<%= IFRMHelper.GetURL("settings.aspx") %>' class="option-settings" ><%= base.GetLocalResourceObject("hlSettings.Text") %></a>
         </td>
      </tr>
    </table>
    <asp:Timer ID="Timer" runat="server" OnTick="Timer_Tick" ></asp:Timer>
</div>