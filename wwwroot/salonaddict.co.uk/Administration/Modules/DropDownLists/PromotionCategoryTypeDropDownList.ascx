<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PromotionCategoryTypeDropDownList.ascx.cs" Inherits="SalonAddict.Administration.Modules.PromotionCategoryTypeDropDownList" %>
<asp:DropDownList ID="ddlValue" runat="server" ></asp:DropDownList>
<asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="ddlValue" ErrorMessage="Quick search category is a required field." Display="None" ></asp:RequiredFieldValidator>
<ajaxToolkit:ValidatorCalloutExtender runat="Server" ID="rfvValueExtender" TargetControlID="rfvValue" HighlightCssClass="validator-highlight" />