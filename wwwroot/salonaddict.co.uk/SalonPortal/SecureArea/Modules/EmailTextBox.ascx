<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailTextBox.ascx.cs" Inherits="SalonPortal.SecureArea.Modules.EmailTextBox" %>
<asp:TextBox ID="txtValue" runat="server" ></asp:TextBox>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="txtValue" Display="None" />
<asp:RegularExpressionValidator ID="revValue" runat="server" ControlToValidate="txtValue" ValidationExpression=".+@.+\..+" Display="None" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueE" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="revValueE" TargetControlID="revValue" HighlightCssClass="validator-highlight" />