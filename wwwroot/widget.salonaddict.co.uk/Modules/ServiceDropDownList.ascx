<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceDropDownList.ascx.cs" Inherits="IFRAME.Modules.ServiceDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" SkinID="ServiceDropDownList" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" Display="None" EnableViewState="false" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueEX" TargetControlID="rfvValue" EnableViewState="false" />