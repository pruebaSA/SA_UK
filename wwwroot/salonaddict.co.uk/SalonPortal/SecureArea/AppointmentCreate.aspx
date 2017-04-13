<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="AppointmentCreate.aspx.cs" Inherits="SalonPortal.SecureArea.AppointmentCreate" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/SecureArea/Modules/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="StaffList" Src="~/SecureArea/Modules/StaffDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="ServiceList" Src="~/SecureArea/Modules/StaffServiceDropDownList.ascx" %>
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
        <div class="options">
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
        </div>
   </div>
   <SA:Message ID="lblMessage" runat="server" Auto="false" />
   <ajaxToolkit:TabContainer runat="server" ID="AppointmentTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlAppointment" >
            <ContentTemplate>
                <asp:UpdatePanel ID="up" runat="server" >
                <ContentTemplate>
                <table class="details">
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblFirstName" 
                            runat="server" 
                            meta:resourceKey="lblFirstName"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        <span class="required" >*</span>
                    </td>
                    <td class="data-item">
                        <SA:TextBox ID="txtFirstName" runat="server" MaxLength="50" meta:resourceKey="txtFirstName" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblLastName" 
                            runat="server" 
                            meta:resourceKey="lblLastName"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        <span class="required" >*</span>
                    </td>
                    <td class="data-item">
                        <SA:TextBox ID="txtLastName" runat="server" MaxLength="50" meta:resourceKey="txtLastName" />
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
                        <asp:TextBox ID="txtPhone" runat="server" MaxLength="50" ></asp:TextBox>
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
                            IsRequired="false"
                            MaxLength="120" >
                        </SA:EmailTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblStartTime" 
                            runat="server" 
                            meta:resourceKey="lblStartTime"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        <span class="required" >*</span>
                    </td>
                    <td class="data-item">
                        <table cellpadding="0" cellspacing="0" >
                           <tr>
                              <td>
                                <asp:DropDownList ID="ddlStartTimeHour" runat="server" >
                                   <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                   <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                   <asp:ListItem Text="09" Value="09" Selected="True" ></asp:ListItem>
                                   <asp:ListItem Text="10" Value="10" ></asp:ListItem>
                                   <asp:ListItem Text="11" Value="11" ></asp:ListItem>
                                   <asp:ListItem Text="12" Value="12" ></asp:ListItem>
                                   <asp:ListItem Text="13" Value="13" ></asp:ListItem>
                                   <asp:ListItem Text="14" Value="14" ></asp:ListItem>
                                   <asp:ListItem Text="15" Value="15" ></asp:ListItem>
                                   <asp:ListItem Text="16" Value="16" ></asp:ListItem>
                                   <asp:ListItem Text="17" Value="17" ></asp:ListItem>
                                   <asp:ListItem Text="18" Value="18" ></asp:ListItem>
                                   <asp:ListItem Text="19" Value="19" ></asp:ListItem>
                                   <asp:ListItem Text="20" Value="20" ></asp:ListItem>
                                </asp:DropDownList>
                              </td>
                              <td>&nbsp;:&nbsp;</td>
                              <td>
                                 <asp:DropDownList ID="ddlStartTimeMinute" runat="server" >
                                    <asp:ListItem Text="00" Value="00" Selected="True" ></asp:ListItem>
                                    <asp:ListItem Text="05" Value="05" ></asp:ListItem>
                                    <asp:ListItem Text="10" Value="10" ></asp:ListItem>
                                    <asp:ListItem Text="15" Value="15" ></asp:ListItem>
                                    <asp:ListItem Text="20" Value="20" ></asp:ListItem>
                                    <asp:ListItem Text="25" Value="25" ></asp:ListItem>
                                    <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                    <asp:ListItem Text="35" Value="35" ></asp:ListItem>
                                    <asp:ListItem Text="40" Value="40" ></asp:ListItem>
                                    <asp:ListItem Text="45" Value="45" ></asp:ListItem>
                                    <asp:ListItem Text="50" Value="50" ></asp:ListItem>
                                    <asp:ListItem Text="55" Value="55" ></asp:ListItem>
                                 </asp:DropDownList>
                              </td>
                           </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblEndTime" 
                            runat="server" 
                            meta:resourceKey="lblEndTime"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        <span class="required" >*</span>
                    </td>
                    <td class="data-item">
                        <table cellpadding="0" cellspacing="0" >
                           <tr>
                              <td>
                                <asp:DropDownList ID="ddlEndTimeHour" runat="server" >
                                   <asp:ListItem Text="07" Value="07" ></asp:ListItem>
                                   <asp:ListItem Text="08" Value="08" ></asp:ListItem>
                                   <asp:ListItem Text="09" Value="09" Selected="True" ></asp:ListItem>
                                   <asp:ListItem Text="10" Value="10" ></asp:ListItem>
                                   <asp:ListItem Text="11" Value="11" ></asp:ListItem>
                                   <asp:ListItem Text="12" Value="12" ></asp:ListItem>
                                   <asp:ListItem Text="13" Value="13" ></asp:ListItem>
                                   <asp:ListItem Text="14" Value="14" ></asp:ListItem>
                                   <asp:ListItem Text="15" Value="15" ></asp:ListItem>
                                   <asp:ListItem Text="16" Value="16" ></asp:ListItem>
                                   <asp:ListItem Text="17" Value="17" ></asp:ListItem>
                                   <asp:ListItem Text="18" Value="18" ></asp:ListItem>
                                   <asp:ListItem Text="19" Value="19" ></asp:ListItem>
                                   <asp:ListItem Text="20" Value="20" ></asp:ListItem>
                                </asp:DropDownList>
                              </td>
                              <td>&nbsp;:&nbsp;</td>
                              <td>
                                 <asp:DropDownList ID="ddlEndTimeMinute" runat="server" >
                                    <asp:ListItem Text="00" Value="00" Selected="True" ></asp:ListItem>
                                    <asp:ListItem Text="05" Value="05" ></asp:ListItem>
                                    <asp:ListItem Text="10" Value="10" ></asp:ListItem>
                                    <asp:ListItem Text="15" Value="15" ></asp:ListItem>
                                    <asp:ListItem Text="20" Value="20" ></asp:ListItem>
                                    <asp:ListItem Text="25" Value="25" ></asp:ListItem>
                                    <asp:ListItem Text="30" Value="30" ></asp:ListItem>
                                    <asp:ListItem Text="35" Value="35" ></asp:ListItem>
                                    <asp:ListItem Text="40" Value="40" ></asp:ListItem>
                                    <asp:ListItem Text="45" Value="45" ></asp:ListItem>
                                    <asp:ListItem Text="50" Value="50" ></asp:ListItem>
                                    <asp:ListItem Text="55" Value="55" ></asp:ListItem>
                                 </asp:DropDownList>
                              </td>
                           </tr>
                        </table>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblDate" 
                            runat="server" 
                            meta:resourceKey="lblDate"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        <span class="required" >*</span>
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtDate" runat="server" ></asp:TextBox>
                        <ajaxToolkit:CalendarExtender ID="ceDateButtonExtender" runat="server" TargetControlID="txtDate" />
                        <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtDate" Display="None" ></asp:RequiredFieldValidator>
                        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvDateE" TargetControlID="rfvDate" HighlightCssClass="validator-highlight" />
                        <asp:CompareValidator ID="cvDate" runat="server" ControlToValidate="txtDate" Operator="DataTypeCheck" Display="None" ></asp:CompareValidator>
                        <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="cvDateE" TargetControlID="cvDate" HighlightCssClass="validator-highlight" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblStaff" 
                            runat="server" 
                            meta:resourceKey="lblStaff"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                       <span class="required" >*</span>
                    </td>
                    <td class="data-item">
                        <SA:StaffList 
                                ID="ddlStaff" 
                                runat="server" 
                                AutoPostback="true"
                                Width="200px"
                                OnSelectedIndexChanged="ddlStaff_OnSelectedIndexChanged"
                                meta:resourceKey="ddlStaff" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblService" 
                            runat="server" 
                            meta:resourceKey="lblService"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        <span class="required" >*</span>
                    </td>
                    <td class="data-item">
                        <SA:ServiceList ID="ddlStaffService" Width="200px" runat="server" meta:resourceKey="ddlStaffService" />
                    </td>
                </tr>
               </table>
                </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
   </ajaxToolkit:TabContainer>
</asp:Content>
