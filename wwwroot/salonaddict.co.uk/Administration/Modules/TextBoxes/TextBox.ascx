<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextBox.ascx.cs" Inherits="SalonAddict.Administration.Modules.TextBox" %>
<asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
<asp:RequiredFieldValidator ID="rfvValue" ControlToValidate="txtValue" runat="server" Display="None"></asp:RequiredFieldValidator>
<asp:RegularExpressionValidator ID="revValue" ControlToValidate="txtValue" runat="server" Display="None" Visible="false" ></asp:RegularExpressionValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="revValueExtender" TargetControlID="revValue" HighlightCssClass="validator-highlight" />