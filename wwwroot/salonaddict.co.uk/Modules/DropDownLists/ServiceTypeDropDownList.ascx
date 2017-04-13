<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceTypeDropDownList.ascx.cs" Inherits="SalonAddict.Modules.ServiceTypeDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" AutoPostBack="true" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" Display="None" Visible="false" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />