<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="SalonPortal.SecureArea.Employees" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Collections.Generic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
   <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-staff.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div class="options">
            <asp:Button ID="btnAdd" runat="server" CausesValidation="false" OnClick="btnAdd_Click" meta:resourceKey="btnAdd" />
        </div>
    </div>
    <br />
    <asp:GridView 
        ID="gvStaff" 
        runat="server" 
        Width="100%"
        DataKeyNames="StaffID"
        AutoGenerateColumns="False" >
        <Columns> 
            <asp:TemplateField>
                <ItemTemplate>
                    <a href="StaffDetails.aspx?StaffGUID=<%# Eval("StaffGUID") %>" >
                        <%# Eval("DisplayName") %>
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# base.Mappings.Count(mapping => mapping.StaffID == (int)Eval("StaffID")) %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# Eval("Mobile") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# (((int)Eval("TotalReviews")) == 0)? String.Empty : Eval("RatingScore") + "%" %>
                </ItemTemplate>
                <ItemStyle Width="80px" HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="50px">
                <ItemTemplate>
                    <a href="StaffDetails.aspx?StaffGUID=<%# Eval("StaffGUID") %>" >
                        <%= base.GetGlobalResourceObject("Global", "GridView_Link_Details").ToString() %>
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
