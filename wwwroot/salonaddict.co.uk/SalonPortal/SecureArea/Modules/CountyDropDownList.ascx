<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CountyDropDownList.ascx.cs" Inherits="SalonPortal.SecureArea.Modules.CountyDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" Display="None" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender ID="rfvValueExtended" runat="Server" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />