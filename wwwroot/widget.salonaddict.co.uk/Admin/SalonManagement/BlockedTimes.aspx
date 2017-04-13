<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="BlockedTimes.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.BlockedTimesPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="DateTextBox" Src="~/Modules/DateTextBox.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="BlockedTimeLine" Src="~/Admin/Modules/BlockedTimeLine.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Scheduling" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-scheduling.png" %>' alt="scheduling" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                   <IFRM:DateTextBox ID="txtDate" runat="server" AutoPostBack="true" OnTextChanged="txtDate_TextChanged" meta:resourceKey="txtDate" />
              </td>
              <td style="vertical-align:middle" >
                 <% if(txtDate.Date.AddDays(-7) > DateTime.Today)
                    { %>
                 <a href='<%= IFRMHelper.GetURL("BlockedTimes.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId), String.Format("{0}={1}", Constants.QueryStrings.DATE, IFRMHelper.ToUrlFriendlyDate(txtDate.Date.AddDays(-7)))) %>' >&lt;</a>
                 <% } %> 
                 <%= base.GetLocaleResourceString("hlWeek.Text") %> <a href='<%= IFRMHelper.GetURL("BlockedTimes.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId), String.Format("{0}={1}", Constants.QueryStrings.DATE, IFRMHelper.ToUrlFriendlyDate(txtDate.Date.AddDays(7)))) %>' >&gt;</a>
              </td>
           </tr>
        </table>
        <asp:Panel ID="pnlt" runat="server" SkinID="SchedulingPanel" >
        <table cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:450px" >
                <div style="margin-bottom:5px;" >
                    <IFRM:BlockedTimeLine ID="cntrlDay1" runat="server" OnItemEditing="BlockedTime_ItemEditing" />
                </div>
                <div style="margin-bottom:5px;" >
                    <IFRM:BlockedTimeLine ID="cntrlDay2" runat="server" OnItemEditing="BlockedTime_ItemEditing" />
                </div>
                <div style="margin-bottom:5px;" >
                    <IFRM:BlockedTimeLine ID="cntrlDay3" runat="server" OnItemEditing="BlockedTime_ItemEditing" />
                </div>
                <div style="margin-bottom:5px;" >
                    <IFRM:BlockedTimeLine ID="cntrlDay4" runat="server" OnItemEditing="BlockedTime_ItemEditing" />
                </div>
                <div style="margin-bottom:5px;" >
                    <IFRM:BlockedTimeLine ID="cntrlDay5" runat="server" OnItemEditing="BlockedTime_ItemEditing" />
                </div>
                <div style="margin-bottom:5px;" >
                    <IFRM:BlockedTimeLine ID="cntrlDay6" runat="server" OnItemEditing="BlockedTime_ItemEditing" />
                </div>
                <div>
                    <IFRM:BlockedTimeLine ID="cntrlDay7" runat="server" OnItemEditing="BlockedTime_ItemEditing" />
                </div>
              </td>
              <td style="vertical-align:top;" >
                 <ul>
                    <li>
                       <p><asp:Literal ID="ltrHelp" runat="server" ></asp:Literal></p>
                    </li>
                    <li><p><a href='<%= IFRMHelper.GetURL("scheduling-readonly.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId)) %>' ><b><u><%= base.GetLocaleResourceString("hlViewAvailability.Text") %></u></b></a></p></li>
                    <li><p><a href='<%= IFRMHelper.GetURL("opening-hours.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId), String.Format("{0}={1}", Constants.QueryStrings.RETURN_URL, System.Web.HttpUtility.UrlEncode(Request.Url.PathAndQuery))) %>' ><b><u><%= base.GetLocaleResourceString("hlEditOpeningHours.Text") %></u></b></a></p></li>
                 </ul>
              </td>
           </tr>
        </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>