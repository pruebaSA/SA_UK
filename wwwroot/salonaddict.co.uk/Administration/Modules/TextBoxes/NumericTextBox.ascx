<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NumericTextBox.ascx.cs" Inherits="SalonAddict.Administration.Modules.NumericTextBox" %>

<asp:TextBox ID="txtValue" runat="server" ></asp:TextBox>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="txtValue" Display="None" ></asp:RequiredFieldValidator>
<asp:RangeValidator ID="rvValue" runat="server" ControlToValidate="txtValue" Type="Integer" Display="None" ></asp:RangeValidator>
<ajaxToolkit:ValidatorCalloutExtender ID="rfvValueExtended" runat="Server" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />
<ajaxToolkit:ValidatorCalloutExtender ID="rvValueExtended"  runat="Server" TargetControlID="rvValue" HighlightCssClass="validator-highlight" />