<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SlideUnitTypeDropDownList.ascx.cs" Inherits="SalonAddict.Administration.Modules.SlideUnitTypeDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" AutoPostBack="true" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" ErrorMessage="Slide unit type is a required field." Display="None" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />