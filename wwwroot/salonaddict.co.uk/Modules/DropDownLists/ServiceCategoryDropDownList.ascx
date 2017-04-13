<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServiceCategoryDropDownList.ascx.cs" Inherits="SalonAddict.Modules.ServiceCategoryDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" Visible="false" Display="None" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />