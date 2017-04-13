<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StaffServiceDropDownList.ascx.cs" Inherits="SalonPortal.SecureArea.Modules.StaffServiceDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" Display="None" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender ID="rfvValueExtended" runat="Server" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />