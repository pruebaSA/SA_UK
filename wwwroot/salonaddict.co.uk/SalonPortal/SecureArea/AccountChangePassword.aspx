<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="AccountChangePassword.aspx.cs" Inherits="SalonPortal.SecureArea.AccountChangePassword" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
    <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-account.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div class="options">
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
        </div>
    </div>
    <SA:Message ID="lblMessage" runat="server" />
    <ajaxToolkit:TabContainer runat="server" ID="PasswordTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlPassword" >
            <ContentTemplate>
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblNewPassword" 
                                runat="server" 
                                meta:resourceKey="lblNewPassword"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtNewPassword" 
                                runat="server" 
                                TextMode="Password"
                                meta:resourceKey="txtNewPassword"
                                MaxLength="50" >
                            </SA:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblConfirmNewPassword" 
                                runat="server" 
                                meta:resourceKey="lblConfirmNewPassword"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtConfirmNewPassword" 
                                runat="server" 
                                TextMode="Password"
                                meta:resourceKey="txtConfirmNewPassword"
                                MaxLength="50" >
                            </SA:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblPassword" 
                                runat="server" 
                                meta:resourceKey="lblPassword"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtPassword" 
                                runat="server" 
                                TextMode="Password"
                                meta:resourceKey="txtPassword"
                                MaxLength="50" >
                            </SA:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
         </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Content>
