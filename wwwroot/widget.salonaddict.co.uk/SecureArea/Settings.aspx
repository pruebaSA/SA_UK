<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="IFRAME.SecureArea.SettingsPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Settings" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-settings.png" %>' alt="settings" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:530px;" >
                <table style="margin-top:10px" class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrSalon.Text") %></td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrSalon" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ></td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrAddress" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ></td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrPhone" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >Company Name:</td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrCompany" runat="server" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrOpeningHours.Text") %></td>
                      <td class="data-item" >
                          <asp:DropDownList ID="ddlOpeningHours" runat="server" SkinID="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="ddlOpeningHours_SelectedIndexChanged" >
                             <asp:ListItem Text="Public" Value="PUBLIC" ></asp:ListItem>
                             <asp:ListItem Text="Private" Value="PRIVATE" ></asp:ListItem>
                          </asp:DropDownList>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrWidgetStatus.Text") %></td>
                      <td class="data-item" >
                          <asp:DropDownList ID="ddlStatus" runat="server" SkinID="DropDownList" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" >
                             <asp:ListItem Text="Public" Value="PUBLIC" ></asp:ListItem>
                             <asp:ListItem Text="Private" Value="PRIVATE" ></asp:ListItem>
                          </asp:DropDownList>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >Ticket Alerts Enabled</td>
                      <td class="data-item" >
                          <asp:CheckBox ID="cbTicketAlerts" runat="server" Enabled="false" />
                      </td>
                   </tr>
                </table>
              </td>
              <td>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("opening-hours.aspx") %>' ><u><%= base.GetLocaleResourceString("hlOpeningHoursEdit.Text") %></u></a></p>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("holidays.aspx") %>' ><u><%= base.GetLocaleResourceString("hlHolidays.Text") %></u></a></p>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("settingcompany-edit.aspx") %>' ><u>Change Company Name</u></a></p>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("settingvat-edit.aspx") %>' ><u><%= base.GetLocaleResourceString("hlVAT.Text") %></u></a></p>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("ticket-alerts.aspx") %>' ><u>Ticket Alerts</u></a></p>
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>
