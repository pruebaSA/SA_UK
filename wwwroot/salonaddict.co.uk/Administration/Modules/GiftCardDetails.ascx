<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GiftCardDetails.ascx.cs" Inherits="SalonAddict.Administration.Modules.GiftCardDetails" %>
<%@ Register TagPrefix="SA" TagName="ToolTipLabel" Src="~/Administration/Modules/Labels/ToolTipLabel.ascx" %>
<%@ Register TagPrefix="SA" TagName="TextBox" Src="~/Administration/Modules/TextBoxes/TextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="NumericTextBox" Src="~/Administration/Modules/TextBoxes/NumericTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DecimalTextBox" Src="~/Administration/Modules/TextBoxes/DecimalTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="EmailTextBox" Src="~/Administration/Modules/TextBoxes/EmailTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="DateTimeTextBox" Src="~/Administration/Modules/TextBoxes/DateTimeTextBox.ascx" %>
<%@ Register TagPrefix="SA" TagName="GiftCardLimitation" Src="~/Administration/Modules/DropDownLists/GiftCardLimitationDropDownList.ascx" %>

<div class="section-header">
<% if(this.Action == ActionType.Edit)
   { %>
    <div class="title">
        <img src="images/ico-sales.png" alt="" />
        Edit Gift Card details <a href="GiftCards.aspx" title="Back to gift card list">(back to gift card list)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ToolTip="Save gift card" />
        <asp:Button ID="DeleteButton" runat="server" Text="Delete" OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="Delete gift card" OnClientClick="return confirm('Are you sure?')" />
    </div>
<% } 
   else
   {%>
    <div class="title">
        <img src="images/ico-configuration.png" alt="" />
        Add a new gift card <a href="GiftCards.aspx" title="Back to areas list">(back to gift card list)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ToolTip="Save area" />
    </div>
<% } %>
</div>
<ajaxToolkit:TabContainer ID="tabs" runat="server" ActiveTabIndex="0" >
   <ajaxToolkit:TabPanel ID="tabInfo" runat="server" HeaderText="Gift card info" >
      <ContentTemplate>
        <table class="details">
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblGiftCardLimitationType" 
                        runat="server" 
                        Text="Gift Card Limitation:" 
                        IsRequired="true"
                        ToolTip="Limitations for the gift card to be applied." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <SA:GiftCardLimitation ID="ddlGiftCardLimitations" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblCustomerID" 
                        runat="server" 
                        Text="Customer ID:" 
                        IsRequired="false"
                        ToolTip="Customer identifier." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <SA:NumericTextBox 
                            ID="txtCustomerID" 
                            runat="server" 
                            IsRequired="false"
                            MinimumValue="0" 
                            MaximumValue="999999999" 
                            RangeErrorMessage="The value must be from 1 to 999999999">
                    </SA:NumericTextBox>
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblGiftCardCouponCode" 
                        runat="server" 
                        Text="Coupon Code:" 
                        IsRequired="true"
                        ToolTip="The coupon code used to redeem the gift card." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item">
                    <SA:TextBox 
                        ID="txtGiftCardCouponCode" 
                        runat="server" 
                        MaxLength="50"
                        AutoPostBack="true"
                        OnTextChanged="txtGiftCardCouponCode_TextChanged"
                        ErrorMessage="Gift card coupon code is required">
                    </SA:TextBox>
                    <asp:UpdatePanel ID="up" runat="server" >
                       <ContentTemplate>
                           <asp:CustomValidator ID="cvGiftCardCouponCode" runat="server" ControlToValidate="txtGiftCardCouponCode" EnableClientScript="false" OnServerValidate="cvGiftCardCouponCode_Validate" ErrorMessage="Gift card coupon code is not available." ></asp:CustomValidator>
                       </ContentTemplate>
                       <Triggers>
                          <asp:AsyncPostBackTrigger ControlID="txtGiftCardCouponCode" />
                       </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblAmount" 
                        runat="server" 
                        Text="Amount:" 
                        IsRequired="true"
                        ToolTip="Amount to apply." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <SA:DecimalTextBox 
                            ID="txtAmount" 
                            runat="server" 
                            RequiredErrorMessage="Amount is a required field."
                            MinimumValue="1" 
                            MaximumValue="999" 
                            RangeErrorMessage="The value must be from 1 to 999">
                    </SA:DecimalTextBox>
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblRecipientName" 
                        runat="server" 
                        Text="Recipient Name:" 
                        IsRequired="true"
                        ToolTip="Recipient's name." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <SA:TextBox 
                        ID="txtRecipientName" 
                        runat="server" 
                        MaxLength="100"
                        ErrorMessage="Recipient's name is a required field." >
                    </SA:TextBox>
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblRecipientEmail" 
                        runat="server" 
                        Text="Recipient Email:" 
                        IsRequired="true"
                        ToolTip="Recipient's email." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <SA:EmailTextBox 
                        ID="txtRecipientEmail" 
                        runat="server" 
                        MaxLength="100" >
                    </SA:EmailTextBox>
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblSendersName" 
                        runat="server" 
                        Text="Sender Name:" 
                        IsRequired="true"
                        ToolTip="Recipient's name." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <SA:TextBox 
                        ID="txtSenderName" 
                        runat="server" 
                        MaxLength="100"
                        ErrorMessage="Sender's name is a required field"
                        Text="SalonAddict" >
                    </SA:TextBox>
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblSendersEmail" 
                        runat="server" 
                        Text="Sender Email:" 
                        IsRequired="true"
                        ToolTip="Sender's email." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <SA:EmailTextBox 
                        ID="txtSendersEmail" 
                        runat="server" 
                        MaxLength="100"
                        ErrorMessage="Sender's email is a required field"
                        Text="info@salonaddict.co.uk" >
                    </SA:EmailTextBox>
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblExpiresOn" 
                        runat="server" 
                        Text="Expires:" 
                        IsRequired="false"
                        ToolTip="Optional expiration date." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <SA:DateTimeTextBox ID="txtExpiresOn" runat="server" IsRequired="false" />
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblMessage" 
                        runat="server" 
                        Text="Message:" 
                        IsRequired="false"
                        ToolTip="Personal message." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Width="400px" Height="80px" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblSenderNotified" 
                        runat="server" 
                        Text="Sender Notified:" 
                        IsRequired="true"
                        ToolTip="Value indicating whether or not the sender has been notified." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <asp:CheckBox ID="cbSenderNotified" runat="server" Checked="true" />
                </td>
            </tr>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblActive" 
                        runat="server" 
                        Text="Active:" 
                        IsRequired="true"
                        ToolTip="Value indicating whether or not the gift card is active." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item" >
                    <asp:CheckBox ID="cbActive" runat="server" Checked="true" />
                </td>
            </tr>
                <% if (this.Action == ActionType.Edit)
               { %>
            <tr>
                <td class="title">
                    <SA:ToolTipLabel 
                        ID="lblCreatedOn" 
                        runat="server" 
                        Text="Created on:"
                        ToolTip="The date/time the gift card was created." 
                        ToolTipImage="~/Administration/images/ico-help.gif" />
                </td>
                <td class="data-item">
                    <asp:Literal ID="ltrCreatedOn" runat="server"></asp:Literal>
                </td>
            </tr>
            <% } %>
        </table>
      </ContentTemplate>
   </ajaxToolkit:TabPanel>
   <ajaxToolkit:TabPanel ID="tabUsage" runat="server" HeaderText="Usage history" >
      <ContentTemplate>
        <asp:GridView 
            ID="gv" 
            runat="server" 
            DataKeyNames="GiftCardUsageHistoryID"
            AutoGenerateColumns="False"
            AllowPaging="true"
            PageSize="10"
            OnPageIndexChanging="gv_PageIndexChanged"
            Width="100%" >
            <Columns>
                <asp:TemplateField HeaderText="Used Amount" >
                    <ItemTemplate>
                        <%# Math.Round((decimal)Eval("UsedValue"), 2) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Order" >
                    <ItemTemplate>
                        <a href='<%# "OrderDetails.aspx?OrderID=" + Eval("OrderID") %>' >view</a>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Used on" >
                    <ItemTemplate>
                        <%# Eval("CreatedOn") %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
      </ContentTemplate>
   </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
