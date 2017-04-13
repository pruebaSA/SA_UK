<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="SupportTicket.aspx.cs" Inherits="SalonPortal.SecureArea.SupportTicket" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/SecureArea/Modules/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
    <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-support.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div class="options">
            <asp:Button ID="btnSend" runat="server" OnClick="btnSend_Click" meta:resourceKey="btnSend" />
        </div>
    </div>
    <SA:Message ID="lblMessage" runat="server" IsError="false" />
    <ajaxToolkit:TabContainer runat="server" ID="ContactTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlContactInfo" >
            <ContentTemplate>
                <p>
                   <%= base.GetLocalResourceObject("Text.Instructions") %>
                </p>
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDepartment" 
                                runat="server" 
                                meta:resourceKey="lblDepartment"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <asp:DropDownList ID="ddlDepartment" runat="server" ></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvDepartment" runat="server" ControlToValidate="ddlDepartment" Display="None" meta:resourceKey="rfvDepartment" ></asp:RequiredFieldValidator>
                            <ajaxToolkit:ValidatorCalloutExtender ID="rfvDepartmentE" runat="Server" TargetControlID="rfvDepartment" HighlightCssClass="validator-highlight" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblName" 
                                runat="server" 
                                meta:resourceKey="lblName"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtName" 
                                runat="server" 
                                meta:resourceKey="txtName"
                                MaxLength="100" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblEmail" 
                                runat="server" 
                                meta:resourceKey="lblEmail"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <SA:EmailTextBox 
                                ID="txtEmail" 
                                runat="server" 
                                meta:resourceKey="txtEmail"
                                MaxLength="100" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblSubject" 
                                runat="server" 
                                meta:resourceKey="lblSubject"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtSubject" 
                                runat="server" 
                                meta:resourceKey="txtSubject"
                                MaxLength="200" />
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align:top;" class="title">
                            <SA:ToolTipLabel 
                                ID="lblBody" 
                                runat="server" 
                                meta:resourceKey="lblBody"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <asp:TextBox 
                                ID="txtBody" 
                                style="width:400px;height:80px"
                                Width="400px"
                                Height="80px"
                                runat="server"
                                TextMode="MultiLine" >
                            </asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvBody" runat="server" ControlToValidate="txtBody" Display="None" meta:resourceKey="rfvBody" ></asp:RequiredFieldValidator>
                            <ajaxToolkit:ValidatorCalloutExtender ID="rfvBodyE" runat="Server" TargetControlID="rfvBody" HighlightCssClass="validator-highlight" />
                        </td>
                    </tr>
                </table>
                <br />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Content>
