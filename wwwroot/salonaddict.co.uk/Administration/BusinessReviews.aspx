<%@ Page Language="C#" MasterPageFile="~/Administration/MasterPages/Main.Master" AutoEventWireup="true" CodeBehind="BusinessReviews.aspx.cs" Inherits="SalonAddict.Administration.BusinessReviews" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="BusinessList" Src="~/Administration/Modules/DropDownLists/BusinessDropDownList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="section-header">
    <div class="title">
        <img src="images/ico-marketing.gif" alt="Marketing" />
        Manage Business Reviews
    </div>
    <div class="options">
        <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" ToolTip="Search for reviews based on the criteria below" />
    </div>
</div>
<table class="details">
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblStartDate" 
                runat="server" 
                Text="From:"
                IsRequired="true"
                ToolTip="The start date of the search (optional)."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox runat="server" ID="txtStartDate" />
            <asp:ImageButton ID="iStartDate" runat="Server" ImageUrl="~/Administration/images/ico-calendar.png" AlternateText="Click to show calendar" /><br />
            <ajaxToolkit:CalendarExtender ID="cStartDateButtonExtender" runat="server" TargetControlID="txtStartDate" PopupButtonID="iStartDate" />
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblEndDate" 
                runat="server" 
                Text="To:"
                IsRequired="true"
                ToolTip="The end date of the search."
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:TextBox ID="txtEndDate" runat="server"  />
            <asp:ImageButton ID="iEndDate" runat="Server" ImageUrl="~/Administration/images/ico-calendar.png"  AlternateText="Click to show calendar" /><br />
            <ajaxToolkit:CalendarExtender ID="cEndDateButtonExtender" runat="server" TargetControlID="txtEndDate" PopupButtonID="iEndDate" />
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblBusiness" 
                runat="server" 
                Text="Business"
                ToolTip="The reviewed business." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:BusinessList 
                ID="ddlBusiness" 
                runat="server" 
                Width="220px"
                DefaultText="Choose a Business"
                DefaultValue="0"  />
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblNeedsApprovalOnly" 
                runat="server" 
                Text="Pending approval only"
                ToolTip="Return reviews that only need approval." 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <asp:CheckBox ID="cbNeedsApprovalOnly" runat="server" Checked="true" />
        </td>
    </tr>
</table>
<br />
<asp:GridView 
    ID="gvBusinessReviews" 
    runat="server" 
    Width="100%"
    DataKeyNames="BusinessReviewID"
    AutoGenerateColumns="False" >
    <Columns>
        <asp:TemplateField HeaderText="Review"  >
            <ItemTemplate>
               <%# HttpUtility.HtmlDecode(Eval("ReviewText").ToString()) %>
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Rating">
            <ItemTemplate>
               <%# Math.Round((decimal)Eval("RatingScore"), 0) + "%" %> 
            </ItemTemplate>
            <ItemStyle Width="50px" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Approved">
            <ItemTemplate>
               <%# Eval("IsApproved")%>
            </ItemTemplate>
            <ItemStyle Width="50px" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Edit" >
            <ItemTemplate>
                <a href="BusinessReviewDetails.aspx?BusinessReviewID=<%#Eval("BusinessReviewID")%>" title="Edit review">
                    Edit
                </a>
            </ItemTemplate>
            <ItemStyle Width="50px" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Created On">
            <ItemTemplate>
               <%# Eval("CreatedOn")%>
            </ItemTemplate>
            <ItemStyle Width="100px" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
