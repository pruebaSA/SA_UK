<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailTextBox.ascx.cs" Inherits="SalonAddict.Modules.EmailTextBox" %>
<asp:TextBox ID="txtValue" runat="server" ></asp:TextBox>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="txtValue" Display="None" meta:resourceKey="rfvValue" />
<asp:RegularExpressionValidator ID="rvValue" runat="server" ControlToValidate="txtValue" ValidationExpression=".+@.+\..+" Display="None" meta:resourceKey="rvValue" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueE" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rvValueE" TargetControlID="rvValue" HighlightCssClass="validator-highlight" />