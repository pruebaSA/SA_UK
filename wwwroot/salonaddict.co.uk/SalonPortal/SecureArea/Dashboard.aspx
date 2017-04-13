<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SalonPortal.SecureArea.Dashboard" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
   <div style="position:relative" class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-dashboard.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div style="position:absolute;top:5px;right:10px;" >
            <%= base.GetLocalResourceObject("Label.View") %>
            <b class="highlight" ><%= base.GetLocalResourceObject("Label.View.Today") %></b> | 
            <asp:LinkButton ID="lbViewAll" runat="server" OnClick="lbViewAll_Click" >
               <%= base.GetLocalResourceObject("Label.View.All") %>
            </asp:LinkButton>
        </div>
   </div>
<SA:Message ID="lblRecordCount" runat="server" IsError="false" Auto="false" />
<asp:GridView 
    ID="gvAppointments" 
    runat="server" 
    AutoGenerateColumns="False"
    Width="100%"
    SkinID="dashboard"
    CellPadding="0"
    CellSpacing="0"
    DataKeyNames="AppointmentID"
    OnRowCommand="gvAppointments_OnRowCommand">
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate>
                <%= base.GetLocalResourceObject("gvAppointments.Columns[0].HeaderText") %>
            </HeaderTemplate>
            <ItemTemplate>
                <table cellpadding="0" cellspacing="0" Width="700px"  >
                   <tr>
                      <td width="20px" rowspan="4">
                         <b><%# this.gvAppointments.Rows.Count + 1 %>.</b>
                      </td>
                      <td width="140px" class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Day") %></td>
                      <td width="160px" class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Customer") %></td>
                      <td width="160px" class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Stylist") %></td>
                      <td width="100px" class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Balance") %></td>
                      <td class="col-style">
                         <asp:Button ID="btnComplete" runat="server" SkinID="ButtonSmallPink" Width="80px" meta:resourceKey="btnComplete" CommandName="MCOMP" CommandArgument='<%# Eval("AppointmentID") %>'  />
                      </td>
                   </tr>
                   <tr>
                      <td class="altcol-style"><%= base.GetLocalResourceObject("gvAppointments.Label.Today") %></td>
                      <td class="altcol-style" ><%# Eval("BillingFullName") %></td>
                      <td class="altcol-style"><%# Eval("StaffName") %></td>
                      <td class="altcol-style">
                            <%# SalonAddict.BusinessAccess.Implementation.CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("BalanceAmount").ToString()), false) %>
                      </td>
                      <td class="altcol-style">
                         <asp:Button ID="btnCancel" runat="server" SkinID="ButtonSmallGrey" Width="80px" meta:resourceKey="btnCancel" CommandName="MC" CommandArgument='<%# Eval("AppointmentID") %>' />
                      </td>
                   </tr>
                   <tr>
                      <td class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Time") %></td>
                      <td class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Ref") %></td>
                      <td class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Service") %></td>
                      <td class="col-style"></td>
                      <td class="col-style"></td>
                   </tr>
                   <tr>
                      <td class="altcol-style"><%# ((DateTime)Eval("Time.StartsOn")).ToShortTimeString() %></td>
                      <td class="altcol-style"><%# Eval("OrderReferenceNo") %></td>
                      <td class="altcol-style"><%# Eval("ServiceName") %></td>
                      <td class="altcol-style"></td>
                      <td style="text-align:right;" class="altcol-style">
                        <a href="AppointmentDetails.aspx?OrderGUID=<%# Eval("OrderGUID") %>&AppointmentID=<%# Eval("AppointmentID") %>" >
                            <%= base.GetGlobalResourceObject("Global", "GridView_Link_Details").ToString() %>
                        </a>
                      </td>
                   </tr>
                </table>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
