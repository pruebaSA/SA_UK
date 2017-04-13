<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessUserDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.BusinessUserDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Administration/Modules/TextBoxes/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="BusinessDropDownList" Src="~/Administration/Modules/DropDownLists/BusinessDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="SMSProvidersDropDownList" Src="~/Administration/Modules/DropDownLists/SMSProvidersDropDownList.ascx" %>

<% if(this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="" />
        Add a new business user <a href="BusinessUsers.aspx" title="Back to user list">(back to user list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ToolTip="Add user" />
    </div>
</div>
<% } 
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="" />
        Edit business user details <a href="BusinessUsers.aspx" title="Back to user list">(back to user list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save user changes" />
        <% if (Roles.IsUserInRole("OWNER") || Roles.IsUserInRole("VP_SALES") || Roles.IsUserInRole("SALES_MGR"))
           { %>
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete user" OnClientClick="return confirm('Are you sure?')" />
        <% } %>
    </div>
</div>
<% } %>
<ajaxToolkit:TabContainer runat="server" ID="UserTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlUserInfo" HeaderText="User Info">
        <ContentTemplate>
            <table class="details">
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblLanguage" 
                            runat="server" 
                            Text="Language:"
                            IsRequired="true"
                            ToolTip="Determines the language of the account." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:DropDownList ID="ddlLanguage" runat="server" ></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblBusiness" 
                            runat="server" 
                            Text="Business:"
                            IsRequired="true"
                            ToolTip="The user's business." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:BusinessDropDownList ID="ddlBusiness" runat="server" DefaultText="Choose a Business" DefaultValue="" IsRequired="true" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblFirstName" 
                            runat="server" 
                            Text="First name:"
                            ToolTip="The user's first name (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtFirstName" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblLastName" 
                            runat="server" 
                            Text="Last name:"
                            ToolTip="The customer's last name (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtLastName" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblPhoneNumber" 
                            runat="server" 
                            Text="Phone number:"
                            ToolTip="The user's phone number (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtPhoneNumber" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSMSProvider" 
                            runat="server" 
                            Text="SMS Provider:"
                            ToolTip="SMS Provider to use (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                       <SA:SMSProvidersDropDownList ID="ddlSMSProvider" runat="server" ></SA:SMSProvidersDropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblMobile"
                            runat="server"  
                            Text="Send SMS to:" 
                            ToolTip="Phone number to send the test SMS to"
                            ToolTipImage="~/Administration/images/ico-help.gif" />
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
                                    ValidationExpression="\d{8}"
                                    ValidationMessage="Invalid phone number" >
                                </SA:TextBox>
                              </td>
                           </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblEmail" 
                            runat="server" 
                            Text="Email address:"
                            ToolTip="The user's email address." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:EmailTextBox 
                            ID="txtEmail" 
                            runat="server" 
                            IsRequired="false" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblUsername" 
                            runat="server" 
                            Text="Username:"
                            IsRequired="true"
                            ToolTip="The user's login name." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:UpdatePanel ID="up" runat="server" >
                           <ContentTemplate>
                            <table class="details" cellpadding="0" cellspacing="0" >
                               <tr>
                                  <td>
                                      <SA:TextBox 
                                        ID="txtUsername" 
                                        runat="server" 
                                        MaxLength="80"
                                        ErrorMessage="username is required" />
                                  </td>
                                  <td class="data-item">
                                     <asp:Button ID="btnCheckUsernameAvailability" runat="server" Text="Check Availability" CausesValidation="false" OnClick="btnCheckUsernameAvailability_Click" />
                                  </td>
                                  <td class="data-item" > 
                                      <nowrap><asp:Label ID="lblAvailability" runat="server" Font-Bold="true" ></asp:Label></nowrap>
                                  </td>
                               </tr>
                            </table>
                           </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblPassword" 
                            runat="server" 
                            Text="Password:"
                            IsRequired="true"
                            ToolTip="The user's password." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtPassword" 
                            runat="server" 
                            MaxLength="50"
                            ErrorMessage="Password is required" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSendEmailNotification" 
                            runat="server" 
                            Text="Send Email Notifications:"
                            ToolTip="Determines whether the account received email notifications" 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbSendEmailNotifications" runat="server" Checked="true" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblSendSMSNotification" 
                            runat="server" 
                            Text="Send SMS Notifications:"
                            ToolTip="Determines whether the account received SMS notifications" 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbSendSMSNotifications" runat="server" />
                    </td>
                </tr>
               <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblIsGuest" 
                            runat="server" 
                            Text="Is Guest:"
                            ToolTip="Determines whether the account is a guest account (cookies should be cleared after this setting is applied)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbIsGuest" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblIsAdmin" 
                            runat="server" 
                            Text="Is Administrator:"
                            ToolTip="Determines whether the account is an adminstrator account (cookies should be cleared after this setting is applied). WARNING - THIS ALLOWS ACCESS TO THE ADMINISTRATION AREA OF YOUR STORE." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbIsAdmin" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblIsActive" 
                            runat="server" 
                            Text="Is Active:"
                            ToolTip="Determines whether the account is an active account (cookies should be cleared after this setting is applied)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:CheckBox ID="cbIsActive" runat="server" />
                    </td>
                </tr>
                <% if (this.Action == ActionType.Edit)
                   { %>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCreatedOn" 
                            runat="server" 
                            Text="Registration Date:"
                            ToolTip="Date when the user account was created." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Literal ID="ltrCreatedOn" runat="server" ></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblUpdatedOn" 
                            runat="server" 
                            Text="Last Updated:"
                            ToolTip="Date when the user account was last updated." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Literal ID="ltrUpdatedOn" runat="server" ></asp:Literal>
                    </td>
                </tr> 
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblLastLogin" 
                            runat="server" 
                            Text="Last Login:"
                            ToolTip="Date when the user last logged in." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:Literal ID="ltrLastLogin" runat="server" ></asp:Literal>
                    </td>
                </tr> 
                <% } %>
             </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlUserRoles" HeaderText="User Roles" >
       <ContentTemplate>
       </ContentTemplate>
    </ajaxToolkit:TabPanel>
 </ajaxToolkit:TabContainer>