<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StateProvinceDropDownList.ascx.cs" Inherits="SalonAddict.Administration.Modules.StateProvinceDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" ErrorMessage="State/Province is a required field." Display="None" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />