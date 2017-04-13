<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="AccountDetails.aspx.cs" Inherits="SalonPortal.SecureArea.AccountDetails" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/SecureArea/Modules/NumericTextBox.ascx" %>
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
    <ajaxToolkit:TabContainer runat="server" ID="UserTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlUserInfo" >
            <ContentTemplate>
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblUsername" 
                                runat="server" 
                                meta:resourceKey="lblUsername"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:Label ID="txtUsername" runat="server" SkinID="DisabledTextBox" ></asp:Label>
                        </td>
                    </tr>
                     <tr>
                        <td style="vertical-align:top;" class="title">
                            <SA:ToolTipLabel 
                                ID="lblEmail" 
                                runat="server" 
                                meta:resourceKey="lblEmail"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:Label ID="txtEmail" runat="server" SkinID="DisabledTextBox" ></asp:Label>
                            <asp:HyperLink ID="hlChangeEmail" runat="server" NavigateUrl="AccountChangeEmail.aspx" meta:resourceKey="hlChangeEmail" ></asp:HyperLink>
                        </td>
                    </tr>
                     <tr>
                        <td style="vertical-align:top;" class="title">
                            <SA:ToolTipLabel 
                                ID="lblPassword" 
                                runat="server" 
                                meta:resourceKey="lblPassword"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:Label ID="txtPassword" runat="server" SkinID="DisabledTextBox" ></asp:Label>
                            <asp:HyperLink ID="hlChangePassword" runat="server" NavigateUrl="AccountChangePassword.aspx" meta:resourceKey="hlChangePassword" ></asp:HyperLink>
                        </td>
                    </tr>
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
                            <SA:TextBox 
                                ID="txtFirstName" 
                                runat="server" 
                                meta:resourceKey="txtFirstName"
                                MaxLength="100" >
                            </SA:TextBox>
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
                            <SA:TextBox 
                                ID="txtLastName" 
                                runat="server" 
                                meta:resourceKey="txtLastName"
                                MaxLength="100" >
                            </SA:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblPhoneNumber" 
                                runat="server" 
                                meta:resourceKey="lblPhoneNumber"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtPhoneNumber" runat="server" MaxLength="50" ></asp:TextBox>
                        </td>
                    </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblMobile"
                            runat="server"  
                            meta:resourceKey="lblMobile"
                            ToolTipImage="~/SecureArea/images/ico-help.gif" />
                    </td>
                    <td class="data-item" >
                        <table cellpadding="0" cellspacing="0" >
                           <tr>
                              <td>
                                <asp:DropDownList ID="ddlAreaCode" runat="server" >
                                   <asp:ListItem Text="--" Value="" ></asp:ListItem>
                                   <asp:ListItem Text="074" Value="074" ></asp:ListItem>
                                   <asp:ListItem Text="075" Value="075" ></asp:ListItem>
                                   <asp:ListItem Text="077" Value="077" ></asp:ListItem>
                                   <asp:ListItem Text="078" Value="078" ></asp:ListItem>
                                   <asp:ListItem Text="079" Value="079" ></asp:ListItem>
                                </asp:DropDownList>
                              </td>
                              <td style="padding-left:5px;">
                                <SA:TextBox 
                                    ID="txtMobile"
                                    runat="server"
                                    Width="120px"
                                    MaxLength="8"
                                    IsRequired="false"
                                    meta:resourceKey="lblMobile"
                                    ValidationExpression="\d{8}" >
                                </SA:TextBox>
                              </td>
                           </tr>
                        </table>
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
                            <asp:Label ID="txtStatus" runat="server" Font-Bold="true" ></asp:Label>
                        </td>
                    </tr>
                 </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
     </ajaxToolkit:TabContainer>
</asp:Content>
