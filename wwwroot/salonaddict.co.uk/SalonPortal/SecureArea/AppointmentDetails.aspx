<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="AppointmentDetails.aspx.cs" Inherits="SalonPortal.SecureArea.Appointment_Details" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/SecureArea/Modules/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="AppointmentStatusList" Src="~/SecureArea/Modules/AppointmentStatusDropDownList.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

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
   </div>
   <ajaxToolkit:TabContainer runat="server" ID="AppointmentTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlAppointment" >
            <ContentTemplate>
                <table class="details">
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblReferenceNo" 
                            runat="server" 
                            meta:resourceKey="lblReferenceNo"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtReferenceNo" runat="server" ></asp:Label>
                    </td>
                 </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblDate" 
                            runat="server" 
                            meta:resourceKey="lblDate"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtDate" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblTime" 
                            runat="server" 
                            meta:resourceKey="lblTime"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtTime" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCustomer" 
                            runat="server" 
                            meta:resourceKey="lblCustomer"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtCustomer" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblPhone" 
                            runat="server" 
                            meta:resourceKey="lblPhone"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtPhone" runat="server" ></asp:Label>
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
                        <asp:Label ID="txtEmail" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblStaff" 
                            runat="server" 
                            meta:resourceKey="lblStaff"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtStaff" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblService" 
                            runat="server" 
                            meta:resourceKey="lblService"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtService" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblBalance" 
                            runat="server" 
                            meta:resourceKey="lblBalance"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtBalance" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblStatus" 
                            runat="server" 
                            meta:resourceKey="lblStatus"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtStatus" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblComment" 
                            runat="server" 
                            meta:resourceKey="lblComment"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Label ID="txtComment" runat="server" ></asp:Label>
                    </td>
                </tr>
               </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
   </ajaxToolkit:TabContainer>
</asp:Content>
