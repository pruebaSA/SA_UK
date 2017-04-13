<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CountryDropDownList.ascx.cs" Inherits="SalonAddict.Modules.CountryDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" AutoPostBack="true" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" Display="None" EnableViewState="false" mata:resourceKey="rfvValue" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" EnableViewState="false" />