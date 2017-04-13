<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoByOne.Master" AutoEventWireup="true"
    CodeBehind="Schedule.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="SalonPortal.SecureArea.SASchedule" %>

<%@ Register TagPrefix="SA" TagName="ScheduleCalendar" Src="~/SecureArea/Modules/CalendarSchedule.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="StaffList" Src="~/SecureArea/Modules/StaffDropDownList.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoByOne.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder"
    runat="server">
    <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
    <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-calendar.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div class="options">
            <asp:Button ID="btnSave" runat="server" meta:resourceKey="btnSave" OnClick="btnSave_Click" ValidationGroup="add" />
        </div>
    </div>
    <SA:Message ID="lblMessage" runat="server" />
    <ajaxToolkit:TabContainer runat="server" ID="ScheduleTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlSchedule">
            <ContentTemplate>
                <table class="details">
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel ID="lblStaff" runat="server" meta:resourceKey="lblStaff" ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required">*</span>
                        </td>
                        <td class="data-item">
                            <SA:StaffList ID="ddlStaff" runat="server" meta:resourceKey="ddlStaff" ValidationGroup="add" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDate" 
                                runat="server" 
                                meta:resourceKey="lblDate"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required">*</span>
                        </td>
                        <td class="data-item">
                            <asp:TextBox runat="server" ID="txtDate" MaxLength="10"  />
                            <asp:ImageButton ID="iDate" runat="Server" ImageUrl="~/SecureArea/images/ico-calendar.png" />
                            <ajaxToolkit:CalendarExtender ID="cDateButtonExtender" runat="server" TargetControlID="txtDate" PopupButtonID="iDate" />
                            <br />
                            <asp:RequiredFieldValidator ID="rfvDate" ControlToValidate="txtDate" runat="server" Display="None" ValidationGroup="add"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cvDate" runat="server" Type="Date" Operator="DataTypeCheck" ControlToValidate="txtDate" Display="None" ValidationGroup="add"></asp:CompareValidator>
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvDateE" TargetControlID="rfvDate" HighlightCssClass="validator-highlight" />
                            <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="cvDateE" TargetControlID="cvDate" HighlightCssClass="validator-highlight" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblStartTime" 
                                runat="server" 
                                meta:resourceKey="lblStartTime"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required">*</span>
                        </td>
                        <td class="data-item">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ddlStartTimeHour" runat="server">
                                            <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                            <asp:ListItem Text="08" Value="08" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="09" Value="09"></asp:ListItem>
                                            <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                            <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                            <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                            <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                            <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                            <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                            <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                            <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                            <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                            <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                            <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                            <asp:ListItem Text="21" Value="21"></asp:ListItem>
                                            <asp:ListItem Text="22" Value="22"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        &nbsp;:&nbsp;
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlStartTimeMinute" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="05" Value="05"></asp:ListItem>
                                            <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                            <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                            <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                            <asp:ListItem Text="25" Value="25"></asp:ListItem>
                                            <asp:ListItem Text="30" Value="30"></asp:ListItem>
                                            <asp:ListItem Text="35" Value="35"></asp:ListItem>
                                            <asp:ListItem Text="40" Value="40"></asp:ListItem>
                                            <asp:ListItem Text="45" Value="45"></asp:ListItem>
                                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                            <asp:ListItem Text="55" Value="55"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel ID="lblEndTime" runat="server" meta:resourceKey="lblEndTime" ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required">*</span>
                        </td>
                        <td class="data-item">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ddlEndTimeHour" runat="server">
                                            <asp:ListItem Text="07" Value="07"></asp:ListItem>
                                            <asp:ListItem Text="08" Value="08"></asp:ListItem>
                                            <asp:ListItem Text="09" Value="09" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                            <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                            <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                            <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                            <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                            <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                            <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                            <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                            <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                            <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                            <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                            <asp:ListItem Text="21" Value="21"></asp:ListItem>
                                            <asp:ListItem Text="22" Value="22"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        &nbsp;:&nbsp;
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlEndTimeMinute" runat="server">
                                            <asp:ListItem Text="00" Value="00" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="05" Value="05"></asp:ListItem>
                                            <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                            <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                            <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                            <asp:ListItem Text="25" Value="25"></asp:ListItem>
                                            <asp:ListItem Text="30" Value="30"></asp:ListItem>
                                            <asp:ListItem Text="35" Value="35"></asp:ListItem>
                                            <asp:ListItem Text="40" Value="40"></asp:ListItem>
                                            <asp:ListItem Text="45" Value="45"></asp:ListItem>
                                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                            <asp:ListItem Text="55" Value="55"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlRecurrence">
            <ContentTemplate>
                <table class="details" cellpadding="0" cellspacing="0" >
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblRecure" 
                                runat="server" 
                                meta:resourceKey="lblRecure" 
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                           <table cellpadding="0" cellspacing="0" width="580px" >
                              <tr>
                                 <td><asp:CheckBox ID="cbMonday" runat="server" meta:resourceKey="cbMonday" /></td>
                                 <td><asp:CheckBox ID="cbTuesday" runat="server" meta:resourceKey="cbTuesday" /></td>
                                 <td><asp:CheckBox ID="cbWednessday" runat="server" meta:resourceKey="cbWednessday" /></td>
                                 <td><asp:CheckBox ID="cbThursday" runat="server" meta:resourceKey="cbThursday" /></td>
                                 <td><asp:CheckBox ID="cbFriday" runat="server" meta:resourceKey="cbFriday" /></td>
                                 <td><asp:CheckBox ID="cbSaturday" runat="server" meta:resourceKey="cbSaturday" /></td>
                                 <td><asp:CheckBox ID="cbSunday" runat="server" meta:resourceKey="cbSunday" /></td>
                              </tr>
                           </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblEndBy" 
                                runat="server" 
                                meta:resourceKey="lblEndBy" 
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item" >
                           <asp:DropDownList ID="ddlDays" runat="server" Width="100px" ></asp:DropDownList>
                        </td>
                   </tr>
                </table>
                <br />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="pnlMaintenance" runat="server" >
           <ContentTemplate>
                <div style="text-align:right;margin-bottom:10px" >
                   <asp:Button ID="btnPurge" runat="server" meta:ResourceKey="btnPurge" OnClick="btnPurge_Click" />
                </div>
                 <%= base.GetLocalResourceObject("Text.PurgeCalendar") %>
           </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="OneRowContentPlaceHolder" runat="server">
    <SA:ScheduleCalendar ID="Calendar" runat="server" />
</asp:Content>
