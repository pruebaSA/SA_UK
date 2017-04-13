<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StateProvinceDropDownList.ascx.cs" Inherits="SalonPortal.SecureArea.Modules.StateProvinceDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" Display="None" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender ID="rfvValueExtended" runat="Server" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />