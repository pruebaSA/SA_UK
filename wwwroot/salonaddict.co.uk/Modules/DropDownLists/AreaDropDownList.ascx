<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AreaDropDownList.ascx.cs" Inherits="SalonAddict.Modules.AreaDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" Display="None" Visible="false" EnableViewState="false" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" EnableViewState="false" />