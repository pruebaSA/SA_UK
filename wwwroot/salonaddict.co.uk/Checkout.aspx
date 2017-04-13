<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="SalonAddict.CheckoutPage" %>
<%@ Register TagPrefix="SA" TagName="Topic" Src="~/Modules/ContentTopic.ascx" %>
<%@ Register TagPrefix="SA" TagName="OrderSummary" Src="~/Templates/OrderSummaryOne.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="ExpiryMonth" Src="~/Modules/DropDownLists/MonthDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="ExpiryYear" Src="~/Modules/DropDownLists/YearDropDownList.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="PaymentMethods" Src="~/Modules/PaymentMethods.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/OneColumn.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <style type="text/css" type="text/css" >
    .salon-booking { position:relative; }
    .salon-booking .title { color:#ef2869; font-size:22px; padding-bottom:5px; border-bottom:solid 1px #d5d5d5; }
    .salon-booking .salon-details { margin-bottom:10px; }
    .salon-booking .salon-details .salon-name { margin-top:10px; font-size:15px; font-weight:bold; color:#333; }
    .salon-booking .salon-details .salon-address { margin-bottom:15px; }
    .salon-booking .personal-details { width:350px; height:390px; padding:20px; background:#eee; }
    .salon-booking .payment-details { width:450px; padding:10px 20px 20px 20px; background:#fff; border-top:solid 4px #fc94b7; }
    .salon-booking .additional-fees { padding:8px; width:470px; background:#eee; }
    .salon-booking .checkout { padding:10px; width:470px; background:#fff; margin-bottom:0px; }
    .salon-booking .amount { font-size:22px; color:#ef2869; }
    .salon-booking .donations  { padding:10px; background:#fff; border:solid 1px #d5d5d5; margin-bottom:25px; }
    </style>
    <div class="salon-booking" >
       <div class="title" >
          <%= base.GetLocalResourceObject("lblBookingSummary.Text") %>
       </div>
       <div class="salon-details" >
           <div class="salon-name" >
               <asp:Label ID="lblBusinessName" runat="server" ></asp:Label>
           </div>
           <div class="salon-address" >
               <asp:Label ID="lblBusinessAddress" runat="server" ></asp:Label>
           </div>
       </div>
       <SA:OrderSummary ID="OrderSummary" runat="server" ShowOrderDetails="false" IsShoppingCart="true" OnDiscountRemoved="OrderSummary_DiscountRemoved" OnGiftCardRemoved="OrderSummary_GiftCardRemoved" />
       <SA:Message ID="lblMessage" runat="server" Auto="false" IsError="true" />
       <table cellpadding="0" cellspacing="0" width="100%" >
          <tr>
             <td style="width:450px;vertical-align:top;">
               <div class="personal-details" >
                  <table cellpadding="0" cellspacing="10" >
                     <tr>
                        <td colspan="2" ><b><%= base.GetLocalResourceObject("ltrHeaderPersonalDetails.Text") %></b></td>
                     </tr>
                     <tr>
                        <td>
                           <%= base.GetLocalResourceObject("lblFirstName.Text") %><br />
                           <SA:TextBox ID="txtFirstName" runat="server" AutoCompleteType="Disabled" meta:resourceKey="txtFirstName"/>
                        </td>
                       <td>
                           <%= base.GetLocalResourceObject("lblLastName.Text") %><br />
                           <SA:TextBox ID="txtLastName" runat="server" AutoCompleteType="Disabled" meta:resourceKey="txtLastName"/>
                       </td>
                     </tr>
                     <tr>
                        <td>
                           <%= base.GetLocalResourceObject("lblEmail.Text") %><br />
                           <asp:TextBox ID="txtEmail" runat="server" AutoCompleteType="Disabled" ></asp:TextBox>
                           <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" Display="None" meta:resourceKey="rfvEmail" />
                           <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ValidationExpression=".+@.+\..+" Display="None" meta:resourceKey="revEmail" />
                           <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvEmailE" TargetControlID="rfvEmail" HighlightCssClass="validator-highlight" />
                           <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="revEmailE" TargetControlID="revEmail" HighlightCssClass="validator-highlight" />
                        </td>
                        <td>
                           <%= base.GetLocalResourceObject("lblConfirmEmail.Text")%><br />
                           <asp:TextBox ID="txtConfirmEmail" runat="server" AutoCompleteType="Disabled" ></asp:TextBox>
                           <asp:RequiredFieldValidator ID="rfvConfirmEmail" runat="server" ControlToValidate="txtConfirmEmail" Display="None" meta:resourceKey="rfvConfirmEmail" />
                           <asp:RegularExpressionValidator ID="revConfirmEmail" runat="server" ControlToValidate="txtConfirmEmail" ValidationExpression=".+@.+\..+" Display="None" meta:resourceKey="revConfirmEmail" />
                           <asp:CompareValidator ID="cvConfirmEmail" runat="server" ControlToValidate="txtConfirmEmail" ControlToCompare="txtEmail" Operator="Equal" Display="None" meta:resourceKey="cvConfirmEmail" />
                           <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvConfirmEmailE" TargetControlID="rfvConfirmEmail" HighlightCssClass="validator-highlight" />
                           <ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="revConfirmEmailE" TargetControlID="revConfirmEmail" HighlightCssClass="validator-highlight" />
                           <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="cvConfirmEmailE" TargetControlID="cvConfirmEmail" HighlightCssClass="validator-highlight" />
                        </td>
                     </tr>
                      <tr>
                         <td>
                            <%= base.GetLocalResourceObject("lblPhone.Text")%><br />
                            <SA:TextBox ID="txtPhoneNumber" runat="server" MaxLength="20" AutoCompleteType="Disabled" meta:resourceKey="txtPhoneNumber" />
                         </td>
                         <td style="padding-left:10px"></td>
                      </tr>
                     <tr>
                        <td colspan="2">
                          <%= base.GetLocalResourceObject("lblComment.Text")%>
                          <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" Height="80px" Width="320px" AutoCompleteType="Disabled" ></asp:TextBox>
                        </td>
                     </tr>
                  </table>
                  <table cellpadding="0" cellspacing="10" >
                     <tr>
                        <td colspan="2" >
                           <asp:Label ID="lblApplyError" runat="server" Font-Bold="true" CssClass="pink" ></asp:Label>
                        </td>
                     </tr>
                     <tr>
                        <td colspan="2" >
                            <b><%= base.GetLocalResourceObject("ltrHeaderGiftCardDetails.Text")%></b>
                        </td>
                     </tr>
                     <tr>
                        <td>
                           <div><%= base.GetLocalResourceObject("txtGiftCard.Label") %></div>
                           <SA:TextBox ID="txtCouponCode" runat="server" MaxLength="50" Width="180px" ValidationGroup="voucher" meta:resourceKey="txtCouponCode" />
                        </td>
                        <td style="vertical-align:bottom;" >
                           <asp:Button ID="btnApply" runat="server" SkinID="PinkButtonXSmall" OnClick="btnApply_Click" ValidationGroup="voucher" meta:resourceKey="btnApply" />
                        </td>
                     </tr>
                  </table>
               </div>
             </td>
             <td>
               <div class="payment-details" >
                  <table cellpadding="0" cellspacing="10" >
                     <tr>
                        <td>
                           <%= base.GetLocalResourceObject("lblCardholderName.Text")%><br />
                           <SA:TextBox ID="txtCardholderName" runat="server" MaxLength="100" Width="200px" AutoCompleteType="Disabled" ValidationExpression="^[a-zA-Z-.'\s]*" meta:resourceKey="txtCardholderName" />
                        </td>
                        <td>
                           <%= base.GetLocalResourceObject("lblCardNumber.Text")%> <br />
                           <SA:TextBox ID="txtCardNumber" runat="server" MaxLength="50" Width="200px" AutoCompleteType="Disabled" meta:resourceKey="txtCardNumber" />
                        </td>
                     </tr>
                     <tr>
                        <td>
                           <%= base.GetLocalResourceObject("lblCardType.Text")%><br />
                           <asp:DropDownList ID="ddlCardType" runat="server" ></asp:DropDownList>
                        </td>
                        <td>
                           <%= base.GetLocalResourceObject("lblExpiry.Text")%> <br />
                           <table cellpadding="0" cellspacing="0" >
                              <tr>
                                 <td>
                                    <SA:ExpiryMonth ID="ddlMonth" runat="server" />
                                 </td>
                                 <td style="padding-left:10px">
                                    <SA:ExpiryYear ID="ddlYear" runat="server" />
                                 </td>
                              </tr>
                           </table>
                        </td>
                     </tr>
                     <tr>
                        <td>
                           <table cellpadding="0" cellspacing="0" >
                              <tr>
                                 <td>
                                    <%= base.GetLocalResourceObject("lblSecurityCode.Text")%><br />
                                    <SA:TextBox ID="txtSecurityCode" runat="server" MaxLength="4" Width="80px" IsRequired="false" AutoCompleteType="Disabled" meta:resourceKey="btnSecurityCode" />
                                 </td>
                                 <td style="padding-left:10px;padding-top:2px;">
                                     <img src="<%= Page.ResolveUrl("~/images/cvv.png") %>" />
                                 </td>
                              </tr>
                           </table>
                        </td>
                        <td style="padding:5px;">
                            <script type="text/javascript" src="https://seal.thawte.com/getthawteseal?host_name=www.salonaddict.co.uk&amp;size=S&amp;lang=en"></script>
                        </td>
                     </tr>
                     <tr>
                        <td colspan="2" >
                            <asp:CheckBox ID="cbTerms" runat="server" /> 
                            <%= base.GetLocalResourceObject("lblTerms.Text") %>
                            <skmValidators:CheckBoxValidator ID="rfvTerms" runat="server" Display="None" ControlToValidate="cbTerms" ></skmValidators:CheckBoxValidator>
                            <ajaxToolkit:ValidatorCalloutExtender runat="server" ID="rfvTermsE" TargetControlID="rfvTerms" HighlightCssClass="validator-highlight" />
                            <div>
                               <asp:CheckBox ID="cbSubscribe" runat="server" Checked="true" />
                               <%= base.GetLocalResourceObject("cbSubscribe.Text") %>
                            </div>
                        </td>
                     </tr>
                  </table>
               </div>
               <div class="checkout" >
                  <table cellpadding="0" cellspacing="0">
                     <tr>
                        <td style="padding-top:8px;" >
                           <asp:Label ID="lblCheckoutAmount" runat="server" Font-Size="24px" Font-Bold="true" CssClass="pink" ></asp:Label>
                        </td>
                        <td style="padding-left:25px;">
                           <asp:Button ID="btnCheckout" runat="server" SkinID="BlackButtonLarge" OnClientClick="WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ctl00$ctl00$ph$ph1c$btnCheckout&quot;, &quot;&quot;, true, &quot;&quot;, &quot;&quot;, false, false)); if(Page_IsValid) { this.disabled = true; __doPostBack('ctl00$ctl00$ph$ph1c$btnCheckout', '') }" OnClick="btnCheckout_Click" meta:resourceKey="btnCheckout" />
                        </td>
                     </tr>
                  </table>
                  <div style="margin-top:15px;margin-bottom:15px;">
                    <asp:Label ID="lblDisclaimer" runat="server" ></asp:Label>
                  </div>
                  <SA:PaymentMethods ID="cntlPaymentMethods" runat="server" />
               </div>
             </td>
          </tr>
       </table>
       <br /><br />
       <div class="donations" >
          <table cellpadding="0" cellspacing="4" >
             <tr>
                <td>
                   <img src="<%= Page.ResolveUrl("~/images/kiva_ico.png") %>" />
                </td>
                <td>
                    <%= base.GetLocalResourceObject("lblKiva.Text") %>
                </td>
             </tr>
          </table>
       </div>
       <SA:Topic ID="ThingsToNote" runat="server" meta:resourceKey="ThingsToNote" />
    </div>
</asp:Content>
