<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Scheduling.aspx.cs" Inherits="IFRAME.Admin.SalonManagement.Scheduling_Page" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/Admin/Modules/SalonMenu.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="SchedulingLine" Src="~/Admin/Modules/SchedulingLine.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Scheduling" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../../App_Themes/" + base.Theme + "/images/overview-scheduling.png" %>' alt="scheduling" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >
                  <asp:Button ID="btnBack" runat="server" SkinID="SubmitButton" OnClick="btnBack_Click" meta:resourceKey="btnBack" />
              </td>
           </tr>
        </table>
        <asp:Panel ID="pnlt" runat="server" SkinID="SchedulingPanel" >
            <table cellpadding="0" cellspacing="0" width="100%" >
               <tr>
                  <td style="width:525px" >
                     <div style="margin-bottom:5px;" >
                        <IFRM:SchedulingLine ID="cntrlMonday" runat="server" DayOfWeek="Monday" OnItemInserting="SchedulingLine_ItemInserting" OnItemUpdating="SchedulingLine_ItemUpdating" OnItemDeleting="SchedulingLine_ItemDeleting" />
                     </div>
                     <div style="margin-bottom:5px;" >
                        <IFRM:SchedulingLine ID="cntrlTuesday" runat="server" DayOfWeek="Tuesday" OnItemInserting="SchedulingLine_ItemInserting" OnItemUpdating="SchedulingLine_ItemUpdating" OnItemDeleting="SchedulingLine_ItemDeleting" />
                     </div>
                     <div style="margin-bottom:5px;" >
                        <IFRM:SchedulingLine ID="cntrlWednesday" runat="server" DayOfWeek="Wednesday" OnItemInserting="SchedulingLine_ItemInserting" OnItemUpdating="SchedulingLine_ItemUpdating" OnItemDeleting="SchedulingLine_ItemDeleting" />
                     </div>
                     <div style="margin-bottom:5px;" >
                        <IFRM:SchedulingLine ID="cntrlThursday" runat="server" DayOfWeek="Thursday" OnItemInserting="SchedulingLine_ItemInserting" OnItemUpdating="SchedulingLine_ItemUpdating" OnItemDeleting="SchedulingLine_ItemDeleting" />
                     </div>
                     <div style="margin-bottom:5px;" >
                        <IFRM:SchedulingLine ID="cntrlFriday" runat="server" DayOfWeek="Friday" OnItemInserting="SchedulingLine_ItemInserting" OnItemUpdating="SchedulingLine_ItemUpdating" OnItemDeleting="SchedulingLine_ItemDeleting" />
                     </div>
                     <div style="margin-bottom:5px;" >
                        <IFRM:SchedulingLine ID="cntrlSaturday" runat="server" DayOfWeek="Saturday" OnItemInserting="SchedulingLine_ItemInserting" OnItemUpdating="SchedulingLine_ItemUpdating" OnItemDeleting="SchedulingLine_ItemDeleting" />
                     </div>
                     <div>
                        <IFRM:SchedulingLine ID="cntrlSunday" runat="server" DayOfWeek="Sunday" OnItemInserting="SchedulingLine_ItemInserting" OnItemUpdating="SchedulingLine_ItemUpdating" OnItemDeleting="SchedulingLine_ItemDeleting" />
                     </div>
                  </td>
                  <td style="vertical-align:top;" >
                     <ul>
                        <li><%= base.GetLocaleResourceString("ltrTimeHelp.Text") %></li>
                        <li><%= base.GetLocaleResourceString("ltrSlotsHelp.Text") %></li>
                        <li>
                           <p><a href='<%= IFRMHelper.GetURL("opening-hours.aspx", String.Format("{0}={1}", Constants.QueryStrings.SALON_ID, this.PostedSalonId), String.Format("{0}={1}", Constants.QueryStrings.RETURN_URL, System.Web.HttpUtility.UrlEncode(Request.Url.PathAndQuery))) %>' ><b><u><%= base.GetLocaleResourceString("hlEditOpeningHours.Text") %></u></b></a></p>
                        </li>
                     </ul>
                  </td>
               </tr>
             </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
