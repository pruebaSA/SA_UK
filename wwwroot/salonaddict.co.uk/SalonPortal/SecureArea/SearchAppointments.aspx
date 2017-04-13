<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="SearchAppointments.aspx.cs" Inherits="SalonPortal.SecureArea.Search_Appointments" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/SecureArea/Modules/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="AppointmentStatusList" Src="~/SecureArea/Modules/AppointmentStatusDropDownList.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>
<%@ Import Namespace="SalonAddict.BusinessAccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
   <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-report.gif") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div class="options">
            <asp:Button ID="btnAdd" runat="server" CausesValidation="false" OnClick="btnAdd_Click" meta:resourceKey="btnAdd" />
            <asp:Button ID="btnSearch" runat="server" ValidationGroup="Search" OnClick="btnSearch_Click" meta:resourceKey="btnSearch" />
        </div>
    </div>
    <table class="details" cellpadding="0" cellspacing="0" >
        <tr>
            <td class="title">
                <SA:ToolTipLabel 
                    ID="lblStartDate" 
                    runat="server" 
                    meta:resourceKey="lblStartDate"
                    ToolTipImage="~/SecureArea/images/ico-help.gif" />
                <span class="required" >*</span>
            </td>
            <td class="data-item">
                <asp:TextBox runat="server" ID="txtStartDate" />
                <asp:ImageButton ID="iStartDate" runat="Server" ImageUrl="~/SecureArea/images/ico-calendar.png" /><br />
                <ajaxToolkit:CalendarExtender ID="cStartDateButtonExtender" runat="server" TargetControlID="txtStartDate" PopupButtonID="iStartDate" />
                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="txtStartDate" Display="None" ValidationGroup="Search" ></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvStartDateE" TargetControlID="rfvStartDate" HighlightCssClass="validator-highlight" />
                <asp:CompareValidator ID="cvStartDate" runat="server" ControlToValidate="txtStartDate" Operator="DataTypeCheck" ValidationGroup="Search" Display="None" ></asp:CompareValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="cvStartDateE" TargetControlID="cvStartDate" HighlightCssClass="validator-highlight" />
            </td>
        </tr>
        <tr>
            <td class="title">
                <SA:ToolTipLabel 
                    ID="lblEndDate" 
                    runat="server" 
                    meta:resourceKey="lblEndDate"
                    ToolTipImage="~/SecureArea/images/ico-help.gif" />
                <span class="required" >*</span>
            </td>
            <td class="data-item">
                <asp:TextBox ID="txtEndDate" runat="server"  />
                <asp:ImageButton ID="iEndDate" runat="Server" ImageUrl="~/SecureArea/images/ico-calendar.png" /><br />
                <ajaxToolkit:CalendarExtender ID="cEndDateButtonExtender" runat="server" TargetControlID="txtEndDate" PopupButtonID="iEndDate" />
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="txtEndDate" Display="None" ValidationGroup="Search" ></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvEndDateE" TargetControlID="rfvEndDate" HighlightCssClass="validator-highlight" />
                <asp:CompareValidator ID="cvEndDate" runat="server" ControlToValidate="txtEndDate" Operator="DataTypeCheck" ValidationGroup="Search" Display="None" ></asp:CompareValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="cvEndDateE" TargetControlID="cvEndDate" HighlightCssClass="validator-highlight" />
            </td>
        </tr>
        <tr>
            <td class="title">
                <SA:ToolTipLabel 
                    ID="lblEmail" 
                    runat="server" 
                    meta:resourceKey="lblEmail"
                    ToolTipImage="~/SecureArea/images/ico-help.gif" />
            </td>
            <td class="data-item">
                <SA:EmailTextBox 
                    ID="txtEmail" 
                    runat="server" 
                    MaxLength="120" 
                    IsRequired="false" 
                    ValidationGroup="Search" />
            </td>
        </tr>
        <tr>
            <td class="title">
                <SA:ToolTipLabel 
                    ID="lblAppointmentStatus" 
                    runat="server" 
                    meta:resourceKey="lblAppointmentStatus"
                    ToolTipImage="~/SecureArea/images/ico-help.gif" />
            </td>
            <td class="data-item">
                <SA:AppointmentStatusList ID="ddlAppointmentStatus" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="title">
                <SA:ToolTipLabel 
                    ID="lblReferenceNo" 
                    runat="server" 
                    meta:resourceKey="lblReferenceNo"
                    ToolTipImage="~/SecureArea/images/ico-help.gif" />
            </td>
            <td class="data-item">
               <SA:TextBox 
                 ID="txtReferenceNo" 
                 runat="server" 
                 Width="180px"
                 MaxLength="12"
                 meta:resourcekey="txtReferenceNo"
                 ValidationGroup="SearchByReference" />
               &nbsp;
               <asp:Button ID="btnGo" runat="server" Text="Go" OnClick="btnGo_Click" ValidationGroup="SearchByReference" />
            </td>
        </tr>
    </table>
