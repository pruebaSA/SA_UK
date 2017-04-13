<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="IFRAME.Admin.Modules.Menu" %>
<div class="module-adminmenu" >
   <ul>
      <li><a class='<%= (this.SelectedItem == MenuItem.Overview)? "option-overview selected" : "option-overview" %>' href='<%= IFRMHelper.GetURL("default.aspx") %>' ><%= base.GetLocaleResourceString("hlOverview.Text") %></a></li>
      <li><a class='<%= (this.SelectedItem == MenuItem.Salons)? "option-salons selected" : "option-salons" %>' href='<%= IFRMHelper.GetURL("salons.aspx") %>' ><%= base.GetLocaleResourceString("hlSalons.Text") %></a></li>
      <li>
         <div style="position:relative;" >
            <a class='<%= (this.SelectedItem == MenuItem.Billing)? "option-billing selected" : "option-billing" %>' href='<%= IFRMHelper.GetURL("billing.aspx") %>' ><%= base.GetLocaleResourceString("hlBilling.Text") %></a>
            <asp:Panel ID="pnlBillingBadge" runat="server" CssClass="billing-badge" Visible="false" >
               <asp:Literal ID="ltrBillingCount" runat="server" ></asp:Literal>
            </asp:Panel>
         </div>
      </li>
      <li><a class='<%= (this.SelectedItem == MenuItem.Profile)? "option-profile selected" : "option-profile" %>' href='<%= IFRMHelper.GetURL("profile.aspx") %>' ><%= base.GetLocaleResourceString("hlProfile.Text") %></a></li>
      <li><a class='<%= (this.SelectedItem == MenuItem.Reports)? "option-reports selected" : "option-reports" %>' href='<%= IFRMHelper.GetURL("reports.aspx") %>' ><%= base.GetLocaleResourceString("hlReports.Text") %></a></li>
      <li><a class='<%= (this.SelectedItem == MenuItem.Audit)? "option-audit selected" : "option-audit" %>' href='<%= IFRMHelper.GetURL("audit.aspx") %>' ><%= base.GetLocaleResourceString("hlAudit.Text") %></a></li>
   </ul>
</div>