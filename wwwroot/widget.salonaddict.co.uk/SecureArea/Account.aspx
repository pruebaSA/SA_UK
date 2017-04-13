<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="IFRAME.SecureArea.AccountPage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server" >
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Account" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-account.png" %>' alt="account" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;"><%= base.GetLocaleResourceString("ltrHeader.Text") %></h1></td>
              <td style="vertical-align:middle" >

              </td>
           </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%" >
           <tr>
              <td style="width:520px;vertical-align:top;padding-top:10px;" >
                <table class="details" cellpadding="0" cellspacing="0" >
                   <tr>
                      <td class="title" >
                          <%= base.GetLocaleResourceString("ltrAccount.Text") %>
                      </td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrAccount" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrAccountType.Text") %></td>
                      <td class="data-item" >
                         <b><asp:Literal ID="ltrPlanType" runat="server" EnableViewState="false" ></asp:Literal></b>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrExpiry.Text") %></td>
                      <td class="data-item" >
                          <asp:Literal ID="ltrPlanEndDate" runat="server" EnableViewState="false" ></asp:Literal>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" ><%= base.GetLocaleResourceString("ltrStatus.Text") %></td>
                      <td class="data-item" >
                         <asp:MultiView ID="mvStatus" runat="server" ActiveViewIndex="0" EnableViewState="false" >
                            <asp:View ID="v0" runat="server" >
                                <a href='<%= IFRMHelper.GetURL("wsp-history.aspx") %>' ><u><b>Activate Your Account</b></u></a>
                            </asp:View>
                            <asp:View ID="v1" runat="server" >
                                <asp:Literal ID="ltrStatus" runat="server" EnableViewState="false" ></asp:Literal>
                            </asp:View>
                         </asp:MultiView>
                      </td>
                   </tr>
                   <tr>
                      <td class="title" >
                          Balance:
                      </td>
                      <td class="data-item" >
                          <a href='<%= IFRMHelper.GetURL("billpay-mybill.aspx") %>' ><u><%= base.GetLocaleResourceString("hlBilling.Text") %></u></a>
                      </td>
                   </tr>
                </table>
              </td>
              <td style="vertical-align:top;" >
                 <p style="padding-top:0px;margin-top:0px;" >&bull; <a href='<%= IFRMHelper.GetURL("profile.aspx") %>' ><u><%= base.GetLocaleResourceString("hlProfile.Text") %></u></a></p>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("wsp-history.aspx") %>' ><u><%= base.GetLocaleResourceString("hlSubscription.Text") %></u></a></p>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("paymentmethods.aspx") %>' ><u><%= base.GetLocaleResourceString("hlPaymentMethods.Text") %></u></a></p>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("reportbookingtotals.aspx") %>' ><u><%= base.GetLocaleResourceString("hlBookingTotalsReport.Text") %></u></a></p>
                 <p>&bull; <a href='<%= IFRMHelper.GetURL("contact-us.aspx") %>' ><u><%= base.GetLocaleResourceString("hlContact.Text") %></u></a></p>
              </td>
           </tr>
         </table>
     </asp:Panel>
</asp:Content>
