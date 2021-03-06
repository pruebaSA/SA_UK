﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true" CodeBehind="Ticket-Alert-Create.aspx.cs" Inherits="IFRAME.SecureArea.Ticket_Alert_CreatePage" %>
<%@ Register TagPrefix="IFRM" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph1c" runat="server">
    <asp:Panel ID="pnl" runat="server" SkinID="BoxPanel" DefaultButton="btnSave" >
        <IFRM:Menu ID="cntlMenu" runat="server" SelectedItem="Settings" />
        <div class="horizontal-line" ></div>
        <table style="margin:-20px" cellpadding="0" cellspacing="20" >
           <tr>
              <td><img src='<%= "../App_Themes/" + base.Theme + "/images/overview-settings.png" %>' alt="settings" /></td>
              <td style="vertical-align:middle" ><h1 style="margin:0px;padding:0px;">Add Alert</h1></td>
              <td style="vertical-align:middle" >
                  
              </td>
           </tr>
        </table>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td class="title" >
                  Receiver Name:
              </td>
              <td class="data-item" >
                  <asp:TextBox ID="txtName" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                  <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" Display="None" ErrorMessage="Name is a required field." ></asp:RequiredFieldValidator>
                  <ajaxToolkit:ValidatorCalloutExtender ID="valNameEx" runat="server" TargetControlID="valName" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" >
                  Email:
              </td>
              <td class="data-item" >
                   <asp:TextBox ID="txtEmail" runat="server" SkinID="TextBox" MaxLength="100" ></asp:TextBox>
                   <asp:RequiredFieldValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" Display="None" ErrorMessage="Email is a required field." ></asp:RequiredFieldValidator>
                   <ajaxToolkit:ValidatorCalloutExtender ID="valEmailEx" runat="server" TargetControlID="valEmail" EnableViewState="false" />
                   <asp:RegularExpressionValidator ID="valEmailRegEx" runat="server" ControlToValidate="txtEmail" Display="None" ValidationExpression=".+@.+\..+" ErrorMessage="Invalid email address." ></asp:RegularExpressionValidator>
                   <ajaxToolkit:ValidatorCalloutExtender ID="valEmailRegExEx" runat="server" TargetControlID="valEmailRegEx" EnableViewState="false" />
              </td>
           </tr>
           <tr>
              <td class="title" ></td>
              <td class="data-item" >
                  <asp:Button ID="btnCancel" runat="server" SkinID="SubmitButtonSecure" CausesValidation="false" OnClick="btnCancel_Click" Text="Cancel" />
                  <asp:Button ID="btnSave" runat="server" SkinID="SubmitButtonSecure" OnClick="btnSave_Click" Text="Save" />
              </td>
           </tr>
        </table>
        <table class="details" cellpadding="0" cellspacing="0" >
           <tr>
              <td colspan="2" >
<%--                 <p>
                    SMS Alerts and Reminders will be sent (and charged accordingly) to the contact numbers listed. 
                    Email Alerts will be sent (free) to the email addresses listed, a maximum of 10 alerts are allowed.
                 </p>--%>
                 <p> 
                    Email Alerts will be sent (free) to the email address listed, a maximum of 10 alerts are allowed.
                 </p>
              </td>
           </tr>
        </table>
    </asp:Panel>
</asp:Content>