<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomerDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.CustomerDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Administration/Modules/TextBoxes/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="GenderList" Src="~/Administration/Modules/DropDownLists/GenderDropDownList.ascx" %>

<%@ Import Namespace="SalonAddict.Common" %>

<% if(this.Action == ActionType.Create)
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="" />
        Add a new customer <a href="Customers.aspx" title="Back to customer list">(back to customer list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Add" OnClick="AddButton_Click" ToolTip="Add customer" />
    </div>
</div>
<% } 
   else
   { %>
<div class="section-header">
    <div class="title">
        <img src="images/ico-customers.png" alt="" />
        Edit customer details <a href="Customers.aspx" title="Back to customer list">(back to customer list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save customer changes" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete customer" OnClientClick="return confirm('Are you sure?')" />
    </div>
</div>
<% } %>
<ajaxToolkit:TabContainer runat="server" ID="CustomerTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerInfo" HeaderText="Customer Info">
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
                            ID="lblCustomerEmail" 
                            runat="server" 
                            Text="Email address:"
                            IsRequired="true"
                            ToolTip="The customer's email address." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:UpdatePanel ID="up" runat="server" >
                           <ContentTemplate>
                            <table class="details" cellpadding="0" cellspacing="0" >
                               <tr>
                                  <td>
                                     <SA:EmailTextBox ID="txtEmail" runat="server" />
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
                            ToolTip="The customer's password." 
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
                            ID="lblFirstName" 
                            runat="server" 
                            Text="First name:"
                            IsRequired="true"
                            ToolTip="The customer's first name." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtFirstName" 
                            runat="server" 
                            MaxLength="50"
                            ErrorMessage="First name is required" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblLastName" 
                            runat="server" 
                            Text="Last name:"
                            IsRequired="true"
                            ToolTip="The customer's last name." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:TextBox 
                            ID="txtLastName" 
                            runat="server" 
                            MaxLength="50"
                            ErrorMessage="Last name is required" />
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblGender" 
                            runat="server" 
                            Text="Gender:"
                            ToolTip="The customer's gender (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <SA:GenderList ID="ddlGender" runat="server" DefaultText="" DefaultValue="0" />
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblDateOfBirth" 
                            runat="server" 
                            Text="Date of birth:"
                            ToolTip="The customer's date of birth (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtDateOfBirth" runat="server" />
                        <asp:ImageButton ID="iDateOfBirth" runat="Server" ImageUrl="~/Administration/images/ico-calendar.png" AlternateText="Click to show calendar" /><br />
                        <ajaxToolkit:CalendarExtender ID="cDateOfBirthButtonExtender" runat="server" TargetControlID="txtDateOfBirth" PopupButtonID="iDateOfBirth" />
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblPhoneNumber" 
                            runat="server" 
                            Text="Phone number:"
                            ToolTip="The customer's phone number (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
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
                            Text="Mobile:"
                            ToolTip="The customer's mobile number (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtMobile" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCompany" 
                            runat="server" 
                            Text="Company:"
                            ToolTip="The customer's company name (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtCompany" runat="server" MaxLength="50" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblIsGuest" 
                            runat="server" 
                            Text="Is Guest:"
                            ToolTip="Determines whether the account is guest/demo account (cookies should be cleared after this setting is applied)." 
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
                            ToolTip="Date when customer account was created." 
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
                            ToolTip="Date when customer account was last updated." 
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
                            ToolTip="Date when customer last logged in." 
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
    <ajaxToolkit:TabPanel runat="server" ID="pnlAddress" HeaderText="Customer Billing Address">
        <ContentTemplate>
            <table class="details">
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblAddressLine1" 
                            runat="server" 
                            Text="Address:"
                            ToolTip="The customer's address (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtAddressLine1" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblAddressLine2" 
                            runat="server" 
                            Text="Address continued:"
                            ToolTip="The customer's address continued (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtAddressLine2" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblStateProvince" 
                            runat="server" 
                            Text="State/Province:"
                            ToolTip="The customer's state/province (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:DropDownList ID="ddlStateProvince" runat="server" ></asp:DropDownList>
                    </td>
                </tr>
                 <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCityTown" 
                            runat="server" 
                            Text="City/Town:"
                            ToolTip="The customer's city/town (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtCityTown" runat="server" MaxLength="100" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblZipPostalCode" 
                            runat="server" 
                            Text="Zip/Postal code:"
                            ToolTip="The customer's zip/postal code (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:TextBox ID="txtZipPostalCode" runat="server" MaxLength="10" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="title">
                        <SA:ToolTipLabel 
                            ID="lblCountry" 
                            runat="server" 
                            Text="Country:"
                            ToolTip="The customer's country (optional)." 
                            ToolTipImage="~/Administration/images/ico-help.gif" />
                    </td>
                    <td class="data-item">
                        <asp:DropDownList ID="ddlCountry" runat="server" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" ></asp:DropDownList>
                    </td>
                </tr>
             </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <% if (this.Action == ActionType.Edit)
       { %>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerOrders" HeaderText="Customer Orders">
        <ContentTemplate>

        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerRoleMappings" HeaderText="Customer Roles">
        <ContentTemplate>
            <asp:GridView 
                ID="gvCustomerRoles" 
                runat="server"
                DataKeyNames="CustomerRoleID" 
                AutoGenerateColumns="false" >
               <Columns>
                  <asp:TemplateField>
                     <ItemTemplate>
                        <asp:CheckBox ID="cbRole" runat="server" AutoPostBack="true" Enabled='<%# Roles.IsUserInRole("SYS_ADMIN") %>' OnCheckedChanged="cbRole_CheckedChanged" /> 
                     </ItemTemplate>
                     <ItemStyle Width="30px" />
                  </asp:TemplateField>
                  <asp:TemplateField HeaderText="Role" >
                     <ItemTemplate>
                        <%# Eval("Name").ToString().HtmlEncode() %> (<%# Eval("Description").ToString().HtmlEncode() %>)
                     </ItemTemplate>
                  </asp:TemplateField>
               </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
   <% } %>
</ajaxToolkit:TabContainer>