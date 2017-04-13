<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateTimeTextBox.ascx.cs" Inherits="SalonAddict.Administration.Modules.DateTimeTextBox" %>
<asp:TextBox runat="server" ID="txtValue" />
<asp:ImageButton ID="iValue" runat="Server" ImageUrl="~/Administration/images/ico-calendar.png" CausesValidation="false" AlternateText="Click to show calendar" /><br />
<ajaxToolkit:CalendarExtender ID="cStartDateButtonExtender" runat="server" TargetControlID="txtValue" PopupButtonID="iValue" />
<asp:RequiredFieldValidator ID="rfvValue" ControlToValidate="txtValue" runat="server" Display="None"></asp:RequiredFieldValidator>
<asp:CompareValidator ID="cvValue" runat="server" ControlToValidate="txtValue" Type="Date" Operator="DataTypeCheck" Display="None" ErrorMessage="Invalid date." />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="cvValueExtender" TargetControlID="cvValue" HighlightCssClass="validator-highlight" />