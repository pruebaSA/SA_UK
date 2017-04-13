<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LatTextBox.ascx.cs" Inherits="SalonPortal.SecureArea.Modules.LatTextBox" %>
<asp:TextBox ID="txtValue" runat="server" ></asp:TextBox>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="txtValue" Display="None" />
<asp:RegularExpressionValidator ID="revValue" runat="server" ControlToValidate="txtValue" ValidationExpression="^-?([1-8]?[1-9]|[1-9]0)\.{1}\d{1,6}" Display="None" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueE" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="revValueE" TargetControlID="revValue" HighlightCssClass="validator-highlight" />