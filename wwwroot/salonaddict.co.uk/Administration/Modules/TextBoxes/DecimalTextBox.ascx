<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DecimalTextBox.ascx.cs" Inherits="SalonAddict.Administration.Modules.DecimalTextBox" %>
<asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
<ajaxToolkit:FilteredTextBoxExtender ID="ftbeValue" runat="server" TargetControlID="txtValue" FilterType="Custom, Numbers" ValidChars=".-" />
<asp:RequiredFieldValidator ID="rfvValue" ControlToValidate="txtValue" runat="server" Display="None"></asp:RequiredFieldValidator>
<asp:RangeValidator ID="rvValue" runat="server" ControlToValidate="txtValue" Type="Double" Display="None"></asp:RangeValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueE" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rvValueE" TargetControlID="rvValue" HighlightCssClass="validator-highlight" />