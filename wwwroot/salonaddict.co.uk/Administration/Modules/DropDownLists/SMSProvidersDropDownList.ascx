<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SMSProvidersDropDownList.ascx.cs" Inherits="SalonAddict.Administration.Modules.SMSProvidersDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" ErrorMessage="SMS provider is a required field." Display="None" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />