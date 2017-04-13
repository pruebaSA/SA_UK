<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TextBox.ascx.cs" Inherits="SalonPortal.Modules.TextBox" %>
<asp:TextBox ID="txtValue" runat="server" ></asp:TextBox>
<asp:RequiredFieldValidator ID="rfvValue" ControlToValidate="txtValue" runat="server" Display="None" ></asp:RequiredFieldValidator>
<asp:RegularExpressionValidator ID="rvValue" ControlToValidate="txtValue" runat="server" Visible="false" Display="None" ></asp:RegularExpressionValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />
<ajaxToolkit:ValidatorCalloutExtender runat="server" ID="revValueExtender" TargetControlID="rvValue" HighlightCssClass="validator-highlight" />