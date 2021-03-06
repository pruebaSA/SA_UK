﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="DayView.aspx.cs" Inherits="IFRAME.DayViewPage" %>
<%@ Register TagPrefix="IFRM" TagName="ViewTypes" Src="~/Modules/ViewTypeMenu.ascx" %>
<%@ Register TagPrefix="IFRM" TagName="TimeSlotsGrid" Src="~/Modules/TimeSlotsGrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <h1><asp:Literal ID="ltrHeader" runat="server" ></asp:Literal></h1>
        <IFRM:ViewTypes ID="cntlViewTypes" runat="server" SelectedView="Day" />
        <asp:Panel ID="pnle" runat="server" >
            <table class="details" cellpadding="0" cellspacing="0" >
               <tr>
                  <td style="min-width:80px" class="title" >
                     <%= base.GetLocaleResourceString("ltrEmployee.Text") %>
                  </td>
                  <td class="data-item" >
                      <asp:DropDownList ID="ddlEmployee" runat="server" SkinID="DropDownList" Width="160px" AutoPostBack="true" OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged" ></asp:DropDownList>
                  </td>
               </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlt" runat="server" SkinID="AvailableTimesPanel" >
            <IFRM:TimeSlotsGrid ID="DayTimes" runat="server" OnTimeSelected="DayTimes_TimeSelected"/>
        </asp:Panel>
        <table class="details" cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td class="title" >
                  <i style="font-size:90%" ><%= base.GetLocaleResourceString("ltrInstructions.Text") %></i>
              </td>
              <td style="text-align:right" class="data-item" >
                  <a href='<%= IFRMHelper.GetURL(Page.ResolveUrl("~/"), String.Format("{0}={1}", Constants.QueryStrings.SERVICE_ID, this.PostedServiceId)) %>' ><u><%= base.GetLocaleResourceString("hlChangeSearch.Text") %></u></a>
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>