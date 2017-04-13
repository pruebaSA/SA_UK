<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="StoreGlobalSettings.aspx.cs" Inherits="SalonAddict.Administration.StoreGlobalSettings" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Administration/Modules/TextBoxes/EmailTextBox.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-configuration.png" alt="Configuration" />
        Store Settings
    </div>
    <div class="options">
        <asp:Button runat="server" Text="Save" ID="btnSave" OnClick="btnSave_Click" ToolTip="Save global store settings" />
    </div>
</div>
<div>
    <ajaxToolkit:TabContainer runat="server" ID="CommonSettingsTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlStoreInfo" HeaderText="General">
            <contenttemplate>
                <table class="details">
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblStoreName" 
                                runat="server" 
                                Text="Store name:" 
                                IsRequired="true"
                                ToolTip="The name of your store"
                                ToolTipImage="~/Administration/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtStoreName" 
                                runat="server"
                                ErrorMessage="Store name is a requried field." 
                                ToolTip="Enter the name of your store e.g. Your Store">
                            </SA:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblStoreUrl" 
                                runat="server" 
                                Text="Store URL:" 
                                IsRequired="true"
                                ToolTip="The URL of your store e.g. http://www.yourstore.com"
                                ToolTipImage="~/Administration/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtStoreURL" 
                                runat="server"
                                ErrorMessage="Store URL is a required field.">
                            </SA:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblStoreTheme" 
                                runat="server" 
                                Text="Store Theme:" 
                                IsRequired="true"
                                ToolTip="The theme of your store"
                                ToolTipImage="~/Administration/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:DropDownList ID="ddlThemes" runat="server" ></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblStoreClosed" 
                                runat="server" 
                                Text="Store closed:"
                                ToolTip="Check to close the store. Uncheck to re-open." 
                                ToolTipImage="~/Administration/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:CheckBox runat="server" ID="cbStoreClosed"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
            </contenttemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlSEODisplay" HeaderText="SEO/Display">
            <contenttemplate>
                <table class="details" >
                    <tr>
                       <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblEnableGoogleAnalytics" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Enable google analytics:" 
                                ToolTip="Enable google analytics." />
                        </td>
                        <td class="data-item">
                            <asp:CheckBox ID="cbEnableGoogleAnalytics" runat="server" />
                        </td>
                    </tr>
                    <tr>
                       <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblGoogleScript" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Google script:" 
                                ToolTip="Google analytics script." />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtGoogleScript" runat="server" ></asp:TextBox>
                        </td>
                    </tr>
                   <tr>
                       <td colspan="2" ><br /><br /></td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblStoreNamePrefix" 
                                runat="server" 
                                Text="Enable store name prefix:"
                                ToolTip="Prefix page titles with the portal name e.g. Your Portal.Title"
                                ToolTipImage="~/Administration/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:CheckBox runat="server" ID="cbStoreNameInTitle"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDefaultTitle" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Default title:" 
                                ToolTip="The default title for pages in your store." />
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtDefaultSEOTitle" 
                                runat="server"
                                MaxLength="100"
                                IsRequired="false" >
                            </SA:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDefaultMetaDescription" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Default meta description:" 
                                ToolTip="The default meta description for pages in your store." />
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtDefaulSEODescription" 
                                runat="server"
                                MaxLength="400"
                                IsRequired="false"  >
                            </SA:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDefaultMetaKeywords" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Default meta keywords:" 
                                ToolTip="The default meta keywords for pages in your store." />
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtDefaulSEOKeywords" 
                                runat="server"
                                MaxLength="400"
                                IsRequired="false"  >
                            </SA:TextBox>
                        </td>
                    </tr>
                </table>
            </contenttemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlMedia" HeaderText="Media">
            <contenttemplate>
                <table class="details" width="100%">
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblMaxThumbnailSize" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Max thumbnail size:" 
                                ToolTip="The default size (pixels) for thumbnail images." />
                        </td>
                        <td class="data-item">
                            <SA:NumericTextBox 
                                ID="txtMaxThumbnailSize"
                                runat="server"
                                MaxLength="4"
                                RequiredErrorMessage="Enter a maximum thumbnail image size" 
                                MinimumValue="1"
                                Width="50px"
                                MaximumValue="999999" 
                                RangeErrorMessage="The value must be from 0 to 999999" >
                           </SA:NumericTextBox>
                            pixels
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblMaxBusinessThumbnailSize" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Max business thumbnail size:" 
                                ToolTip="The default size (pixels) for business thumbnail images." />
                        </td>
                        <td class="data-item">
                            <SA:NumericTextBox 
                                ID="txtMaxBusinessThumbnailSize"
                                runat="server"
                                MaxLength="4"
                                Width="50px"
                                RequiredErrorMessage="Enter a thumbnail image size" 
                                MinimumValue="1"
                                MaximumValue="999999" 
                                RangeErrorMessage="The value must be from 0 to 999999" >
                           </SA:NumericTextBox>
                            pixels
                        </td>
                    </tr>
                </table>
            </contenttemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlMailSettings" HeaderText="Mail Settings">
            <contenttemplate>
                <table class="details" >
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDefaultAdminEmail" 
                                runat="server" ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Default admin email:" 
                                IsRequired="true"
                                ToolTip="The default email address." />
                        </td>
                        <td class="data-item">
                            <SA:EmailTextBox 
                                ID="txtDefaultAdminEmailAddress"
                                runat="server" >
                            </SA:EmailTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDefaultAdminEmailDisplayName" 
                                runat="server" ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Default admin email display:" 
                                ToolTip="The display name to use for the default admin email address." />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtDefaultAdminEmailDisplayName" runat="server" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblPasswordRecoveryEmail" 
                                runat="server" ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Password recovery email:" 
                                IsRequired="true"
                                ToolTip="The email address to send password recovery emails from." />
                        </td>
                        <td class="data-item">
                            <SA:EmailTextBox 
                                ID="txtPasswordRecoveryEmail"
                                runat="server" >
                            </SA:EmailTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblPasswordRecoveryDisplayName" 
                                runat="server" ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Password recovery email display:" 
                                ToolTip="The display name to use for the password recovery email address." />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtPasswordRecoveryDisplayName" runat="server" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                       <td colspan="2" >&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <p>
                                <b>Send Test Email (save settings first by clicking "Save" button)</b>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblTestEmailTo" 
                                IsRequired="true"
                                runat="server" ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Send email to:" 
                                ToolTip="The email address to which you want to send your test email." />
                        </td>
                        <td class="data-item">
                            <SA:EmailTextBox 
                                ID="txtSendEmailTo"
                                runat="server"  
                                ValidationGroup="SendTestEmail" >
                            </SA:EmailTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title"></td>
                        <td class="data-item">
                            <asp:Button 
                                ID="btnSendTestEmail" 
                                runat="server" 
                                Text="Send Test Email" 
                                OnClick="btnSendTestEmail_Click" 
                                ValidationGroup="SendTestEmail" 
                                ToolTip="Send the test email" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                        </td>
                        <td>
                            <asp:Label ID="lblSendTestEmailResult" runat="server" EnableViewState="false"></asp:Label>
                        </td>
                    </tr>
                </table>
            </contenttemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlOther" HeaderText="Other">
            <contenttemplate>
                <table class="details" >
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDefaultCountry" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Default country:" 
                                IsRequired="true"
                                ToolTip="Default country three-letter iso code" />
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtDefaultCountry" 
                                runat="server" 
                                Width="50px"
                                MaxLength="3"
                                ErrorMessage="Default country a required field." > 
                            </SA:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblDefaultCulture" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Default culture:" 
                                IsRequired="true"
                                ToolTip="The default store locale culture." />
                        </td>
                        <td class="data-item">
                            <SA:TextBox 
                                ID="txtDefaultCulture" 
                                runat="server" 
                                Width="50px"
                                MaxLength="5"
                                ErrorMessage="Default culture is a required field." > 
                            </SA:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblBusinessReviewApproval" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Business Review Approval:" 
                                ToolTip="Determines whether or not business reviews require approval before being published." />
                        </td>
                        <td class="data-item">
                            <asp:CheckBox ID="cbBusinessReviewApproval" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblStaffReviewApproval" 
                                runat="server" 
                                ToolTipImage="~/Administration/images/ico-help.gif"
                                Text="Staff Review Approval:" 
                                ToolTip="Determines whether or not staff reviews require approval before being published." />
                        </td>
                        <td class="data-item">
                            <asp:CheckBox ID="cbStaffReviewApproval" runat="server" />
                        </td>
                    </tr>
                </table>
            </contenttemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</div>
</asp:Content>