<br />
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
    OnRowCommand="gvAppointments_OnRowCommand" 
    AllowPaging="true"
    PageSize="15"
    OnPageIndexChanging="gvAppointments_OnPageIndexChanging" >
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate>
                <%= base.GetLocalResourceObject("gvAppointments.Columns[0].HeaderText") %>
                <asp:Label ID="lblResults" runat="server" ></asp:Label>
            </HeaderTemplate>
            <ItemTemplate>
                <table cellpadding="0" cellspacing="0" Width="700px"  >
                   <tr>
                      <td width="20px" rowspan="4">
                         <b><%# this.gvAppointments.PageIndex * this.gvAppointments.PageSize + this.gvAppointments.Rows.Count + 1 %>.</b>
                      </td>
                      <td width="140px" class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Day") %></td>
                      <td width="160px" class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Customer") %></td>
                      <td width="160px" class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Stylist") %></td>
                      <td width="100px" class="col-style"><%= base.GetLocalResourceObject("gvAppointments.Header.Balance") %></td>
                      <td class="col-style">
                         <asp:MultiView ID="mvStatus1" runat="server" ActiveViewIndex='<%# (((AppointmentStatusType)Eval("AppointmentStatus")) == AppointmentStatusType.Scheduled)? 0 : 1 %>' >
                            <asp:View ID="v1" runat="server" >
                               <asp:Button ID="btnComplete" runat="server" SkinID="ButtonSmallPink" Text="Completed" Width="80px" meta:resourceKey="btnComplete" CommandName="MCOMP" CommandArgument='<%# Eval("AppointmentID") %>' />
                            </asp:View>
                            <asp:View ID="v2" runat="server" >
                                <%= base.GetLocalResourceObject("gvAppointments.Header.Status")%>
                            </asp:View>
                         </asp:MultiView>
                      </td>
                   </tr>
                   <tr>
                      <td class="altcol-style"><%# ((DateTime)Eval("Time.StartsOn")).ToShortDateString() %></td>
                      <td class="altcol-style" ><%# Eval("BillingFullName") %></td>
                      <td class="altcol-style"><%# Eval("StaffName") %></td>
                      <td class="altcol-style">
                            <%# SalonAddict.BusinessAccess.Implementation.CurrencyManager.FormatPrice(Convert.ToDecimal(Eval("BalanceAmount").ToString()), false) %>
                      </td>
                      <td class="altcol-style">
                         <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex='<%# (((AppointmentStatusType)Eval("AppointmentStatus")) == AppointmentStatusType.Scheduled)? 0 : 1 %>' >
                            <asp:View ID="View1" runat="server" >
                               <asp:Button ID="btnCancel" runat="server" SkinID="ButtonSmallGrey" Width="80px" meta:resourceKey="btnCancel" CommandName="MC" CommandArgument='<%# Eval("AppointmentID") %>' />
                            </asp:View>
                            <asp:View ID="View2" runat="server" >
                                <b><%# GetAppointmentStatusLocalizedText((AppointmentStatusType)Eval("AppointmentStatus")) %></b>
                            </asp:View>
                         </asp:MultiView>
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