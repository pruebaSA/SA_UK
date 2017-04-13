<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="BusinessDetails.aspx.cs" Inherits="SalonPortal.SecureArea.BusinessDetails" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/SecureArea/Modules/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/SecureArea/Modules/NumericTextBox.ascx" %>

<%@ Import Namespace="SalonAddict.BusinessAccess.Configuration" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
    <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-business.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div class="options">
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
        </div>
    </div>
    <SA:Message ID="lblMessage" runat="server" />
    <ajaxToolkit:TabContainer runat="server" ID="BusinessTabs" ActiveTabIndex="0" >
        <ajaxToolkit:TabPanel runat="server" ID="pnlBusinessInfo"  >
            <ContentTemplate>
                <table class="details">
                     <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblName" 
                                runat="server" 
                                meta:resourceKey="lblName"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:Label ID="txtName" runat="server" SkinID="DisabledTextBox" ></asp:Label>
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
                                ID="lblEmail" 
                                runat="server" 
                                meta:resourceKey="lblEmail"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <SA:EmailTextBox ID="txtEmail" runat="server" IsRequired="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblFaxNumber" 
                                runat="server" 
                                meta:resourceKey="lblFaxNumber"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtFaxNumber" runat="server" MaxLength="50" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblWebsite" 
                                runat="server" 
                                meta:resourceKey="lblWebsite"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtWebsite" runat="server" MaxLength="400" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblRating" 
                                runat="server" 
                                meta:resourceKey="lblRating"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:Label ID="txtRating" runat="server" SkinID="DisabledTextBox" ></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblPublished" 
                                runat="server" 
                                meta:resourceKey="lblPublished"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:CheckBox ID="cbPublished" runat="server" Enabled="false" />
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
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblCreatedOn" 
                                runat="server" 
                                meta:resourceKey="lblCreatedOn"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:Label ID="txtCreatedOn" runat="server" Font-Bold="true" ></asp:Label>
                        </td>
                    </tr>
                 </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlSEO" >
            <ContentTemplate>
                <table class="details">
                     <tr>
                        <td style="vertical-align:top;" class="title">
                            <SA:ToolTipLabel 
                                ID="lblLogo" 
                                runat="server" 
                                meta:resourceKey="lblLogo"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <table cellpadding="0" cellpadding="0" >
                               <tr>
                                  <td>
                                    <div class="salon-logo" >
                                        <asp:Image ID="imgLogo" runat="server" />
                                    </div>
                                  </td>
                                  <td style="padding-left:20px;">
                                     <asp:Literal ID="ltrLogoRequirements" runat="server" meta:resourceKey="ltrLogoRequirements" ></asp:Literal>
                                  </td>
                               </tr>
                            </table>
                            <br />
                            <asp:FileUpload ID="fuLogo" runat="server" />
                            &nbsp;
                            <asp:Button 
                                ID="btnUploadLogo" 
                                runat="server" 
                                CausesValidation="false" 
                                OnClick="btnUploadLogo_Click"
                                meta:resourceKey="btnUploadLogo" />
                        </td>
                    </tr>
                    <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblMetaKeywords" 
                                runat="server" 
                                meta:resourceKey="lblMetaKeywords"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtMetaKeywords" runat="server" MaxLength="200" style="width:300px;" ></asp:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="title">
                            <SA:ToolTipLabel 
                                ID="lblShortDescription" 
                                runat="server" 
                                meta:resourceKey="lblShortDescription"
                                ToolTipImage="~/SecureArea/images/ico-help.gif" />
                            <div>
                                <b id="short_chars">400</b>
                            </div>
                        </td>
                        <td class="data-item">
                            <asp:TextBox ID="txtShortDescription" runat="server" TextMode="MultiLine" style="width:400px;" Height="80px" onkeyUp="txtShortDescription_onkeyUp(this)" ></asp:TextBox>
                        </td>
                    </tr>
               </table>
               <br /><br /><br />
            <script type="text/javascript" language="javascript" >
                function txtShortDescription_onkeyUp(sender)
                {
                    if(sender != null)
                    {
                        var max = 400;
                        var label = document.getElementById("short_chars");
                        if(label != null)
                        {
                           var used = sender.value.length;
                           if(used > max)
                           {
                               sender.value = sender.value.substring(0,max);
                               used = max;
                           }
                           var left = max - used;
                           label.innerHTML = left + "";
                        }
                    }
                }
            </script>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlAddress" >
            <ContentTemplate>
                <asp:UpdatePanel ID="pnlAddress_Ajax" runat="server" >
                    <ContentTemplate>
                       <div style="position:relative" >
                       <div id="map_canvas" style="position:absolute;top:5px;right:10px;width:330px;height:245px;" ></div>
                       <table class="details" >
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblAddressLine1" 
                                        runat="server" 
                                        meta:resourceKey="lblAddressLine1"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                </td>
                                <td class="data-item">
                                    <asp:Label ID="txtAddressLine1" runat="server" SkinID="DisabledTextBox" ></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblAddressLine2" 
                                        runat="server" 
                                        meta:resourceKey="lblAddressline2"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                </td>
                                <td class="data-item">
                                    <asp:Label ID="txtAddressLine2" runat="server" SkinID="DisabledTextBox" ></asp:Label>
                                </td>
                             </tr>
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblCountry" 
                                        runat="server" 
                                        meta:resourceKey="lblCountry"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                </td>
                                <td class="data-item">
                                    <asp:Label ID="txtCountry" runat="server" SkinID="DisabledTextBox" ></asp:Label>
                                </td>
                             </tr>
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblCityTown" 
                                        runat="server" 
                                        meta:resourceKey="lblCityTown"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                </td>
                                <td class="data-item">
                                    <asp:Label ID="txtCityTown" runat="server" SkinID="DisabledTextBox" ></asp:Label>
                                </td>
                             </tr>
                            <tr>
                                <td class="title">
                                    <SA:ToolTipLabel 
                                        ID="lblZipPostalCode" 
                                        runat="server" 
                                        meta:resourceKey="lblZipPostalCode"
                                        ToolTipImage="~/SecureArea/images/ico-help.gif" />
                                </td>
                                <td class="data-item">
                                    <asp:Label ID="txtZipPostalCode" runat="server" SkinID="DisabledTextBox" ></asp:Label>
                                </td>
                             </tr>
                        </table>
                       </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Content>
