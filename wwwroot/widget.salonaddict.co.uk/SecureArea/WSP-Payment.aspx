<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="WSP-Payment.aspx.cs" Inherits="IFRAME.SecureArea.WSP_PaymentPage" %>
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
        <asp:Label ID="lblError" runat="server" SkinID="ErrorLabel" EnableViewState="false" ></asp:Label>
        <table class="details" cellpadding="0" cellspacing="0" >
          <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrAccount.Text") %>
              </td>
              <td class="data-item" >
                <asp:Literal ID="ltrAccount" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrPlanType.Text") %>
              </td>
              <td class="data-item" >
                <asp:Literal ID="ltrPlanType" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrPeriod.Text") %>
              </td>
              <td class="data-item" >
                <asp:Literal ID="ltrPeriod" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrPlanPrice.Text") %>
              </td>
              <td class="data-item" >
                <asp:Literal ID="ltrPlanPrice" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrVAT.Text") %>
              </td>
              <td class="data-item" >
                <asp:Literal ID="ltrVat" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" >
                 <%= base.GetLocaleResourceString("ltrAmountDue.Text") %>
              </td>
              <td class="data-item" >
                <asp:Literal ID="ltrAmountDue" runat="server" ></asp:Literal>
              </td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" >
                 <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButton" CausesValidation="false" OnClick="btnCancel_Click" meta:resourceKey="btnCancel" />
                 <asp:Button ID="btnSave" runat="server" SkinID="SubmitButton" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" OnClick="btnSave_Click" meta:resourceKey="btnSave" />
                 <p>By clicking "Make Payment" you agree to our <a href="#" onclick="$('#dialog').dialog({ title:'Terms and Conditions', width:480, height:320, position: ['center',50] });" ><u>Terms and Conditions.</u></a></p>
              </td>
           </tr>
        </table>
        <div id="dialog" style="display:none;" >
            <p>
                Your access to the SalonAddict Reservation Widget is valid for the full duration of the subscription plan 
                you have selected (the “Contract Period”). A Contract Period equal to 1 calendar year applies to all 
                subscription plans. Billing periods may vary. At the end of the Contract Period you will be automatically 
                re-billed (“Automatic Renewal”), and your account will enter a new Contract Period.
            </p>
            <p>
                If you wish to cancel your subscription plan before a new Contract Period begins, please provide
                SalonAddict with a minimum notice of 28 days (the “Minimum Notice”) in advance of the end of
                your Contract Period. If you cancel your subscription plan without providing the Minimum Notice
                you are liable for contract penalties, which will be charged in your final bill. The contract penalties
                consist of the remaining payment(s) due on your current subscription plan, at time of cancellation.
            </p>
        </div>
   </asp:Panel>
</asp:Content>
