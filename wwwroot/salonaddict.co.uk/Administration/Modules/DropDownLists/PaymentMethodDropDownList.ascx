<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentMethodDropDownList.ascx.cs" Inherits="SalonAddict.Administration.Modules.DropDownLists.PaymentMethodDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" ErrorMessage="Payment method is a required field." Display="None" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />