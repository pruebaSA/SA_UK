<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessReviewDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.BusinessReviewDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="Rating" Src="~/Modules/Rating.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="images/ico-marketing.gif" alt="" />
        Edit business review <a href="BusinessReviews.aspx" title="Back to business reviews">(back to reviews list)</a>
    </div>
    <div class="options">
        <asp:Button ID="btnApproveButton" runat="server" Text="Approve" OnClick="ApproveButton_Click" ToolTip="Approve business review" />
        <asp:Button ID="btnDeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" OnClientClick="return confirm('Are you sure?')" ToolTip="Delete business review" />
    </div>
</div>
<table class="details" >
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
            <asp:Literal ID="ltrBusinessName" runat="server" ></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblFirstName" 
                runat="server" 
                Text="First name"
                ToolTip="First name of the individual that gave the review. " 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <asp:Literal ID="ltrFirstName" runat="server" ></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblLastName" 
                runat="server" 
                Text="Last name"
                ToolTip="Last name of the individual that gave the review. " 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <asp:Literal ID="ltrLastName" runat="server" ></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblRating1" 
                runat="server" 
                ToolTip="Rating" 
                Text="Overall Satisfaction:"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:Rating ID="Rating1" runat="server" ReadOnly="true" />
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblRating2" 
                runat="server" 
                ToolTip="Rating" 
                Text="Value:"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:Rating ID="Rating2" runat="server" ReadOnly="true" />
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblRating3" 
                runat="server" 
                ToolTip="Rating" 
                Text="Customer Service:"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:Rating ID="Rating3" runat="server" ReadOnly="true" />
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblRating4" 
                runat="server" 
                ToolTip="Rating" 
                Text="Atmosphere:"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:Rating ID="Rating4" runat="server" ReadOnly="true" />
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblRating5" 
                runat="server" 
                ToolTip="Rating" 
                Text="Cleanliness:"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:Rating ID="Rating5" runat="server" ReadOnly="true" />
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblRating6" 
                runat="server" 
                ToolTip="Rating" 
                Text="Expertise:"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:Rating ID="Rating6" runat="server" ReadOnly="true" />
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblRating7" 
                runat="server" 
                ToolTip="Rating"
                Text="Liklihood of Repeat:" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:Rating ID="Rating7" runat="server" ReadOnly="true" />
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblReviewText" 
                runat="server" 
                Text="Review" 
                ToolTip="The review text"
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <SA:TextBox 
                ID="txtReviewText" 
                runat="server" 
                Width="400px"
                Rows="5"
                TextMode="MultiLine"
                IsRequired="false" />
            <asp:Literal ID="ltrReviewText" runat="server" ></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblAnonymous" 
                runat="server" 
                Text="Anonymous:" ToolTip="A value indicating whether the review is anonymous" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrAnonymous" runat="server" ></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title">
            <SA:ToolTipLabel 
                ID="lblApproved" 
                runat="server" 
                Text="Approved:" ToolTip="A value indicating whether the review has been approved by an administrator" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item">
            <asp:Literal ID="ltrApproved" runat="server" ></asp:Literal>
        </td>
    </tr>
    <tr>
        <td class="title" >
            <SA:ToolTipLabel 
                ID="lblCreatedOn" 
                runat="server" 
                Text="Created on"
                ToolTip="Date and time when the review was created" 
                ToolTipImage="~/Administration/images/ico-help.gif" />
        </td>
        <td class="data-item" >
            <asp:Literal ID="ltrCreatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
</table>
