<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="IFRAME.Admin.ReportsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Reports" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px;margin-bottom:0px;" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-reports.png" %>' alt="reports" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <ul>
           <li style="margin-bottom:10px;" >
              <a href='<%= IFRMHelper.GetURL("reportsalonplantype.aspx") %>' >
                 <u><%= base.GetLocaleResourceString("hlPlanReport.Text") %></u>
              </a>
           </li>
           <li style="margin-bottom:10px;" >
              <a href='<%= IFRMHelper.GetURL("reportplanexpiry.aspx") %>' >
                 <u><%= base.GetLocaleResourceString("hlPlanExpiryReport.Text") %></u>
              </a>
           </li>
           <li style="margin-bottom:10px;" >
              <a href='<%= IFRMHelper.GetURL("reportplantotals.aspx") %>' >
                 <u><%= base.GetLocaleResourceString("hlPlanTotalsReport.Text") %></u>
              </a>
           </li>
           <li style="margin-bottom:10px;" >
              <a href='<%= IFRMHelper.GetURL("reportbookingtotals.aspx") %>' >
                 <u><%= base.GetLocaleResourceString("hlBookingTotalsReport.Text") %></u>
              </a>
           </li>
           <li style="margin-bottom:10px;" >
              <a href='<%= IFRMHelper.GetURL("reportnondelivery.aspx") %>' >
                 <u><%= base.GetLocaleResourceString("hlNonDeliveryReport.Text")%> (<asp:Literal ID="ltrNonDeliveryCount" runat="server" EnableViewState="false" Text="0" ></asp:Literal>)</u>
              </a>
           </li>
           <li style="margin-bottom:10px;" >
              <a href='<%= IFRMHelper.GetURL("reportwidgetoffline.aspx") %>' >
                 <u><%= base.GetLocaleResourceString("hlWidgetOfflineReport.Text") %> (<asp:Literal ID="ltrWidgetOfflineCount" runat="server" EnableViewState="false" Text="0" ></asp:Literal>)</u>
              </a>
           </li>
        </ul>
     </asp:Panel>
</asp:Content>
