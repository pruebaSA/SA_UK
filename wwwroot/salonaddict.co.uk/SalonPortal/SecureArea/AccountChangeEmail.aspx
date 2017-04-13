<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="AccountChangeEmail.aspx.cs" Inherits="SalonPortal.SecureArea.AccountChangeEmail" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/SecureArea/Modules/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
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
    <ajaxToolkit:TabContainer runat="server" ID="ChangeEmailTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlEmail" >
            <ContentTemplate>
                <table class="details" cellpadding="0" cellspacing="0" >
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
                                MaxLength="100" >
                            </SA:EmailTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblConfirmEmail" 
                                runat="server" 
                                meta:resourceKey="lblConfirmEmail"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <span class="required" >*</span>
                        </td>
                        <td class="data-item">
                            <SA:EmailTextBox 
                                ID="txtConfirmEmail" 
                                runat="server" 
                                MaxLength="100" >
                            </SA:EmailTextBox>
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
