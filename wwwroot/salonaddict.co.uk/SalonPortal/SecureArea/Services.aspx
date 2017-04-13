<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true" CodeBehind="Services.aspx.cs" Inherits="SalonPortal.SecureArea.Services" %>
<%@ Register TagPrefix="SA" TagName="Menu" Src="~/SecureArea/Modules/Menu.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Modules/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Message" Src="~/Modules/Message.ascx" %>
<%@ Register TagPrefix="SA" TagName="BackLink" Src="~/SecureArea/Modules/BackLink.ascx" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/SecureArea/Modules/ToolTipLabel.ascx" %>
<%@ MasterType VirtualPath="~/MasterPages/TwoColumn.master" %>
<%@ Import Namespace="System.Linq" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TwoColumnSideContentPlaceHolder" runat="server">
   <SA:Menu ID="cntlMenu" runat="server" />
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="TwoColumnContentPlaceHolder" runat="server">
   <div class="section-header">
        <div class="title">
            <img src="<%= Page.ResolveUrl("~/SecureArea/images/ico-services.png") %>" alt="" />
            <%= base.GetLocalResourceObject("Header.Text") %>
            <SA:BackLink ID="cntlBackLink" runat="server" />
        </div>
        <div class="options">
            <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" CausesValidation="false" meta:resourceKey="btnAdd" />
        </div>
    </div>
    <br />
    <asp:GridView 
        ID="gvServices" 
        runat="server" 
        Width="100%"
        DataKeyNames="ServiceID"
        AutoGenerateColumns="False" >
        <Columns> 
            <asp:TemplateField>
                <ItemTemplate>
                    <a href="ServiceDetails.aspx?ServiceGUID=<%# Eval("ServiceGUID") %>" >
                        <%# Eval("DisplayName") %>
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# SalonAddict.BusinessAccess.Implementation.CurrencyManager.FormatPrice((decimal)Eval("Price"), false) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
               <ItemTemplate>
                    <%# base.Mappings.Count(mapping => mapping.ServiceID == (int)Eval("ServiceID")) %>
               </ItemTemplate>
               <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                       <%# SalonAddict.BusinessAccess.Implementation.ServiceManager.GetLocalizedServiceType(
                           int.Parse(Eval("ServiceTypeID").ToString()),
                           SalonPortal.SAContext.Current.WorkingLanguage.LanguageID).Title %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                       <%# SalonAddict.BusinessAccess.Implementation.ServiceManager.GetLocalizedServiceCategory(
                           int.Parse(Eval("ServiceCategoryID").ToString()),
                           SalonPortal.SAContext.Current.WorkingLanguage.LanguageID).Title %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="50px">
                <ItemTemplate>
                    <a href="ServiceDetails.aspx?ServiceGUID=<%# Eval("ServiceGUID") %>" >
                        <%= base.GetGlobalResourceObject("Global", "GridView_Link_Details").ToString() %>
                    </a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
